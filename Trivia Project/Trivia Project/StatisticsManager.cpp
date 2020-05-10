#include "StatisticsManager.h"
#include "SqliteDatabase.h"
#include <algorithm>

#define TOP_PLAYERS 3

/*
constructor initialize the variables of the object
*/
StatisticsManager::StatisticsManager()
{
	m_database = SqliteDatabase::getInstance();
	m_loginManager = m_loginManager->getInstance();
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
distructor
frees allocated memory
*/
StatisticsManager::~StatisticsManager()
{
	instances--;
	if (instances == 0)
	{
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
/*
function gets the top logged in players in the server
input: reference to the statistics data
output: none
*/
void StatisticsManager::getTopPlayers(StatisticsData& data)
{
	std::vector<LoggedUser> users = m_loginManager->getUsers(); // the function can rank only yhe connected players
	std::vector<std::pair<std::string, float>> ranks;
	for (std::vector<LoggedUser>::iterator i = users.begin(); i != users.end(); ++i)
	{
		std::string name = (*i).getUsername();
		float score = m_database->getNumOfCorrectAnswers(name);
		score /= m_database->getNumOfTotalAnswers(name);
		score /= m_database->getPlayerAverageAnswerTime(name);
		ranks.push_back({ name, score });
		// score value = (CorrectAns/TotalAns) / AverageTimePerAns
	}
	std::sort(ranks.begin(), ranks.end(), sortByVal); // sorts the vector by the score
	for (int i = 0; i < ranks.size() && i < TOP_PLAYERS; i++)
	{
		data._topPlayers.push_back(ranks[i].first);
	}
}

bool sortByVal(const std::pair<std::string, float>& a,
	const std::pair<std::string, int>& b)
{
	return (a.second > b.second);
}