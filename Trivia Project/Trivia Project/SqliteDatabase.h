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
	static SqliteDatabase* getInstance();
	~SqliteDatabase();
	bool doesUserExist(std::string name);
	bool doesPasswordMatch(std::string name, std::string password);
	bool addNewUser(std::string name, std::string password, std::string email, std::string address, std::string phone, std::string birthdate);

private:
	SqliteDatabase();
	static SqliteDatabase* instance;
	static int instances;

	sqlite3* _db;
	std::vector<SignupRequest> _rows;
	friend int callback(void* data, int size, char** argv, char** colName);
	
	std::mutex _using_db;
};
