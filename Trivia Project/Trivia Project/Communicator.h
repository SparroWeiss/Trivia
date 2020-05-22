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

#define CODE_SIZE 1
#define LENGTH_SIZE 4
#define SPACE ' '
#define LISTENING_PORT 8998

class RequestHandlerFactory;

enum messageCode
{
	ERRORCODE = 0,
	SIGNUPCODE,
	LOGINCODE,
	SIGNOUT,
	GETROOMS,
	GETPLAYERSINROOM,
	GETSTATISTICS,
	JOINROOM,
	CREATEROOM,
	CLOSEROOM,
	STARTGAME,
	GETROOMSTATE,
	LEAVEROOM,
	GETGAMERESULTS,
	SUBMITANSWER,
	GETQUESTION,
	LEAVEGAME
};

class Communicator
{
public:
	static Communicator* getInstance();
	~Communicator();
	SOCKET bindAndListen();
	void startHandleRequests();
	std::map<SOCKET, IRequestHandler*> getClients();

private:
	Communicator();
	static Communicator* instance;//singleton
	static int instances;

	std::map<SOCKET, IRequestHandler*> m_clients;
	RequestHandlerFactory* m_handlerFactory;
	
	void handleNewClient(SOCKET client_socket);
	void send_data(SOCKET, std::string);
	Buffer recv_data(SOCKET, int);
	RequestInfo getRequest(SOCKET);
};