import socket
import json

ERROR_CODE = 0
SIGNUP_CODE = 1
LOGIN_CODE = 2

CODE_SIZE = 1
LEN_SIZE = 4
BYTES_ORDER = 'big'

CONFIG_PATH = "..\\Trivia Project\\config.txt"

MIN_PORT = 1024
MAX_PORT = 65535


def create_msg(msg_code, msg):
    """
    this function create the message to the server
    :param msg_code: the code of the message
    :param msg: the message
    :type msg_code: int
    :type msg: dict
    :return: the message to the server
    :rtype: bytes
    """

    msg_to_server = msg_code.to_bytes(CODE_SIZE, BYTES_ORDER) \
                    + len(json.dumps(msg)).to_bytes(LEN_SIZE, BYTES_ORDER) \
                    + json.dumps(msg).encode()

    return msg_to_server


def get_msg_from_server(sock):
    """
    this function get the server message and return the code and the message
    :param sock: the socket with the server
    :type sock: socket.socket
    :return: the code and the message
    :rtype: tuple
    """
    msg_code = int.from_bytes(sock.recv(CODE_SIZE), BYTES_ORDER)
    msg_len = int.from_bytes(sock.recv(LEN_SIZE), BYTES_ORDER)
    msg_data = sock.recv(msg_len).decode()

    return msg_code, msg_data


def main():
    server_ip = ""
    server_port = 0

    config = 0
    try:
        config = open(CONFIG_PATH, 'r')
    except:
        print("Could not open config file")
        quit()

    lines = list(config)
    # get the ip the and port of the server
    for line in lines:
        if "server_ip=" in line:
            server_ip = line[10:-1]
        elif "port=" in line:
            server_port = int(line[5:])
            if server_port < MIN_PORT or server_port > MAX_PORT:
                print("port is invalid!")
                quit()

    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print("socket has created")
    server_address = (server_ip, server_port)

    try:
        sock.connect(server_address)
    except Exception as e:
        print("ERROR -", e)
        sock.close()
        quit()
    print("socket has connected")

    login = {"username": "user1", "password": "1234"}
    try:
        sock.sendall(create_msg(LOGIN_CODE, login))
    except Exception as e:
        print("ERROR -", e)
        sock.close()
        quit()

    try:
        msg_code, server_msg = get_msg_from_server(sock)
        print("login:", msg_code, server_msg)
    except Exception as e:
        print("ERROR -", e)
        sock.close()
        quit()
    aa = input("press to continue")
    sock.close()  # client 2 Login Req


if __name__ == '__main__':
    main()
