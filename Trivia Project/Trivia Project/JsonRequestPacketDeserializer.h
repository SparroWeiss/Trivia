#pragma once
#include "Buffer.h"

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
	static SignupRequest deserializeRequest(Buffer buff);
};
