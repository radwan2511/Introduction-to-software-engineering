using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Board
    {
        private int id;
        private string name;
        private string creatorMail;
        private Dictionary<int, Task> backlog;
        private int backlogLimit;
        private Dictionary<int, Task> inProgress;
        private int inProgressLimit;
        private Dictionary<int, Task> done;
        private int doneLimit;

        /*
        Board constructor

        input: int id, string name, string creatorMail
        output: none
         */
        public Board(int id, string name, string creatorMail)
        {
            this.id = id;
            this.name = name;
            this.creatorMail = creatorMail;
            backlog = new Dictionary<int, Task>();
            this.backlogLimit = -1;
            inProgress = new Dictionary<int, Task>();
            this.inProgressLimit = -1;
            done = new Dictionary<int, Task>();
            this.doneLimit = -1;
        }

        /*
        updating backlog limit

        input: int limit
        output: none
         */
        public void limitBacklog(int limit)
        {
            if (limit >= this.backlog.Count | limit == -1)
            {
                this.backlogLimit = limit;
            }
            else
            {
                throw new Exception("limit should be more than current number of tasks in backlog or -1(which is unlimited)");
            }
        }

        /*
        updating inProgress limit

        input: int limit
        output: none
         */
        public void limitinProgress(int limit)
        {

            if (limit >= this.inProgress.Count | limit == -1)
            {
                this.inProgressLimit = limit;
            }
            else
            {
                throw new Exception("limit should be more than current number of tasks in backlog or -1(which is unlimited)");
            }
        }

        /*
        updating done limit

        input: int limit
        output: none
         */
        public void limitdone(int limit)
        {

            if (limit >= this.done.Count | limit == -1)
            {
                this.doneLimit = limit;
            }
            else
            {
                throw new Exception("limit should be more than current number of tasks in backlog or -1(which is unlimited)");
            }
        }


        /*
         adding task to backlog

        input: DateTime dueDate, string title, string description, string boardName
        output: new Tasks' id
         */
        public int addToBacklog(DateTime dueDate, string title, string description, string boardName)
        {
            int id = 0;
            if (this.backlogLimit > this.backlog.Count | this.backlogLimit == -1)
            {
                id = this.backlog.Count + this.inProgress.Count + this.done.Count + 1;
                this.backlog.Add(id, new Task(id, title, description, dueDate));
            }
            else
            {
                throw new Exception("cannot add task due to backlog limit error.");
            }
            return id;
        }

        /*
         moving task from backlog to inProgress

        input: int taskId
        output: none
         */
        public void moveBacklogToInProgress(int taskId)
        {
            if (this.backlog.ContainsKey(taskId))
            {


                if (this.inProgressLimit > this.inProgress.Count | this.inProgressLimit == -1)
                {
                    this.inProgress.Add(taskId, this.backlog[taskId]);
                    this.backlog.Remove(taskId);
                }
                else
                {
                    throw new Exception("cannot add task due to inProgress limit error.");
                }
            }
            else
            {
                throw new Exception("Task id is incorrect.");
            }
        }


        /*
         moving task from inProgress to done

        input: int taskId
        output: none
         */
        public void moveInProgressToDone(int taskId)
        {
            if (this.inProgress.ContainsKey(taskId))
            {


                if (this.doneLimit > this.done.Count | this.doneLimit == -1)
                {
                    this.done.Add(taskId, this.inProgress[taskId]);
                    this.inProgress[taskId].setDone();
                    this.inProgress.Remove(taskId);
                }
                else
                {
                    throw new Exception("cannot add task due to done(tasks) limit error.");
                }
            }
            else
            {
                throw new Exception("Task id is incorrect.");
            }
        }




        /*
         return task by its id and its column

        input: int taskId, int column
        output: Task task
         */
        public Task getTaskByCol(int taskId, int column)
        {
            Task task = null;
            if (column < 0 || column > 2)
            {
                throw new Exception("Invalid column ordinal");
            }
            if (this.backlog.ContainsKey(taskId)&&column==0)
            {
                task = this.backlog[taskId];
            }

            if (this.inProgress.ContainsKey(taskId)&&column==1)
            {
                task = this.inProgress[taskId];
            }

            if (this.done.ContainsKey(taskId)&&column==2)
            {
                task = this.done[taskId];
            }

            if (task == null)
            {
                throw new Exception("task id is not found in the given column.");
            }
            else
            {
                return task;
            }
        }

        /*
        return task by its id

       input: int taskId
       output: Task task
        */
        public Task getTask(int taskId)
        {
            Task task = null;
            if (this.backlog.ContainsKey(taskId))
            {
                task = this.backlog[taskId];
            }

            if (this.inProgress.ContainsKey(taskId))
            {
                task = this.inProgress[taskId];
            }

            if (this.done.ContainsKey(taskId))
            {
                task = this.done[taskId];
            }

            if (task == null)
            {
                throw new Exception("task id is not found.");
            }
            else
            {
                return task;
            }
        }

        /*
         return all in progress tasks

        input: none
        output: Dictionary<int, Task> in progress tasks
         */
        public Dictionary<int, Task> getInProgress()
        {
            return this.inProgress;
        }

        public int GetColumnLimit(int columnOrdinal)
        {
            if (columnOrdinal == 0)
            {
                return backlogLimit;
            }
            if (columnOrdinal == 1)
            {
                return inProgressLimit;
            }
            if (columnOrdinal == 2)
            {
                return doneLimit;
            }
            throw new Exception("illegal column ordinal");
            
        }
        /*
        return tasks list of a specific column

       input: int columnOrdinal
       output: IList<Task>
        */

        public IList<Task> GetColumn(int columnOrdinal)
        {
            IList < Task > l= new List<Task>();
            if (columnOrdinal == 0)
            {
                return backlog.Values.ToList<Task>();
            }
            if (columnOrdinal == 1)
            {
                return inProgress.Values.ToList<Task>();
            }
            if (columnOrdinal == 2)
            {
                return done.Values.ToList<Task>();
            }
            throw new Exception("illegal column ordinal");
        }
    }
}
