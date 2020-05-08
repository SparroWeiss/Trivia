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
