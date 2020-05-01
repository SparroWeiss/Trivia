#pragma once
#include "Buffer.h"

struct 
{
	std::string message;
}typedef ErrorResponse;

struct
{
	unsigned int status;
}typedef LoginResponse;

struct
{
	unsigned int status;
}typedef SignupResponse;

class JsonResponsePacketSerializer
{
public:
	static Buffer serializeResponse(ErrorResponse error);
	static Buffer serializeResponse(LoginResponse login);
	static Buffer serializeResponse(SignupResponse signup);
};

