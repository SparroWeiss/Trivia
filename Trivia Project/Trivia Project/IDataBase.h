#pragma once
#include <iostream>
#include <list>
#include <string>
#include <vector>

struct
{
	std::string _question;
	std::string _correctAnswer;
	std::vector<std::string> _wrongAnswers;
} typedef Question;

class IDataBase
{
public:
	virtual bool doesUserExist(std::string name) = 0;
	virtual bool doesPasswordMatch(std::string name, std::string password) = 0;
	virtual bool addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate) = 0;
	virtual std::list<Question> getQuestions(int) = 0;
};
