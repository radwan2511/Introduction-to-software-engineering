using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class BoardController
    {
        private Dictionary<string, Board> boardsByName;
        private string email;

        public BoardController(string email)
        {
            this.email = email;
            boardsByName = new Dictionary<string, Board>();
        }

        /*
         creating new board and adding it to the dictionary

        input: string name
        output: none
         */
        public void addBoard(string name)
        {
            if (!boardNameExists(name))
            {
                boardsByName.Add(name, new Board(boardsByName.Count + 1, name, this.email));
            }
            else
            {
                throw new Exception("board name exists, please choose diffrent name for your board.");
            }
        }

        /*
         check if board name exists 

        input: string name
        output: boolean (exists or not)
         */
        public Boolean boardNameExists(string name)
        {
            return boardsByName.ContainsKey(name);
        }


        /*
         first checking if the board name exists and second removing it

        input: string name
        output: none 
        */
        public void removeBoard(string name)
        {
            if (boardNameExists(name))
            {
                boardsByName.Remove(name);
            }
            else
            {
                throw new Exception("board name doesn't exist, please choose one of your exsiting boards.");
            }
        }

        /*
         getting all inProgressTask for the user from all the existing boards related to this user

        input: none
        output: List<Task> tasks 
        */

        public IList<Task> getInProgressTasks()
        {
            IList<Task> inProgressTasks = new List<Task>();
            foreach (KeyValuePair<string, Board> entry in boardsByName)
            {
                foreach (Task t in entry.Value.getInProgress().Values.ToList<Task>())
                {
                    inProgressTasks.Add(t);
                }
            }
            return inProgressTasks;
        }
        /*
         adding task to a specific board choosen by the user

        input: string dueDate, string title, string description, string boardName
        output: none
        */
        public void addTaskToBoard(DateTime dueDate, string title, string description, string boardName)
        {

            boardsByName[boardName].addToBacklog(dueDate, title, description, boardName);
        }

        /*
        getting task by its id from a specific board choosen by the user

        input: string name, int taskId
        output: Task (if the task is not found throwing exception)
        */
        public Task getTask(string name, int taskId)
        {
            return boardsByName[name].getTask(taskId);
        }

        /*
        limits a specific column of a specific board

        input: string name, int column ordinal, int limit
        output: none
        */
        public void limitColumn(string boardName, int columnOrdinal, int limit)
        {
            if (!boardNameExists(boardName))
            {
                throw new Exception("there isn't a board named by the given name");
            }
            Board b = boardsByName[boardName];
            if (columnOrdinal == 0)
            {
                b.limitBacklog(limit);
            }
            if (columnOrdinal == 1)
            {
                b.limitinProgress(limit);
            }
            if (columnOrdinal == 2)
            {
                b.limitdone(limit);
            }
            if (columnOrdinal < 0 || columnOrdinal > 2)
            {
                throw new Exception("illegal column ordinal");
            }
        }

        /*
        getting board by its name

        input: string name
        output: Board (if the board is not found throwing exception)
        */
        public Board GetBoard(string boardName)
        {
            if (!boardNameExists(boardName))
            {
                throw new Exception("there isn't a board named by the given name");
            }
            return boardsByName[boardName];
        }
    }
}
