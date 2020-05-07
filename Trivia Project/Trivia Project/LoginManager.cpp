#include "LoginManager.h"
#include "SqliteDatabase.h"

std::mutex _mutex_loggedUsers;
std::mutex _mutex_db;

/*
constructor initialize the variables of the object
*/
LoginManager::LoginManager() 
{
	m_database = SqliteDatabase::getInstance();
	m_loggedUsers = std::vector<LoggedUser>();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
LoginManager* LoginManager::getInstance()
{
	if (instance == 0)
	{
		instance = new LoginManager();
	}

	return instance;
}

/*
distructor
frees allocated memory
*/
LoginManager::~LoginManager()
{
	// nothing to free
}

/*
function signs up a new user
input: user's specs: username, password, email
output: true - signed up, false - invalid name
*/
bool LoginManager::signup(std::string name, std::string password, std::string email)
{
	std::unique_lock<std::mutex> locker1(_mutex_db);
	if (m_database->addNewUser(name, password, email)) // if the user's specs are valid
	{
		locker1.unlock();
		std::unique_lock<std::mutex> locker2(_mutex_loggedUsers);
		m_loggedUsers.push_back(LoggedUser(name));
		locker2.unlock();
		return true;
	}
	locker1.unlock();
	return false;
}

/*
function logs in a user by it's name and password
input: username, password
output: true - logged in, false - something went wrong
*/
bool LoginManager::login(std::string name, std::string password)
{
	std::unique_lock<std::mutex> locker1(_mutex_db);
	bool pass_match = m_database->doesPasswordMatch(name, password);
	locker1.unlock();
	if (pass_match)
	{ // if the password matches the username
		std::unique_lock<std::mutex> locker2(_mutex_loggedUsers);
		if (findUsername(name) == m_loggedUsers.end())
		{//didn't find the user 
			m_loggedUsers.push_back(LoggedUser(name));
			locker2.unlock();
			return true;
		}
		locker2.unlock();
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
	std::unique_lock<std::mutex> locker1(_mutex_db);
	bool user_exist = m_database->doesUserExist(name);
	locker1.unlock();
	if (user_exist)
	{ // if the user is in the data base
		std::unique_lock<std::mutex> locker2(_mutex_loggedUsers);
		std::vector<LoggedUser>::iterator iter = findUsername(name);
		if (iter != m_loggedUsers.end())
		{
			m_loggedUsers.erase(iter);
			locker2.unlock();
			return true;
		}
		locker2.unlock();
	}
	return false;
}

/*
helper function finds the iterator of a user by his name
input: username
output: if user found - the iterator of the user, if user not found - m_loggedUsers.end()
*/
std::vector<LoggedUser>::iterator LoginManager::findUsername(std::string username)
{
	for (std::vector<LoggedUser>::iterator i = m_loggedUsers.begin(); i != m_loggedUsers.end(); ++i)
	{
		if (i->getUsername() == username)
		{ // if the name matches
			return i;
		}
	}
	return m_loggedUsers.end();
}
