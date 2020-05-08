#pragma once
#include <iostream>

class IDataBase
{
public:
	virtual bool doesUserExist(std::string name) = 0;
	virtual bool doesPasswordMatch(std::string name, std::string password) = 0;
	virtual bool addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate) = 0;
};
