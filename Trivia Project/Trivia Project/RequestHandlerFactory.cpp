#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

/*
constructor
sets the database and login manager
*/
RequestHandlerFactory::RequestHandlerFactory()
{
	m_database = SqliteDatabase::getInstance();
	m_loginManager = m_loginManager->getInstance();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
RequestHandlerFactory* RequestHandlerFactory::getInstance()
{
	if (instance == 0)
	{
		instance = new RequestHandlerFactory();
	}

	return instance;
}

RequestHandlerFactory::~RequestHandlerFactory()
{
}
/*
function creates new Login request handler
input: none
output: Login request handler
*/
LoginRequestHandler RequestHandlerFactory::createLoginRequestHandler()
{
	return LoginRequestHandler();
}
/*
MenuRequestHandler RequestHandlerFactory::createMenuRequestHandler()
{
	return MenuRequestHandler(this);
}
*/
/*
function returns the Login Manager
input: none
output: Login Manager
*/
LoginManager* RequestHandlerFactory::getLoginManager() const
{
	return m_loginManager;
}
