#include "Room.h"

std::mutex _mutex_users;
std::mutex _mutex_data;
/*
constructor
initialize the class's variables
*/
Room::Room()
{
	m_users = std::vector<LoggedUser>();
	m_metaData = { 0, "Room", 0, 0, ActiveMode::DONE };
}

/*
distructor
clears the vector
*/
Room::~Room()
{
	m_users.clear();
}


/*
function insert a new user into the room
input: user
output: true - success, false - the room is full
*/
bool Room::addUser(LoggedUser user)
{
	std::lock_guard<std::mutex> locker(_mutex_users);
	// prevents few people to enter in the same time into a full room
	if (m_metaData.maxPlayers > m_users.size())
	{ // the room isn't full
		m_users.push_back(user);
		if (m_metaData.maxPlayers == m_users.size())
		{ // the room is now full
			std::lock_guard<std::mutex> locker2(_mutex_data);
			m_metaData.isActive = ActiveMode::PLAYING;
		}
		return true;
	}
	return false;
}

/*
function removes a user from the room
input: username (might be changed later)
output: true - removed, false - can't be found
*/
bool Room::removeUser(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_users); // prevent any access to the users while someone logs out
	for (std::vector<LoggedUser>::iterator i = m_users.begin(); i != m_users.end(); ++i)
	{
		if (name == (*i).getUsername())
		{
			m_users.erase(i);
			m_metaData.isActive = ActiveMode::WAITING;
			return true;
		}
	}
	return false;
}

/*
function sets the room data
input: room data
output: none
*/
void Room::setData(RoomData data)
{
	std::lock_guard<std::mutex> locker(_mutex_data);
	m_metaData = RoomData(data);
}

/*
function gets the room data
input: none
output: room data
*/
RoomData Room::getData() const
{
	std::lock_guard<std::mutex> locker(_mutex_data);
	return m_metaData;
}

/*
function gets the list of users
input: none
output: vector of the users
*/
std::vector<LoggedUser> Room::getAllUsers() const
{
	return std::vector<LoggedUser>(m_users);
}