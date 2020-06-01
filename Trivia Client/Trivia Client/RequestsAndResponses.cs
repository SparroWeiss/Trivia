using System;
using System.Collections.Generic;

namespace Trivia_Client
{
    /********* Data Types *********/

    public enum Windows
    {
        ENTRY,
        LOGIN,
        SIGNUP,
        MENU,
        CREATE_ROOM,
        JOIN_ROOM,
        ROOM,
        STATISTICS,
        USER_STATISTICS,
        HIGH_SCORES,
        GAME
    }

    enum messageCode
    {
        ERRORCODE = 0,
        SIGNUPCODE,
        LOGINCODE,
        SIGNOUTCODE,
        GETROOMSCODE,
        GETPLAYERSINROOMCODE,
        GETSTATISTICSCODE,
        JOINROOMCODE,
        CREATEROOMCODE,
        CLOSEROOMCODE,
        STARTGAMECODE,
        GETROOMSTATECODE,
        LEAVEROOMCODE,
        GETGAMERESULTSCODE,
        SUBMITANSWERCODE,
        GETQUESTIONCODE,
        LEAVEGAMECODE
    };

    enum ActiveMode
    {
        WAITING = 1, START_PLAYING, DONE
    }

    enum GameMode
    {
        FINISHED = 1, WAITING_FOR_PLAYERS
    };

    struct RoomData
    {
        public uint id;
        public string name;
        public uint maxPlayers;
        public uint timePerQuestion;
        public uint isActive;
        public uint questionCount;
    }

    struct PlayerResults
    {
        public string username;
        public uint correctAnswerCount;
        public uint wrongAnswerCount;
        public float averageAnswerTime;
    }

    /**** Communication Structs ****/

    struct ErrorRes
    {
        public string message;
    }

    struct LoginReq
    {
        public string username;
        public string password;
    }

    struct LoginRes
    {
        public int status;
    }

    struct SignupReq
    {
        public string username;
        public string password;
        public string email;
        public string address;
        public string phone;
        public string birthdate;
    };
    
    struct SignupRes
    {
        public int status;
    }

    struct GetPlayersInRoomReq
    {
        public uint roomId;
    }

    struct GetPlayersInRoomRes
    {
        public List<string> players;
    }

    struct JoinRoomReq
    { 
        public uint roomId;
    }

    struct JoinRoomRes
    {
        public uint status;
    }

    struct CreateRoomReq
    {
        public string roomName;
        public uint maxUsers;
        public uint questionCount;
        public uint answerTimeout;
    }

    struct CreateRoomRes
    {
        public uint status;
    }
    
    struct LogoutRes
    {
        public uint status;
    }

    struct GetRoomsRes
    {
        public uint status;
        public List<RoomData> rooms;
    }

    struct GetStatisticsRes
    {
        public uint status;
        public List<string> statistics;
    }

    struct CloseRoomRes
    {
        public uint status;
    }

    struct StartGameRes
    {
        public uint status;
    }

    struct GetRoomStateRes
    {
        public uint status;
        public bool hasGameBegun;
        public List<string> players;
        public uint questionCount;
        public uint answerTimeout;
    }

    struct LeaveRoomRes
    {
        public uint status;
    }

    struct SubmitAnswerReq
    {
        public uint answerId;
        public float time;
    }

    struct SubmitAnswerRes
    {
        public uint status;
        public uint correctAnswerId;
    }

    struct LeaveGameRes
    {
        public uint status;
    }

    struct GetQuestionRes
    {
        public uint status;
        public string question;
        public Dictionary<uint, string> answers;
    }

    struct GetGameResultsRes
    {
        public uint status;
        public List<PlayerResults> results;
    }
}