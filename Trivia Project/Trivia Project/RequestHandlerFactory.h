#pragma once
#include "LoginManager.h"
#include "LoginRequestHandler.h"

class LoginRequestHandler;

class RequestHandlerFactory
{
public:
	static RequestHandlerFactory* getInstance();
	~RequestHandlerFactory();
	LoginRequestHandler* createLoginRequestHandler();
	LoginManager* getLoginManager() const;

private:
	RequestHandlerFactory();
	static RequestHandlerFactory* instance;
	static int instances;

	LoginManager* m_loginManager;
	IDataBase* m_database;
};
