#include "Server.h"

Server::Server()
{
}

Server::~Server()
{
}

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
