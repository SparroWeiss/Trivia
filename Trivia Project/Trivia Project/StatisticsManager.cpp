#include "StatisticsManager.h"
#include "SqliteDatabase.h"
#include <algorithm>

#define TOP_PLAYERS 5

/*
Constructor:
Initializes the variables of the object
*/
StatisticsManager::StatisticsManager()
{
	m_database = SqliteDatabase::getInstance();
}

/*
Function make sure that there is only one instance of the object
Input: none
Output: pointer of the only instance
*/
StatisticsManager* StatisticsManager::getInstance()
{
	if (instance == 0)
	{
		instance = new StatisticsManager();
	}
	return instance;
}

/*
Destructor
*/
StatisticsManager::~StatisticsManager() {}

/*
Function gets all the statistics of a user
Input: username
Output: struct of the statistics
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

/********** Helper ***********/

/*
This function sort the vector by it's value
Input: the vector
Output: the sorted vector
*/
std::vector<std::pair<std::string, float>> sortByVal(std::vector<std::pair<std::string, float>> ranks)
{
	int size = ranks.size();
	for (int i = 0; i < size; i++)
	{
		for (int j = 0; j < size; j++)
		{
			if (ranks[i].second > ranks[j].second )
			{
				std::pair<std::string, float> temp = ranks[i];
				ranks[i] = ranks[j];
				ranks[j] = temp;
			}
		}
	}
	return ranks;
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
		
		if ((*i)._totalAnswers)
		{
			score = score / (*i)._totalAnswers;
			score = score / m_database->getPlayerAverageAnswerTime(name);
		}
		ranks.push_back({ name, score });
		// score value = (CorrectAns/TotalAns) / AverageTimePerAns
	}
	ranks = sortByVal(ranks);
	for (int i = 0; i < ranks.size() && i < TOP_PLAYERS; i++)
	{
		data._topPlayers.push_back(ranks[i].first); // insert the first five to the data variables
		data._topScores.push_back(ranks[i].second);
	}
}