#pragma once
#include "Buffer.h"
#include "Communicator.h"
#include "json.hpp"
#include "Room.h"
//#include <map>

using json = nlohmann::json;

class Comunicator;

#define BITSOFBYTE 8

struct
{
	std::string username;
	unsigned int correctAnswerCount;
	unsigned int wrongAnswerCount;
	unsigned int averageAnswerTime;
}typedef PlayerResults;


struct 
{
	std::string message;
}typedef ErrorResponse;

struct
{
	unsigned int status;
}typedef LoginResponse;

struct
{
	unsigned int status;
}typedef SignupResponse;

struct
{
	unsigned int status;
}typedef LogoutResponse;

struct
{
	unsigned int status;
	std::vector<RoomData> rooms;
}typedef GetRoomResponse;

struct
{
	std::vector<std::string> players;
}typedef GetPlayersInRoomResponse;

struct
{
	unsigned int status;
	std::vector<std::string> statiatics;
}typedef GetStatisticsResponse;

struct
{
	unsigned int status;
}typedef JoinRoomResponse;

struct
{
	unsigned int status;
}typedef CreateRoomResponse;

struct
{
	unsigned int status;
}typedef CloseRoomResponse;

struct
{
	unsigned int status;
}typedef StartGameResponse;

struct
{
	unsigned int status;
	bool hasGameBegun;
	std::vector<std::string> players;
	unsigned int questionCount;
	unsigned int answerTimeout;
}typedef GetRoomStateResponse;

struct
{
	unsigned int status;
}typedef LeaveRoomResponse;

struct
{
	unsigned int status;
}typedef LeaveGameResponse;

struct
{
	unsigned int status;
	std::string question;
	std::map<unsigned int, std::string> answers;
}typedef GetQuestionResponse;

struct
{
	unsigned int status;
	unsigned int correctAnswerId;
}typedef SubmitAnswerResponse;

struct
{
	unsigned int status;
	std::vector<PlayerResults> results;
}typedef GetGameResultsResponse;


class JsonResponsePacketSerializer
{
public:
	static Buffer serializeResponse(ErrorResponse error);
	static Buffer serializeResponse(LoginResponse login);
	static Buffer serializeResponse(SignupResponse signup);
	static Buffer serializeResponse(LogoutResponse logout);
	static Buffer serializeResponse(GetRoomResponse get_room);
	static Buffer serializeResponse(GetPlayersInRoomResponse get_players);
	static Buffer serializeResponse(JoinRoomResponse join_room);
	static Buffer serializeResponse(CreateRoomResponse create_room);
	static Buffer serializeResponse(GetStatisticsResponse get_statistics);
	static Buffer serializeResponse(CloseRoomResponse close_room);
	static Buffer serializeResponse(StartGameResponse start_game);
	static Buffer serializeResponse(GetRoomStateResponse get_room_state);
	static Buffer serializeResponse(LeaveRoomResponse leave_room);
	static Buffer serializeResponse(GetGameResultsResponse get_game_results);
	static Buffer serializeResponse(SubmitAnswerResponse submit_answer);
	static Buffer serializeResponse(GetQuestionResponse get_question);
	static Buffer serializeResponse(LeaveGameResponse leave_game);

	static Buffer intToBytes(int);
	static Buffer stringToBytes(std::string);
	static Buffer charToBytes(char*, unsigned int);
	static Buffer createResponseBuf(unsigned char, Buffer, Buffer);
};

void to_json(json&, const RoomData&);
void from_json(const json&, RoomData&);

void to_json(json&, const PlayerResults&);
void from_json(const json&, PlayerResults&);

void to_json(json&, const ErrorResponse&);
void from_json(const json&, ErrorResponse&);

void to_json(json&, const LoginResponse&);
void from_json(const json&, LoginResponse&);

void to_json(json&, const SignupResponse&);
void from_json(const json&, SignupResponse&);

void to_json(json&, const LogoutResponse&);
void from_json(const json&, LogoutResponse&);

void to_json(json&, const GetRoomResponse&);
void from_json(const json&, GetRoomResponse&);

void to_json(json&, const GetPlayersInRoomResponse&);
void from_json(const json&, GetPlayersInRoomResponse&);

void to_json(json&, const JoinRoomResponse&);
void from_json(const json&, JoinRoomResponse&);

void to_json(json&, const CreateRoomResponse&);
void from_json(const json&, CreateRoomResponse&);

void to_json(json&, const GetStatisticsResponse&);
void from_json(const json&, GetStatisticsResponse&);

void to_json(json&, const CloseRoomResponse&);
void from_json(const json&, CloseRoomResponse&);

void to_json(json&, const StartGameResponse&);
void from_json(const json&, StartGameResponse&);

void to_json(json&, const GetRoomStateResponse&);
void from_json(const json&, GetRoomStateResponse&);

void to_json(json&, const LeaveRoomResponse&);
void from_json(const json&, LeaveRoomResponse&);

void to_json(json&, const GetGameResultsResponse&);
void from_json(const json&, GetGameResultsResponse&);

void to_json(json&, const SubmitAnswerResponse&);
void from_json(const json&, SubmitAnswerResponse&);

void to_json(json&, const GetQuestionResponse&);
void from_json(const json&, GetQuestionResponse&);

void to_json(json&, const LeaveGameResponse&);
void from_json(const json&, LeaveGameResponse&);