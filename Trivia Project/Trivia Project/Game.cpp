#include "Game.h"

///////////////////////////////////////Question
/*
constructor
initializes the variables of the object
*/
Question::Question() : m_correctAnswer(0), m_question("") {}

/*
destructor
frees allocated memory
*/
Question::~Question()
{
	m_possibleAnswers.clear();
}

/*
function gets the question
input: none 
output: the question 
*/
std::string Question::getQuestion()
{
	return m_question;
}

/*
function sets the question
input: the question
output: none
*/
void Question::setQuestion(std::string que)
{
	m_question = que;
}

/*
function gets the possible answers
input: none
output: the question
*/
std::map<unsigned int, std::string> Question::getPossibleAnswers()
{
	return m_possibleAnswers;
}

/*
function adds an answer to the possible answers
input: new answer
output: none
*/
void Question::addPossibleAnswers(std::string answer)
{
	curr_id++;
	m_possibleAnswers[curr_id] = answer;
}

/*
function gets the correct answer id
input: none
output: the answer id
*/
unsigned int Question::getCorrectAnswer()
{
	return m_correctAnswer;
}

/*
function sets the correct answer
input: none
output: the correct answer
*/
void Question::setCorrectAnswer(std::string correct)
{
	addPossibleAnswers(correct);
	m_correctAnswer = curr_id;
}


////////////////////////////////////////Game

std::mutex _mutex_players;

/*
constructors
initializes the variables of the object
*/
Game::Game(){}
Game::Game(std::vector<LoggedUser> users, std::vector<Question> Questions)
{
	m_questions = Questions;
	for (std::vector<LoggedUser>::iterator i = users.begin(); i != users.end(); ++i)
	{
		m_players[(*i).getUsername()] = { m_questions[0], 0, 0, 0 };
	}
}

/*
destructor
frees allocated memory
*/
Game::~Game()
{
	m_players.clear();
	m_questions.clear();
}

/*
function gets the question of the user
input: the user
ouput: the question
*/
Question Game::getQuestionForUser(LoggedUser& user)
{
	return m_players[user.getUsername()].currentQuestion;
}

/*
function submits the user's answer
input: answer id, the user, the game, the time it took to answer
output: the id of the correct answer
*/
unsigned int Game::submitAnswer(unsigned int answerId, LoggedUser& user, float timeForAnswer)
{
	unsigned int correct = this->getQuestionForUser(user).getCorrectAnswer();
	std::unique_lock<std::mutex> locker(_mutex_players);
	GameData data = m_players[user.getUsername()];
	locker.unlock();
	unsigned int answerCount = data.wrongAnswersCount + data.correctAnswersCount;
	if (answerId == correct)
		data.correctAnswersCount++;
	else
		data.wrongAnswersCount++;

	data.averageAnswerTime = (unsigned int)((answerCount * data.averageAnswerTime + timeForAnswer) / (answerCount + 1));

	locker.lock();
	data.currentQuestion = getNextQuestion(m_players[user.getUsername()].currentQuestion);
	m_players[user.getUsername()] = data;
	locker.unlock();
	return correct;
}

/*
function removes a player
input: the player
output: true - removed, false - couldn't be removed
*/
bool Game::removePlayer(LoggedUser& user)
{
	std::lock_guard<std::mutex> locker(_mutex_players);
	return m_players.erase(user.getUsername());
}

/*
function gets the users data (helps for the game results function)
input: none
output: map of the users and their data
*/
std::map<std::string, GameData> Game::getUsersData()
{
	std::lock_guard<std::mutex> locker(_mutex_players);
	return m_players;
}

unsigned int Game::getUsersAmount()
{
	return m_players.size();
}

/////////////////////////HELPER

/*
function gets the next question in the game
input: current question
output: next question
*/
Question Game::getNextQuestion(Question current)
{
	if (current.getQuestion() == "")
	{
		return m_questions.front(); // get first question
	}
	for (std::vector<Question>::iterator i = m_questions.begin(); i != m_questions.end(); ++i)
	{
		if (i->getQuestion() == current.getQuestion())
		{
			++i;
			if (i == m_questions.end())
			{
				return Question(); // finished all the questions
			}
			return (*i);
		}
	}
	return Question();
}
