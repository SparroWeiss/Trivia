#include "SqliteDatabase.h"

#define DB_NAME std::string("TriviaDB.sqlite")
#define NAME_COLUMN std::string("NAME")
#define PASSWORD_COLUMN std::string("PASSWORD")
#define EMAIL_COLUMN std::string("EMAIL")
#define ADDRESS_COLUMN std::string("ADDRESS")
#define PHONE_COLUMN std::string("PHONE")
#define BIRTHDATE_COLUMN std::string("BIRTHDATE")

#define QUESTION_COLUMN std::string("QUESTION")
#define CORRECT_ANSWER_COLUMN std::string("CORRECT_ANSWER")
#define WRONG_ANSWERS_COLUMN std::string("WRONG_ANSWERS")

#define MAX_QUESTIONS 50
#define SCRIPT_PATH "python \"..\\Trivia Project\\scripts\\getQuestions.py\" "
#define DELIMITER "~~~"


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
			temp._question = std::string(argv[i]);
		}
		else if (column == CORRECT_ANSWER_COLUMN)
		{
			temp._correctAnswer = std::string(argv[i]);
		}
		else if (column == WRONG_ANSWERS_COLUMN)
		{
			tempWrongAnswer = strtok(argv[i], DELIMITER);
			while (tempWrongAnswer != NULL)
			{
				printf("%s\n", tempWrongAnswer);
				temp._wrongAnswers.push_back(tempWrongAnswer);
				tempWrongAnswer = strtok(NULL, DELIMITER);
			}
		}
	}

	static_cast<std::list<Question>*>(data)->push_back(temp);
	return 0;
}

/*
constructor
it checks if the data base file is initialized
and if there isn't a file, the function creates a new one
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
	}
	_usersRows = std::vector<SignupRequest>();
	_questionsRows = std::list<Question>();
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
distructor
frees allocated memory, the only new allocated memory in the class is the instance
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
	std::string query = "SELECT * FROM USERS WHERE NAME LIKE '" + name + "';";
	_usersRows.clear(); // remove the previous data
	std::lock_guard<std::mutex> locker(_using_db);
	sqlite3_exec(_db, query.c_str(), usersCallback, &_usersRows, nullptr);
	return !_usersRows.empty();
}

/*
function checks if a password matches to the username
input: username, password
output: true - the password matches the username, false - the password doesn't match the username
*/
bool SqliteDatabase::doesPasswordMatch(std::string name, std::string password)
{
	return doesUserExist(name) /*if the user exists*/
		&& _usersRows.front().password == password /*if exists, his data is aved in _rows, checks if the password matches*/;
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
	return true;
}

std::list<Question> SqliteDatabase::getQuestions(int amount)
{
	std::string query = "SELECT * FROM QUESTIONS;";
	_questionsRows.clear(); // remove the previous data

	refreshQuestions(amount); // refreshing DB questions

	std::lock_guard<std::mutex> locker(_using_db);
	sqlite3_exec(_db, query.c_str(), questionsCallback, &_questionsRows, nullptr);


	return std::list<Question>(_questionsRows);
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

	std::lock_guard<std::mutex> locker(_using_db); // python script accessing db

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