#include "GameManager.h"
#include "SqliteDatabase.h"

std::mutex _using_db;

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

/*
This function creates a new game for a room.
Input: the room that hosts the game
Output: the new game
*/
Game* GameManager::createGame(Room room)
{
	std::list<Question> newQuestions = m_database->getQuestions(room.getData().questionCount);
	std::vector<LoggedUser> gameUsers;

	for (std::string username : room.getAllUsers())
	{
		gameUsers.push_back(LoggedUser(username));
	}
	m_games.push_back(new Game(gameUsers, std::vector<Question>(newQuestions.begin(), newQuestions.end())));
	return m_games.back();
}

/*
This function deletes a game.
Input: the game to be deleted
Output: if the game was deleted - true, else - false
*/
bool GameManager::deleteGame(Game* game)
{
	std::vector<Game*>::iterator it = std::find(m_games.begin(), m_games.end(), game);

	if (it != m_games.end())
	{
		std::unique_lock<std::mutex> locker(_using_db);
		m_database->updateStatistics(game->getUsersData());
		locker.unlock();
		m_games.erase(it);
		delete game;
		return true;
	}
	return true;
}
