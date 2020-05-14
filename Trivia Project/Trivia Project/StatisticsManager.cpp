#include "StatisticsManager.h"
#include "SqliteDatabase.h"
#include <algorithm>

#define TOP_PLAYERS 5

/*
constructor
initializes the variables of the object
*/
StatisticsManager::StatisticsManager()
{
	m_database = SqliteDatabase::getInstance();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
StatisticsManager* StatisticsManager::getInstance()
{
	if (instance == 0)
	{
		instance = new StatisticsManager();
	}
	instances++;
	return instance;
}

/*
destructor
frees allocated memory
*/
StatisticsManager::~StatisticsManager()
{
	instances--;
	if (instances == 0)
	{
		delete m_database;
		delete instance;
	}
}

/*
function gets all the statistics of a user
input: username
output: struct of the statistics
*/
StatisticsData StatisticsManager::getStatistics(std::string name)
{
	StatisticsData data = StatisticsData();
	data._username = name;
	data._totalAnswers = m_database->getNumOfTotalAnswers(name);
	data._correctAnswers = m_database->getNumOfCorrectAnswers(name);
	data._incorrectAnswers = data._totalAnswers - data._correctAnswers;
	data._numOfGames = m_database->getNumOfPlayerGames(name);
	data._averageAnswerTime = m_database->getPlayerAverageAnswerTime(name);
	getTopPlayers(data);
	return data;
}

////////////////////////////////////////////////////////HELPER
bool sortByVal(const std::pair<std::string, float>& a,
	const std::pair<std::string, int>& b)
{
	return (a.second > b.second);
}
/*
function gets the top logged in players in the server
input: reference to the statistics data
output: none
*/
void StatisticsManager::getTopPlayers(StatisticsData& data)
{
	std::vector<Statistic> users = m_database->getStatistics();
	std::vector<std::pair<std::string, float>> ranks;
	for (std::vector<Statistic>::iterator i = users.begin(); i != users.end(); ++i)
	{
		std::string name = (*i)._name;
		float score = (*i)._correctAnswers;
		score /= (*i)._totalAnswers;
		score /= m_database->getPlayerAverageAnswerTime(name);
		ranks.push_back({ name, score });
		// score value = (CorrectAns/TotalAns) / AverageTimePerAns
	}
	std::sort(ranks.begin(), ranks.end(), sortByVal); // sorts the vector by the score
	for (int i = 0; i < ranks.size() && i < TOP_PLAYERS; i++)
	{
		data._topPlayers.push_back(ranks[i].first); // insert the first three to the data variables
	}
}
