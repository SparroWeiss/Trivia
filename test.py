import socket

MSG = "Hello"
CONFIG_PATH = "..\\Trivia Project\\config.txt"


def main():
    server_ip = ""
    server_port = 0
    config = open(CONFIG_PATH, 'r')
    lines = list(config)

    for line in lines:
        if "server_ip=" in line:
            server_ip = line[10:-1]
        elif "port=" in line:
            server_port = int(line[5:])

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

    server_msg = sock.recv(1024)
    server_msg = server_msg.decode()
    print(server_msg)

    try:
        sock.sendall(MSG.encode())
    except Exception as e:
        print("ERROR -", e)
        sock.close()
        quit()

    sock.close()


if __name__ == '__main__':
    main()
