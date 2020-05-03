#pragma once
#include "Buffer.h"
#include "Communicator.h"
#include "json.hpp"

using json = nlohmann::json;

class Communicator;

struct 
{
	std::string username;
	std::string password;
}typedef LoginRequest;

struct
{
	std::string username;
	std::string password;
	std::string email;
}typedef SignupRequest;

class JsonRequestPacketDeserializer
{
public:
	static LoginRequest deserializeLoginRequest(Buffer buff);
	static SignupRequest deserializeSignupRequest(Buffer buff);

	static unsigned int bytesToInt(Buffer);
	static std::string bytesToString(Buffer);
};

void to_json(json&, const LoginRequest&);
void from_json(const json&, LoginRequest&);

void to_json(json&, const SignupRequest&);
void from_json(const json&, SignupRequest&);