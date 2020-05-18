#pragma once
#include "LoginManager.h"

enum
{
	WAITING = 1, PLAYING, DONE
}typedef ActiveMode;

struct
{
	unsigned int id;
	std::string name;
	unsigned int maxPlayers;
	unsigned int timePerQuestion;
	unsigned int isActive;
}typedef RoomData;

class Room
{
public:
	Room();
	~Room();
	bool addUser(LoggedUser user);
	bool removeUser(std::string name);
	void setData(RoomData data);
	RoomData getData() const;
	std::vector<std::string> getAllUsers();
private:
	RoomData m_metaData;
	std::vector<LoggedUser> m_users;
};