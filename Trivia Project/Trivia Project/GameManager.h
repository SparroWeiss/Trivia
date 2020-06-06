#pragma once
#include "Game.h"
#include "Room.h"
#include <vector>
#include <mutex>

class Game;

class GameManager
{
public:
	static GameManager* getInstance();
	~GameManager();
	Game* createGame(Room room);
	bool deleteGame(Game* game);
	void updateUserStatistics(Game* game, std::string username);

private:
	GameManager();
	static GameManager* instance;
	static int instances;

	IDataBase* m_database;
	std::vector<Game*> m_games;
};