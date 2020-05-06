#pragma once
#include "IDataBase.h"
#include "JsonRequestPacketDeserializer.h"
#include "sqlite3.h"
#include <vector>
#include <io.h>
#include <string>

class SqliteDatabase : public IDataBase
{
public:
	SqliteDatabase();
	~SqliteDatabase();
	bool doesUserExist(std::string name);
	bool doesPasswordMatch(std::string name, std::string password);
	bool addNewUser(std::string name, std::string password, std::string email);

private:
	sqlite3* _db;
	std::vector<SignupRequest> _rows;
	friend int callback(void* data, int size, char** argv, char** colName);
	
	std::mutex _using_db;
};