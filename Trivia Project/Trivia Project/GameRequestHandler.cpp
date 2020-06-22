#include "GameRequestHandler.h"


std::mutex _mutex_game;

/*
Donstructor:
Initializes the variables of the object
*/
GameRequestHandler::GameRequestHandler(LoggedUser user, Room* room)
{
	m_user = user;
	m_gameManager = m_gameManager->getInstance();
	m_game = room->getGame();
	m_handlerFactory = m_handlerFactory->getInstance();
	m_updateStatistics = false;
}

/*
Destructor
*/
GameRequestHandler::~GameRequestHandler() {}

/*
Function checks if a request is relevant to the handler
Input : request info
Output : true - request is relevant, false - request isn't relevant
*/
bool GameRequestHandler::isRequestRelevant(RequestInfo info)
{
	return info.id == LEAVEGAME ||
		info.id == GETQUESTION || 
		info.id == SUBMITANSWER || 
		info.id == GETGAMERESULTS;
}

/*
Function gets the result of a request
Input: request info
Output: request result
*/
RequestResult GameRequestHandler::handleRequest(RequestInfo info)
{
	switch (info.id)
	{
	case LEAVEGAME:
		return leaveGame(info);
	case GETQUESTION:
		return getQuestion(info);
	case SUBMITANSWER:
		return submitAnswer(info);
	case GETGAMERESULTS:
		return getGameResults(info);
	default:
		return RequestResult();
	}
}

/*
Function gets the question of the user
Input: request info
Output: request result
*/
RequestResult GameRequestHandler::getQuestion(RequestInfo info)
{
	GetQuestionResponse getQuestionRes = { };
	std::unique_lock<std::mutex> locker(_mutex_game);
	Question current = m_game->getQuestionForUser(m_user);
	locker.unlock();
	getQuestionRes.answers = current.getPossibleAnswers();
	getQuestionRes.question = current.getQuestion();
	getQuestionRes.status = 1;
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getQuestionRes), this };
}

/*
Function submits a user's answer
Input: request info
Output: request result
*/
RequestResult GameRequestHandler::submitAnswer(RequestInfo info)
{
	SubmitAnswerRequest submitAnsReq = JsonRequestPacketDeserializer::deserializeSubmitAnswerRequest(info.buffer);
	SubmitAnswerResponse submitAnsRes;
	submitAnsRes.status = 1;
	std::unique_lock<std::mutex> locker(_mutex_game);
	submitAnsRes.correctAnswerId = m_game->submitAnswer(submitAnsReq.answerId,
		m_user, submitAnsReq.time);
	locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(submitAnsRes), this };
}

/*
Function gets the results of the game
Input: request info
Output: request result
*/
RequestResult GameRequestHandler::getGameResults(RequestInfo info)
{
	GetGameResultsResponse getGameRes = { GameMode::FINISHED };

	std::unique_lock<std::mutex> locker(_mutex_game);
	std::map<std::string, GameData> results = m_game->getUsersData();

	if (!m_updateStatistics)
	{
		m_gameManager->updateUserStatistics(m_game, m_user.getUsername());
		m_updateStatistics = true;
	}
	locker.unlock();

	for (std::map<std::string, GameData>::iterator i = results.begin(); i != results.end(); ++i)
	{
		if ((*i).second.playing == PlayerMode::PLAYING)
		{
			getGameRes.status = GameMode::WAITING_FOR_PLAYERS;
		}
		getGameRes.results.push_back({ i->first, i->second.correctAnswersCount, i->second.wrongAnswersCount, i->second.averageAnswerTime });
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getGameRes), this };
}

/*
Function signs out a user from the game
Input: request info
Output: request result
*/
RequestResult GameRequestHandler::leaveGame(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the leave game request isn't valid, stay in same handler
	LeaveGameResponse leaveGameRes = { 0 }; // status: 0

	std::unique_lock<std::mutex> locker(_mutex_game);
	if (m_game->removePlayer(m_user))
	{
		if (!m_updateStatistics)
		{
			m_gameManager->updateUserStatistics(m_game, m_user.getUsername());
			m_updateStatistics = true;
		}
		if (m_game->getUsersAmount() == 0) // If all players left the game
		{
			m_gameManager->deleteGame(m_game); // delete the game
		}
		locker.unlock();
		leaveGameRes = { 1 };
		newHandle = m_handlerFactory->createMenuRequestHandler(m_user.getUsername()); // pointer to the previous handle : menu
	}
	else
		locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(leaveGameRes), newHandle };
}
