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
    /// Task class
    /// </summary>
    class Task : ITask
    {
        /// <summary>
        /// unit tests interface getters and setters(here they throw not implemented exception)
        /// </summary>
        int ITask.Id { get => this.Id; set => throw new NotImplementedException(); }
        DateTime ITask.CreationTime { get => this.CreationTime; set => throw new NotImplementedException(); }
        string ITask.title { get => this.Title; set => throw new NotImplementedException(); }
        string ITask.description { get => this.Description; set => throw new NotImplementedException(); }
        DateTime ITask.dueDate { get => this.DueDate; set => throw new NotImplementedException(); }
        string ITask.Assignee { get => this.assignee; set => throw new NotImplementedException(); }
        bool ITask.IsDone
        {
            get => isDone;
            set
            {
                isDone = value;
                this.taskDTO.IsDone = value;
            }
        }
        TaskDTO ITask.taskDTO { get => taskDTO; set => throw new NotImplementedException(); }


        internal readonly TaskDTO taskDTO;
        public readonly int Id;
        public readonly DateTime CreationTime;
        /// <summary>
        /// Title field and its getter
        /// </summary>
        private string Title;
        public string title
        {
            get => Title;
        }
        /// <summary>
        /// description field and its getter
        /// </summary>
        private string Description;
        public string description
        {
            get => Description;
        }
        /// <summary>
        /// DueDate field and its getter
        /// </summary>
        private DateTime DueDate;
        public DateTime dueDate
        {
            get => DueDate;
        }
        /// <summary>
        /// assignee field and its getter
        /// </summary>
        private string assignee;
        public string Assignee
        {
            get => assignee;
        }
        /// <summary>
        /// isDone field and its getter and setter(sets the field to be true)
        /// </summary>
        private bool isDone;
        public bool IsDone
        {
            get => isDone;
            set
            {
                isDone = value;
                this.taskDTO.IsDone = value;
            }
        }
        /// <summary>
        /// useful constants:
        /// </summary>
        private readonly int TITLE_MIN_LENGTH = 0;
        private readonly int TITLE_MAX_LENGTH = 50;
        private readonly int DESCRIPTION_MAX_LENGTH = 300;

        /// <summary>
        /// Task constructor for non-persisted(new) tasks
        /// </summary>
        /// <param name="id">task id</param>
        /// <param name="title">task title in length 1-50 </param>
        /// <param name="description">task description which can be null or at max. length of 300</param>
        /// <param name="DueDate">task due date, later then now</param>
        /// <param name="assignee">task assignee</param>
        /// <param name="bId">Id of the board which contains the task</param>
        public Task(int id, string title, string description, DateTime DueDate, string assignee, long bId)
        {
            if (title == null || (title.Length == TITLE_MIN_LENGTH || TITLE_MAX_LENGTH < title.Length))
            {
                throw new Exception("title should be not empty and should have max. 50 chars if is defined");
            }
            if (description != null && (description.Length > DESCRIPTION_MAX_LENGTH))
            {
                throw new Exception("description should have max. 300 chars if is defined");
            }
            if (DueDate < DateTime.Now)
            {
                throw new Exception("the given due date has already passed");
            }
            this.Id = id;
            this.CreationTime = DateTime.Now;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.assignee = assignee;
            this.isDone = false;
            int BACKLOG_IDX = 0;
            this.taskDTO = new TaskDTO(id, CreationTime, DueDate, title, description, assignee, IsDone, BACKLOG_IDX, bId);
        }


        /// <summary>
        /// task constructor for persisted tasks
        /// </summary>
        /// <param name="task">task dto to be converted to a task</param>
        internal Task(TaskDTO task)
        {
            this.Id = (int)task.Id;
            this.CreationTime = task.CreationTime;
            this.Title = task.Title;
            this.Description = task.Description;
            this.DueDate = task.DueDate;
            this.assignee = task.Assignee;
            this.isDone = task.IsDone;
            this.taskDTO = task;
        }


        /// <summary>
        /// updates task due date
        /// </summary>
        /// <param name="dueDate">new due date, should be later than now</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public virtual void UpdateTaskDueDate(DateTime dueDate, string userEmail)
        {
            if (userEmail != assignee)
            {
                throw new Exception("only assignee can edit a task");
            }
            if (dueDate < DateTime.Now)
            {
                throw new Exception("the given due date has already passed");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.DueDate = dueDate;
            this.taskDTO.DueDate = dueDate;
        }

        /// <summary>
        /// updates task title
        /// </summary>
        /// <param name="title">new title, should be non empty max. length 50</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public virtual void UpdateTaskTitle(string title, string userEmail)
        {
            if (userEmail != assignee)
            {
                throw new Exception("only assignee can edit a task");
            }
            if (title == null || (title.Length == TITLE_MIN_LENGTH || title.Length > TITLE_MAX_LENGTH))
            {
                throw new Exception("title should be not empty and should have max. 50 chars if is defined");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.Title = title;
            this.taskDTO.Title = title;
        }


        /// <summary>
        /// updates task description
        /// </summary>
        /// <param name="description">new description, should be null or at max. length 300</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public virtual void UpdateTaskDescription(string description, string userEmail)
        {
            if (userEmail != assignee)
            {
                throw new Exception("only assignee can edit a task");
            }
            if (description != null && (description.Length > DESCRIPTION_MAX_LENGTH))
            {
                throw new Exception("description should have max. 300 chars if is defined");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.Description = description;
            this.taskDTO.Description = description;
        }


        /// <summary>
        /// assign task to a user
        /// </summary>
        /// <param name="userEmail">user email, should be current assignee</param>
        /// <param name="toBeAssigned">new assignee</param>
        public virtual void AssignTask(string userEmail, string toBeAssigned)
        {
            if (userEmail != assignee)
            {
                throw new Exception("only the current assignee can assign task");
            }
            if (toBeAssigned == assignee)
            {
                throw new Exception("the user is already the assignee");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            assignee = toBeAssigned;
            this.taskDTO.Assignee = assignee;
        }
    }
}
