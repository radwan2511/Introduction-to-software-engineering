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
    /// Column class
    /// contains tasks, the task amount can be limited for a fixed number of tasks or unlimited.
    /// </summary>
    internal class Column
    {
        public readonly ColumnDTO cDTO;
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
        public readonly string Name;
        /// <summary>
        /// column Tasks field(dictionary which matches task id and its task) and its getter(returns a list of all tasks in this column)
        /// </summary>
        private readonly Dictionary<int, Task> Tasks;
        public IList<Task> tasks
        {
            get => Tasks.Values.ToList<Task>();
        }
        /// <summary>
        /// useful constant:
        /// </summary>
        private readonly int UNLIMITED = -1;
        
        public Column(string Name, ColumnDTO c)
        {
            TasksLimit = UNLIMITED;
            this.Name = Name;
            Tasks = new Dictionary<int, Task>();
            cDTO = c;
        }


        /// <summary>
        /// adds a task for this column(if it doesn't exceed the limit)
        /// </summary>
        /// <param name="t">the task to be added</param>
        public void AddTask(Task t)
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
        public Task GetTask(int taskId)
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
        public Task RemoveTask(int taskId)
        {
            if (!Tasks.ContainsKey(taskId))
            {
                throw new Exception("task not found");
            }
            Task ans = Tasks[taskId];
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
            foreach(TaskDTO task in tasks)
            {
                this.Tasks.Add((int)task.Id,new Task((int)task.Id, task.Title, task.Description, task.DueDate, task.Assignee, task.CreationTime,task.BoardId));
            }
        }
    }
}
