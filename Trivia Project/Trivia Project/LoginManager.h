#pragma once
#include "IDataBase.h"
#include <vector>

class LoggedUser
{
public:
	/*
	constructor
	*/
	LoggedUser(std::string name)
	{
		m_username = name;
	}
	~LoggedUser(){}
	/*
	function gets the username
	input: none
	output: the username
	*/
	std::string getUsername() const
	{
		return std::string(m_username);
	}
private:
	std::string m_username;
};

class LoginManager
{
public:
	LoginManager();
	~LoginManager();
	void signup(std::string, std::string, std::string);
	void login(std::string, std::string);
	void logout(std::string);

private:
	IDataBase* m_database;
	std::vector<LoggedUser> m_loggedUsers;
};