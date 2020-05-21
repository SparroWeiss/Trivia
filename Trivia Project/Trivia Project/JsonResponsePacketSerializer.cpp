#include "JsonResponsePacketSerializer.h"

/******************** Json Helper Methods ********************/

/*
This is a helper function that enables direct assignment from 'ErrorResponse' to 'json'.
Input: json and ErrorResponse objects
Output: none
*/
void to_json(json& j, const ErrorResponse& error)
{
	j = json{ {"message", error.message} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'ErrorResponse'.
Input: json and ErrorResponse objects
Output: none
*/
void from_json(const json& j, ErrorResponse& error)
{
	j.at("message").get_to(error.message);
}

/*
This is a helper function that enables direct assignment from 'LoginResponse' to 'json'.
Input: json and LoginResponse objects
Output: none
*/
void to_json(json& j, const LoginResponse& login)
{
	j = json{ {"status", login.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'LoginResponse'.
Input: json and LoginResponse objects
Output: none
*/
void from_json(const json& j, LoginResponse& login)
{
	j.at("status").get_to(login.status);
}

/*
This is a helper function that enables direct assignment from 'SignupResponse' to 'json'.
Input: json and SignupResponse objects
Output: none
*/
void to_json(json& j, const SignupResponse& signup)
{
	j = json{ {"status", signup.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'SignupResponse'.
Input: json and SignupResponse objects
Output: none
*/
void from_json(const json& j, SignupResponse& signup)
{
	j.at("status").get_to(signup.status);
}

/*
This is a helper function that enables direct assignment from 'LogoutResponse' to 'json'.
Input: json and LogoutResponse objects
Output: none
*/
void to_json(json& j, const LogoutResponse& logout)
{
	j = json{ {"status", logout.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'LogoutResponse'.
Input: json and LogoutResponse objects
Output: none
*/
void from_json(const json& j, LogoutResponse& logout)
{
	j.at("status").get_to(logout.status);
}

/*
This is a helper function that enables direct assignment from 'RoomData' to 'json'.
Input: json and RoomData objects
Output: none
*/
void to_json(json& j, const RoomData& room_data)
{
	j = json{ {"id", room_data.id} , {"name", room_data.name}, {"maxPlayers", room_data.maxPlayers},
		{"timePerQuestion", room_data.timePerQuestion}, {"isActive", room_data.isActive}, {"questionCount", room_data.questionCount} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'RoomData'.
Input: json and RoomData objects
Output: none
*/
void from_json(const json& j, RoomData& room_data)
{
	j.at("id").get_to(room_data.id);
	j.at("name").get_to(room_data.name);
	j.at("maxPlayers").get_to(room_data.maxPlayers);
	j.at("timePerQuestion").get_to(room_data.timePerQuestion);
	j.at("isActive").get_to(room_data.isActive);
	j.at("questionCount").get_to(room_data.questionCount);
}

/*
This is a helper function that enables direct assignment from 'GetRoomResponse' to 'json'.
Input: json and GetRoomResponse objects
Output: none
*/
void to_json(json& j, const GetRoomResponse& get_room)
{
	j = json{ {"status", get_room.status} , {"rooms", get_room.rooms} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'GetRoomResponse'.
Input: json and GetRoomResponse objects
Output: none
*/
void from_json(const json& j, GetRoomResponse& get_room)
{
	j.at("status").get_to(get_room.status);
	j.at("rooms").get_to(get_room.rooms);
}

/*
This is a helper function that enables direct assignment from 'GetPlayersInRoomResponse' to 'json'.
Input: json and GetPlayersInRoomResponse objects
Output: none
*/
void to_json(json& j, const GetPlayersInRoomResponse& get_players)
{
	j = json{ {"players", get_players.players} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'GetPlayersInRoomResponse'.
Input: json and GetPlayersInRoomResponse objects
Output: none
*/
void from_json(const json& j, GetPlayersInRoomResponse& get_players)
{
	j.at("players").get_to(get_players.players);
}

/*
This is a helper function that enables direct assignment from 'JoinRoomResponse' to 'json'.
Input: json and JoinRoomResponse objects
Output: none
*/
void to_json(json& j, const JoinRoomResponse& join_room)
{
	j = json{ {"status", join_room.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'JoinRoomResponse'.
Input: json and JoinRoomResponse objects
Output: none
*/
void from_json(const json& j, JoinRoomResponse& join_room)
{
	j.at("status").get_to(join_room.status);
}

/*
This is a helper function that enables direct assignment from 'CreateRoomResponse' to 'json'.
Input: json and CreateRoomResponse objects
Output: none
*/
void to_json(json& j, const CreateRoomResponse& create_room)
{
	j = json{ {"status", create_room.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'CreateRoomResponse'.
Input: json and CreateRoomResponse objects
Output: none
*/
void from_json(const json& j, CreateRoomResponse& create_room)
{
	j.at("status").get_to(create_room.status);
}

/*
This is a helper function that enables direct assignment from 'GetStatisticsResponse' to 'json'.
Input: json and GetStatisticsResponse objects
Output: none
*/
void to_json(json& j, const GetStatisticsResponse& get_statistics)
{
	j = json{ {"status", get_statistics.status}, {"statistics", get_statistics.statiatics} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'GetStatisticsResponse'.
Input: json and GetStatisticsResponse objects
Output: none
*/
void from_json(const json& j, GetStatisticsResponse& get_statistics)
{
	j.at("status").get_to(get_statistics.status);
	j.at("statistics").get_to(get_statistics.statiatics);
}

/*
This is a helper function that enables direct assignment from 'CloseRoomResponse' to 'json'.
Input: json and CloseRoomResponse objects
Output: none
*/
void to_json(json& j, const CloseRoomResponse& close_room)
{
	j = json{ {"status", close_room.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'CloseRoomResponse'.
Input: json and CloseRoomResponse objects
Output: none
*/
void from_json(const json& j, CloseRoomResponse& close_room)
{
	j.at("status").get_to(close_room.status);
}

/*
This is a helper function that enables direct assignment from 'StartGameResponse' to 'json'.
Input: json and StartGameResponse objects
Output: none
*/
void to_json(json& j, const StartGameResponse& start_game)
{
	j = json{ {"status", start_game.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'StartGameResponse'.
Input: json and StartGameResponse objects
Output: none
*/
void from_json(const json& j, StartGameResponse& start_game)
{
	j.at("status").get_to(start_game.status);
}

/*
This is a helper function that enables direct assignment from 'GetRoomStateResponse' to 'json'.
Input: json and GetRoomStateResponse objects
Output: none
*/
void to_json(json& j, const GetRoomStateResponse& get_room_state)
{
	j = json{ {"status", get_room_state.status},
		{"answerTimeout", get_room_state.answerTimeout},
		{"hasGameBegun", get_room_state.hasGameBegun},
		{"players", get_room_state.players},
		{"questionCount", get_room_state.questionCount} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'GetRoomStateResponse'.
Input: json and GetRoomStateResponse objects
Output: none
*/
void from_json(const json& j, GetRoomStateResponse& get_room_state)
{
	j.at("status").get_to(get_room_state.status);
	j.at("answerTimeout").get_to(get_room_state.answerTimeout);
	j.at("hasGameBegun").get_to(get_room_state.hasGameBegun);
	j.at("players").get_to(get_room_state.players);
	j.at("questionCount").get_to(get_room_state.questionCount);
}

/*
This is a helper function that enables direct assignment from 'LeaveRoomResponse' to 'json'.
Input: json and LeaveRoomResponse objects
Output: none
*/
void to_json(json& j, const LeaveRoomResponse& leave_room)
{
	j = json{ {"status", leave_room.status} };
}
/*
This is a helper function that enables direct assignment from 'json' to 'LeaveRoomResponse'.
Input: json and LeaveRoomResponse objects
Output: none
*/
void from_json(const json& j, LeaveRoomResponse& leave_room)
{
	j.at("status").get_to(leave_room.status);
}

/******************** Class Methods ********************/

/*
This method converts an 'ErrorResponse' struct to a 'Buffer' struct.
Input: ErrorResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(ErrorResponse error)
{
	json errorJson = error;
	Buffer errorData = stringToBytes(errorJson.dump()), errorSize = intToBytes(errorJson.dump().size());

	return createResponseBuf(ERRORCODE, errorSize, errorData);
}

/*
This method converts a 'LoginResponse' struct to a 'Buffer' struct.
Input: LoginResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(LoginResponse login)
{
	json loginJson = login;
	Buffer loginData = stringToBytes(loginJson.dump()), loginSize = intToBytes(loginJson.dump().size());

	return createResponseBuf(LOGINCODE, loginSize, loginData);
}

/*
This method converts a 'SignupResponse' struct to a 'Buffer' struct.
Input: SignupResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(SignupResponse signup)
{
	json signupJson = signup;
	Buffer signupData = stringToBytes(signupJson.dump()), signupSize = intToBytes(signupJson.dump().size());

	return createResponseBuf(SIGNUPCODE, signupSize, signupData);
}

/*
This method converts an 'LogoutResponse' struct to a 'Buffer' struct.
Input: LogoutResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(LogoutResponse logout)
{
	json logoutJson = logout;
	Buffer logoutData = stringToBytes(logoutJson.dump()), logoutSize = intToBytes(logoutJson.dump().size());

	return createResponseBuf(SIGNOUT, logoutSize, logoutData);
}

/*
This method converts an 'GetRoomResponse' struct to a 'Buffer' struct.
Input: GetRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(GetRoomResponse get_room)
{
	json get_roomJson = get_room;
	Buffer get_roomData = stringToBytes(get_roomJson.dump()), get_roomSize = intToBytes(get_roomJson.dump().size());

	return createResponseBuf(GETROOMS, get_roomSize, get_roomData);
}

/*
This method converts an 'GetPlayersInRoomResponse' struct to a 'Buffer' struct.
Input: GetPlayersInRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(GetPlayersInRoomResponse get_players)
{
	json get_playersJson = get_players;
	Buffer get_playersData = stringToBytes(get_playersJson.dump()), get_playersSize = intToBytes(get_playersJson.dump().size());

	return createResponseBuf(GETPLAYERSINROOM, get_playersSize, get_playersData);
}

/*
This method converts an 'JoinRoomResponse' struct to a 'Buffer' struct.
Input: JoinRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(JoinRoomResponse join_room)
{
	json join_roomJson = join_room;
	Buffer join_roomData = stringToBytes(join_roomJson.dump()), join_roomSize = intToBytes(join_roomJson.dump().size());

	return createResponseBuf(JOINROOM, join_roomSize, join_roomData);
}

/*
This method converts an 'CreateRoomResponse' struct to a 'Buffer' struct.
Input: CreateRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(CreateRoomResponse create_room)
{
	json create_roomJson = create_room;
	Buffer create_roomData = stringToBytes(create_roomJson.dump()), create_roomSize = intToBytes(create_roomJson.dump().size());

	return createResponseBuf(CREATEROOM, create_roomSize, create_roomData);
}

/*
This method converts an 'GetStatisticsResponse' struct to a 'Buffer' struct.
Input: GetStatisticsResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(GetStatisticsResponse get_statistics)
{
	json get_statisticsJson = get_statistics;
	Buffer get_statisticsData = stringToBytes(get_statisticsJson.dump()), get_statisticsSize = intToBytes(get_statisticsJson.dump().size());

	return createResponseBuf(GETSTATISTICS, get_statisticsSize, get_statisticsData);
}

/*
This method converts an 'CloseRoomResponse' struct to a 'Buffer' struct.
Input: CloseRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(CloseRoomResponse close_room)
{
	json close_roomJson = close_room;
	Buffer close_roomData = stringToBytes(close_roomJson.dump()), close_roomSize = intToBytes(close_roomJson.dump().size());

	return createResponseBuf(CLOSEROOM, close_roomSize, close_roomData);
}

/*
This method converts an 'StartGameResponse' struct to a 'Buffer' struct.
Input: StartGameResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(StartGameResponse start_game)
{
	json start_gameJson = start_game;
	Buffer start_gameData = stringToBytes(start_gameJson.dump()), start_gameSize = intToBytes(start_gameJson.dump().size());

	return createResponseBuf(CLOSEROOM, start_gameSize, start_gameData);
}

/*
This method converts an 'GetRoomStateResponse' struct to a 'Buffer' struct.
Input: GetRoomStateResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(GetRoomStateResponse get_room_state)
{
	json get_room_stateJson = get_room_state;
	Buffer get_room_stateData = stringToBytes(get_room_stateJson.dump()), get_room_stateSize = intToBytes(get_room_stateJson.dump().size());

	return createResponseBuf(CLOSEROOM, get_room_stateSize, get_room_stateData);
}

/*
This method converts an 'LeaveRoomResponse' struct to a 'Buffer' struct.
Input: LeaveRoomResponse
Outuput: Buffer
*/
Buffer JsonResponsePacketSerializer::serializeResponse(LeaveRoomResponse leave_room)
{
	json leave_roomJson = leave_room;
	Buffer leave_roomData = stringToBytes(leave_roomJson.dump()), leave_roomSize = intToBytes(leave_roomJson.dump().size());

	return createResponseBuf(LEAVEROOM, leave_roomSize, leave_roomData);
}

/*
This helper method converts 'int' type to 'Buffer' type.
Input: int
Output: Buffer
*/
Buffer JsonResponsePacketSerializer::intToBytes(int x)
{
	Buffer bytesOfInt;

	bytesOfInt.m_buffer.resize(sizeof(int));

	for (int i = 0; i < sizeof(int); i++)
	{
		bytesOfInt.m_buffer[sizeof(int) - i - 1] = (x >> (i * BITSOFBYTE));
	}

	return bytesOfInt;
}

/*
This helper method converts 'string' type to 'Buffer' type.
Input: string
Output : Buffer
*/
Buffer JsonResponsePacketSerializer::stringToBytes(std::string s)
{
	Buffer bytesOfString;

	bytesOfString.m_buffer = std::vector<byte>(s.begin(), s.end());

	return bytesOfString;
}

/*
This helper method converts 'char*' type to 'Buffer' type.
Input: char*
Output : Buffer
*/
Buffer JsonResponsePacketSerializer::charToBytes(char* ch, unsigned int size)
{
	Buffer bytesOfChar;

	for (int i = 0; i < size; i++)
	{
		bytesOfChar.m_buffer.push_back(ch[i]);
	}

	return bytesOfChar;
}

/*
This helper method creates a response buffer.
Input: response code, response data size, response data
Output : Buffer
*/
Buffer JsonResponsePacketSerializer::createResponseBuf(unsigned char responseCode, Buffer responseSize, Buffer responseData)
{
	Buffer responseBuf;

	responseBuf.m_buffer.push_back(responseCode);
	responseBuf.m_buffer.insert(responseBuf.m_buffer.end(), responseSize.m_buffer.begin(), responseSize.m_buffer.end());
	responseBuf.m_buffer.insert(responseBuf.m_buffer.end(), responseData.m_buffer.begin(), responseData.m_buffer.end());

	return responseBuf;
}
