#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

/*
constructor
sets the database and login manager
*/
RequestHandlerFactory::RequestHandlerFactory() : m_loginManager(*m_loginManager.getInstance()),
												 m_roomManager(*m_roomManager.getInstance()), 
												 m_statisticsManager(*m_statisticsManager.getInstance())
{
	m_database = SqliteDatabase::getInstance();
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
	instances++;
	return instance;
}

/*
distructor
frees allocated memory, the only new allocated memory in the class is the instance
*/
RequestHandlerFactory::~RequestHandlerFactory()
{
	instances--;
	if (instances == 0)
	{
		delete instance;
	}
}

/*
function creates new Login request handler
input: none
output: Login request handler
*/
LoginRequestHandler* RequestHandlerFactory::createLoginRequestHandler()
{
	return new LoginRequestHandler();
}

/*
function creates new Menu request handler
input: none
output: Menu request handler
*/
MenuRequestHandler* RequestHandlerFactory::createMenuRequestHandler()
{
	return new MenuRequestHandler();
}

/*
function returns the Login Manager
input: none
output: Login Manager
*/
LoginManager& RequestHandlerFactory::getLoginManager()
{
	return m_loginManager;
}

/*
function returns the Room Manager
input: none
output: Room Manager
*/
RoomManager& RequestHandlerFactory::getRoomManager()
{
	return m_roomManager;
}

/*
function returns the Statistics Manager
input: none
output: Statistics Manager
*/
StatisticsManager& RequestHandlerFactory::getStatisticsManager()
{
	return m_statisticsManager;
}
