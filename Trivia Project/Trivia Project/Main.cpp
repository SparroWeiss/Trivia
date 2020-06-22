#include "Server.h"
#include "SqliteDatabase.h"

// initializing singleton class's instanses counters
Server* Server::instance = 0;
Communicator* Communicator::instance = 0;
LoginManager* LoginManager::instance = 0;
RequestHandlerFactory* RequestHandlerFactory::instance = 0;
SqliteDatabase* SqliteDatabase::instance = 0;
RoomManager* RoomManager::instance = 0;
StatisticsManager* StatisticsManager::instance = 0;
GameManager* GameManager::instance = 0;

unsigned int Question::curr_id = 0;

int main()
{
	Server* server = Server::getInstence();

	try
	{
		WSAInitializer wsaInit;
		server->run();
	}
	catch (const std::exception& e)
	{
		std::cout << e.what() << std::endl;
	}
	Sleep(1000);
	delete Communicator::getInstance();
	delete Server::getInstence();
	delete SqliteDatabase::getInstance();
	delete LoginManager::getInstance();
	delete GameManager::getInstance();
	delete RoomManager::getInstance();
	delete RequestHandlerFactory::getInstance();
	delete StatisticsManager::getInstance();

	return 0;
}