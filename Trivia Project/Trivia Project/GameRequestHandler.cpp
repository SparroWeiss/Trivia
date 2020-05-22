#include "GameRequestHandler.h"


std::mutex _mutex_game;


GameRequestHandler::GameRequestHandler(LoggedUser user, Room* room)
{
	m_user = user;
	m_gameManager = m_gameManager->getInstance();
	m_game = m_gameManager->createGame(*room);
	m_handlerFactory = m_handlerFactory->getInstance();
}

GameRequestHandler::~GameRequestHandler()
{
}

bool GameRequestHandler::isRequestRelevant(RequestInfo info)
{
	return info.id == LEAVEGAME ||
		info.id == GETQUESTION || 
		info.id == SUBMITANSWER || 
		info.id == GETGAMERESULTS;
}

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

RequestResult GameRequestHandler::getQuestion(RequestInfo info)
{
	GetQuestionResponse getQuestionRes = { };
	std::unique_lock<std::mutex> locker(_mutex_game);
	Question current = m_game->getQuestionForUser(m_user);
	locker.unlock();
	getQuestionRes.answers = current.getPossibleAnswers();
	getQuestionRes.question = current.getQuestion();
	getQuestionRes.status = 1;
	m_startTime = clock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getQuestionRes), this };
}

RequestResult GameRequestHandler::submitAnswer(RequestInfo info)
{
	SubmitAnswerRequest submitAnsReq = JsonRequestPacketDeserializer::deserializeSubmitAnswerRequest(info.buffer);
	SubmitAnswerResponse submitAnsRes;
	submitAnsRes.status = 1;
	submitAnsRes.correctAnswerId = m_game->submitAnswer(submitAnsReq.answerId,
		m_user, *m_game, float(clock() - m_startTime) / CLOCKS_PER_SEC);
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(submitAnsRes), this };
}

RequestResult GameRequestHandler::getGameResults(RequestInfo info)
{
	GetGameResultsResponse getGameRes = { 1 };
	std::unique_lock<std::mutex> locker(_mutex_game);
	std::map<std::string, GameData> results = m_game->getUsersData();
	for (std::map<std::string, GameData>::iterator i = results.begin(); i != results.end(); ++i)
	{
		getGameRes.results.push_back({ (*i).first,(*i).second.correctAnswersCount, (*i).second.wrongAnswersCount, (*i).second.averageAnswerTime });
	}
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(getGameRes), this };
}

RequestResult GameRequestHandler::leaveGame(RequestInfo info)
{
	IRequestHandler* newHandle = this; // if the leave game request isn't valid, stay in same handler
	LeaveGameResponse leaveGameRes = { 0 }; // status: 0
	std::unique_lock<std::mutex> locker(_mutex_game);
	if (m_game->removePlayer(m_user))
	{
		locker.unlock();
		leaveGameRes = { 1 };
		newHandle = m_handlerFactory->createMenuRequestHandler(m_user.getUsername()); // pointer to the previous handle : menu
	}
	else
		locker.unlock();
	return RequestResult{ JsonResponsePacketSerializer::serializeResponse(leaveGameRes), newHandle };
}