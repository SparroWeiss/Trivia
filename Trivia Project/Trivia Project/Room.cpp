#include "Room.h"

std::mutex _mutex_users;
std::mutex _mutex_data;

/*
Constructor:
Initializes the variables of the object
*/
Room::Room()
{
	m_users = std::vector<LoggedUser>();
	m_metaData = RoomData{ 0, "Room", 0, 0, ActiveMode::DONE, 0 };

	m_game = nullptr;
}

/*
Destructor:
Frees allocated memory
*/
Room::~Room()
{
	m_users.clear();
}

/*
Function insert a new user into the room
Input: user
Output: true - success, false - the room is full
*/
bool Room::addUser(LoggedUser user)
{
	std::lock_guard<std::mutex> locker(_mutex_users);
	if (m_metaData.maxPlayers > m_users.size()) // the room isn't full
	{ 
		m_users.push_back(user);
		return true;
	}
	return false;
}

/*
Function removes a user from the room
Input: username (might be changed later)
Output: true - removed, false - can't be found
*/
bool Room::removeUser(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_users);
	for (std::vector<LoggedUser>::iterator i = m_users.begin(); i != m_users.end(); ++i)
	{
		if (name == (*i).getUsername())
		{
			m_users.erase(i);
			return true;
		}
	}
	return false;
}

/*
Function sets the room data
Input: room data
Output: none
*/
void Room::setData(RoomData data)
{
	std::lock_guard<std::mutex> locker(_mutex_data);
	m_metaData = RoomData(data);
}

/*
Function gets the room data
Input: none
Output: room data
*/
RoomData Room::getData() const
{
	std::lock_guard<std::mutex> locker(_mutex_data);
	return m_metaData;
}

/*
Function gets the list of users
Input: none
Output: vector of the users
*/
std::vector<std::string> Room::getAllUsers()
{
	std::vector<std::string> names;
	try
	{
		std::unique_lock<std::mutex> locker(_mutex_users);
		std::vector<LoggedUser> users = m_users;
		locker.unlock();
		for (std::vector<LoggedUser>::iterator i = users.begin(); i != users.end(); ++i)
		{
			names.push_back((*i).getUsername());
		}
	}
	catch (const std::exception&)
	{
		// there is nothing to do, just catch
	}
	return names;
}

/********* Game Helpers **********/

/*
Function sets the game of the room
Input: pointer to the game
Output: none
*/
void Room::setGame(Game* game)
{
	m_game = game;
}

/*
Function gets the game of the room
Input: none
Output: pointer to the game
*/
Game* Room::getGame()
{
	return m_game;
}
