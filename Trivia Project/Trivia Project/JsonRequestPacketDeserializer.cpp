#include "JsonRequestPacketDeserializer.h"

/******************** Json Helper Methods ********************/

/*
This is a helper function that enables direct assignment from 'LoginRequest' to 'json'.
Input: json and LoginRequest objects
Output: none
*/
void to_json(json& j, const LoginRequest& login)
{
	j = json{ {"username", login.username}, {"password", login.password} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'LoginRequest'.
Input: json and LoginRequest objects
Output: none
*/
void from_json(const json& j, LoginRequest& login)
{
	j.at("username").get_to(login.username);
	j.at("password").get_to(login.password);
}

/*
This is a helper function that enables direct assignment from 'SignupRequest' to 'json'.
Input: json and SignupRequest objects
Output: none
*/
void to_json(json& j, const SignupRequest& signup)
{
	j = json{ {"username", signup.username}, {"password", signup.password}, {"email", signup.email},
		{"address", signup.address}, {"phone", signup.phone}, {"birthdate", signup.birthdate} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'SignupRequest'.
Input: json and SignupRequest objects
Output: none
*/
void from_json(const json& j, SignupRequest& signup)
{
	j.at("username").get_to(signup.username);
	j.at("password").get_to(signup.password);
	j.at("email").get_to(signup.email);
	j.at("address").get_to(signup.address);
	j.at("phone").get_to(signup.phone);
	j.at("birthdate").get_to(signup.birthdate);
}

/*
This is a helper function that enables direct assignment from 'GetPlayersInRoomRequest' to 'json'.
Input: json and GetPlayersInRoomRequest objects
Output: none
*/
void to_json(json& j, const GetPlayersInRoomRequest& get_players)
{
	j = json{ {"roomId", get_players.roomId} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'GetPlayersInRoomRequest'.
Input: json and GetPlayersInRoomRequest objects
Output: none
*/
void from_json(const json& j, GetPlayersInRoomRequest& get_players)
{
	j.at("roomId").get_to(get_players.roomId);
}

/*
This is a helper function that enables direct assignment from 'JoinRoomRequest' to 'json'.
Input: json and JoinRoomRequest objects
Output: none
*/
void to_json(json& j, const JoinRoomRequest& join_room)
{
	j = json{ {"roomId", join_room.roomId} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'JoinRoomRequest'.
Input: json and JoinRoomRequest objects
Output: none
*/
void from_json(const json& j, JoinRoomRequest& join_room)
{
	j.at("roomId").get_to(join_room.roomId);
}

/*
This is a helper function that enables direct assignment from 'CreateRoomRequest' to 'json'.
Input: json and CreateRoomRequest objects
Output: none
*/
void to_json(json& j, const CreateRoomRequest& create_room)
{
	j = json{ {"roomName", create_room.roomName}, {"maxUsers", create_room.maxUsers},
		{"questionCount", create_room.questionCount}, {"answerTimeout", create_room.answerTimeout} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'CreateRoomRequest'.
Input: json and CreateRoomRequest objects
Output: none
*/
void from_json(const json& j, CreateRoomRequest& create_room)
{
	j.at("roomName").get_to(create_room.roomName);
	j.at("maxUsers").get_to(create_room.maxUsers);
	j.at("questionCount").get_to(create_room.questionCount);
	j.at("answerTimeout").get_to(create_room.answerTimeout);
}

/*
This is a helper function that enables direct assignment from 'SubmitAnswerRequest' to 'json'.
Input: json and SubmitAnswerRequest objects
Output: none
*/
void to_json(json& j, const SubmitAnswerRequest& submit_answer)
{
	j = { {"answerId", submit_answer.answerId}, {"time", submit_answer.time} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'SubmitAnswerRequest'.
Input: json and SubmitAnswerRequest objects
Output: none
*/
void from_json(const json& j, SubmitAnswerRequest& submit_answer)
{
	j.at("answerId").get_to(submit_answer.answerId);
	j.at("time").get_to(submit_answer.time);
}

/******************** Class Methods ********************/

/*
This method converts a 'Buffer' struct to a 'LoginRequest' struct.
Input: Buffer buff
Outuput: LoginRequest
*/
LoginRequest JsonRequestPacketDeserializer::deserializeLoginRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<LoginRequest>();
}

/*
This method converts a 'Buffer' struct to a 'SignupRequest' struct.
Input: Buffer buff
Outuput: SignupRequest
*/
SignupRequest JsonRequestPacketDeserializer::deserializeSignupRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<SignupRequest>();
}

/*
This method converts a 'Buffer' struct to a 'GetPlayersInRoomRequest' struct.
Input: Buffer buff
Outuput: GetPlayersInRoomRequest
*/
GetPlayersInRoomRequest JsonRequestPacketDeserializer::deserializeGetPlayersInRoomRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<GetPlayersInRoomRequest>();
}

/*
This method converts a 'Buffer' struct to a 'JoinRoomRequest' struct.
Input: Buffer buff
Outuput: JoinRoomRequest
*/
JoinRoomRequest JsonRequestPacketDeserializer::deserializeJoinRoomRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<JoinRoomRequest>();
}

/*
This method converts a 'Buffer' struct to a 'CreateRoomRequest' struct.
Input: Buffer buff
Outuput: CreateRoomRequest
*/
CreateRoomRequest JsonRequestPacketDeserializer::deserializeCreateRoomRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<CreateRoomRequest>();
}

/*
This method converts a 'Buffer' struct to a 'SubmitAnswerRequest' struct.
Input: Buffer buff
Outuput: SubmitAnswerRequest
*/
SubmitAnswerRequest JsonRequestPacketDeserializer::deserializeSubmitAnswerRequest(Buffer buff)
{
	json j = json::parse(bytesToString(buff));
	return j.get<SubmitAnswerRequest>();
}

/*
This helper method converts 'Buffer' type to 'int' type.
Input: Buffer
Output: int
*/
unsigned int JsonRequestPacketDeserializer::bytesToInt(Buffer buff)
{
	unsigned int x = 0;

	for (int i = 0; i < sizeof(int); i++)
	{
		x = x | (buff.m_buffer[i] << ((sizeof(int) - i - 1) * BITSOFBYTE));
	}

	return x;
}

/*
This helper method converts 'Buffer' type to 'string' type.
Input: Buffer
Output: string
*/
std::string JsonRequestPacketDeserializer::bytesToString(Buffer buff)
{
	return std::string(buff.m_buffer.begin(), buff.m_buffer.end());
}
