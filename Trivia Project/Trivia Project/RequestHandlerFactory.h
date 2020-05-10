#pragma once
#include "LoginManager.h"
#include "LoginRequestHandler.h"
#include "MenuRequestHandler.h"
#include "RoomManager.h"
#include "StatisticsManager.h"

class LoginRequestHandler;
class MenuRequestHandler;

class RequestHandlerFactory
{
public:
	static RequestHandlerFactory* getInstance();
	~RequestHandlerFactory();
	LoginRequestHandler* createLoginRequestHandler();
	MenuRequestHandler* createMenuRequestHandler();
	LoginManager& getLoginManager();
	RoomManager& getRoomManager();
	StatisticsManager& getStatisticsManager();

private:
	RequestHandlerFactory();
	static RequestHandlerFactory* instance;
	static int instances;

	LoginManager m_loginManager;
	IDataBase* m_database;
	RoomManager m_roomManager;
	StatisticsManager m_statisticsManager;
};
