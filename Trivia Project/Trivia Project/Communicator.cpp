#include "Communicator.h"
#include "MenuRequestHandler.h"

std::mutex _using_clients;

/*
constructor
initializes the variables of the object
*/
Communicator::Communicator()
{
	m_clients = std::map<SOCKET, IRequestHandler*>();
	m_handlerFactory = m_handlerFactory->getInstance();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
Communicator* Communicator::getInstance()
{
	if (instance == 0)
	{
		instance = new Communicator();
	}
	instances++;
	return instance;
}

/*
destructor
frees allocated memory
*/
Communicator::~Communicator()
{
	instances--;
	if (instances == 0)
	{
		for (std::map<SOCKET, IRequestHandler*>::iterator i = m_clients.begin(); i != m_clients.end(); ++i)
		{
			closesocket((*i).first);
			delete (*i).second;
		}
		delete instance;
	}
}

/*
this function create the listening socket,
connects between the socket and the configuration and return it
input: none
output: the listening socket
*/
SOCKET Communicator::bindAndListen()
{
	SOCKET listening_socket;
	struct sockaddr_in sa = { 0 };

	listening_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (listening_socket == INVALID_SOCKET)
	{
		std::cout << __FUNCTION__ " - error with creating socket" << std::endl;
		exit(-1);
	}

	sa.sin_port = htons(LISTENING_PORT);
	sa.sin_family = AF_INET;
	sa.sin_addr.s_addr = INADDR_ANY;

	if (bind(listening_socket, (struct sockaddr*) & sa, sizeof(sa)) == SOCKET_ERROR)
	{
		std::cout << __FUNCTION__ " - error with bind socket" << std::endl;
		exit(-1);
	}

	if (listen(listening_socket, SOMAXCONN) == SOCKET_ERROR)
	{
		std::cout << __FUNCTION__ " - error with listening" << std::endl;
		exit(-1);
	}

	std::cout << "listening socket has created." << std::endl;
	return listening_socket;
}

/*
this function listen, accept client and create new thread for each client
input: none
output: none
*/
void Communicator::startHandleRequests()
{
	SOCKET client_socket, listening_socket = bindAndListen();

	std::cout << "server is listening" << std::endl;
	std::cout << "Waiting for client connection request" << std::endl;

	while (true)
	{
		
		
		client_socket = ::accept(listening_socket, NULL, NULL);
		if (client_socket == INVALID_SOCKET)
		{
			std::cout << __FUNCTION__ " - error with accept client";
			continue;
		}
	
		//std::cout << "Client accepted. Server and client can speak" << std::endl;
		
		std::unique_lock<std::mutex> locker(_using_clients);
		m_clients.insert({ client_socket, m_handlerFactory->createLoginRequestHandler() });
		locker.unlock();
		std::thread(&Communicator::handleNewClient, this, client_socket).detach();
	}
}

/*
function will help the room admin
input: none
output: map of the users
*/
std::map<SOCKET, IRequestHandler*> Communicator::getClients()
{
	return m_clients;
}

/*
this function "talk" with the client
input: the client socket
output: none
*/
void Communicator::handleNewClient(SOCKET client_socket)
{
	std::string name = "%unknown%";
	bool loggedIn = false;
	while (true)
	{
		try
		{
			RequestInfo currRequest = getRequest(client_socket);
	
			std::unique_lock<std::mutex> locker(_using_clients);
			if (m_clients[client_socket] && m_clients[client_socket]->isRequestRelevant(currRequest))
			{
				RequestResult currResult = m_clients[client_socket]->handleRequest(currRequest); // deserialize request
				std::cout << JsonRequestPacketDeserializer::bytesToString(currResult.response) << std::endl;
				send_data(client_socket, JsonRequestPacketDeserializer::bytesToString(currResult.response)); // send serialized response to client
				if (m_clients[client_socket] != currResult.newHandler)
				{ // if the handler has changed
					delete m_clients[client_socket];
					m_clients[client_socket] = currResult.newHandler; // updating client state
				}
				locker.unlock();
				if (!loggedIn) // not enter to this block more than once
				{
					try
					{
						json j = json::parse(JsonRequestPacketDeserializer::bytesToString(currResult.response).substr(5));
						LoginResponse loginRes = j.get<LoginResponse>(); // extracting the login response from the result of the user's action 
						if (loginRes.status == 1) // the user logged in into the server
						{
							name = JsonRequestPacketDeserializer::deserializeLoginRequest(currRequest.buffer).username;
							std::cout << "### "<< name << " joined" << std::endl; // rememberring the name for the thread
							loggedIn = true; // there is no need to enter this block again, we got what we need
						}
					}
					catch (const std::exception& e)
					{
						std::cout << e.what() << std::endl;
					}
					
				}
				if (m_clients[client_socket] == nullptr)
				{ // if the user logged out
					std::cout << "### " << name << " logged out" << std::endl;
					std::lock_guard<std::mutex> locker(_using_clients);
					m_clients.erase(client_socket);
					closesocket(client_socket);
					return;
				}
			}
			else
			{
				locker.unlock();
				send_data(client_socket, JsonRequestPacketDeserializer::bytesToString(
					JsonResponsePacketSerializer::serializeResponse(
						ErrorResponse{ "Invalid request per state" })));
			}
		}
		catch (const std::exception & e)
		{
			if (m_clients[client_socket]->isRequestRelevant(RequestInfo{ CLOSEROOM, "", Buffer() }))
			{
				m_clients[client_socket]->handleRequest(RequestInfo{ CLOSEROOM, "", Buffer() });
			}
			else if (m_clients[client_socket]->isRequestRelevant(RequestInfo{ LEAVEROOM, "", Buffer() }))
			{
				m_clients[client_socket]->handleRequest(RequestInfo{ LEAVEROOM, "", Buffer() });
			}
			else if (m_clients[client_socket]->isRequestRelevant(RequestInfo{ LEAVEGAME, "", Buffer() }))
			{
				m_clients[client_socket]->handleRequest(RequestInfo{ LEAVEGAME, "", Buffer() });
			}
			std::cout << "Error with socket: " << client_socket << ". client " << name << " disconnected." << std::endl;
			delete m_clients[client_socket];
			std::unique_lock<std::mutex> locker(_using_clients);
			m_clients.erase(client_socket);
			locker.unlock();
			m_handlerFactory->getLoginManager().logout(name);
			closesocket(client_socket); 
			return;
		}
	}
}

/*
this function send a message to the given socket
input: the socket and the message
output: none
*/
void Communicator::send_data(SOCKET sock, std::string msg)
{
	const char* data = msg.c_str();

	if (send(sock, data, msg.size(), 0) == INVALID_SOCKET)
	{
		throw std::exception("Error while sending message to client");
	}
}

/*
this function receive a message from the given socket
input: the socket and the lenght of the message
output: the message
*/
Buffer Communicator::recv_data(SOCKET sock, int bytes_num)
{
	if (bytes_num == 0)
	{
		return Buffer();
	}
	char temp = SPACE;
	Buffer bytesOfData;
	for (int i = 0; i < bytes_num; i++)
	{
		if (recv(sock, &temp, 1, 0) == 0)
		{
			std::string s = "Error while recieving from socket: ";
			s += std::to_string(sock);
			throw std::exception(s.c_str());
		}
		bytesOfData.m_buffer.push_back(temp);
	}
	return bytesOfData;
}

/*
This method create a RequestInfo struct that contains details about a client's request.
Input: socket of client
Output: RequestInfo struct
*/
RequestInfo Communicator::getRequest(SOCKET client_socket)
{
	unsigned int requestCode = recv_data(client_socket, CODE_SIZE).m_buffer[0];
	
	if (requestCode == SPACE)
	{ // the user disconnected
		throw std::exception("disconnected in the middle of the conversation");
	}

	int requestSize = JsonRequestPacketDeserializer::bytesToInt(
		recv_data(client_socket, LENGTH_SIZE));

	Buffer messageData = recv_data(client_socket, requestSize);

	time_t now = time(0);

	return RequestInfo{ requestCode, ctime(&now), messageData};
}
