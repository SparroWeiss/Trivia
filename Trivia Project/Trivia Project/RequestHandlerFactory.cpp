#include "RequestHandlerFactory.h"
#include "SqliteDatabase.h"

/*
Constructor:
Initializes the variables of the object
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
Function make sure that there is only one instance of the object
Input: none
Output: pointer of the only instance
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
Destructor
*/
RequestHandlerFactory::~RequestHandlerFactory() {}

/*
Function creates new Login request handler
Input: none
Output: Login request handler
*/
LoginRequestHandler* RequestHandlerFactory::createLoginRequestHandler()
{
	return new LoginRequestHandler();
}

/*
Function creates new Menu request handler
Input: none
Output: Menu request handler
*/
MenuRequestHandler* RequestHandlerFactory::createMenuRequestHandler(std::string username)
{
	return new MenuRequestHandler(username);
}

/*
Function creates new Room Admin handler
Input: logged user, pointer to the room
Output: Room Admin handler
*/
RoomAdminRequestHandler* RequestHandlerFactory::createRoomAdminRequestHandler(LoggedUser user, Room* room)
{
	return new RoomAdminRequestHandler(user, room);
}

/*
Function creates new Room Member handler
Input: logged user, pointer to the room
Output: Room Member handler
*/
RoomMemberRequestHandler* RequestHandlerFactory::createRoomMemberRequestHandler(LoggedUser user, Room* room)
{
	return new RoomMemberRequestHandler(user, room);
}

/*
Function creates new Game handler
Input: logged user, pointer to the room
Output: Game handler
*/
GameRequestHandler* RequestHandlerFactory::createGameRequestHandler(LoggedUser user, Room* room)
{
	return new GameRequestHandler(user, room);
}

/*
Function returns the Login Manager
Input: none
Output: Login Manager
*/
LoginManager& RequestHandlerFactory::getLoginManager()
{
	return *m_loginManager;
}

/*
Function returns the Room Manager
Input: none
Output: Room Manager
*/
RoomManager& RequestHandlerFactory::getRoomManager()
{
	return *m_roomManager;
}

/*
Function returns the Statistics Manager
Input: none
Output: Statistics Manager
*/
StatisticsManager& RequestHandlerFactory::getStatisticsManager()
{
	return *m_statisticsManager;
}

/*
Function returns the Game Manager
Input: none
Output: Game Manager
*/
GameManager& RequestHandlerFactory::getGameManager()
{
	return *m_gameManager;
}
