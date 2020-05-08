#include "Server.h"
#include "SqliteDatabase.h"

// initializing singleton class's instanses counters
Server* Server::instance = 0;
Communicator* Communicator::instance = 0;
LoginManager* LoginManager::instance = 0;
RequestHandlerFactory* RequestHandlerFactory::instance = 0;
SqliteDatabase* SqliteDatabase::instance = 0;
RoomManager* RoomManager::instance = 0;

int Server::instances = 0;
int Communicator::instances = 0;
int LoginManager::instances = 0;
int RequestHandlerFactory::instances = 0;
int SqliteDatabase::instances = 0;
int RoomManager::instances = 0;

int main()
{
	Server* server = server->getInstence();

	try
	{
		WSAInitializer wsaInit;
		server->run();
	}
	catch (const std::exception& e)
	{
		std::cout << e.what() << std::endl;
		exit(-1);
	}

	return 0;
}