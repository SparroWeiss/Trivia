#include "LoginManager.h"
#include "SqliteDatabase.h"

/*
constructor initialize the variables of the object
*/
LoginManager::LoginManager() : LoginManager(nullptr){}
LoginManager::LoginManager(IDataBase * db)
{
	m_database = db;
	m_loggedUsers = std::vector<LoggedUser>();
}

/*
distructor frees allocated memory
*/
LoginManager::~LoginManager()
{
}

/*
function signs up a new user
input: user's specs: username, password, email
output: true - signed up, false - invalid name
*/
bool LoginManager::signup(std::string name, std::string password, std::string email)
{
	if (m_database->addNewUser(name, password, email))
	{ // if the user's specs are valid
		m_loggedUsers.push_back(LoggedUser(name));
		return true;
	}
	return false;
}

/*
function logs in a user by it's name and password
input: username, password
output: true - logged in, false - something went wrong
*/
bool LoginManager::login(std::string name, std::string password)
{
	if (m_database->doesPasswordMatch(name, password))
	{ // if the password matches the username
		m_loggedUsers.push_back(LoggedUser(name));
		return true;
	}
	return false;
}

/*
function logs out the sellected user
input: username to logout
output: true - logged out, false - something went wrong
*/
bool LoginManager::logout(std::string name)
{
	if (m_database->doesUserExist(name))
	{ // if the user is in the data base
		for (std::vector<LoggedUser>::iterator i = m_loggedUsers.begin(); i != m_loggedUsers.end(); ++i)
		{
			if ((*i).getUsername() != name)
			{ // if the name matches
				m_loggedUsers.erase(i);
				return true;
			}
		}
	}
	return false;
}