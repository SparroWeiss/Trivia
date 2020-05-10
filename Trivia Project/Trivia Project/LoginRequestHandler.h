#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"

#include <regex>

class RequestHandlerFactory;

#define PASSWORD_REGEX std::regex(R"(^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8}$)")
#define EMAIL_REGEX std::regex(R"(^[a-zA-Z0-9]+@[a-zA-Z]+\.[a-zA-Z]{2,}$)")
#define ADDRESS_REGEX std::regex(R"(^[a-zA-Z]+,\ \d+,\ [a-zA-Z\ ]+$)")
#define PHONE_REGEX std::regex(R"(^((0[2-48-9])|(05\d))-\d{7}$)")
#define BIRTHDATE_REGEX std::regex(R"(^(((3[0-3]|[0-2]\d)\/(0\d|1[0-2])\/(\d{4}))|((3[0-3]|[0-2]\d)\.(0\d|1[0-2])\.(\d{4})))$)")

class LoginRequestHandler : public IRequestHandler
{
public:
	LoginRequestHandler();
	~LoginRequestHandler();
	bool isRequestRelevent(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	LoginManager* m_loginManager;
	RequestHandlerFactory* m_handlerFactory;

	RequestResult login(RequestInfo info);
	RequestResult signup(RequestInfo info);

	bool signupValidation(std::string, std::string, std::string, std::string, std::string);
};