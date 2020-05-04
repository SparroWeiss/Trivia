#pragma once
#include "Buffer.h"
#include "Communicator.h"
#include "json.hpp"

using json = nlohmann::json;

class Comunicator;

#define BITSOFBYTE 8

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

	static Buffer intToBytes(int);
	static Buffer stringToBytes(std::string);
	static Buffer createResponseBuf(unsigned char, Buffer, Buffer);
	
	static Buffer charToBytes(char*, unsigned int);
};

void to_json(json&, const ErrorResponse&);
void from_json(const json&, ErrorResponse&);

void to_json(json&, const LoginResponse&);
void from_json(const json&, LoginResponse&);

void to_json(json&, const SignupResponse&);
void from_json(const json&, SignupResponse&);