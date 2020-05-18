#pragma once
#include <map>
#include "Room.h"
#include "JsonResponsePacketSerializer.h"


#define MAX_PLAYERS 3
#define TIME_PER_QUE 10 // seconds

class RoomManager
{
public:
	static RoomManager* getInstance();
	~RoomManager();
	Room* createRoom(LoggedUser first_user, RoomData data);
	bool deleteRoom(unsigned int id);
	unsigned int getRoomState(unsigned int id);
	std::vector<Room*> getRooms();
private:
	RoomManager();
	static RoomManager* instance;
	static int instances;

	std::map<unsigned int, Room*> m_rooms;
	unsigned int curr_id;
};
