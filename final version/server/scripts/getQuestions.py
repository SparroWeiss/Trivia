import requests
import json
import sys
import html
import sqlite3
from sqlite3 import Error


DELIMITER = '~~~'


def create_connection(db_file):
    """
    This function connects to a sqlite3 DB
    :param db_file: the DB file path
    :type db_file: string
    :return conn: sqlite3 connection to DB
    """
    conn = None
    try:
        conn = sqlite3.connect(db_file)
    except Error as e:
        print(e)

    return conn


def insert_question(db, question):
    """
    This function inserts questions to an sqlite DB.
    :param db: the DB
    :type db: sqlite3 connection to DB
    :param question: the question and its answers
    :type question: tuple
    """
    sql_statement = '''INSERT INTO QUESTIONS(QUESTION, CORRECT_ANSWER, WRONG_ANSWERS) VALUES(?, ?, ?);'''
    db.execute(sql_statement, question)


def create_questions(questions_list):
    """
    This function receives a list of dictionaries,
    that contains questions' data, and converts it
    to a tuple that contains only the necessary data.
    :param questions_list: a list of dictionaries,
    that contains questions' data
    :type questions_list: list
    :return: list of tuples
    """
    questions = list()

    for question_dict in questions_list:
        questions.append(
            (html.unescape(question_dict['question']),
             html.unescape(question_dict['correct_answer']),
             html.unescape(DELIMITER.join(question_dict['incorrect_answers']))))

    return questions


def main():
    """
    This python script automates the process of adding
    trivia questions to 'TriviaDB.sqlite' DataBase,
    by using the 'opentdb.com' API.
    :param argv[1]: the amount of questions to create
    :param argv[2]: the path of sqlite3 DB
    """
    db = sqlite3.connect(sys.argv[2])
    db.execute("DELETE FROM QUESTIONS;") # clearing DB questions

    url = 'https://opentdb.com/api.php?amount=' + sys.argv[1] + '&type=multiple' # fetching new questions
    response_dict = json.loads(requests.get(url).content.decode())
    questions = create_questions(response_dict['results'])

    for question in questions: # inserting new questions to DB
        insert_question(db, question)

    db.commit() # saving changes
    db.close()


if __name__ == "__main__":
    main()
