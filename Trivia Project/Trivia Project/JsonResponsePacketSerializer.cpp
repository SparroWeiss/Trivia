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
