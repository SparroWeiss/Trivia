#include "GameManager.h"
#include "SqliteDatabase.h"

std::mutex _using_db;
std::mutex _using_games;

/*
Constructor:
Initializes the variables of the object
*/
GameManager::GameManager()
{
	m_database = SqliteDatabase::getInstance();
	m_games = std::vector<Game*>();
}

/*
Function make sure that there is only one instance of the object
Input: none
Output: pointer of the only instance
*/
GameManager* GameManager::getInstance()
{
	if (instance == 0)
	{
		instance = new GameManager();
	}
	return instance;
}

/*
Destructor:
Frees allocated memory
*/
GameManager::~GameManager()
{
	m_games.clear();
}

/*
This function creates a new game for a room.
Input: the room that hosts the game
Output: the new game
*/
Game* GameManager::createGame(Room room)
{
	std::vector<Question> newQuestions = m_database->getQuestions(room.getData().questionCount);
	std::vector<LoggedUser> gameUsers;

	for (std::string username : room.getAllUsers())
	{
		gameUsers.push_back(LoggedUser(username));
	}
	
	std::lock_guard<std::mutex> locker(_using_games);
	m_games.push_back(new Game(gameUsers, newQuestions));
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
		m_games.erase(it);
		try
		{
			delete game;
			return true;
		}
		catch (...)
		{
			return false;
		}
	}
	return true;
}

/*
This function update the user statistics in the DB
Input: the game that the user played in
Output: none
*/
void GameManager::updateUserStatistics(Game* game, std::string username)
{
	m_database->updateStatistics(game->getUsersData(), username, game->getNumOfQuestions());
}
