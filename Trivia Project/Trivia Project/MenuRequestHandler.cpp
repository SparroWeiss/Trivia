#include "MenuRequestHandler.h"

MenuRequestHandler::MenuRequestHandler()
{
}


MenuRequestHandler::~MenuRequestHandler()
{
}

bool MenuRequestHandler::isRequestRelevent(RequestInfo)
{
	return false;
}

RequestResult MenuRequestHandler::handleRequest(RequestInfo)
{
	return RequestResult();
}