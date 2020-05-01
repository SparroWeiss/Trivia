#include "JsonRequestPacketDeserializer.h"

LoginRequest JsonRequestPacketDeserializer::deserializeLoginRequest(Buffer buff)
{
	return LoginRequest();
}

SignupRequest JsonRequestPacketDeserializer::deserializeRequest(Buffer buff)
{
	return SignupRequest();
}
