#pragma once
#include "LoginManager.h"
#include "LoginRequestHandler.h"
#include "MenuRequestHandler.h"
#include "RoomManager.h"
#include "StatisticsManager.h"
#include "RoomAdminRequestHandler.h"
#include "RoomMemberRequestHandler.h"
#include "GameRequestHandler.h"

class LoginRequestHandler;
class MenuRequestHandler;
class RoomAdminRequestHandler;
class RoomMemberRequestHandler;
class RoomManager;
class GameRequestHandler;
class GameManager;

class RequestHandlerFactory
{
public:
	static RequestHandlerFactory* getInstance();
	~RequestHandlerFactory();
	LoginRequestHandler* createLoginRequestHandler();
	MenuRequestHandler* createMenuRequestHandler(std::string username);
	RoomAdminRequestHandler* createRoomAdminRequestHandler(LoggedUser user, Room* room);
	RoomMemberRequestHandler* createRoomMemberRequestHandler(LoggedUser user, Room* room);
	GameRequestHandler* createGameRequestHandler(LoggedUser user, Room* room);

	LoginManager& getLoginManager();
	RoomManager& getRoomManager();
	StatisticsManager& getStatisticsManager();
	GameManager& getGameManager();

private:
	RequestHandlerFactory();
	static RequestHandlerFactory* instance;
	static int instances;

	LoginManager* m_loginManager;
	IDataBase* m_database;
	RoomManager* m_roomManager;
	StatisticsManager* m_statisticsManager;
	GameManager* m_gameManager;
};
