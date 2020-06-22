#pragma once
#include "IDataBase.h"
#include <vector>
#include <mutex>

enum loginStatus
{
	SUCCESS = 1, 
	WRONGPASSWORD, 
	WRONGUSERNAME, 
	ALREADYINGAME
};

class LoggedUser
{
public:
	// Constractors
	LoggedUser() : LoggedUser("") {}
	LoggedUser(std::string name)
	{
		m_username = name;
	}
	~LoggedUser(){}
	
	/*
	Function gets the username
	Input: none
	Output: the username
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
	std::vector<LoggedUser>::iterator findUsername(std::string username);

	static LoginManager* instance;
	IDataBase* m_database;
	std::vector<LoggedUser> m_loggedUsers;
};