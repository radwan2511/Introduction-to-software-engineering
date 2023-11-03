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
        private readonly IList<Column> columns;
        public IList<Column> Columns { get => columns; }
        private readonly ColumnDalController columnDalController;
        /// <summary>
        /// useful constants:
        /// </summary>
        private int BACKLOG_COLUMN_INDEX = 0;
        private readonly int IN_PROGRESS_COLUMN_INDEX = 1;
        private int DONE_COLUMN_INDEX = 2;


        /// <summary>
        /// Board constructor for non persisted boards
        /// </summary>
        /// <param name="id">board id</param>
        /// <param name="email">board creator email</param>
        /// <param name="name">board name</param>
        public Board(int id, string email, string name)
        {
            Id = id;
            CreatorEmail = email;
            Name = name;
            columnDalController = new ColumnDalController();
            columnDalController.Insert(new ColumnDTO(-1, "backlog", id, BACKLOG_COLUMN_INDEX));
            columnDalController.Insert(new ColumnDTO(-1, "in progress", id, IN_PROGRESS_COLUMN_INDEX));
            columnDalController.Insert(new ColumnDTO(-1, "done", id, DONE_COLUMN_INDEX));
            columns = new List<Column>();
            columns.Add(new Column("backlog", new ColumnDTO(-1, "backlog", id, BACKLOG_COLUMN_INDEX), BACKLOG_COLUMN_INDEX));
            columns.Add(new Column("in progress", new ColumnDTO(-1, "in progress", id, IN_PROGRESS_COLUMN_INDEX), IN_PROGRESS_COLUMN_INDEX));
            columns.Add(new Column("done", new ColumnDTO(-1, "done", id, DONE_COLUMN_INDEX), DONE_COLUMN_INDEX));
        }

        /// <summary>
        /// load from DB constructor-creates a board from dto object
        /// </summary>
        /// <param name="board">board dto object</param>
        internal Board(BoardDTO board)
        {
            Id = (int)board.ID;
            Name = board.BoardName;
            CreatorEmail = board.Email;
            columns = new List<Column>();
            columnDalController = new ColumnDalController();
        }


        /// <summary>
        /// Specific board column getter
        /// </summary>
        /// <param name="columnOrdinal">0 for backlog column, 1 for in progress column, 2 for done column
        /// any other value is invalid</param>
        /// <returns>the specified board column</returns>
        public Column GetColumn(int columnOrdinal)
        {
            if (columnOrdinal > DONE_COLUMN_INDEX || columnOrdinal < BACKLOG_COLUMN_INDEX)
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
        public IList<ITask> GetColumnTasks(int columnOrdinal)
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
            if (columnOrdinal < BACKLOG_COLUMN_INDEX || columnOrdinal >= DONE_COLUMN_INDEX)
            {
                throw new Exception("Invalid column ordinal- tasks can be advanced only from an existing not rightmost column.");
            }
            if (GetColumn(columnOrdinal).GetTask(taskId).Assignee == email)
            {
                ITask toBeAdvanced = GetColumn(columnOrdinal).RemoveTask(taskId);
                try
                {
                    columns[columnOrdinal + 1].AddTask(toBeAdvanced);
                    if (columnOrdinal == DONE_COLUMN_INDEX - 1)
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
            else
            {
                throw new Exception("only assignee can advance his tasks");
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
            int id = 1;
            for (int i = 0; i < columns.Count; i++)
            {
                id += columns[i].tasks.Count;
            }
            Task toBeAdded = new Task(id, title, description, dueDate, assignee, this.Id);
            GetColumn(BACKLOG_COLUMN_INDEX).AddTask(toBeAdded);
            new TaskDalController().Insert(toBeAdded.taskDTO);
            return toBeAdded;
        }

        /// <summary>
        /// Finds all in progress tasks assigned to a given user
        /// </summary>
        /// <param name="userEmail">the specified user email</param>
        /// <returns>all in progress tasks assigned to the specified user in this board</returns>
        public IList<ITask> GetInProgressTasks(string userEmail)
        {
            IList<ITask> ans = new List<ITask>();
            foreach (Column c in columns)
            {
                if (c.ColumnOrdinal != BACKLOG_COLUMN_INDEX && c.ColumnOrdinal != DONE_COLUMN_INDEX)
                {
                    IList<ITask> tasks = c.tasks;
                    foreach (ITask t in tasks)
                    {
                        if (t.Assignee == userEmail)
                        {
                            ans.Add(t);
                        }
                    }
                }
            }
            return ans;
        }

        /// <summary>
        /// Loads all the data related to this board from the DB(the boards' columns and its tasks)
        /// </summary>
        internal void LoadData()
        {
            List<ColumnDTO> columns = columnDalController.SelectAllColumns(new BoardDTO(Id, this.CreatorEmail, Name));
            IList<Column> xUnsorted = new List<Column>();
            foreach (ColumnDTO c in columns)
            {
                if (c.ID == this.Id)
                {
                    Column cBL = new Column(c);
                    xUnsorted.Add(cBL);
                }
            }
            IList<Column> x = xUnsorted.OrderBy((Column c) => c.ColumnOrdinal).ToList();
            foreach (Column c in x)
            {
                this.columns.Add(c);
                c.LoadData();
            }
            this.BACKLOG_COLUMN_INDEX = 0;
            this.DONE_COLUMN_INDEX = columns.Count - 1;
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>
        public void AddColumn(int columnOrdinal, string columnName)
        {
            if (columnOrdinal < 0 || columnOrdinal > columns.Count)
            {
                throw new Exception("column ordinal must be non negative and at most as large as the current amount of columns");
            }
            if (columnName == null)
            {
                throw new Exception("column name should not be null");
            }
            if (columnOrdinal == columns.Count)
            {
                UnDoneColumn(GetColumn(DONE_COLUMN_INDEX));
            }
            for (int i = columns.Count - 1; i >= columnOrdinal; i--)
            {
                columns[i].ColumnOrdinal = columns[i].ColumnOrdinal + 1;
            }
            columns.Insert(columnOrdinal, new Column(columnName, new ColumnDTO(-1, columnName, Id, columnOrdinal), columnOrdinal));
            columnDalController.Insert(new ColumnDTO(-1, columnName, Id, columnOrdinal));
            if (columnOrdinal < BACKLOG_COLUMN_INDEX)
            {
                BACKLOG_COLUMN_INDEX = columnOrdinal;
            }
            DONE_COLUMN_INDEX = DONE_COLUMN_INDEX + 1;
        }

        /// <summary>
        /// updates the previous done column and its tasks to be none-done
        /// </summary>
        /// <param name="currDone">the column which is not done anymore</param>
        private void UnDoneColumn(Column currDone)
        {
            foreach (ITask t in currDone.tasks)
            {
                t.IsDone = false;
            }
        }

        /// <summary>
        /// removes a column
        /// </summary>
        /// <param name="columnOrdinal">column ordinal of the being removed column</param>
        public void RemoveColumn(int columnOrdinal)
        {
            if (columnOrdinal < 0 || columnOrdinal >= columns.Count)
            {
                throw new Exception("column ordinal must be non negative and at most as large as the current amount of columns");
            }
            if (columns.Count == 2)
            {
                throw new Exception("Board should contain at least 2 columns");
            }
            Column removed = GetColumn(columnOrdinal);
            Column followingCol = (columnOrdinal == BACKLOG_COLUMN_INDEX) ? GetColumn(DONE_COLUMN_INDEX) : GetColumn(columnOrdinal - 1);
            followingCol.GetTasksFrom(removed);
            columns.Remove(removed);
            columnDalController.Delete(removed.cDTO);
            for (int i = columnOrdinal; i < columns.Count; i++)
            {
                columns[i].ColumnOrdinal = columns[i].ColumnOrdinal - 1;
            }
            if (DONE_COLUMN_INDEX == columns.Count)
            {
                foreach (ITask t in removed.tasks)
                {
                    t.IsDone = false;
                }
                DONE_COLUMN_INDEX = DONE_COLUMN_INDEX - 1;
                foreach (Task t in columns[DONE_COLUMN_INDEX].tasks)
                {
                    t.IsDone = true;
                }
            }
        }

        /// <summary>
        /// moves a column of the board
        /// </summary>
        /// <param name="columnOrdinal">the current column position</param>
        /// <param name="shiftSize">how much places to move the desired column(positive for right negative for left)</param>
        public void MoveColumn(int columnOrdinal, int shiftSize)
        {
            if (columnOrdinal < 0 || columnOrdinal > columns.Count)
            {
                throw new Exception("column ordinal must be non negative and at most as large as the current amount of columns");
            }
            if (columnOrdinal + shiftSize < 0 || columnOrdinal + shiftSize > DONE_COLUMN_INDEX)
            {
                throw new Exception("shift size and column ordinal combination exceeds the board column ordinals limits");
            }
            Column ToBeMoved = GetColumn(columnOrdinal);
            if (ToBeMoved.tasks.Count > 0)
            {
                throw new Exception("Moved column should be empty of tasks");
            }
            ToBeMoved.ColumnOrdinal = DONE_COLUMN_INDEX + 1;//avoiding db primary keys issues
            if (shiftSize > 0)
            {
                for (int i = columnOrdinal + 1; i < columnOrdinal + shiftSize + 1; i++)
                {
                    columns[i].ColumnOrdinal = (columns[i].ColumnOrdinal - 1);
                }
            }
            else
            {
                for (int i = columnOrdinal - 1; i >= columnOrdinal + shiftSize; i--)
                {
                    columns[i].ColumnOrdinal = (columns[i].ColumnOrdinal + 1);
                }
            }
            columns.Remove(ToBeMoved);
            ToBeMoved.ColumnOrdinal = columnOrdinal + shiftSize;
            columns.Insert(columnOrdinal + shiftSize, ToBeMoved);
            if (DONE_COLUMN_INDEX == columnOrdinal && (columnOrdinal + shiftSize) != DONE_COLUMN_INDEX)
            {
                foreach (ITask t in ToBeMoved.tasks)
                {
                    t.IsDone = false;
                }
                foreach (Task t in columns[DONE_COLUMN_INDEX].tasks)
                {
                    t.IsDone = true;
                }
            }
            else if (DONE_COLUMN_INDEX == columnOrdinal + shiftSize && shiftSize != 0)
            {
                foreach (ITask t in columns[DONE_COLUMN_INDEX - 1].tasks)
                {
                    t.IsDone = false;
                }
                foreach (Task t in columns[DONE_COLUMN_INDEX].tasks)
                {
                    t.IsDone = true;
                }
            }
        }
    }
}