#include "Server.h"
#include "SqliteDatabase.h"

/*
constructor
sets the database, the request handler factory and the communicator
*/
Server::Server()
{
	m_database = SqliteDatabase::getInstance();
	m_RequestHandlerFactory = m_RequestHandlerFactory->getInstance();
	m_communicator = m_communicator->getInstance();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
Server* Server::getInstence()
{
	if (instance == 0)
	{
		instance = new Server();
	}

	return instance;
}

Server::~Server()
{
}

/*
function creates the main thread of the program and detach a thread that waits for clients
input: none
output: none
*/
void Server::run()
{
	std::string input = "";
	std::thread(&Communicator::startHandleRequests, m_communicator).detach();
	std::cout << "thread startHandleRequests has created and detached." << std::endl;

	while (true)
	{
		std::cin >> input;
		if (input == EXIT)
		{
			std::cout << "goodbye!" << std::endl;
			exit(0);
		}
	}
}
