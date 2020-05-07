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
	static RequestHandlerFactory* instance;
	RequestHandlerFactory();
	LoginManager* m_loginManager;
	IDataBase* m_database;
};
