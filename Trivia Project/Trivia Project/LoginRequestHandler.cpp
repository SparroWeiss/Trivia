#include "LoginRequestHandler.h"
#include "Communicator.h"
#include "MenuRequestHandler.h"

/*
constructor
sets the handler factory and the login manager
*/
LoginRequestHandler::LoginRequestHandler()
{
	m_handlerFactory = m_handlerFactory->getInstance();
	m_loginManager = m_handlerFactory->getLoginManager();
}

LoginRequestHandler::~LoginRequestHandler()
{
}

/*
function checks if a request is relevent to the handler
input: request info
output: true - request is relevent, false - request isn't relevent
*/
bool LoginRequestHandler::isRequestRelevent(RequestInfo info)
{
	return (info.id == LOGINCODE || info.id == SIGNUPCODE);
}

/*
function checks if the request is login or sign up
input: request info
output: request result
*/
RequestResult LoginRequestHandler::handleRequest(RequestInfo info)
{
	if (info.id == LOGINCODE)
	{
		return login(info);
	}
	return signup(info); // the only two options available are signin or signup
}

/*
function sends the login request to the manager
input: request info
output: request result
*/
RequestResult LoginRequestHandler::login(RequestInfo info)
{
	LoginRequest loginReq = JsonRequestPacketDeserializer::deserializeLoginRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the login request isn't valid, stay in same handler
	LoginResponse loginRes = { 0 }; // status: 0
	if (m_loginManager->login(loginReq.username, loginReq.password))
	{ // if the login request is valid
		loginRes = { 1 }; // status: 1
		newHandle = new MenuRequestHandler(); // pointer to the next handle : menu
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(loginRes), newHandle };
}

/*
function sends the signup request to the manager
input: request info
output: request result
*/
RequestResult LoginRequestHandler::signup(RequestInfo info)
{
	SignupRequest signReq = JsonRequestPacketDeserializer::deserializeSignupRequest(info.buffer);
	IRequestHandler* newHandle = this; // if the login request isn't valid, stay in same handler
	SignupResponse signRes = { 0 }; // status: 0
	if (m_loginManager->signup(signReq.username, signReq.password, signReq.email))
	{ // if the login request is valid
		signRes = { 1 }; // status: 1
		newHandle = new MenuRequestHandler(); // pointer to the next handle : menu
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(signRes), newHandle };
}
