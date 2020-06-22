#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"
#include "RoomManager.h"

class RequestHandlerFactory;

class RoomAdminRequestHandler : public IRequestHandler
{
public:
	RoomAdminRequestHandler(LoggedUser user, Room* room);
	~RoomAdminRequestHandler();
	bool isRequestRelevant(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	Room* m_room;
	LoggedUser m_user;
	RequestHandlerFactory* m_handlerFactory;

	RequestResult closeRoom(RequestInfo info);
	RequestResult startGame(RequestInfo info);
	RequestResult getRoomState(RequestInfo info);

};

static std::mutex _mutex_room;