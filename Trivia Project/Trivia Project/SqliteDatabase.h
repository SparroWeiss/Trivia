#pragma once
#include "IDataBase.h"
#include "JsonRequestPacketDeserializer.h"
#include "sqlite3.h"
#include <vector>
#include <io.h>
#include <string>

class SqliteDatabase : public IDataBase
{
public:
	static SqliteDatabase* getInstance();
	~SqliteDatabase();
	bool doesUserExist(std::string name);
	bool doesPasswordMatch(std::string name, std::string password);
	bool addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate);
	virtual std::list<Question> getQuestions(int);
	virtual float getPlayerAverageAnswerTime(std::string name);
	virtual int getNumOfCorrectAnswers(std::string name);
	virtual int getNumOfTotalAnswers(std::string name);
	virtual int getNumOfPlayerGames(std::string name);

private:
	SqliteDatabase();
	static SqliteDatabase* instance;
	static int instances;

	sqlite3* _db;
	std::vector<SignupRequest> _usersRows;
	std::vector<Statistic> _statisticsRows;
	std::list<Question> _questionsRows;

	void refreshQuestions(int);
	void getStatistics(std::string name);

	friend int usersCallback(void* data, int size, char** argv, char** colName);
	friend int questionsCallback(void* data, int size, char** argv, char** colName);
	friend int statisticsCallback(void* data, int size, char** argv, char** colName);

	
	std::mutex _using_db;
	std::mutex _mutex_statistics;
	std::mutex _mutex_users;
	std::mutex _mutex_questions;
};
