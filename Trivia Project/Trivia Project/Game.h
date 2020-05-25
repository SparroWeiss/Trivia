#pragma once
#include "IDataBase.h"
#include "LoginManager.h"

enum GameMode
{
	FINISHED = 1, WAITING_FOR_PLAYERS
};

enum PlayerMode
{
	LEFT = 1, PLAYING, WAITING_FOR_RESULTS
};

class Game
{
public:
	Game();
	Game(std::vector<LoggedUser> users, std::vector<Question> Questions);
	~Game();
	Question getQuestionForUser(LoggedUser& user);
	unsigned int submitAnswer(unsigned int answerId, LoggedUser& user, float timeForAnswer);
	bool removePlayer(LoggedUser& user);
	std::map<std::string, GameData> getUsersData();
	unsigned int getUsersAmount();
private:
	std::map<std::string, GameData> m_players;
	std::vector<Question> m_questions;

	Question getNextQuestion(Question current);
};