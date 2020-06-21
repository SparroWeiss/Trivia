import subprocess
import requests
import time
import socket
import setServer

LOGIN = "1dJa9zaxL0PLcBbq6r5W4PPFr4W_6yxMxy7cdkiDwS8qMnuiv"


def main():
    # log in to ngrok
    subprocess.call(["ngrok", "authtoken", LOGIN])
    # creating a tcp tunnel
    ngrok = subprocess.Popen(["ngrok", "tcp", "8998"])

    time.sleep(3)

    # get ngrok server ip and listening port
    res = requests.get("http://127.0.0.1:4040/api/tunnels")
    domain = res.content.decode().split('public_url":"tcp://')[1].split(":")[0]
    ip = socket.gethostbyname(domain)
    print("IP:", ip)
    port = res.content.decode().split("tcp.ngrok.io:")[1].split('","')[0]
    print("PORT:", port)

    # updating virtual config file
    setServer.update(ip, port)


if __name__ == '__main__':
    main()
