using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// task interface for unit tests
    /// </summary>
    interface ITask
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime dueDate { get; set; }
        public string Assignee { get; set; }
        public bool IsDone { get; set; }
        public TaskDTO taskDTO { get; set; }
        /// <summary>
        /// updates task due date
        /// </summary>
        /// <param name="dueDate">new due date, should be later than now</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public void UpdateTaskDueDate(DateTime dueDate, string userEmail);

        /// <summary>
        /// updates task title
        /// </summary>
        /// <param name="title">new title, should be non empty max. length 50</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public void UpdateTaskTitle(string title, string userEmail);


        /// <summary>
        /// updates task description
        /// </summary>
        /// <param name="description">new description, should be null or at max. length 300</param>
        /// <param name="userEmail">user email, should be the assignee</param>
        public void UpdateTaskDescription(string description, string userEmail);


        /// <summary>
        /// assign task to a user
        /// </summary>
        /// <param name="userEmail">user email, should be current assignee</param>
        /// <param name="toBeAssigned">new assignee</param>
        public void AssignTask(string userEmail, string toBeAssigned);
    }
}
