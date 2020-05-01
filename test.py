import socket

MSG_LEN = 5
MSG = "Hello"
CONFIG_PATH = "..\\Trivia Project\\config.txt"
MIN_PORT = 1024
MAX_PORT = 65535


def main():
    server_ip = ""
    server_port = 0
    config = open(CONFIG_PATH, 'r')
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

    server_msg = sock.recv(MSG_LEN)
    server_msg = server_msg.decode()

    if server_msg == MSG:
        print(server_msg)
        try:
            sock.sendall(server_msg.encode())
        except Exception as e:
            print("ERROR -", e)
            sock.close()
            quit()

    sock.close()


if __name__ == '__main__':
    main()
