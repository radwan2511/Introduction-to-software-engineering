using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Column
    {
        public readonly IList<Task> tasks;
        public readonly int limit;
        public readonly string name;
        public readonly string creatorEmail;
        public readonly string boardName;
        public readonly int ordinal;
        /// <summary>
        /// Column(Service object) constructor
        /// </summary>
        /// <param name="tasks">list of tasks of the column</param>
        /// <param name="limit">column tasks limit</param>
        /// <param name="name">column name(backlohg/in progress/done)</param>
        internal Column(string creatorEmail,string boardName,IList<BusinessLayer.ITask> tasks, int limit, string name,int ordinal)
        {
            this.ordinal = ordinal;
            this.creatorEmail = creatorEmail;
            this.boardName = boardName;
            this.tasks = new List<Task>();
            foreach (BusinessLayer.ITask t in tasks)
            {
                this.tasks.Add(new Task(ordinal,t.Id, t.CreationTime, t.title, t.description, t.dueDate, t.Assignee,boardName,creatorEmail));
            }
            this.limit = limit;
            this.name = name;
        }
    }
}