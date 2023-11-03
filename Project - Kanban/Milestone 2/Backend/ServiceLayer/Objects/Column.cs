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
        /// <summary>
        /// Column(Service object) constructor
        /// </summary>
        /// <param name="tasks">list of tasks of the column</param>
        /// <param name="limit">column tasks limit</param>
        /// <param name="name">column name(backlohg/in progress/done)</param>
        internal Column(IList<BusinessLayer.Task> tasks, int limit, string name)
        {
            this.tasks = new List<Task>();
            foreach(BusinessLayer.Task t in tasks)
            {
                this.tasks.Add(new Task(t.Id, t.CreationTime, t.title, t.description, t.dueDate, t.Assignee));
            }
            this.limit = limit;
            this.name = name;
        }
    }
}
