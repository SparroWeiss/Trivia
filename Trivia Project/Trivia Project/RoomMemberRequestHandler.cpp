#include "RoomMemberRequestHandler.h"
#include "Communicator.h"


/*
constructor
initializes the variables of the object
*/
RoomMemberRequestHandler::RoomMemberRequestHandler(LoggedUser user, Room * room) : m_handlerFactory(m_handlerFactory->getInstance())
{
	m_room = room;
	m_user = user;
}

/*
destructor
frees allocated memory
*/
RoomMemberRequestHandler::~RoomMemberRequestHandler()
{
}

/*
function checks if a request is relevant to the handler
input : request info
output : true - request is relevant, false - request isn't relevant
*/
bool RoomMemberRequestHandler::isRequestRelevant(RequestInfo info)
{
	return info.id == LEAVEROOM || info.id == GETROOMSTATE;
}

/*
function gets the result of a request
input: request info
output: request result
*/
RequestResult RoomMemberRequestHandler::handleRequest(RequestInfo info)
{
	if (info.id == LEAVEROOM)
	{
		return leaveRoom(info);
	}
	return getRoomState(info);
}

/*
function leaves the room
input: request info
output: request result
*/
RequestResult RoomMemberRequestHandler::leaveRoom(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the leave room request isn't valid, stay in same handler
	LeaveRoomResponse leaveRoomRes = { 0 }; // status: 0
	std::unique_lock<std::mutex> locker(_mutex_room);
	if (m_room->removeUser(m_user.getUsername()))
	{
		locker.unlock();
		leaveRoomRes = { ActiveMode::DONE }; // status: 3
		newHandle = m_handlerFactory->createMenuRequestHandler(m_user.getUsername()); // pointer to the previous handle : menu
	}
	else
		locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(leaveRoomRes), newHandle };
}

/*
function gets the room state
input: request info
output: request result
*/
RequestResult RoomMemberRequestHandler::getRoomState(RequestInfo info)
{
	std::unique_lock<std::mutex> locker(_mutex_room);
	if (m_room->getData().isActive == ActiveMode::DONE)
	{ // room has been closed by the admin
		locker.unlock();
		return leaveRoom({ LEAVEROOM });
	}
	if (m_room->getData().isActive == ActiveMode::PLAYING)
	{ // game has started by the admin
		// TODO : change the handler to a game handler (4.0.0)
		locker.unlock();
		return  RequestResult{ JsonResponsePacketSerializer::serializeResponse(
			StartGameResponse{ ActiveMode::PLAYING }) /* status: 2 */, nullptr };
	}

	GetRoomStateResponse getStateRes;
	getStateRes.answerTimeout = m_room->getData().timePerQuestion;
	getStateRes.players = m_room->getAllUsers();
	locker.unlock();

	getStateRes.status = ActiveMode::WAITING;  // status: 1
	getStateRes.hasGameBegun = false;
	getStateRes.questionCount = 0; // what?

	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getStateRes), this };
}
