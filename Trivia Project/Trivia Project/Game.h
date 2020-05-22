#pragma once
#include "IDataBase.h"
#include "JsonResponsePacketSerializer.h"
#include "LoginManager.h"


struct
{
	Question currentQuestion;
	unsigned int correctAnswersCount;
	unsigned int wrongAnswersCount;
	unsigned int averageAnswerTime;
}typedef GameData;

class Game
{
public:
	Game(std::vector<LoggedUser> users, std::vector<Question> Questions);
	~Game();
	Question getQuestionForUser(LoggedUser& user);
	unsigned int submitAnswer(unsigned int answerId, LoggedUser& user, Game game, float timeForAnswer);
	bool removePlayer(LoggedUser& user);
	std::map<std::string, GameData> getUsersData();
private:
	std::map<std::string, GameData> m_players;
	std::vector<Question> m_questions;

	Question getNextQuestion(Question current);
};