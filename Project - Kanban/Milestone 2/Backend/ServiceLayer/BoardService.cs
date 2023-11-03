using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// BoardService class
    /// serves board "missions"
    /// </summary>
    class BoardService
    {
        private readonly BusinessLayer.BoardController controller;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// BoardService controller
        /// </summary>
        /// <param name="empty">an empty list, will hold the connected user each time later</param>
        public BoardService(IList<string> empty)
        {
            controller = new BusinessLayer.BoardController(empty);
        }

        ///<summary>This method loads the boards(including columns and tasks) data from the persistance.
        ///         You should call this function when the program starts. </summary>
        public Response LoadData()
        {
            try
            {
                controller.LoadData();
                log.Info("Loaded boards data successfully");
                return Response<string>.FromValue("Loaded boards data successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        ///<summary>Removes all persistent boards(including columns and tasks) data.</summary>
        public Response DeleteData()
        {
            try
            {
                controller.Delete();
                log.Info("Deleted boards data successfully");
                return Response<string>.FromValue("Deleted boards data successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            try
            {
                controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).tasksLimit = limit;
                log.Info("limited column successfully");
                return Response<string>.FromValue("limited column successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        public Response<int> GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                int limit=controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).tasksLimit;
                log.Info("succeeded to get column limit");
                return Response<int>.FromValue(limit);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<int>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        public Response<string> GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                string name = controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).Name;
                log.Info("got column name");
                return Response<string>.FromValue(name);
            }
            catch(Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
		/// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                BusinessLayer.Task t=controller.GetBoard(userEmail, creatorEmail, boardName).AddTask(title,description,dueDate,userEmail);
                log.Info("Task has been added successfully");
                return Response<Task>.FromValue(new Task(t.Id,t.CreationTime,t.title,t.description,t.dueDate,t.Assignee));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).GetTask(taskId).UpdateTaskDueDate(dueDate,userEmail);
                log.Info("Updated task due date successfully");
                return Response<string>.FromValue("Updated task due date successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).GetTask(taskId).UpdateTaskTitle(title,userEmail);
                log.Info("Updated task title successfully");
                return Response<string>.FromValue("Updated task title successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).GetTask(taskId).UpdateTaskDescription(description, userEmail);
                log.Info("Updated task description successfully");
                return Response<string>.FromValue("Updated task description successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                controller.GetBoard(userEmail, creatorEmail, boardName).AdvanceTask(userEmail,columnOrdinal,taskId);
                log.Info("Advanced task successfully");
                return Response<string>.FromValue("Advanced task successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                BusinessLayer.Column c = controller.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal);
                Column column= new Column(c.tasks, c.tasksLimit,c.Name);
                log.Info("Got column successfully");
                return Response<IList<Task>>.FromValue(column.tasks);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<IList<Task>>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Creates a new board for the logged-in user.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string userEmail, string boardName)
        {
            try
            {
                controller.AddBoard(userEmail,boardName);
                log.Info("Added board successfully");
                return Response<string>.FromValue("Added board successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                controller.JoinBoard(userEmail,creatorEmail,boardName);
                log.Info("Joined board successfully");
                return Response<string>.FromValue("Joined board successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Removes a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                controller.RemoveBoard(userEmail, creatorEmail, boardName);
                log.Info("Removed board successfully");
                return Response<string>.FromValue("Removed board successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> InProgressTasks(string userEmail)
        {
            try
            {
                Column c=new Column(controller.InProgressTasks(userEmail),-1,"in progress");
                log.Info("Got in progress tasks successfully");
                return Response<IList<Task>>.FromValue(c.tasks);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<IList<Task>>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                controller.GetBoard(userEmail,creatorEmail,boardName).GetColumn(columnOrdinal).GetTask(taskId).AssignTask(userEmail,emailAssignee);
                log.Info("Assigned task successfully");
                return Response<string>.FromValue("Assigned task successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<IList<String>> GetBoardNames(string userEmail)
        {
            try
            {
                IList<string> ans = controller.GetBoardNames(userEmail);
                log.Info("Got board names successfully");
                return Response<IList<string>>.FromValue(ans);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<IList<string>>.FromError(e.Message);
            }
        }
    }
}
