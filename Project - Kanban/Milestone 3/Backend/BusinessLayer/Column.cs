using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Column class
    /// contains tasks, the task amount can be limited for a fixed number of tasks or unlimited.
    /// </summary>
    class Column
    {
        internal readonly IColumnDTO cDTO;
        /// <summary>
        /// column ordinal field and its getter and setter
        /// </summary>
        private int columnOrdinal;
        public int ColumnOrdinal
        {
            get => columnOrdinal;
            set
            {
                columnOrdinal = value; this.cDTO.ColOrdinal = value;
                foreach (Task t in tasks) { t.taskDTO.ColumnOrdinal = value; }
            }
        }
        /// <summary>
        /// TasksLimit field and its getter and setter(setter parameter for task limit should
        /// be more than current number of tasks in this column or -1 which is unlimited)
        /// </summary>
        private int TasksLimit;
        public int tasksLimit
        {
            get => TasksLimit;
            set
            {
                if (value >= this.Tasks.Count || value == UNLIMITED)
                {
                    this.TasksLimit = value;
                    this.cDTO.Limit = value;
                }
                else
                {
                    throw new Exception("limit should be more than current number of tasks in backlog or -1(which is unlimited)");
                }
            }
        }
        /// <summary>
        /// column name and its getter & setter
        /// </summary>
        private string name;
        public string Name { get => name; set { name = value; this.cDTO.Name = value; } }
        /// <summary>
        /// column Tasks field(dictionary which matches task id and its task) and its getter(returns a list of all tasks in this column)
        /// </summary>
        internal readonly Dictionary<int, ITask> Tasks;
        public IList<ITask> tasks
        {
            get => Tasks.Values.ToList<ITask>();
        }
        /// <summary>
        /// useful constant:
        /// </summary>
        private readonly int UNLIMITED = -1;

        /// <summary>
        /// column constructor for non-persisted column
        /// </summary>
        /// <param name="Name">column name</param>
        /// <param name="c">column dto</param>
        /// <param name="col">column ordinal</param>
        internal Column(string Name, IColumnDTO c, int col)
        {
            TasksLimit = UNLIMITED;
            this.name = Name;
            Tasks = new Dictionary<int, ITask>();
            cDTO = c;
            columnOrdinal = col;
        }

        /// <summary>
        /// column constructor for persisted column
        /// </summary>
        /// <param name="Name">column name</param>
        /// <param name="c">column dto</param>
        /// <param name="col">column ordinal</param>
        internal Column(ColumnDTO c)
        {
            TasksLimit = (int)c.Limit;
            this.name = c.Name;
            Tasks = new Dictionary<int, ITask>();
            cDTO = c;
            columnOrdinal = (int)c.ColOrdinal;
        }

        /// <summary>
        /// adds a task for this column(if it doesn't exceed the limit)
        /// </summary>
        /// <param name="t">the task to be added</param>
        public void AddTask(ITask t)
        {
            if (TasksLimit == UNLIMITED || Tasks.Keys.Count < TasksLimit)
                Tasks[t.Id] = t;
            else
                throw new Exception("reached to the column limit, can't add the given task");
        }

        /// <summary>
        /// gets a task from this column
        /// </summary>
        /// <param name="taskId">task ID, should exist in the column</param>
        /// <returns>the specified task</returns>
        public ITask GetTask(int taskId)
        {
            if (this.Tasks.ContainsKey(taskId))
            {
                return this.Tasks[taskId];
            }
            throw new Exception("task id is not found in this column.");
        }

        /// <summary>
        /// removes a task from this column
        /// </summary>
        /// <param name="taskId">task Id, should exist in this column</param>
        /// <returns>the removed task</returns>
        public ITask RemoveTask(int taskId)
        {
            if (!Tasks.ContainsKey(taskId))
            {
                throw new Exception("task not found");
            }
            ITask ans = Tasks[taskId];
            Tasks.Remove(taskId);
            return ans;
        }

        /// <summary>
        /// loads this column tasks from the DB
        /// </summary>
        internal void LoadData()
        {
            TaskDalController taskDalController = new TaskDalController();
            IList<TaskDTO> tasks = taskDalController.SelectAllTasks(this.cDTO);
            foreach (TaskDTO task in tasks)
            {
                this.Tasks.Add((int)task.Id, new Task(task));
            }
        }

        /// <summary>
        /// transfers tasks from another column to this column(if doesn't exceed this tasks limit)
        /// </summary>
        /// <param name="other">other column which we want to transfer its tasks to this column</param>
        internal void GetTasksFrom(Column other)
        {
            IList<ITask> transferedTasks = other.tasks;
            if (tasksLimit < transferedTasks.Count + tasks.Count && tasksLimit != UNLIMITED)
            {
                throw new Exception("tasks cannot be moved from the removed column to its previous(cyclic) column, remove column failed");
            }
            foreach (ITask t in transferedTasks)
            {
                Tasks.Add(t.Id, t);
                t.taskDTO.ColumnOrdinal = this.columnOrdinal;
            }
        }
    }
}

