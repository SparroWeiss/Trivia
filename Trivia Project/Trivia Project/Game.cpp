#include "Game.h"

Question::Question() : m_correctAnswer(0), m_question("") {}

Question::~Question()
{
	m_possibleAnswers.clear();
}

std::string Question::getQuestion()
{
	return m_question;
}

void Question::setQuestion(std::string que)
{
	m_question = que;
}

std::map<unsigned int, std::string> Question::getPossibleAnswers()
{
	return m_possibleAnswers;
}

void Question::addPossibleAnswers(std::string answer)
{
	curr_id++;
	m_possibleAnswers[curr_id] = answer;
}

unsigned int Question::getCorrectAnswer()
{
	return m_correctAnswer;
}

void Question::setCorrectAnswer(std::string correct)
{
	addPossibleAnswers(correct);
	m_correctAnswer = curr_id;
}


////////////////////////////////////////Game

/*
constructor
initializes the variables of the object
*/
Game::Game(std::vector<LoggedUser> users, std::vector<Question> Questions)
{
	m_questions = Questions;
	for (std::vector<LoggedUser>::iterator i = users.begin(); i != users.end(); ++i)
	{
		m_players[(*i).getUsername()] = GameData();
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
	GameData * temp = &(m_players.find(user.getUsername()))->second;
	temp->currentQuestion = getNextQuestion(temp->currentQuestion);
	return temp->currentQuestion;
}

/*
function submits the user's answer
input: answer id, the user, the game, the time it took to answer
output: the id of the correct answer
*/
unsigned int Game::submitAnswer(unsigned int answerId, LoggedUser& user, Game game, float timeForAnswer)
{
	unsigned int correct = game.getQuestionForUser(user).getCorrectAnswer();
	GameData data = m_players[user.getUsername()];
	unsigned int answerCount = data.wrongAnswersCount + data.correctAnswersCount;
	if (answerId == correct)
		data.correctAnswersCount++;
	else
		data.wrongAnswersCount++;

	data.averageAnswerTime = (unsigned int)((answerCount * data.averageAnswerTime + timeForAnswer) / (answerCount + 1));

	m_players[user.getUsername()] = data;
	return correct;
}

/*
function removes a player
input: the player
output: true - removed, false - couldn't be removed
*/
bool Game::removePlayer(LoggedUser& user)
{
	m_players.erase(m_players.find(user.getUsername()));
	return true;
}

std::map<std::string, GameData> Game::getUsersData()
{
	return m_players;
}

/////////////////////////HELPER

Question Game::getNextQuestion(Question current)
{
	if (current.getQuestion() == "")
	{
		return m_questions.front(); // get first question
	}
	for (std::vector<Question>::iterator i = m_questions.begin(); i != m_questions.end(); ++i)
	{
		if ((*i).getQuestion() == current.getQuestion())
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
