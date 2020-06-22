#pragma once
#include "IDataBase.h"
#include "Communicator.h"
#include "RequestHandlerFactory.h"
#include <tlhelp32.h>
#include <comdef.h>

#define EXIT "EXIT"
#define NGROK_PATH "python \"scripts\\ngrok.py\""

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
	PROCESS_INFORMATION _ngrokProcessInfo;
};
