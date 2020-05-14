#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

/*
constructor
initializes the variables of the object
*/
RequestHandlerFactory::RequestHandlerFactory()
{
	m_database = SqliteDatabase::getInstance();
	m_loginManager = m_loginManager->getInstance();
	m_roomManager = m_roomManager->getInstance();
	m_statisticsManager = m_statisticsManager->getInstance();
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
destructor
frees allocated memory
*/
RequestHandlerFactory::~RequestHandlerFactory()
{
	instances--;
	if (instances == 0)
	{
		delete m_database;
		delete m_loginManager;
		delete m_roomManager;
		delete m_statisticsManager;
		delete instance;
	}
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
function creates new Menu request handler
input: none
output: Menu request handler
*/
MenuRequestHandler RequestHandlerFactory::createMenuRequestHandler(std::string username)
{
	return MenuRequestHandler(username);
}

/*
function returns the Login Manager
input: none
output: Login Manager
*/
LoginManager& RequestHandlerFactory::getLoginManager()
{
	return *m_loginManager;
}

/*
function returns the Room Manager
input: none
output: Room Manager
*/
RoomManager& RequestHandlerFactory::getRoomManager()
{
	return *m_roomManager;
}

/*
function returns the Statistics Manager
input: none
output: Statistics Manager
*/
StatisticsManager& RequestHandlerFactory::getStatisticsManager()
{
	return *m_statisticsManager;
}
