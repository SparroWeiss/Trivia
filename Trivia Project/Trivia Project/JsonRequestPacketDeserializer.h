#pragma once
#include "Buffer.h"
#include "Communicator.h"
#include "json.hpp"

using json = nlohmann::json;

class Communicator;

struct 
{
	std::string username;
	std::string password;
}typedef LoginRequest;

struct
{
	std::string username;
	std::string password;
	std::string email;
	std::string address;
	std::string phone;
	std::string birthdate;
}typedef SignupRequest;

struct
{
	unsigned int roomId;
}typedef GetPlayersInRoomRequest;

struct
{
	unsigned int roomId;
}typedef JoinRoomRequest;

struct
{
	std::string roomName;
	unsigned int maxUsers;
	unsigned int questionCount;
	unsigned int answerTimeout;
}typedef CreateRoomRequest;

class JsonRequestPacketDeserializer
{
public:
	static LoginRequest deserializeLoginRequest(Buffer buff);
	static SignupRequest deserializeSignupRequest(Buffer buff);
	static GetPlayersInRoomRequest deserializeGetPlayersInRoomRequest(Buffer buff);
	static JoinRoomRequest deserializeJoinRoomRequest(Buffer buff);
	static CreateRoomRequest deserializeCreateRoomRequest(Buffer buff);

	static unsigned int bytesToInt(Buffer);
	static std::string bytesToString(Buffer);
};

void to_json(json&, const LoginRequest&);
void from_json(const json&, LoginRequest&);

void to_json(json&, const SignupRequest&);
void from_json(const json&, SignupRequest&);

void to_json(json&, const GetPlayersInRoomRequest&);
void from_json(const json&, GetPlayersInRoomRequest&);

void to_json(json&, const JoinRoomRequest&);
void from_json(const json&, JoinRoomRequest&);

void to_json(json&, const CreateRoomRequest&);
void from_json(const json&, CreateRoomRequest&);