#include "Server.h"

int main()
{
	Server server;

	try
	{
		WSAInitializer wsaInit;
		server.run();
	}
	catch (const std::exception& e)
	{
		std::cout << e.what() << std::endl;
		exit(-1);
	}

	return 0;
}