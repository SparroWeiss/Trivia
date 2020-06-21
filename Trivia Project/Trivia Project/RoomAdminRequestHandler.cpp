#include "RoomAdminRequestHandler.h"
#include "Communicator.h"

/*
constructor
initializes the variables of the object
*/
RoomAdminRequestHandler::RoomAdminRequestHandler(LoggedUser user, Room* room)
{
	m_handlerFactory = m_handlerFactory->getInstance();
	m_user = user;
	m_room = room;
}

/*
destructor
frees allocated memory
*/
RoomAdminRequestHandler::~RoomAdminRequestHandler()
{
}

/*
function checks if a request is relevant to the handler
input : request info
output : true - request is relevant, false - request isn't relevant
*/
bool RoomAdminRequestHandler::isRequestRelevant(RequestInfo info)
{
	return info.id == CLOSEROOM ||
		info.id == STARTGAME ||
		info.id == GETROOMSTATE;
}

/*
function gets the result of a request
input: request info
output: request result
*/
RequestResult RoomAdminRequestHandler::handleRequest(RequestInfo info)
{
	switch (info.id)
	{
	case CLOSEROOM:
		return closeRoom(info);
	case STARTGAME:
		return startGame(info);
	case GETROOMSTATE:
		return getRoomState(info);
	default:
		return RequestResult();
	}
	
}

/*
function closes room
input: request info
output: request result
*/
RequestResult RoomAdminRequestHandler::closeRoom(RequestInfo info)
{
	RoomMemberRequestHandler* handler = new RoomMemberRequestHandler(m_user, m_room);
	RequestInfo leaveReq = { LEAVEROOM };
	
	RoomData data = m_room->getData();
	data.isActive = ActiveMode::DONE;

	std::unique_lock<std::mutex> locker(_mutex_room);
	m_room->setData(data); // set the data to inform the players that the room is closed
	locker.unlock();

	RequestResult res = handler->handleRequest(leaveReq); // leaving the room
	delete handler;

	res.newHandler = m_handlerFactory->createMenuRequestHandler(m_user.getUsername());
	return res;
}

/*
function starts the game
input: request info
output: request result
*/
RequestResult RoomAdminRequestHandler::startGame(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the starting request isn't valid, stay in same handler
	StartGameResponse startRes = { 0 }; // status: 0
	std::lock_guard<std::mutex> locker(_mutex_room);
	if (m_room->getAllUsers().size() > 1)
	{
		startRes = { 1 }; // status: 1
		m_room->setGame(m_handlerFactory->getGameManager().createGame(*m_room));
		newHandle = m_handlerFactory->createGameRequestHandler(m_user, m_room); // pointer to the next handle : Game
		RoomData data = m_room->getData();
		data.isActive = ActiveMode::START_PLAYING;
		m_room->setData(data);
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(startRes), newHandle };
}

/*
function gets the room state
input: request info
output: request result
*/
RequestResult RoomAdminRequestHandler::getRoomState(RequestInfo info)
{
	RoomMemberRequestHandler* handler = new RoomMemberRequestHandler(m_user, m_room);
	RequestResult res = handler->handleRequest(info);
	delete handler;
	res.newHandler = this;
	return res;
}
