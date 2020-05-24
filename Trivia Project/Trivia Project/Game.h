#pragma once
#include "IDataBase.h"
#include "LoginManager.h"

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