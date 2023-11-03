using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class User
    {
        private string email;
        private string password;
        //private Dictionary<string,Board> boardsByName;
        private Boolean isLoggedIn;
        private readonly BoardController boards;

        /*
        User constructor

        input: string email, string password
        output: none
         */
        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
            //boardsByName = new Dictionary<string, Board>();
            isLoggedIn = false;
            boards = new BoardController(email);
        }

        /*
        logging the User in

        input: none
        output: none
         */
        public void logIn(string password)
        {
            if (this.password == password)
                isLoggedIn = true;
            else
                throw new Exception("incorrect password");
        }

        /*
        logging the User out

        input: none
        output: none
         */
        public void logout()
        {
            if (!isLoggedIn)
            {
                throw new Exception("the user should be logged in");
            }
            isLoggedIn = false;
        }


        /*
         creating new board and adding it to the dictionary

        input: string name
        output: none
         */
        public void addBoard(string name)
        {
            boards.addBoard(name);
        }

        /*
         check if board name exists 

        input: string name
        output: boolean (exists or not)
         */
        private Boolean boardNameExists(string name)
        {
            return boards.boardNameExists(name);
        }


        /*
         first checking if the board name exists and second removing it

        input: string name
        output: none 
        */
        public void removeBoard(string name)
        {
            boards.removeBoard(name);
        }


        /*
         adding task to a specific board choosen by the user

        input: string dueDate, string title, string description, string boardName
        output: none
        */
        public void addTaskToBoard(DateTime dueDate, string title, string description, string boardName)
        {
            boards.addTaskToBoard(dueDate, title, description, boardName);
        }

        /*
        getting task by its id from a specific board choosen by the user

        input: string name, int taskId
        output: Task (if the task is not found throwing exception)
        */
        public Task getTask(string name, int taskId)
        {
            return boards.getTask(name,taskId);
        }

        /*
        limits the capacity of a speficic column in a specific board

       input: string(board name), int(column ordinal), int(limit)
       output: BoardController
        */

        public void limitColumn(string boardName, int columnOrdinal, int limit)
        {
            boards.limitColumn(boardName, columnOrdinal, limit);
        }

        /*
        getter of a specific board

       input: string boardName
       output: Board
        */
        public Board GetBoard(string boardName)
        {
            return boards.GetBoard(boardName);
        }

        /*
        return true or false whether the user is logged in or not respectively

       input: none
       output: bool
        */

        public bool getLoggedIn()
        {
            return isLoggedIn;
        }
        /*
        getter of board controller

       input: none
       output: BoardController
        */
        public BoardController GetBoardController()
        {
            return boards;
        }
    }
}
