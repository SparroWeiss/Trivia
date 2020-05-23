#include "GameManager.h"
#include "SqliteDatabase.h"

/*
constructor
initializes the variables of the object
*/
GameManager::GameManager()
{
	m_database = SqliteDatabase::getInstance();
	m_games = std::vector<Game*>();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
GameManager* GameManager::getInstance()
{
	if (instance == 0)
	{
		instance = new GameManager();
	}
	instances++;
	return instance;
}

/*
destructor
frees allocated memory
*/
GameManager::~GameManager()
{
	instances--;
	if (instances == 0)
	{
		m_games.clear();
		delete instance;
	}
}

Game* GameManager::createGame(Room room)
{
	// TODO : get the questions from the data base and set the vector of the players
	return nullptr;
}

bool GameManager::deleteGame(Game* game)
{
	// TODO : wait to see if a room is empty and delete it
	return false;
}
