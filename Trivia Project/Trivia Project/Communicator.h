#pragma once
#pragma comment(lib, "ws2_32.lib")

#include "JsonResponsePacketSerializer.h"
#include "IRequestHandler.h"
#include "RequestHandlerFactory.h"
#include "WSAInitializer.h"

#include <map>
#include <WinSock2.h>
#include <thread>
#include <fstream>
#include <string>
#include <Windows.h>

#define CONFIG_PATH "..\\config.txt"
#define HELLO_MSG "Hello"
#define FIRST_MSG_LEN 5

class Communicator
{
public:
	Communicator();
	~Communicator();
	SOCKET bindAndListen();
	void startHandleRequests();

private:
	std::map<SOCKET, IRequestHandler*> m_clients;
	RequestHandlerFactory m_handlerFactory;
	
	void handleNewClient(SOCKET client_socket);
	void send_data(SOCKET, std::string);
	std::string recv_data(SOCKET, int);
};