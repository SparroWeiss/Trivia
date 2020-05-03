#include "LoginRequestHandler.h"
#include "Communicator.h"

LoginRequestHandler::LoginRequestHandler()
{
}

LoginRequestHandler::~LoginRequestHandler()
{
}

bool LoginRequestHandler::isRequestRelevent(RequestInfo info)
{
	return (info.id == LOGINCODE || info.id == SIGNUPCODE);
}

RequestResult LoginRequestHandler::handleRequest(RequestInfo info)
{
	if (info.id == LOGINCODE)
	{
		LoginRequest loginReq = JsonRequestPacketDeserializer::deserializeLoginRequest(info.buffer);
		// Missing at this stage: check if req is valid.
		LoginResponse loginRes = { 1 };
		
		return RequestResult{ JsonResponsePacketSerializer::serializeResponse(loginRes), NULL }; // In this version there is no menu handler
	}

	else if (info.id == SIGNUPCODE)
	{
		SignupRequest signReq = JsonRequestPacketDeserializer::deserializeSignupRequest(info.buffer);
		// Missing at this stage: check if req is valid.
		SignupResponse signRes = { 1 };

		return RequestResult{ JsonResponsePacketSerializer::serializeResponse(signRes), NULL }; // In this version there is no menu handler
	}
}

RequestResult LoginRequestHandler::login(RequestInfo info)
{
	return RequestResult();
}

RequestResult LoginRequestHandler::signup(RequestInfo info)
{
	return RequestResult();
}
