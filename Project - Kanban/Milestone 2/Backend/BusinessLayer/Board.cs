using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Board class
    /// has 3 columns(backlog, in progress, done) of tasks
    /// </summary>
    class Board
    {
        public readonly int Id;
        public readonly string CreatorEmail;
        public readonly string Name;
        private readonly Column[] columns;
        private readonly ColumnDalController columnDalController;
        /// <summary>
        /// useful constants:
        /// </summary>
        private readonly int BACKLOG_COLUMN_INDEX = 0;
        private readonly int IN_PROGRESS_COLUMN_INDEX = 1;
        private readonly int DONE_COLUMN_INDEX = 2;
        private readonly int TOTAL_COLUMNS = 3;


        /// <summary>
        /// Board constructor
        /// </summary>
        /// <param name="id">board id</param>
        /// <param name="email">board creator email</param>
        /// <param name="name">board name</param>
        /// <param name="load">true if the board is persisted, otherwise(also by default)-false</param>
        public Board(int id, string email, string name,bool load=false)
        {
            Id = id;
            CreatorEmail = email;
            Name = name;
            columns = new Column[TOTAL_COLUMNS];
            columnDalController = new ColumnDalController();
            columns[BACKLOG_COLUMN_INDEX] = new Column("backlog", new ColumnDTO(-1, "backlog", id));
            if(!load)
                columnDalController.Insert(new ColumnDTO(-1, "backlog",id));
            columns[IN_PROGRESS_COLUMN_INDEX] = new Column("in progress", new ColumnDTO(-1, "in progress", id));
            if(!load)
                columnDalController.Insert(new ColumnDTO(-1, "in progress",id));
            columns[DONE_COLUMN_INDEX] = new Column("done",new ColumnDTO(-1,"done",id));
            if(!load)
                columnDalController.Insert(new ColumnDTO(-1, "done",id));
        }


        /// <summary>
        /// Specific board column getter
        /// </summary>
        /// <param name="columnOrdinal">0 for backlog column, 1 for in progress column, 2 for done column
        /// any other value is invalid</param>
        /// <returns>the specified board column</returns>
        public Column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal >= TOTAL_COLUMNS || columnOrdinal < 0)
            {
                throw new Exception("Invalid column ordinal");
            }
            return columns[columnOrdinal];
        }

        /// <summary>
        /// Specific column tasks getter
        /// </summary>
        /// <param name="columnOrdinal">0 for backlog column, 1 for in progress column, 2 for done column
        /// any other value is invalid</param>
        /// <returns>a list with the specified column tasks</returns>
        public IList<Task> GetColumnTasks(int columnOrdinal)
        {
            return GetColumn(columnOrdinal).tasks;
        }

        /// <summary>
        /// Advances a task to the next column
        /// </summary>
        /// <param name="email">the email of the current user, should be the task assignee</param>
        /// <param name="columnOrdinal">0 for backlog column, 1 for in progress column any other value is invalid</param>
        /// <param name="taskId">the task id</param>
        public void AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            if (columnOrdinal != BACKLOG_COLUMN_INDEX && columnOrdinal != IN_PROGRESS_COLUMN_INDEX)
            {
                throw new Exception("Invalid column ordinal- tasks can be advanced only from backlog(0)/in progress(1) columns.");
            }
            if (GetColumn(columnOrdinal).GetTask(taskId).Assignee == email)
            {
                Task toBeAdvanced = GetColumn(columnOrdinal).RemoveTask(taskId);
                try
                {
                    columns[columnOrdinal + 1].AddTask(toBeAdvanced);
                    if (columnOrdinal == IN_PROGRESS_COLUMN_INDEX)
                    {
                        columns[columnOrdinal + 1].GetTask(taskId).IsDone = true;
                    }
                    toBeAdvanced.taskDTO.ColumnOrdinal = columnOrdinal + 1;
                }
                catch (Exception e)
                {
                    columns[columnOrdinal].AddTask(toBeAdvanced);
                    throw new Exception(e.Message);
                }
            }
        }


        /// <summary>
        /// Adds a new task to the board(into the backlog column)
        /// </summary>
        /// <param name="title">the task title(not empty, max. length 50)</param>
        /// <param name="description">the task description(optional- can be null, max. length 300)</param>
        /// <param name="dueDate">the task due date(should be later then now)</param>
        /// <param name="assignee">the task assignee</param>
        /// <returns>the new task which has been added</returns>
        public Task AddTask(string title, string description, DateTime dueDate, string assignee)
        {
            int id = GetColumnTasks(BACKLOG_COLUMN_INDEX).Count + GetColumnTasks(IN_PROGRESS_COLUMN_INDEX).Count + GetColumnTasks(DONE_COLUMN_INDEX).Count + 1;
            Task toBeAdded = new Task(id, title, description, dueDate, assignee,this.Id);
            columns[BACKLOG_COLUMN_INDEX].AddTask(toBeAdded);
            new TaskDalController().Insert(toBeAdded.taskDTO);
            return toBeAdded;
        }

        /// <summary>
        /// Finds all in progress tasks assigned to a given user
        /// </summary>
        /// <param name="userEmail">the specified user email</param>
        /// <returns>all in progress tasks assigned to the specified user in this board</returns>
        public IList<Task> GetInProgressTasks(string userEmail)
        {
            IList<Task> all=GetColumnTasks(IN_PROGRESS_COLUMN_INDEX);
            IList<Task> ans = new List<Task>();
            foreach(Task t in all)
            {
                if (t.Assignee == userEmail)
                {
                    ans.Add(t);
                }
            }
            return ans;
        }

        /// <summary>
        /// Loads all the data related to this board from the DB(the boards' columns and its tasks)
        /// </summary>
        internal void LoadData()
        {
            List<ColumnDTO> columns = columnDalController.SelectAllColumns(new BoardDTO(Id,this.CreatorEmail,Name));
            foreach(ColumnDTO c in columns)
            {
                if (c.ID==this.Id)
                {
                    if (c.Name == "backlog")
                    {
                        this.columns[BACKLOG_COLUMN_INDEX].tasksLimit=(int)(c.Limit);
                        this.columns[BACKLOG_COLUMN_INDEX].LoadData();
                    }
                    else if (c.Name == "in progress")
                    {
                        this.columns[IN_PROGRESS_COLUMN_INDEX].tasksLimit = (int)(c.Limit);
                        this.columns[IN_PROGRESS_COLUMN_INDEX].LoadData();
                    }
                    else
                    {
                        this.columns[DONE_COLUMN_INDEX].tasksLimit = (int)(c.Limit);
                        this.columns[DONE_COLUMN_INDEX].LoadData();
                    }
                }
            }
        }
    }
}
