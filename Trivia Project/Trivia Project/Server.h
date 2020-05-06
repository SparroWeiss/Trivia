#pragma once

#include "IDataBase.h"
#include "Communicator.h"
#include "RequestHandlerFactory.h"

#define EXIT "EXIT"

class Server
{
public:
	static Server* getInstence();
	~Server();
	void run();

private:
	Server();
	static Server* instance;
	IDataBase* m_database;
	Communicator* m_communicator;
	RequestHandlerFactory* m_RequestHandlerFactory;
};
