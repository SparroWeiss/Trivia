#pragma once
#include "IDataBase.h"
#include "Communicator.h"
#include "RequestHandlerFactory.h"

class Server
{
public:
	Server();
	~Server();
	void run();

private:
	IDataBase* m_database;
	Communicator m_communicator;
	RequestHandlerFactory m_RequestHandlerFactory;
};