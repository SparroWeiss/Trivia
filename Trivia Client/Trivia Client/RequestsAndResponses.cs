using System;
using System.Collections.Generic;

namespace Trivia_Client
{
    /**** Communication Structs and Data Types ****/

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
        HIGH_SCORES
    }

    enum ActiveMode
    {
        WAITING = 1, START_PLAYING, DONE
    }

    enum GameMode
    {
        FINISHED = 1, WAITING_FOR_PLAYERS
    };

    enum PlayerMode
    {
        LEFT = 1, PLAYING, WAITING_FOR_RESULTS
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

    struct Question
    {
        public uint id;
        public string question;
        public uint correct_id;
        public Dictionary<uint, string> answers;
    }

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
}