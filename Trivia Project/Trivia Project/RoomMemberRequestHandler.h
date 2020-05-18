#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"
#include "RoomManager.h"

class RequestHandlerFactory;


class RoomMemberRequestHandler : public IRequestHandler
{
public:
	RoomMemberRequestHandler(LoggedUser user, Room* room);
	~RoomMemberRequestHandler();
	bool isRequestRelevant(RequestInfo info);
	RequestResult handleRequest(RequestInfo info);

private:
	Room* m_room;
	LoggedUser m_user;
	RequestHandlerFactory* m_handlerFactory;

	RequestResult leaveRoom(RequestInfo info);
	RequestResult getRoomState(RequestInfo info);

};