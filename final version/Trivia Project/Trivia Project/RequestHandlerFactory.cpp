#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

/*
constructor
initializes the variables of the object
*/
RequestHandlerFactory::RequestHandlerFactory()
{
	m_database = SqliteDatabase::getInstance();
	m_loginManager = LoginManager::getInstance();
	m_roomManager = RoomManager::getInstance();
	m_statisticsManager = StatisticsManager::getInstance();
	m_gameManager = GameManager::getInstance();
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

/*
destructor
frees allocated memory
*/
RequestHandlerFactory::~RequestHandlerFactory()
{
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
MenuRequestHandler* RequestHandlerFactory::createMenuRequestHandler(std::string username)
{
	return new MenuRequestHandler(username);
}

/*
function creates new Room Admin handler
input: logged user, pointer to the room
output: Room Admin handler
*/
RoomAdminRequestHandler* RequestHandlerFactory::createRoomAdminRequestHandler(LoggedUser user, Room* room)
{
	return new RoomAdminRequestHandler(user, room);
}

/*
function creates new Room Member handler
input: logged user, pointer to the room
output: Room Member handler
*/
RoomMemberRequestHandler* RequestHandlerFactory::createRoomMemberRequestHandler(LoggedUser user, Room* room)
{
	return new RoomMemberRequestHandler(user, room);
}

/*
function creates new Game handler
input: logged user, pointer to the room
output: Game handler
*/
GameRequestHandler* RequestHandlerFactory::createGameRequestHandler(LoggedUser user, Room* room)
{
	return new GameRequestHandler(user, room);
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

/*
function returns the Game Manager
input: none
output: Game Manager
*/
GameManager& RequestHandlerFactory::getGameManager()
{
	return *m_gameManager;
}
