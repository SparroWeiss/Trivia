#include "SqliteDatabase.h"

#define DB_NAME std::string("TriviaDB.sqlite")

// USER
#define NAME_COLUMN std::string("NAME")
#define PASSWORD_COLUMN std::string("PASSWORD")
#define EMAIL_COLUMN std::string("EMAIL")
#define ADDRESS_COLUMN std::string("ADDRESS")
#define PHONE_COLUMN std::string("PHONE")
#define BIRTHDATE_COLUMN std::string("BIRTHDATE")

// QUESTIONS
#define QUESTION_COLUMN std::string("QUESTION")
#define CORRECT_ANSWER_COLUMN std::string("CORRECT_ANSWER")
#define WRONG_ANSWERS_COLUMN std::string("WRONG_ANSWERS")

// STATISTICS
#define TOTAL_ANSWERS_COLUMN std::string("TOTAL_ANSWERS")
#define CORRECT_ANSWERS_COLUMN std::string("CORRECT_ANSWERS")
#define NUM_OF_GAMES_COLUMN std::string("NUM_OF_GAMES")
#define ANSWERS_TIME_COLUMN std::string("ANSWERS_TIME") // (summery not average)

#define MAX_QUESTIONS 50
#define SCRIPT_PATH "python \"..\\Trivia Project\\scripts\\getQuestions.py\" "
#define DELIMITER "~~~"
#define NAME_MATCH_CHECK "."



/*
this helper function gets the data that the DB returned
and transforms it into a reachable variable
input: data - pointer to the reachable variable, size - number of columns in the table,
		argv - array of the data of each column, colName - the name of the column
output: 0, it doesn't realy matters
*/
int usersCallback(void* data, int size, char** argv, char** colName)
{
	SignupRequest temp = SignupRequest();
	std::string column;
	for (int i = 0; i < size; i++)
	{
		column = std::string(colName[i]);
		if (column == NAME_COLUMN)
		{
			temp.username = std::string(argv[i]);
		}
		else if (column == PASSWORD_COLUMN)
		{
			temp.password = std::string(argv[i]);
		}
		else if (column == EMAIL_COLUMN)
		{
			temp.email = std::string(argv[i]);
		}
		else if (column == ADDRESS_COLUMN)
		{
			temp.address = std::string(argv[i]);
		}
		else if (column == PHONE_COLUMN)
		{
			temp.phone = std::string(argv[i]);
		}
		else if (column == BIRTHDATE_COLUMN)
		{
			temp.birthdate = std::string(argv[i]);
		}
	}
	static_cast<std::vector<SignupRequest>*>(data)->push_back(temp);
	return 0;
}

/*
This helper function gets the data that the DB returned
and transforms it into a reachable variable.
Input: data - pointer to the reachable variable,
       size - number of columns in the table,
       argv - array of the data of each column, colName - the name of the column
output: 0
*/
int questionsCallback(void* data, int size, char** argv, char** colName)
{
	Question temp = Question();
	std::string column;
	char* tempWrongAnswer;

	for (int i = 0; i < size; i++)
	{
		column = std::string(colName[i]);
		if (column == QUESTION_COLUMN)
		{
			temp.setQuestion(std::string(argv[i]));
		}
		else if (column == CORRECT_ANSWER_COLUMN)
		{
			temp.setCorrectAnswer(std::string(argv[i]));
		}
		else if (column == WRONG_ANSWERS_COLUMN)
		{
			tempWrongAnswer = strtok(argv[i], DELIMITER);
			while (tempWrongAnswer != NULL)
			{
				temp.addPossibleAnswers(tempWrongAnswer);
				tempWrongAnswer = strtok(NULL, DELIMITER);
			}
		}
	}

	static_cast<std::vector<Question>*>(data)->push_back(temp);
	return 0;
}

/*
this helper function gets the data that the DB returned
and transforms it into a reachable variable
input: data - pointer to the reachable variable, size - number of columns in the table,
		argv - array of the data of each column, colName - the name of the column
output: 0, it doesn't realy matters
*/
int statisticsCallback(void* data, int size, char** argv, char** colName)
{
	Statistic temp = Statistic();
	std::string column;
	for (int i = 0; i < size; i++)
	{
		column = std::string(colName[i]);
		if (column == NAME_COLUMN)
		{
			temp._name = std::string(argv[i]);
		}
		else if (column == ANSWERS_TIME_COLUMN)
		{
			temp._answersTime = stoi(std::string(argv[i]));
		}
		else if (column == CORRECT_ANSWERS_COLUMN)
		{
			temp._correctAnswers = stoi(std::string(argv[i]));
		}
		else if (column == NUM_OF_GAMES_COLUMN)
		{
			temp._numOfGames = stoi(std::string(argv[i]));
		}
		else if (column == TOTAL_ANSWERS_COLUMN)
		{
			temp._totalAnswers = stoi(std::string(argv[i]));
		}
	}
	static_cast<std::vector<Statistic>*>(data)->push_back(temp);
	return 0;
}

/*
constructor
initializes the variables of the object
*/
SqliteDatabase::SqliteDatabase()
{
	int fileNotExists = _access(DB_NAME.c_str(), 0); // checks if file already exists
	int res = sqlite3_open(DB_NAME.c_str(), &_db); // create the file if doesn't exists, open the file if exists.
	if (res != SQLITE_OK)
	{ // never happened
		_db = nullptr;
	}
	if (fileNotExists)
	{ // file doesn't exists
		std::string query = "CREATE TABLE USERS(" + NAME_COLUMN +
			" TEXT PRIMARY KEY NOT NULL, " + PASSWORD_COLUMN +
			" TEXT NOT NULL, " + EMAIL_COLUMN +
			" TEXT NOT NULL, " + ADDRESS_COLUMN +
			" TEXT NOT NULL, " + PHONE_COLUMN +
			" TEXT NOT NULL, " + BIRTHDATE_COLUMN +
			" TEXT NOT NULL);";
		sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr); // set users table

		query = "CREATE TABLE QUESTIONS(" + QUESTION_COLUMN +
			" TEXT PRIMARY KEY NOT NULL, " + CORRECT_ANSWER_COLUMN +
			" TEXT NOT NULL, " + WRONG_ANSWERS_COLUMN +
			" TEXT NOT NULL);";

		sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr); // set questions table

		query = "CREATE TABLE STATISTICS(" + NAME_COLUMN +
			" TEXT PRIMARY KEY NOT NULL, " + TOTAL_ANSWERS_COLUMN +
			" INTIGER NOT NULL, " + CORRECT_ANSWERS_COLUMN +
			" INTIGER NOT NULL, " + NUM_OF_GAMES_COLUMN +
			" INTIGER NOT NULL, " + ANSWERS_TIME_COLUMN +
			" INTIGER NOT NULL);";

		sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr); // set statistics table
	}
	_usersRows = std::vector<SignupRequest>();
	_questionsRows = std::vector<Question>();
}

/*
function make sure that there is only one instance of the object
input: none
output: pointer of the only instance
*/
SqliteDatabase* SqliteDatabase::getInstance()
{
	if (instance == 0)
	{
		instance = new SqliteDatabase();
	}
	instances++;
	return instance;
}

/*
destructor
frees allocated memory
*/
SqliteDatabase::~SqliteDatabase()
{
	instances--;
	if (instances == 0)
	{
		sqlite3_close(_db);
		_db = nullptr;
		_usersRows.clear();
		_questionsRows.clear();
		delete instance;
	}
}

/*
function checks if a username already exists
input: username
output: true - the username exists in the DB, false - the username can't be found in
*/
bool SqliteDatabase::doesUserExist(std::string name)
{
	return doesPasswordMatch(name, NAME_MATCH_CHECK);
}

/*
function checks if a password matches to the username
input: username, password
output: true - the password matches the username, false - the password doesn't match the username
*/
bool SqliteDatabase::doesPasswordMatch(std::string name, std::string password)
{
	std::string query = "SELECT * FROM USERS WHERE NAME = '" + name + "';";
	std::lock_guard<std::mutex> locker(_mutex_users);
	_usersRows.clear(); // remove the previous data
	std::lock_guard<std::mutex> locker2(_using_db);
	sqlite3_exec(_db, query.c_str(), usersCallback, &_usersRows, nullptr);
	return !_usersRows.empty() && (password == _usersRows.front().password || password == NAME_MATCH_CHECK);
	
}

/*
function adds a new user
input: the user specs: username, password, email
output: true - the user's specs are valid, false - there is already someone with that username
*/
bool SqliteDatabase::addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate)
{
	if (doesUserExist(name))
	{ // can't make two users with the same name
		return false;
	}
	std::string query = "INSERT INTO USERS(NAME, PASSWORD, EMAIL, ADDRESS, PHONE, BIRTHDATE) VALUES ('" +
		name + "', '" + password + "', '" + email + "', '" + address + "', '" + phone + "', '" + birthdate +  "');";
	std::lock_guard<std::mutex> locker(_using_db);
	sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr);
	query = "INSERT INTO STATISTICS(NAME, TOTAL_ANSWERS, CORRECT_ANSWERS, NUM_OF_GAMES, ANSWERS_TIME) VALUES ('" +
		name + "', 0, 0, 0, 0);";
	sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr);
	return true;
}

/*
function gets the questions from the DB
input: amount of questions
output: list of questions
*/
std::vector<Question> SqliteDatabase::getQuestions(int amount)
{
	std::string query = "SELECT * FROM QUESTIONS;";
	std::lock_guard<std::mutex> locker(_using_db); // python script accessing db
	std::lock_guard<std::mutex> locker2(_mutex_questions);
	_questionsRows.clear(); // remove the previous data

	refreshQuestions(amount); // refreshing DB questions

	sqlite3_exec(_db, query.c_str(), questionsCallback, &_questionsRows, nullptr);


	return std::vector<Question>(_questionsRows);
}

/*
function gets the average time per answer
input: username
output: the average time for a answer
*/
float SqliteDatabase::getPlayerAverageAnswerTime(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_statistics);
	getStatistics(name);
	if (_statisticsRows.front()._totalAnswers)
	{
		return  _statisticsRows.front()._answersTime / _statisticsRows.front()._totalAnswers;
	}
	return 0;
}

/*
function gets the number of correct answers the user answered
input: username
output: the number of correct answers
*/
int SqliteDatabase::getNumOfCorrectAnswers(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_statistics);
	getStatistics(name);
	return  _statisticsRows.front()._correctAnswers;
}

/*
function gets the number of answers the user answered
input: username
output: the number of answers
*/
int SqliteDatabase::getNumOfTotalAnswers(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_statistics);
	getStatistics(name);
	return  _statisticsRows.front()._totalAnswers;
}

/*
function gets the number of games the user played
input: username
output: the number of games
*/
int SqliteDatabase::getNumOfPlayerGames(std::string name)
{
	std::lock_guard<std::mutex> locker(_mutex_statistics);
	getStatistics(name);
	return  _statisticsRows.front()._numOfGames;
}

/*
This function activate a python script that refresh the DB's questions.
Input: the amount of new questions
Output: none
*/
void SqliteDatabase::refreshQuestions(int amount)
{
	STARTUPINFOA info = { sizeof(info) };
	PROCESS_INFORMATION processInfo;

	if (amount > MAX_QUESTIONS) // opentdb api supports up to 50 questions per request
	{
		amount = MAX_QUESTIONS;
	}

	std::string scriptCommandLine = SCRIPT_PATH + std::to_string(amount) + " \"..\\Trivia Project\\" + DB_NAME + "\"";

	if (CreateProcessA(NULL, LPSTR(scriptCommandLine.c_str()), NULL, NULL, FALSE, NULL, NULL, NULL, &info, &processInfo))
	{
		WaitForSingleObject(processInfo.hProcess, INFINITE);
		CloseHandle(processInfo.hProcess);
		CloseHandle(processInfo.hThread);
	}

	else
	{
		std::cout << "Faild to fetch questions. Error " << GetLastError() << std::endl; // should not happen
	}
}

/*
this function creates a row of statistics info of a user
input: username
output: none
*/
void SqliteDatabase::getStatistics(std::string name)
{
	std::string query = "SELECT * FROM STATISTICS WHERE " + NAME_COLUMN + " = '" + name + "';";
	_statisticsRows.clear();
	std::lock_guard<std::mutex> locker2(_using_db);
	sqlite3_exec(_db, query.c_str(), statisticsCallback, &_statisticsRows, nullptr);
}

/*
function gets the statistic of all the users
input: none
output: vector of statistics
*/
std::vector<Statistic> SqliteDatabase::getStatistics()
{
	std::string query = "SELECT * FROM STATISTICS;";
	std::lock_guard<std::mutex> locker(_mutex_statistics);
	_statisticsRows.clear();
	std::lock_guard<std::mutex> locker2(_using_db);
	sqlite3_exec(_db, query.c_str(), statisticsCallback, &_statisticsRows, nullptr);
	return std::vector<Statistic>(_statisticsRows);
}

/*
function updates the statistics of the users
input: the usernames and their data
output: none
*/
void SqliteDatabase::updateStatistics(std::map<std::string, GameData> usersGameData)
{
	for (Statistic curr : getStatistics())
	{
		for (std::pair<std::string, GameData> player : usersGameData)
		{
			if (curr._name == player.first)
			{
				int gameQuestions = player.second.correctAnswersCount + player.second.wrongAnswersCount;

				updateUserStatistic(Statistic{curr._name,
					curr._totalAnswers+gameQuestions,
					curr._correctAnswers + static_cast<int>(player.second.correctAnswersCount),
					curr._numOfGames+1,
					static_cast<int>((curr._answersTime*curr._totalAnswers + player.second.averageAnswerTime*gameQuestions)
					/ curr._totalAnswers + gameQuestions) });
			}
		}
	}
}

/*
function updates a user's statistics
input: the statistics of the user
output: none
*/
void SqliteDatabase::updateUserStatistic(Statistic userStatistic)
{
	std::string query = "UPDATE STATISTICS SET " + TOTAL_ANSWERS_COLUMN + " = " + std::to_string(userStatistic._totalAnswers) +
		", " + CORRECT_ANSWERS_COLUMN + " = " + std::to_string(userStatistic._correctAnswers) + ", " +
		NUM_OF_GAMES_COLUMN + " = " + std::to_string(userStatistic._numOfGames) + ", " +
		ANSWERS_TIME_COLUMN + " = " + std::to_string(userStatistic._answersTime) +
		" WHERE " + NAME_COLUMN + " = " + userStatistic._name + ";";

	std::lock_guard<std::mutex>locker(_using_db);
	sqlite3_exec(_db, query.c_str(), nullptr, nullptr, nullptr);
}
