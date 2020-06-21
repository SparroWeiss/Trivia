import gspread


def update(ip, port):
    # create an account
    gc = gspread.service_account(filename='scripts//credentials.json')
    # open the sheet group
    sh = gc.open_by_key("1zf5ov6wUMEzfYPRukwDjKY7qafM6bkP37nl1U02De94")
    # select the first sheet
    worksheet = sh.sheet1

    worksheet.update("A2", ip)  # updating the ip cell
    worksheet.update("B2", port)  # updating the port cell
