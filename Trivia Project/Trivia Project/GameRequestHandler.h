#pragma once
#include "RequestHandlerFactory.h"
#include "IRequestHandler.h"
#include "GameManager.h"
#include <ctime>

class RequestHandlerFactory;
class Game;
class GameManager;

class GameRequestHandler : public IRequestHandler
{
public:
	GameRequestHandler(LoggedUser user, Room* room);
	~GameRequestHandler();
	bool isRequestRelevant(RequestInfo);
	RequestResult handleRequest(RequestInfo);

private:
	Game* m_game;
	LoggedUser m_user;
	GameManager* m_gameManager;
	RequestHandlerFactory* m_handlerFactory;
	clock_t m_startTime;

	RequestResult getQuestion(RequestInfo);
	RequestResult submitAnswer(RequestInfo);
	RequestResult getGameResults(RequestInfo);
	RequestResult leaveGame(RequestInfo);
};