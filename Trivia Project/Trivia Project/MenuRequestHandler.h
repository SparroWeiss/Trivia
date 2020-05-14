#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"
#include "RoomManager.h"

class RequestHandlerFactory;


class MenuRequestHandler : public IRequestHandler
{
public:
	MenuRequestHandler(std::string username);
	~MenuRequestHandler();
	bool isRequestRelevant(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	LoggedUser m_user;
	RequestHandlerFactory* m_handlerFactory;

	RequestResult signout(RequestInfo info);
	RequestResult getRooms(RequestInfo info);
	RequestResult getPlayersInRoom(RequestInfo info);
	RequestResult getStatistics(RequestInfo info);
	RequestResult joinRoom(RequestInfo info);
	RequestResult createRoom(RequestInfo info);

	std::vector<Room>::iterator findRoom(unsigned int id);
};