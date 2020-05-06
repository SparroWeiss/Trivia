#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"

class RequestHandlerFactory;


class LoginRequestHandler : public IRequestHandler
{
public:
	LoginRequestHandler();
	LoginRequestHandler(RequestHandlerFactory * handleFactory);
	~LoginRequestHandler();
	bool isRequestRelevent(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	LoginManager m_loginManager;
	RequestHandlerFactory* m_handlerFactory;

	RequestResult login(RequestInfo info);
	RequestResult signup(RequestInfo info);
};