#include "MenuRequestHandler.h"

MenuRequestHandler::MenuRequestHandler() : MenuRequestHandler(nullptr){}

MenuRequestHandler::MenuRequestHandler(LoginRequestHandler* login)
{
	m_login = login;
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