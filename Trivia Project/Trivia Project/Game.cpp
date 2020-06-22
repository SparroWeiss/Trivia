#include "Game.h"
#define NOT_A_QUESTION ""


/********** Question ************/
/*
Constructor:
Initializes the variables of the object
*/
Question::Question() : m_correctAnswer(0), m_question(NOT_A_QUESTION) {}

/*
Destructor:
frees allocated memory
*/
Question::~Question()
{
	m_possibleAnswers.clear();
}

/*
Function gets the question
Input: none 
Output: the question 
*/
std::string Question::getQuestion()
{
	return m_question;
}

/*
Function sets the question
Input: the question
Output: none
*/
void Question::setQuestion(std::string que)
{
	m_question = que;
}

/*
Function gets the possible answers
Input: none
Output: the question
*/
std::map<unsigned int, std::string> Question::getPossibleAnswers()
{
	return m_possibleAnswers;
}

/*
Function adds an answer to the possible answers
Input: new answer
Output: none
*/
void Question::addPossibleAnswers(std::string answer)
{
	curr_id++;
	m_possibleAnswers[curr_id] = answer;
}

/*
Function gets the correct answer id
Input: none
Output: the answer id
*/
unsigned int Question::getCorrectAnswer()
{
	return m_correctAnswer;
}

/*
Function sets the correct answer
Input: none
Output: the correct answer
*/
void Question::setCorrectAnswer(std::string correct)
{
	addPossibleAnswers(correct);
	m_correctAnswer = curr_id;
}


/*************** Game ***************/

std::mutex _mutex_players;

/*
Constructors:
Initializes the variables of the object
*/
Game::Game(){}
Game::Game(std::vector<LoggedUser> users, std::vector<Question> Questions)
{
	m_questions = Questions;
	for (std::vector<LoggedUser>::iterator i = users.begin(); i != users.end(); ++i)
	{
		m_players[(*i).getUsername()] = { m_questions.front(), 0, 0, 0, PlayerMode::PLAYING};
	}
}

/*
Destructor:
Frees allocated memory
*/
Game::~Game()
{
	m_players.clear();
	m_questions.clear();
}

/*
Function gets the question of the user
Input: the user
Ouput: the question
*/
Question Game::getQuestionForUser(LoggedUser& user)
{
	return m_players[user.getUsername()].currentQuestion;
}

/*
Function submits the user's answer
Input: answer id, the user, the game, the time it took to answer
Output: the id of the correct answer
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

	data.averageAnswerTime = (answerCount * data.averageAnswerTime + timeForAnswer) / (answerCount + 1);

	data.currentQuestion = getNextQuestion(data.currentQuestion);
	if (data.currentQuestion.getQuestion() == NOT_A_QUESTION) // finished all questions
	{ 
		data.playing = PlayerMode::WAITING_FOR_RESULTS;
	}

	locker.lock();
	m_players[user.getUsername()] = data;
	locker.unlock();
	return correct;
}

/*
Function removes a player
Input: the player
Output: true - removed, false - couldn't be removed
*/
bool Game::removePlayer(LoggedUser& user)
{
	std::lock_guard<std::mutex> locker(_mutex_players);
	m_players[user.getUsername()].playing = PlayerMode::LEFT;
	return true;
}

/*
Function gets the users data (helps for the game results function)
Input: none
Output: map of the users and their data
*/
std::map<std::string, GameData> Game::getUsersData()
{
	std::lock_guard<std::mutex> locker(_mutex_players);
	return m_players;
}

/*
Function gets the user amount
Input: none
Output: number of users
*/
unsigned int Game::getUsersAmount()
{
	unsigned int sum = 0;
	std::lock_guard<std::mutex> locker(_mutex_players);
	for (std::pair<std::string, GameData> curr : m_players)
	{
		if (curr.second.playing != PlayerMode::LEFT)
		{
			sum++;
		}
	}
	return sum;
}

/*
Function gets how many question there are in the game
Input: none
Output: the number of questions
*/
unsigned int Game::getNumOfQuestions()
{
	return m_questions.size();
}

/********* Helper **********/

/*
Function gets the next question in the game
Input: current question
Output: next question
*/
Question Game::getNextQuestion(Question current)
{
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
