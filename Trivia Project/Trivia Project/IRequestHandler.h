#pragma once
#include <ctime>
#include "Buffer.h"


class IRequestHandler;

struct
{
	unsigned int id;// Request Code
	std::string receivalTime;// ctime = std::string
	Buffer buffer;
}typedef RequestInfo;

struct
{
	Buffer response;
	IRequestHandler* newHandler = nullptr;
}typedef RequestResult;

class IRequestHandler
{
public:
	virtual bool isRequestRelevant(RequestInfo) = 0;
	virtual RequestResult handleRequest(RequestInfo) = 0;
};

