﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using Newtonsoft.Json;

namespace Trivia_Client
{
    class Communicator
    {
        private string _serverIp;
        private int _serverPort;
        private const string CONFIG_PATH = "config.txt";
        private Socket _serverSocket;

        /*
        constructor:
        get the server ip and port from the config file
        connect to the server
        */
        public Communicator()
        {
            // get the server ip and port from the config file
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C python Resources\\getServer.py",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();

            List<string> address = new List<string>();
            while (!process.StandardOutput.EndOfStream)
            { // reading what the python file is printing
                address.Add(process.StandardOutput.ReadLine());
                // address[0] = ip, address[1] = port
            }
            _serverIp = address[0];
            _serverPort = Int32.Parse(address[1]);
            
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] iPs = Dns.GetHostAddresses(_serverIp);

            try
            {
                _serverSocket.Connect(iPs, _serverPort);
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect to server.\n Do you wish to try again?");
            }
        }

        /*
        destructor - close the socket with the server
        */
        ~Communicator()
        {
            if (_serverSocket != null)
            {
                _serverSocket.Close();
            }
        }

        /*
        this function build the message and send it to the server
        input: the code of the message and the message
        output: none
        */
        public void send_data(messageCode code, string msg = "")
        {
            byte[] buffer = new byte[sizeof(byte) + sizeof(int) + msg.Length];
            buffer[0] = BitConverter.GetBytes((int)code)[0]; // add the message code to the message

            byte[] msg_len = BitConverter.GetBytes((int)msg.Length);
            Array.Reverse(msg_len);
            System.Buffer.BlockCopy(msg_len, 0, buffer, sizeof(byte), sizeof(int)); // add the message length to the message

            // add the message to the message for the server
            System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(msg), 0, buffer, sizeof(byte) + sizeof(int), msg.Length); 

            try
            {
                _serverSocket.Send(buffer);
            }
            catch (Exception)
            {
                throw new Exception("Lost connection to the server :(");
            }
        }

        /*
        this function is using template, it receive message from the server and deserialize it
        input: none
        output: the deserialized message by it`s template
        */
        public Res recv_data<Res>()
        {
            byte[] code, msg;
            try
            {
                code = new byte[sizeof(byte)];
                _serverSocket.Receive(code); // receive the message code

                byte[] msg_len = new byte[sizeof(int)];
                _serverSocket.Receive(msg_len); // receive the message length
                Array.Reverse(msg_len);

                msg = new byte[BitConverter.ToInt32(msg_len, 0)];
                _serverSocket.Receive(msg); // receive the message
            }
            catch (Exception)
            {
                throw new Exception("Lost connection to the server :(");
            }

            if (code[0] == (byte)messageCode.ERRORCODE)
            {
                string server_msg = "";
                try
                {
                    server_msg = JsonConvert.DeserializeObject<ErrorRes>(System.Text.Encoding.ASCII.GetString(msg)) + " :(";
                }
                catch
                {
                    throw new Exception("Lost connection to the server :(");
                }
                throw new Exception(server_msg);
            }
            else if (code[0] == (byte)messageCode.GETQUESTIONCODE)
            {
                string message = System.Text.Encoding.ASCII.GetString(msg);

                message = message.Replace("[[", "{").Replace("]]", "}");
                message = message.Replace("],[", ",");
                message = message.Replace(",\"", ":\"").Replace("}:\"", "},\"").Replace("\":\"status\"", "\",\"status\"");

                return JsonConvert.DeserializeObject<Res>(message);
            }

            return JsonConvert.DeserializeObject<Res>(System.Text.Encoding.ASCII.GetString(msg));
        }

        /*
        this function send a login request to the server
        input: the username and the password
        output: did login or not (true \ false)
        */
        public LoginStatus login(string username, string password)
        {
            LoginReq login;
            login.username = username;
            login.password = password;
            
            send_data(messageCode.LOGINCODE, JsonConvert.SerializeObject(login));
            LoginRes result = recv_data<LoginRes>();
            return (LoginStatus)result.status; 
        }

        /*
        this function send a signup request to the server
        input: the username, password, email, address, phone and birth date
        output: did signup or not (true \ false)
        */
        public int signup(string username, string password, string email, string address, string phone, string birthdate)
        {
            SignupReq signup;
            signup.username = username;
            signup.password = password;
            signup.email = email;
            signup.address = address;
            signup.phone = phone;
            signup.birthdate = birthdate;

            send_data(messageCode.SIGNUPCODE, JsonConvert.SerializeObject(signup));
            SignupRes result = recv_data<SignupRes>();
            return (int)result.status;
        }

        /*
        this function send a logout request to the server
        input: none
        output: did logout or not (true \ false)
        */
        public bool logout()
        {
            send_data(messageCode.SIGNOUTCODE);
            LogoutRes result = recv_data<LogoutRes>();
            return (result.status == 1);
        }

        /*
        this function send a get user statistics request to the server
        input: none
        output: list of the user statistics
        */
        public List<string> getUserStatistics()
        {
            send_data(messageCode.GETSTATISTICSCODE);
            GetStatisticsRes result = recv_data<GetStatisticsRes>();
            if(result.status == 1)
            {
                return result.statistics.Skip(1).Take(5).ToList<string>(); // delete not important information
            }
            else
            {
                return null;
            }
        }

        /*
        this function send a get high scores request to the server
        input: none
        output: dictionary of the top 5 users and their score
        */
        public Dictionary<string, string> getHighScores()
        {
            send_data(messageCode.GETSTATISTICSCODE);
            GetStatisticsRes result = recv_data<GetStatisticsRes>();
            if (result.status == 1)
            {
                result.statistics = result.statistics.Skip(6).ToList<string>(); // delete not important information
                Dictionary<string, string> res = new Dictionary<string, string>();

                // insert the keys and values to the dictionary
                for (int i = 0; i < result.statistics.Count(); i++)
                {
                    res.Add(result.statistics[i], result.statistics[++i]);
                }

                return res;
            }
            else
            {
                return null;
            }
        }

        /*
        this function send a create room request to the server
        input: the room name, maximum users, number of question and time for question
        output: did create room or not (true \ false)
        */
        public bool createRoom(string roomName, string maxUsers, string questionCount, string answerTimeout)
        {
            CreateRoomReq createRoom;
            createRoom.roomName = roomName;

            try
            {
                // check if the input is correct
                createRoom.maxUsers = UInt32.Parse(maxUsers);
                createRoom.questionCount = UInt32.Parse(questionCount);
                createRoom.answerTimeout = UInt32.Parse(answerTimeout);
                if (roomName == "" || createRoom.maxUsers <= 0 || createRoom.questionCount <= 0 || createRoom.answerTimeout <= 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            send_data(messageCode.CREATEROOMCODE, JsonConvert.SerializeObject(createRoom));
            CreateRoomRes result = recv_data<CreateRoomRes>();
            return (result.status == 1);
        }

        /*
        this function send a join room request to the server
        input: the room name
        output: did join room or not (true \ false)
        */
        public bool joinRoom(string roomName)
        {
            JoinRoomReq joinRoom;
            joinRoom.roomId = 0;
            bool foundRoom = false;

            string[] delimeter = { " >>> (" };

            roomName = roomName.Split(delimeter, StringSplitOptions.RemoveEmptyEntries).First();

            // get the room id
            foreach (RoomData r in getAvailableRooms())
            {
                if (r.name == roomName)
                {
                    joinRoom.roomId = r.id;
                    foundRoom = true;
                    break;
                }
            }

            if (foundRoom)
            {
                send_data(messageCode.JOINROOMCODE, JsonConvert.SerializeObject(joinRoom));
                JoinRoomRes result = recv_data<JoinRoomRes>();
                return (result.status == 1);
            }

            else
            {
                return false;
            }
        }

        /*
        this function get the room data from the server
        input: the room name
        output: the room data
        */
        public RoomData getRoomData(string roomName)
        {
            foreach (RoomData r in getAvailableRooms())
            {
                if (r.name == roomName)
                {
                    return r;
                }
            }

            return new RoomData();
        }

        /*
        this function send a get availabe rooms request to the server
        input: none
        output: list of each room data
        */
        public List<RoomData> getAvailableRooms()
        {
            List<RoomData> availableRooms = new List<RoomData>();

            send_data(messageCode.GETROOMSCODE);
            GetRoomsRes result = recv_data<GetRoomsRes>();
            
            foreach(RoomData room in result.rooms)
            {
                if (room.isActive == (uint)ActiveMode.WAITING && room.maxPlayers != getPlayersInRoom(room.id))
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }

        /*
        this function send a get players in a room request to the server
        input: the room id
        output: number of players
        */
        public int getPlayersInRoom(uint room_id)
        {
            GetPlayersInRoomReq getPlayers;
            getPlayers.roomId = room_id;

            send_data(messageCode.GETPLAYERSINROOMCODE, JsonConvert.SerializeObject(getPlayers));
            GetPlayersInRoomRes result = recv_data<GetPlayersInRoomRes>();

            return result.players.Count();
        }

        /*
        this function get the room admin from the server
        input: none
        output: the room admin
        */
        public string getRoomAdmin()
        {
            return getRoomState().players[0];
        }

        /*
        this function send a get room state request to the server
        input: none
        output: the room state
        */
        public GetRoomStateRes getRoomState()
        {
            send_data(messageCode.GETROOMSTATECODE);
            GetRoomStateRes result = recv_data<GetRoomStateRes>();
            return result;
        }

        /*
        this function send a close room request to the server
        input: none
        output: did close room or not (true \ false)
        */
        public bool closeRoom()
        {
            send_data(messageCode.CLOSEROOMCODE);
            CloseRoomRes result = recv_data<CloseRoomRes>();
            return (result.status == 1);
        }

        /*
        this function send a leave room request to the server
        input: none
        output: did leave room or not (true \ false)
        */
        public bool leaveRoom()
        {
            send_data(messageCode.LEAVEROOMCODE);
            LeaveRoomRes result = recv_data<LeaveRoomRes>();
            return (result.status == 1);
        }

        /*
        this function send a start game request to the server
        input: none
        output: did start game or not (true \ false)
        */
        public bool startGame()
        {
            send_data(messageCode.STARTGAMECODE);
            StartGameRes result = recv_data<StartGameRes>();
            return (result.status == 1);
        }

        /*
        this function send a submit answer request to the server
        input: the client answer id and the time 
        output: the id of the correct answer, zero if request faild
        */
        public uint submitAnswer(uint answer_id, float time)
        {
            SubmitAnswerReq submitAnswer;
            submitAnswer.answerId = answer_id;
            submitAnswer.time = time;

            send_data(messageCode.SUBMITANSWERCODE, JsonConvert.SerializeObject(submitAnswer));
            SubmitAnswerRes result = recv_data<SubmitAnswerRes>();
            if(result.status == 0)
            {
                return 0;
            }
            return result.correctAnswerId;
        }

        /*
        this function send a get game results request to the server
        input: none
        output: list of the players result sorted by the score, empty list if request faild
        */
        public List<PlayerResults> getGameResults()
        {
            send_data(messageCode.GETGAMERESULTSCODE);
            GetGameResultsRes result = recv_data<GetGameResultsRes>();
            if(result.status == (uint)GameMode.FINISHED)
            {
                // sort the list
                List<PlayerResults> results = result.results.OrderBy(o => (1 / o.averageAnswerTime * o.correctAnswerCount / (o.correctAnswerCount + o.wrongAnswerCount))).ToList();
                results.Reverse();
                return results;
            }
            return new List<PlayerResults>();
        }

        /*
        this function send a get question request to the server
        input: none
        output: a GetQuestionRes struct (the question and the answers), empty GetQuestionRes if request faild
        */
        public GetQuestionRes getQuestion()
        {
            send_data(messageCode.GETQUESTIONCODE);
            GetQuestionRes result = recv_data<GetQuestionRes>();
            if (result.status == 1)
            {
                return result;
            }
            return new GetQuestionRes();
        }

        /*
        this function send a leave game request to the server
        input: none
        output: did leave game or not (true \ false)
        */
        public bool leaveGame()
        {
            send_data(messageCode.LEAVEGAMECODE);
            LeaveGameRes result = recv_data<LeaveGameRes>();
            return (result.status == 1);
        }
    }
}
