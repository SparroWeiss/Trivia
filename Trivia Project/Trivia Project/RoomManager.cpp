#include "RoomManager.h"

std::mutex _mutex_rooms;
std::mutex _mutex_curr_id;

RoomManager::RoomManager()
{
	m_rooms = std::map<unsigned int, Room>();
	curr_id = 1;
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
RoomManager* RoomManager::getInstance()
{
	if (instance == 0)
	{
		instance = new RoomManager();
	}
	instances++;
	return instance;
}

/*
distructor
frees allocated memory, the only new allocated memory in the class is the instance
*/
RoomManager::~RoomManager()
{
	instances--;
	if (instances == 0)
	{
		delete instance;
		m_rooms.clear();
	}
}

/*
function creates a new room and inserts it into the map
input: the user that created the room
output: none
*/
void RoomManager::createRoom(LoggedUser first_user)
{
	Room room = Room();
	room.addUser(first_user);
	std::unique_lock<std::mutex> locker(_mutex_curr_id); // can't be two rooms with the same id
	room.setData({ curr_id++, "Room", MAX_PLAYERS, TIME_PER_QUE, ActiveMode::WAITING });
	locker.unlock();
	std::lock_guard<std::mutex> locker2(_mutex_rooms);
	m_rooms.insert({ room.getData().id, room });
}

/*
function deletes a room from the map
input: the room id
output: true - room deleted, false - room couldn't be found
*/
bool RoomManager::deleteRoom(unsigned int id)
{
	std::lock_guard<std::mutex> locker(_mutex_rooms);
	std::map<unsigned int, Room>::iterator iter = m_rooms.find(id);
	if (iter == m_rooms.end())
	{
		return false;
	}
	m_rooms.erase(iter);
	return true;
}

/*
function gets a room state
input: the room id
output: the state of a room
*/
unsigned int RoomManager::getRoomState(unsigned int id)
{
	std::lock_guard<std::mutex> locker(_mutex_rooms);
	std::map<unsigned int, Room>::iterator iter = m_rooms.find(id);
	if (iter == m_rooms.end())
	{
		return ActiveMode::DONE; // room not found
	}
	return (*iter).second.getData().isActive;
}

/*
function gets a list of the rooms
input: none
output: vector of the rooms in the server
*/
std::vector<Room> RoomManager::getRooms()
{
	std::lock_guard<std::mutex> locker(_mutex_rooms);
	std::vector<Room> rooms = std::vector<Room>();
	for (std::map<unsigned int, Room>::iterator i = m_rooms.begin(); i != m_rooms.end(); ++i)
	{
		rooms.push_back((*i).second);
	}
	return rooms;
}
