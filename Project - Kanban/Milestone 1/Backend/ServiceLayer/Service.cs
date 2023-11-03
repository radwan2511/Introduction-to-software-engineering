using System.Collections.Generic;
using System;
using System.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Service
    {
        private readonly UserService uService;
        private readonly BoardService bService;
        public Service()
        {
            uService = new UserService();
            bService = new BoardService();
        }
        ///<summary>This method loads the data from the persistance.
        ///         You should call this function when the program starts. </summary>
        public Response LoadData()
        {
            throw new NotImplementedException();
        }
        ///<summary>Removes all persistent data.</summary>
        public Response DeleteData()
        {
            throw new NotImplementedException();
        }
        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response Register(string email, string password)
        {
            Response r= uService.Register(email, password);
            return bService.Register(email, r);
        }
        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            return uService.login(email, password);
        }
        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            return uService.logout(email);
        }
        
        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return r;
            }
            else if (r.Value == false)
            {
                return Response<bool>.FromError("the given user isn't logged in");
            }
            return bService.LimitColumn(email, boardName, columnOrdinal, limit);
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        public Response<int> GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<int>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<int>.FromError("the given user isn't logged in");
            }
            return bService.GetColumnLimit(email, boardName, columnOrdinal);
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        public Response<string> GetColumnName(string email, string boardName, int columnOrdinal)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<string>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<string>.FromError("the given user isn't logged in");
            }
            return bService.GetColumnName(email, boardName, columnOrdinal);
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<Task>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<Task>.FromError("the given user isn't logged in");
            }
            return bService.AddTask(email, boardName, title, description, dueDate);
        }
        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                 return Response<Task>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<Task>.FromError("the given user isn't logged in");
            }
            return bService.updateTaskDueDate(email,boardName,columnOrdinal,taskId,dueDate);
        }
        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<Task>.FromError(r.ErrorMessage); 
            }
            else if (r.Value == false)
            {
                return Response<Task>.FromError("the given user isn't logged in");
            }
            return bService.UpdateTaskTitle(email,boardName,columnOrdinal,taskId,title);
        }
        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<Task>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<Task>.FromError("the given user isn't logged in");
            }
            return bService.UpdateTaskDescription(email, boardName, columnOrdinal, taskId, description);
        }
        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<Task>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<Task>.FromError("the given user isn't logged in");
            }
            return bService.AdvanceTask(email, boardName, columnOrdinal, taskId);
        }
        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> GetColumn(string email, string boardName, int columnOrdinal)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<IList<Task>>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<IList<Task>>.FromError("the given user isn't logged in");
            }
            return bService.GetColumn(email,boardName,columnOrdinal);
        }
        /// <summary>
        /// Adds a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string email, string name)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return r;
            }
            else if (r.Value == false)
            {
                return Response<bool>.FromError("the given user isn't logged in");
            }
            return bService.AddBoard(email, name);
        }
        /// <summary>
        /// Removes a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string email, string name)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return r;
            }
            else if (r.Value == false)
            {
                return Response<bool>.FromError("the given user isn't logged in");
            }
            return bService.RemoveBoard(email, name);
        }
        /// <summary>
        /// Returns all the In progress tasks of the user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> InProgressTasks(string email)
        {
            Response<bool> r = uService.isLoggedIn(email);
            if (r.ErrorMessage != null)
            {
                return Response<IList<Task>>.FromError(r.ErrorMessage);
            }
            else if (r.Value == false)
            {
                return Response<IList<Task>>.FromError("the given user isn't logged in");
            }
            return bService.InProgressTasks(email);
        }
    }
}
