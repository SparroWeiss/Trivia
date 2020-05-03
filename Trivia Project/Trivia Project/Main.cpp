#include "Server.h"

int main()
{
	/*
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
	*/

	/*
	Buffer b[3];


	ErrorResponse e = { "aaa" };
	LoginResponse l = { 1 };
	SignupResponse s = { 1 };

	b[0] = JsonResponsePacketSerializer::serializeResponse(e);
	b[1] = JsonResponsePacketSerializer::serializeResponse(l);
	b[2] = JsonResponsePacketSerializer::serializeResponse(s);

	for (int i = 0; i < 3; i++)
	{
		for (int j = 0; j < b[i].m_buffer.size(); j++)
		{
			if (j < 5)
			{
				std::cout << " " << (int)b[i].m_buffer[j];
			}
			else
			{
				std::cout << " " << b[i].m_buffer[j];
			}
		}
		std::cout << std::endl;
	}
	*/

	return 0;
}