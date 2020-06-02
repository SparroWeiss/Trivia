#include "LoginRequestHandler.h"
#include "Communicator.h"
#include "MenuRequestHandler.h"

/*
constructor
initializes the variables of the object
*/
LoginRequestHandler::LoginRequestHandler()
{
	m_handlerFactory = m_handlerFactory->getInstance();
	m_loginManager = m_loginManager->getInstance();
}

/*
destructor
frees allocated memory 
*/
LoginRequestHandler::~LoginRequestHandler()
{
	
}

/*
function checks if a request is relevant to the handler
input: request info
output: true - request is relevant, false - request isn't relevant
*/
bool LoginRequestHandler::isRequestRelevant(RequestInfo info)
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
	LoginResponse loginRes = { m_loginManager->login(loginReq.username, loginReq.password) }; 
	if (loginRes.status == loginStatus::SUCCESS)
	{ // if the login request is valid
		newHandle = m_handlerFactory->createMenuRequestHandler(loginReq.username); // pointer to the next handle : menu
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
	signupStatus status = signupValidation(signReq.password, signReq.email, signReq.address, signReq.phone, signReq.birthdate);
	if (status == signupStatus::SIGNUP_SUCCESS) // if parameters are valid
	{
		if (m_loginManager->signup(signReq.username, signReq.password, signReq.email, signReq.address, signReq.phone, signReq.birthdate))
		{ // if the login request is valid
			signRes = { signupStatus::SIGNUP_SUCCESS }; // status: 1
			newHandle = m_handlerFactory->createMenuRequestHandler(signReq.username); // pointer to the next handle : menu
		}
		else
		{
			signRes = { signupStatus::SOMETHING_WENT_WRONG }; // status: 0
		}
	}
	else
	{
		signRes = { (unsigned int)status }; // status: 0
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(signRes), newHandle };
}

/*
This function checks a signup req parameters are valid 
Input: parameters to check
Output: bool
*/
signupStatus LoginRequestHandler::signupValidation(std::string password, std::string email, std::string address, std::string phone, std::string birthdate)
{
	if (!std::regex_match(password, PASSWORD_REGEX)) // Password must be 8 chars, and contain at least one:
	{												 // uppercase letter, lowercase letter, digit, special character
		return signupStatus::INVALID_PASSWORD;
	}
	
	if (!std::regex_match(email, EMAIL_REGEX)) // Email must be in the format:
	{										   // <email-prefix>@<domain>
		return signupStatus::INVALID_EMAIL;
	}
	
	if (!std::regex_match(address, ADDRESS_REGEX)) // Address must be in format:
	{											   // <street>, <apartment-number>, <city>
		return signupStatus::INVALID_ADDRESS;
	}
	
	if (!std::regex_match(phone, PHONE_REGEX)) // Phone must be in format:
	{										   // <phone-prefix>-<phone-number(7 digits)>
		return signupStatus::INVALID_PHONE;
	}
	
	if (!std::regex_match(birthdate, BIRTHDATE_REGEX)) // Birthdate must be in format:
	{												   // DD.MM.YYYY or DD/MM/YYYY
		return signupStatus::INVALID_BIRTHDATE;
	}

	return signupStatus::SIGNUP_SUCCESS;
}
