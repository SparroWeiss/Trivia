#pragma once
#include <iostream>

class IDataBase
{
public:
	virtual bool doesUserExist(std::string) = 0;
	virtual bool doesPasswordMatch(std::string) = 0;
	virtual bool addNewUser(std::string, std::string, std::string) = 0;
};
