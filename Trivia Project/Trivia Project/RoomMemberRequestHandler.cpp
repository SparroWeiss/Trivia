#include "RoomMemberRequestHandler.h"
#include "Communicator.h"

/*
Constructor:
Initializes the variables of the object
*/
RoomMemberRequestHandler::RoomMemberRequestHandler(LoggedUser user, Room * room) : m_handlerFactory(m_handlerFactory->getInstance())
{
	m_room = room;
	m_user = user;
}

/*
Destructor
*/
RoomMemberRequestHandler::~RoomMemberRequestHandler() {}

/*
Function checks if a request is relevant to the handler
Input : request info
Output : true - request is relevant, false - request isn't relevant
*/
bool RoomMemberRequestHandler::isRequestRelevant(RequestInfo info)
{
	return info.id == LEAVEROOM || info.id == GETROOMSTATE;
}

/*
Function gets the result of a request
Input: request info
Output: request result
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
Function leaves the room
Input: request info
Output: request result
*/
RequestResult RoomMemberRequestHandler::leaveRoom(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the leave room request isn't valid, stay in same handler
	LeaveRoomResponse leaveRoomRes = { LeaveRoomStatus::FAILED }; // status: 0
	std::unique_lock<std::mutex> locker(_mutex_room);
	if (m_room->removeUser(m_user.getUsername()))
	{
		if (m_room->getData().isActive == ActiveMode::START_PLAYING) // if the game has started, the user needs to join the game handler
		{ 
			leaveRoomRes = { LeaveRoomStatus::PLAY }; // status: 2
			newHandle = m_handlerFactory->createGameRequestHandler(m_user, m_room); // pointer to the next handle : game
		}
		else // if the game hasn't started yet and the user wants to log out
		{ 
			leaveRoomRes = { LeaveRoomStatus::LEFT_ROOM }; // status: 1
			newHandle = m_handlerFactory->createMenuRequestHandler(m_user.getUsername()); // pointer to the previous handle : menu
		}
		if (m_room->getAllUsers().empty()) // the room is empty
		{ 
			m_handlerFactory->getRoomManager().deleteRoom(m_room->getData().id);
		}
		locker.unlock();
	}
	else
		locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(leaveRoomRes), newHandle };
}

/*
Function gets the room state
Input: request info
Output: request result
*/
RequestResult RoomMemberRequestHandler::getRoomState(RequestInfo info)
{
	std::unique_lock<std::mutex> locker(_mutex_room);

	GetRoomStateResponse getStateRes;
	getStateRes.answerTimeout = m_room->getData().timePerQuestion;
	getStateRes.players = m_room->getAllUsers();
	getStateRes.questionCount = m_room->getData().questionCount;

	getStateRes.status = m_room->getData().isActive;
	getStateRes.hasGameBegun = m_room->getData().isActive == ActiveMode::START_PLAYING;
	locker.unlock();

	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getStateRes), this };
}