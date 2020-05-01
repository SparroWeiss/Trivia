#pragma once
#include "IDataBase.h"
#include <vector>

class LoggedUser
{
public:
	LoggedUser(std::string name);
	~LoggedUser();
	std::string getUsername() const;
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