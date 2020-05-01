#pragma once
#include "IDataBase.h"

class SqliteDatabase : public IDataBase
{
public:
	SqliteDatabase();
	~SqliteDatabase();
	bool doesUserExist(std::string name);
	bool doesPasswordMatch(std::string password);
	bool addNewUser(std::string name, std::string password, std::string email);

private:

};