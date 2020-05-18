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
        LEAVEROOMCODE
    };

    class Communicator
    {
        private string SERVER_IP;
        private int SERVER_PORT;
        private const string CONFIG_PATH = "..\\..\\..\\config.txt";

        public Communicator()
        {
            string line;
 
            System.IO.StreamReader file = new System.IO.StreamReader(CONFIG_PATH);
            while ((line = file.ReadLine()) != null)
            {
                if(line.Contains("port="))
                {
                    SERVER_PORT = Int32.Parse(line.Substring(5));
                }
                else if (line.Contains("server_ip="))
                {
                    SERVER_IP = line.Substring(10);
                }
            }

            file.Close();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] iPs = Dns.GetHostAddresses(SERVER_IP);

            try
            {
                _serverSocket.Connect(iPs, SERVER_PORT);
            }
            catch (Exception)
            {
                throw new Exception("Faild to connect to server.\n Do you wish to try again?");
            }
        }

        ~Communicator()
        {
            _serverSocket.Close();
        }

        public void send_data(messageCode code, string msg = "")
        {
            byte[] buffer = new byte[sizeof(byte) + sizeof(int) + msg.Length];
            buffer[0] = BitConverter.GetBytes((int)code)[0];

            byte[] msg_len = BitConverter.GetBytes((int)msg.Length);
            Array.Reverse(msg_len);
            System.Buffer.BlockCopy(msg_len, 0, buffer, sizeof(byte), sizeof(int));

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

        public Res recv_data<Res>()
        {
            byte[] code, msg;
            try
            {
                code = new byte[sizeof(byte)];
                _serverSocket.Receive(code);

                byte[] msg_len = new byte[sizeof(int)];
                _serverSocket.Receive(msg_len);
                Array.Reverse(msg_len);

                msg = new byte[BitConverter.ToInt32(msg_len, 0)];
                _serverSocket.Receive(msg);
            }
            catch (Exception)
            {
                throw new Exception("Lost connection to the server :(");
            }

            if (code[0] == (byte)messageCode.ERRORCODE)
            {
                throw new Exception(JsonConvert.DeserializeObject<ErrorRes>(System.Text.Encoding.ASCII.GetString(msg)) + " :(");
            }

            return JsonConvert.DeserializeObject<Res>(System.Text.Encoding.ASCII.GetString(msg));
        }

        public bool login(string username, string password)
        {
            LoginReq login;
            login.username = username;
            login.password = password;
            
            send_data(messageCode.LOGINCODE, JsonConvert.SerializeObject(login));
            LoginRes result = recv_data<LoginRes>();
            return (result.status == 1); 
        }

        public bool signup(string username, string password, string email, string address, string phone, string birthdate)
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
            return (result.status == 1);
        }

        public bool logout()
        {
            send_data(messageCode.SIGNOUTCODE);
            LogoutRes result = recv_data<LogoutRes>();
            return (result.status == 1);
        }

        private Socket _serverSocket;
    }
}
