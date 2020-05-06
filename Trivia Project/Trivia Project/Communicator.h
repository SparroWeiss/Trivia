#pragma once
#pragma comment(lib, "ws2_32.lib")
#pragma warning(disable : 4996) // allowing use of ctime() function

#include <mutex>
#include "JsonResponsePacketSerializer.h"
#include "JsonRequestPacketDeserializer.h"
#include "IRequestHandler.h"
#include "RequestHandlerFactory.h"
#include "WSAInitializer.h"

#include <map>
#include <WinSock2.h>
#include <thread>
#include <fstream>
#include <string>
#include <Windows.h>

class JsonResponsePacketSerializer;
class JsonRequestPacketDeserializer;

#define CONFIG_PATH "..\\config.txt"
#define HELLO_MSG "Hello"
#define FIRST_MSG_LEN 5

#define CODE_SIZE 1
#define LENGTH_SIZE 4

enum messageCode
{
	ERRORCODE = 0,
	SIGNUPCODE,
	LOGINCODE
};

class Communicator
{
public:
	static Communicator* getInstance();
	~Communicator();
	SOCKET bindAndListen();
	void startHandleRequests();

private:
	Communicator();
	static Communicator* instance;//singleton

	std::map<SOCKET, IRequestHandler*> m_clients;
	RequestHandlerFactory* m_handlerFactory;
	
	void handleNewClient(SOCKET client_socket);
	void send_data(SOCKET, std::string);
	Buffer recv_data(SOCKET, int);
	RequestInfo getRequest(SOCKET);
};