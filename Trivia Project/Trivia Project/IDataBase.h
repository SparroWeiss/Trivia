#pragma once
#include <iostream>
#include <list>
#include <string>
#include <vector>
#include <map>

class Question
{
public:
	Question();
	~Question();

	std::string getQuestion();
	void setQuestion(std::string);

	std::map<unsigned int, std::string> getPossibleAnswers();
	void addPossibleAnswers(std::string);

	unsigned int getCorrectAnswer();
	void setCorrectAnswer(std::string);
private:
	static unsigned int curr_id;
	std::string m_question;
	unsigned int m_correctAnswer;
	std::map<unsigned int, std::string> m_possibleAnswers;
};

struct
{
	std::string _name;
	int _totalAnswers;
	int _correctAnswers;
	int _numOfGames;
	int _answersTime;
}typedef Statistic;

struct
{
	Question currentQuestion;
	unsigned int correctAnswersCount;
	unsigned int wrongAnswersCount;
	float averageAnswerTime;
	unsigned int playing;
}typedef GameData;

class IDataBase
{
public:
	virtual bool doesUserExist(std::string name) = 0;
	virtual bool doesPasswordMatch(std::string name, std::string password) = 0;
	virtual bool addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate) = 0;
	virtual std::vector<Question> getQuestions(int) = 0;
	virtual float getPlayerAverageAnswerTime(std::string name) = 0;
	virtual int getNumOfCorrectAnswers(std::string name) = 0;
	virtual int getNumOfTotalAnswers(std::string name) = 0;
	virtual int getNumOfPlayerGames(std::string name) = 0;
	virtual std::vector<Statistic> getStatistics() = 0;
	virtual void updateStatistics(std::map<std::string, GameData> usersGameData) = 0;
};
