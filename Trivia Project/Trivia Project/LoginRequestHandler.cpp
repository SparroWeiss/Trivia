#include "LoginRequestHandler.h"

LoginRequestHandler::LoginRequestHandler()
{
}

LoginRequestHandler::~LoginRequestHandler()
{
}

bool LoginRequestHandler::isRequestRelevent(RequestInfo)
{
	return false;
}

RequestResult LoginRequestHandler::handleRequest(RequestInfo info)
{
	return RequestResult();
}

RequestResult LoginRequestHandler::login(RequestInfo info)
{
	return RequestResult();
}

RequestResult LoginRequestHandler::signup(RequestInfo info)
{
	return RequestResult();
}
