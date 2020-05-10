#pragma once
#include "IDataBase.h"
#include <vector>
#include <mutex>

class LoggedUser
{
public:
	/*
	constructors
	*/
	LoggedUser() : LoggedUser("") {}
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
	static LoginManager* getInstance();
	~LoginManager();
	bool signup(std::string, std::string, std::string, std::string, std::string, std::string);
	bool login(std::string, std::string);
	bool logout(std::string);
	std::vector<LoggedUser> getUsers() const;

private:
	LoginManager();
	static LoginManager* instance;
	static int instances;

	IDataBase* m_database;
	std::vector<LoggedUser> m_loggedUsers;
	std::vector<LoggedUser>::iterator findUsername(std::string username);
};