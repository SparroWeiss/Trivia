#include "Communicator.h"

Communicator::Communicator()
{
}

Communicator::~Communicator()
{
}

/*
this function create the listening socket,
connects between the socket and the configuration and return it
input: none
output: the listening socket
*/
SOCKET Communicator::bindAndListen()
{
	int port = 0;
	SOCKET listening_socket;
	struct sockaddr_in sa = { 0 };

	std::ifstream config;
	std::string line = "";

	//get listening port from config file
	config.open(CONFIG_PATH);
	while (getline(config, line))
	{
		if (line.find("port=") != std::string::npos)
		{
			port = atoi(line.substr(5).c_str());
			break;
		}
	}
	config.close();

	listening_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (listening_socket == INVALID_SOCKET)
	{
		std::cout << __FUNCTION__ " - error with creating socket" << std::endl;
		exit(-1);
	}

	sa.sin_port = htons(port);
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

	while (true)
	{
		std::cout << "server is listening" << std::endl;
		std::cout << "Waiting for client connection request" << std::endl;
		
		client_socket = ::accept(listening_socket, NULL, NULL);
		if (client_socket == INVALID_SOCKET)
		{
			std::cout << __FUNCTION__ " - error with accept client";
			continue;
		}
	
		std::cout << "Client accepted. Server and client can speak" << std::endl;
		
		m_clients.insert({ client_socket, &(m_handlerFactory.createLoginRequestHandler()) });
		std::thread(&Communicator::handleNewClient, this, client_socket).detach();
	}
}

/*
this function "talk" with the client
input: the client socket
output: none
*/
void Communicator::handleNewClient(SOCKET client_socket)
{
	while (true)
	{
		try
		{
			RequestInfo currRequest = getRequest(client_socket);


			if (m_clients[client_socket] && m_clients[client_socket]->isRequestRelevent(currRequest))
			{
				RequestResult currResult = m_clients[client_socket]->handleRequest(currRequest);
				// send result to client --> next stage
				m_clients[client_socket] = currResult.newHandler; // in this version: NULL
			}
		}
		catch (const std::exception & e)
		{
			std::cout << e.what() << std::endl;
			m_clients.erase(client_socket);
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
std::string Communicator::recv_data(SOCKET sock, int bytes_num)
{
	if (bytes_num == 0)
	{
		return (char*)"";
	}

	char* data = new char[bytes_num + 1];
	if (recv(sock, data, bytes_num, 0) == INVALID_SOCKET)
	{
		std::string s = "Error while recieving from socket: ";
		s += std::to_string(sock);
		throw std::exception(s.c_str());
	}

	data[bytes_num] = 0;
	return data;
}

/*
This method create a RequestInfo struct that contains details about a client's request.
Input: socket of client
Output: RequestInfo struct
*/
RequestInfo Communicator::getRequest(SOCKET client_socket)
{
	unsigned int requestCode = (unsigned int)stoi(recv_data(client_socket, CODE_SIZE));

	int requestSize = JsonRequestPacketDeserializer::bytesToInt(
		JsonResponsePacketSerializer::stringToBytes(recv_data(client_socket, LENGTH_SIZE)));

	Buffer messageData = JsonResponsePacketSerializer::stringToBytes(
		recv_data(client_socket, requestSize));

	time_t now = time(0);

	return RequestInfo{ requestCode, ctime(&now), messageData};
}
