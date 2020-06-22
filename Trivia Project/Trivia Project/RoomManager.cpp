#include "RoomManager.h"

std::mutex _mutex_rooms;
std::mutex _mutex_curr_id;

/*
Constructor:
Initializes the variables of the object
*/
RoomManager::RoomManager()
{
	m_rooms = std::map<unsigned int, Room*>();
	curr_id = 1;
}

/*
Function make sure that there is only one instance of the object
Input: none
Output: pointer of the only instance
*/
RoomManager* RoomManager::getInstance()
{
	if (instance == 0)
	{
		instance = new RoomManager();
	}
	return instance;
}

/*
Destructor:
Frees allocated memory
*/
RoomManager::~RoomManager()
{
	m_rooms.clear();
}

/*
Function creates a new room and inserts it into the map
Input: the user that created the room
Output: room id
*/
Room* RoomManager::createRoom(LoggedUser first_user, RoomData data)
{
	Room* room = new Room();
	std::unique_lock<std::mutex> locker(_mutex_curr_id); // can't be two rooms with the same id
	room->setData({ curr_id++, data.name, data.maxPlayers, data.timePerQuestion, ActiveMode::WAITING, data.questionCount });
	locker.unlock();
	room->addUser(first_user);
	std::unique_lock<std::mutex> locker2(_mutex_rooms);
	m_rooms.insert({ room->getData().id, room });
	locker2.unlock();
	return room;
}

/*
Function deletes a room from the map
Input: the room id
Output: true - room deleted, false - room couldn't be found
*/
bool RoomManager::deleteRoom(unsigned int id)
{
	std::unique_lock<std::mutex> locker(_mutex_rooms);
	std::map<unsigned int, Room*>::iterator it = m_rooms.find(id);
	if (it == m_rooms.end())
	{
		return false; // user not found
	}
	delete it->second; // delete room
	m_rooms.erase(it); // erase from map
	return true;
}

/*
Function gets a room state
Input: the room id
Output: the state of a room
*/
unsigned int RoomManager::getRoomState(unsigned int id)
{
	std::lock_guard<std::mutex> locker(_mutex_rooms);
	std::map<unsigned int, Room*>::iterator iter = m_rooms.find(id);
	if (iter == m_rooms.end())
	{
		return ActiveMode::DONE; // room not found
	}
	return (*iter).second->getData().isActive;
}

/*
Function gets a list of the rooms
Input: none
Output: vector of the rooms in the server
*/
std::vector<Room*> RoomManager::getRooms()
{
	std::lock_guard<std::mutex> locker(_mutex_rooms);
	std::vector<Room*> rooms = std::vector<Room*>();
	for (std::map<unsigned int, Room*>::iterator i = m_rooms.begin(); i != m_rooms.end(); ++i)
	{
		rooms.push_back((*i).second);
	}
	return rooms;
}
