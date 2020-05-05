#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

RequestHandlerFactory::RequestHandlerFactory(): RequestHandlerFactory(nullptr){}
/*
constructor
sets the database and login manager
*/
RequestHandlerFactory::RequestHandlerFactory(IDataBase * db)
{
	m_database = db;
	m_loginManager = LoginManager(db);
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
	return LoginRequestHandler(this);
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
LoginManager RequestHandlerFactory::getLoginManager() const
{
	return m_loginManager;
}
