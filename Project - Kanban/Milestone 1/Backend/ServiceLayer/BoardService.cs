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
    class BoardService
    {
        private readonly Dictionary<string, BusinessLayer.BoardController> userBoards;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public BoardService()
        {
            userBoards = new Dictionary<string, BusinessLayer.BoardController>();
        }
        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="r">Response which retrieved from UserService register.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response Register(string email,Response r)
        {
            if (r.ErrorMessage != null)
            {
                return r;
            }
            Response<BusinessLayer.BoardController> rB =(Response < BusinessLayer.BoardController >) r;
            userBoards[email] = rB.Value;
            return Response<User>.FromValue(new User(email));
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<bool>.FromError("email doesn't exist in the system");
                }
                userBoards[email].limitColumn(boardName, columnOrdinal, limit);
                log.Info("limited column successfully");
                return Response<bool>.FromValue(true);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<bool>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<int>.FromError("email doesn't exist in the system");
                }
                int lim=userBoards[email].GetBoard(boardName).GetColumnLimit(columnOrdinal);
                log.Info("succeeded to get column limit");
                return Response<int>.FromValue(lim);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<int>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<Task>.FromError("email doesn't exist in the system");
                }
                int id=userBoards[email].GetBoard(boardName).addToBacklog(dueDate, title, description, boardName);
                BusinessLayer.Task t = userBoards[email].GetBoard(boardName).getTask(id);
                log.Info("added new task successfully");
                return Response<Task>.FromValue(new Task(id,t.getCreationTime(),title,description,dueDate));
            }
            catch(Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
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

        public Response<Task> updateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<Task>.FromError("email doesn't exist in the system");
                }
                BusinessLayer.Task t = userBoards[email].GetBoard(boardName).getTaskByCol(taskId,columnOrdinal);
                t.setDueDate(dueDate);
                log.Info("tasks' due date has been updated successfully");
                return Response<Task>.FromValue(new Task(taskId, t.getCreationTime(), t.getTitle(), t.getDescription(), dueDate));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<Task>.FromError("email doesn't exist in the system");
                }
                BusinessLayer.Task t = userBoards[email].GetBoard(boardName).getTaskByCol(taskId,columnOrdinal);
                t.setTitle(title);
                log.Info("tasks' title has been updated successfully");
                return Response<Task>.FromValue(new Task(taskId, t.getCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate()));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<Task>.FromError("email doesn't exist in the system");
                }
                BusinessLayer.Task t = userBoards[email].GetBoard(boardName).getTaskByCol(taskId,columnOrdinal);
                t.setDescription(description);
                log.Info("tasks' description has been updated successfully");
                return Response<Task>.FromValue(new Task(taskId, t.getCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate()));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<Task>.FromError("email doesn't exist in the system");
                }
                BusinessLayer.Board b = userBoards[email].GetBoard(boardName);
                BusinessLayer.Task t = b.getTaskByCol(taskId,columnOrdinal);
                if (columnOrdinal == 0)
                {
                    b.moveBacklogToInProgress(taskId);
                }
                if (columnOrdinal == 1)
                {
                    b.moveInProgressToDone(taskId);
                }
                if (columnOrdinal < 0 || columnOrdinal >= 2)
                {
                    log.Debug("illegal column ordinal");
                    return Response<Task>.FromError("illegal column ordinal");
                }
                log.Info("task has been advanced successfully");
                return Response<Task>.FromValue(new Task(taskId, t.getCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate()));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<Task>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string email, string name)
        {
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<bool>.FromError("email doesn't exist in the system");
                }
                userBoards[email].addBoard(name);
                log.Info("board has been added successfully");
                return Response<bool>.FromValue(true);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<bool>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Removes a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string email, string name)
        {
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<bool>.FromError("email doesn't exist in the system");
                }
                userBoards[email].removeBoard(name);
                log.Info("board has been removed successfully");
                return Response<bool>.FromValue(true);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<bool>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Returns all the In progress tasks of the user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> InProgressTasks(string email)
        {
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<IList<Task>>.FromError("email doesn't exist in the system");
                }
                IList<Task> ans = new List<Task>();
                IList<BusinessLayer.Task> l=userBoards[email].getInProgressTasks();
                foreach(BusinessLayer.Task t in l)
                {
                    ans.Add(new Task(t.getId(), t.getCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate()));
                }
                log.Info("got all in progress tasks successfully");
                return Response<IList<Task>>.FromValue(ans);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<IList<Task>>.FromError(e.Message);
            }
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
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<string>.FromError("email doesn't exist in the system");
                }
                userBoards[email].GetBoard(boardName);
                if (columnOrdinal == 0)
                {
                    log.Info("got column name=backlog");
                    return Response<string>.FromValue("backlog");
                }
                if (columnOrdinal == 1)
                {
                    log.Info("got column name=in progress");
                    return Response<string>.FromValue("in progress");
                }
                if (columnOrdinal == 2)
                {
                    log.Info("got column name=done");
                    return Response<string>.FromValue("done");
                }
                log.Debug("Invalid column ordinal");
                return Response<string>.FromError("invalid column ordinal");
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
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                if (!userBoards.ContainsKey(email))
                {
                    log.Debug("email doesn't exist in the system");
                    return Response<IList<Task>>.FromError("email doesn't exist in the system");
                }
                IList<Task> ans = new List<Task>();
                IList<BusinessLayer.Task> l = userBoards[email].GetBoard(boardName).GetColumn(columnOrdinal);
                foreach (BusinessLayer.Task t in l)
                {
                    ans.Add(new Task(t.getId(), t.getCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate()));
                }
                log.Info("got column successfully");
                return Response<IList<Task>>.FromValue(ans);
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<IList<Task>>.FromError(e.Message);
            }
        }
    }
}
