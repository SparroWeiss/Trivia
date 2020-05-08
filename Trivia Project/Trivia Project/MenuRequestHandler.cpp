#include "MenuRequestHandler.h"
#include "Communicator.h"

MenuRequestHandler::MenuRequestHandler()
{
	m_handlerFactory = m_handlerFactory->getInstance();
	m_user = new LoggedUser(""); // for now, we need to understand how to do it
}

MenuRequestHandler::~MenuRequestHandler()
{
	delete m_user;
}

bool MenuRequestHandler::isRequestRelevent(RequestInfo info)
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
	return RequestResult();
}

RequestResult MenuRequestHandler::getRooms(RequestInfo info)
{
	return RequestResult();
}

RequestResult MenuRequestHandler::getPlayersInRoom(RequestInfo info)
{
	return RequestResult();
}

RequestResult MenuRequestHandler::getStatistics(RequestInfo info)
{
	return RequestResult();
}

RequestResult MenuRequestHandler::joinRoom(RequestInfo info)
{
	return RequestResult();
}

RequestResult MenuRequestHandler::createRoom(RequestInfo info)
{
	return RequestResult();
}
