#pragma once
#include "IDataBase.h"
#include <vector>
#include <mutex>

enum loginStatus
{
	SUCCESS = 1, WRONGPASSWORD, WRONGUSERNAME, ALREADYINGAME
};

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
		return m_username;
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
	loginStatus login(std::string, std::string);
	bool logout(std::string);

private:
	LoginManager();
	static LoginManager* instance;
	static int instances;

	IDataBase* m_database;
	std::vector<LoggedUser> m_loggedUsers;
	std::vector<LoggedUser>::iterator findUsername(std::string username);
};