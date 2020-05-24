#pragma once
///#include "LoginManager.h"
#include "Game.h"

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
	unsigned int questionCount;
}typedef RoomData;

class Game;

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

	void setGame(Game* game); // helper : for the admin
	Game* getGame(); // helper : for game handler
private:
	Game* m_game; // helper : to notify the member to which game to go
	RoomData m_metaData;
	std::vector<LoggedUser> m_users;
};