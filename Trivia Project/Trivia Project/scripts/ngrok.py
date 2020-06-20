import subprocess
import requests
import time
import socket

LOGIN = "1dJa9zaxL0PLcBbq6r5W4PPFr4W_6yxMxy7cdkiDwS8qMnuiv"


def main():
    subprocess.call(["ngrok", "authtoken", LOGIN])
    ngrok = subprocess.Popen(["ngrok", "tcp", "8998"])

    time.sleep(3)

    res = requests.get("http://127.0.0.1:4040/api/tunnels")
    domain = res.content.decode().split('public_url":"tcp://')[1].split(":")[0]
    ip = socket.gethostbyname(domain)
    print(ip)
    port = res.content.decode().split("tcp.ngrok.io:")[1].split('","')[0]
    print(port)


if __name__ == '__main__':
    main()
