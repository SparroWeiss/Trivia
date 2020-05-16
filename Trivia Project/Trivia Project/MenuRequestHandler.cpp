#include "MenuRequestHandler.h"
#include "Communicator.h"
#include <string>

std::mutex _mutex_room_vector;

/*
constructor
initializes the variables of the object
*/
MenuRequestHandler::MenuRequestHandler(std::string username) : 
	m_handlerFactory(m_handlerFactory->getInstance())
{
	m_user = LoggedUser(username);
}

/*
destructor
frees allocated memory
*/
MenuRequestHandler::~MenuRequestHandler()
{
}

/*
function checks if a request is relevant to the handler
input : request info
output : true - request is relevant, false - request isn't relevant
*/
bool MenuRequestHandler::isRequestRelevant(RequestInfo info)
{
	return (info.id == SIGNOUT || info.id == GETROOMS 
		|| info.id == GETPLAYERSINROOM || info.id == GETSTATISTICS
		|| info.id == JOINROOM || info.id == CREATEROOM);
}

/*
function gets the result of a request
input: request info
output: request result
*/
RequestResult MenuRequestHandler::handleRequest(RequestInfo info)
{
	switch (info.id)
	{
	case SIGNOUT:
		return signout(info);
	case GETROOMS:
		return getRooms(info);
	case GETPLAYERSINROOM:
		return getPlayersInRoom(info);
	case GETSTATISTICS:
		return getStatistics(info);
	case JOINROOM:
		return joinRoom(info);
	case CREATEROOM:
		return createRoom(info);
	default:
		return RequestResult();
	}
}

/*
function signs out a user
input: request info
output: request result
*/
RequestResult MenuRequestHandler::signout(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the logout request isn't valid, stay in same handler
	SignupResponse signOutRes = { 0 }; // status: 0
	if (m_handlerFactory->getLoginManager().logout(m_user.getUsername()))
	{ // if the sign out request is valid
		signOutRes = { 1 }; // status: 1
		newHandle = nullptr; // pointer to the next handle : nullptr
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(signOutRes), newHandle };
}

/*
function gets the rooms in the server
input: request info
output: request result
*/
RequestResult MenuRequestHandler::getRooms(RequestInfo info)
{
	GetRoomResponse roomsRes = { 1 }; // status: 1

	std::unique_lock<std::mutex> locker(_mutex_room_vector);
	std::vector<Room> rooms = m_handlerFactory->getRoomManager().getRooms();
	locker.unlock();

	for (std::vector<Room>::iterator i = rooms.begin(); i != rooms.end(); ++i)
	{
		roomsRes.rooms.push_back((*i).getData());
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(roomsRes), this };
}

/*
function gets the players in a room
input: request info
output: request result
*/
RequestResult MenuRequestHandler::getPlayersInRoom(RequestInfo info)
{
	GetPlayersInRoomRequest playersReq = JsonRequestPacketDeserializer::deserializeGetPlayersInRoomRequest(info.buffer);
	GetPlayersInRoomResponse playersRes;
	
	playersRes.players = (*findRoom(playersReq.roomId)).getAllUsers();
	
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(playersRes), this };
}

/*
function gets the statistics of a user
input: request info
output: request result
*/
RequestResult MenuRequestHandler::getStatistics(RequestInfo info)
{
	GetStatisticsResponse statisticsRes = { 1 }; // status: 1
	StatisticsData data = m_handlerFactory->getStatisticsManager().getStatistics(m_user.getUsername());
	statisticsRes.statiatics.push_back(data._username); // [0] - username
	statisticsRes.statiatics.push_back(std::to_string(data._totalAnswers)); // [1] - total answers
	statisticsRes.statiatics.push_back(std::to_string(data._correctAnswers)); // [2] - coorect answers
	statisticsRes.statiatics.push_back(std::to_string(data._incorrectAnswers)); // [3] - incorrect answers
	statisticsRes.statiatics.push_back(std::to_string(data._averageAnswerTime)); // [4] - average time per answer
	statisticsRes.statiatics.push_back(std::to_string(data._numOfGames)); // [5] - number of games

	std::vector<float>::iterator j = data._topScores.begin();
	for (std::vector<std::string>::iterator i = data._topPlayers.begin(); i != data._topPlayers.end(); ++i)
	{
		statisticsRes.statiatics.push_back((*i)); // [6, 8, 10, 12, 14] - top players
		statisticsRes.statiatics.push_back(std::to_string(*j)); // [7, 9, 11, 13, 15] - top scores
		++j;
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(statisticsRes), this };
}

/*
function joins a member to a room
input: request info
output: request result
*/
RequestResult MenuRequestHandler::joinRoom(RequestInfo info)
{
	JoinRoomRequest joinRoomReq = JsonRequestPacketDeserializer::deserializeJoinRoomRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the joining request isn't valid, stey in same handler
	JoinRoomResponse joinRoomRes = { 0 }; // status: 0
	Room * room = &(*findRoom(joinRoomReq.roomId));
	if (room->addUser(m_user))
	{
		joinRoomRes.status = 1; // status: 1
		newHandle = m_handlerFactory->createRoomMemberRequestHandler(m_user, room); // pointer to the next handle : room member
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(joinRoomRes), newHandle };
}

/*
function creates room by demand
input: request info
output: request result
*/
RequestResult MenuRequestHandler::createRoom(RequestInfo info)
{
	CreateRoomRequest createRoomReq = JsonRequestPacketDeserializer::deserializeCreateRoomRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the creating request isn't valid, stey in same handler
	CreateRoomResponse createRoomRes = { 1 }; // status: 1
	RoomData data = { 0, createRoomReq.roomName, createRoomReq.maxUsers, createRoomReq.answerTimeout, ActiveMode::WAITING };
	std::unique_lock<std::mutex> locker(_mutex_room_vector);
	newHandle = m_handlerFactory->createRoomAdminRequestHandler(m_user, 
		&(*findRoom(m_handlerFactory->getRoomManager().createRoom(m_user, data)))); // pointer to the next handle : room admin
	locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(createRoomRes), newHandle };
}

//////////////////////////////////HELPER
/*
function finds the room
input: room id
output: iterator for the room
*/
std::vector<Room>::iterator MenuRequestHandler::findRoom(unsigned int id)
{
	std::unique_lock<std::mutex> locker(_mutex_room_vector);
	std::vector<Room> rooms = m_handlerFactory->getRoomManager().getRooms();
	locker.unlock();

	for (std::vector<Room>::iterator i = rooms.begin(); i != rooms.end(); ++i)
	{
		if ((*i).getData().id == id)
		{
			return i;
		}
	}
	return rooms.end();
}
