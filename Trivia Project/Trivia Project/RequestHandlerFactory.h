#pragma once
#include "LoginManager.h"
#include "LoginRequestHandler.h"

class LoginRequestHandler;

class RequestHandlerFactory
{
public:
	RequestHandlerFactory();
	RequestHandlerFactory(IDataBase* db);
	~RequestHandlerFactory();
	LoginRequestHandler createLoginRequestHandler();
	LoginManager getLoginManager() const;

private:
	LoginManager m_loginManager;
	IDataBase* m_database;
};
