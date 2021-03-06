#pragma once
#include "IDataBase.h"
#include "LoginManager.h"
#include <mutex>

struct
{
	std::string _username;
	int _totalAnswers;
	int _correctAnswers;
	int _incorrectAnswers;
	int _numOfGames;
	float _averageAnswerTime;
	std::vector<std::string> _topPlayers;
	std::vector<float> _topScores;
}typedef StatisticsData;

class StatisticsManager
{
public:
	static StatisticsManager* getInstance();
	~StatisticsManager();
	StatisticsData getStatistics(std::string name);

private:
	StatisticsManager();
	void getTopPlayers(StatisticsData& data);

	static StatisticsManager* instance;
	IDataBase* m_database;
};