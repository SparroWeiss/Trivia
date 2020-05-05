#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"

class RequestHandlerFactory;


class MenuRequestHandler : public IRequestHandler
{
public:
	MenuRequestHandler();
	MenuRequestHandler(LoginRequestHandler* login);
	~MenuRequestHandler();
	bool isRequestRelevent(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	LoginRequestHandler* m_login; // for now, we will see what we will do with this class
};