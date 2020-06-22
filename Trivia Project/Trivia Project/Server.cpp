#include "Server.h"
#include "SqliteDatabase.h"

/*
Constructor:
Initializes the variables of the object
*/
Server::Server()
{
	m_database = SqliteDatabase::getInstance();
	m_RequestHandlerFactory = m_RequestHandlerFactory->getInstance();
	m_communicator = m_communicator->getInstance();
	
	STARTUPINFOA info = { sizeof(info) };
	std::string scriptCommandLine = NGROK_PATH;

	if (!CreateProcessA(NULL, LPSTR(scriptCommandLine.c_str()), NULL, NULL, FALSE, NULL, NULL, NULL, &info, &_ngrokProcessInfo))
	{
		std::cout << "Faild to open tunnel with ngrok. Error " << GetLastError() << std::endl; // should not happen
	}
}

/*
Function make sure that there is only one instance of the object
Input: none
Output: pointer of the only instance
*/
Server* Server::getInstence()
{
	if (instance == 0)
	{
		instance = new Server();
	}
	return instance;
}

/*
Destructor:
Terminate ngrok.exe process
*/
Server::~Server()
{
	PROCESSENTRY32 entry;
	entry.dwSize = sizeof(PROCESSENTRY32);

	HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

	if (Process32First(snapshot, &entry) == TRUE)
	{
		while (Process32Next(snapshot, &entry) == TRUE)
		{
			_bstr_t processName(entry.szExeFile);
			if (strnicmp((const char*)processName, "ngrok.exe", 9) == 0)
			{
				HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, entry.th32ProcessID);
				TerminateProcess(hProcess, 0);
				CloseHandle(hProcess);
			}
		}
	}
	CloseHandle(snapshot);
	CloseHandle(_ngrokProcessInfo.hProcess);
	CloseHandle(_ngrokProcessInfo.hThread);
}

/*
function creates the main thread of the program and detach a thread that waits for clients
input: none
output: none
*/
void Server::run()
{
	std::cout << std::endl << std::endl << std::endl << std::endl << std::endl << std::endl << std::endl;
	std::string input = "";
	std::thread(&Communicator::startHandleRequests, m_communicator).detach();
	std::cout << "thread startHandleRequests has created and detached." << std::endl;

	while (true)
	{
		std::cin >> input;
		if (input == EXIT)
		{
			std::cout << "goodbye!" << std::endl;
			return;
		}
	}
}
