#include "MenuRequestHandler.h"
#include "Communicator.h"
#include <string>

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
	delete m_handlerFactory;
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

RequestResult MenuRequestHandler::getRooms(RequestInfo info)
{
	GetRoomResponse roomsRes = { 1 }; // status: 1
	std::vector<Room> rooms = m_handlerFactory->getRoomManager().getRooms();
	for (std::vector<Room>::iterator i = rooms.begin(); i != rooms.end(); ++i)
	{
		roomsRes.rooms.push_back((*i).getData());
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(roomsRes), this };
}

RequestResult MenuRequestHandler::getPlayersInRoom(RequestInfo info)
{
	GetPlayersInRoomRequest playersReq = JsonRequestPacketDeserializer::deserializeGetPlayersInRoomRequest(info.buffer);
	GetPlayersInRoomResponse playersRes;
	
	std::vector<LoggedUser> players = (*findRoom(playersReq.roomId)).getAllUsers();
	for (std::vector<LoggedUser>::iterator i = players.begin(); i != players.end(); ++i)
	{
		playersRes.players.push_back((*i).getUsername());
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(playersRes), this };
}

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
	for (std::vector<std::string>::iterator i = data._topPlayers.begin(); i != data._topPlayers.end(); ++i)
	{
		statisticsRes.statiatics.push_back((*i)); // [6-10] - number of games
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(statisticsRes), this };
}

RequestResult MenuRequestHandler::joinRoom(RequestInfo info)
{
	JoinRoomRequest joinRoomReq = JsonRequestPacketDeserializer::deserializeJoinRoomRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the joining request isn't valid, stey in same handler
	JoinRoomResponse joinRoomRes = { 0 }; // status: 0
	if ((*findRoom(joinRoomReq.roomId)).addUser(m_user))
	{
		joinRoomRes.status = 1; // status: 1
		newHandle = nullptr; // pointer to the next handle : room member (will be added in 3.0.0)
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(joinRoomRes), newHandle };
}

RequestResult MenuRequestHandler::createRoom(RequestInfo info)
{
	CreateRoomRequest createRoomReq = JsonRequestPacketDeserializer::deserializeCreateRoomRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the creating request isn't valid, stey in same handler
	CreateRoomResponse createRoomRes = { 1 }; // status: 0
	m_handlerFactory->getRoomManager().createRoom(m_user);
	newHandle = nullptr; // pointer to the next handle : room admin (will be added in 3.0.0)
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(createRoomRes), newHandle };
}

//////////////////////////////////HELPER
/*
function finds the room
*/
std::vector<Room>::iterator MenuRequestHandler::findRoom(unsigned int id)
{
	std::vector<Room> rooms = m_handlerFactory->getRoomManager().getRooms();
	for (std::vector<Room>::iterator i = rooms.begin(); i != rooms.end(); ++i)
	{
		if ((*i).getData().id == id)
		{
			return i;
		}
	}
	return rooms.end();
}
