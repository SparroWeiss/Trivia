#include "Server.h"
#include "SqliteDatabase.h"

/*
constructor
sets the database, the request handler factory and the communicator
*/
Server::Server()
{
	m_database = new SqliteDatabase();
	m_RequestHandlerFactory = RequestHandlerFactory(m_database);
	m_communicator = Communicator(m_RequestHandlerFactory);
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
