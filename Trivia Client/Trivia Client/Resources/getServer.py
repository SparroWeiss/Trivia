import gspread


def main():
    # create an account
    gc = gspread.service_account(filename='Resources\\credentials.json')
    # open the sheet group
    sh = gc.open_by_key("1zf5ov6wUMEzfYPRukwDjKY7qafM6bkP37nl1U02De94")
    # select the first sheet
    worksheet = sh.sheet1
    # get the table data
    res = worksheet.get_all_values()
    ip = res[1][0]  # the ip cell
    port = res[1][1]  # the port cell
    # write it into an output
    print(ip)
    print(port)


if __name__ == "__main__":
    main()
