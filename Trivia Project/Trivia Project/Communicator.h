#pragma once
#include "JsonResponsePacketSerializer.h"
#include "IRequestHandler.h"
#include "RequestHandlerFactory.h"
#include <map>
#include <WinSock2.h>

class Communicator
{
public:
	Communicator();
	~Communicator();
	void bindAndListen();
	void startHandleRequests();

private:
	std::map<SOCKET, IRequestHandler*> m_clients;
	RequestHandlerFactory m_handlerFactory;
	void handleNewClient();
};