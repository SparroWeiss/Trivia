#pragma once
#include <ctime>
#include "Buffer.h"

class IRequestHandler;

struct
{
	unsigned int id;// RequestId?
	time_t receivalTime;// ctime?
	Buffer buffer;
}typedef RequestInfo;

struct
{
	Buffer response;
	IRequestHandler* newHandler;
}typedef RequestResult;

class IRequestHandler
{
public:
	virtual bool isRequestRelevent(RequestInfo) = 0;
	virtual RequestResult handleRequest(RequestInfo) = 0;
};

