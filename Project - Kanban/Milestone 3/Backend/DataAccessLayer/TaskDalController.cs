using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// TaskDalController class
    /// responsible for accessing Tasks table in the DB
    /// which stores all the tasks in our system
    /// </summary>
    class TaskDalController : DalController
    {
        /// <summary>
        /// TaskDalController constructor
        /// relates the controller to Tasks table in DB
        /// </summary>
        public TaskDalController() : base("Tasks")
        {

        }

        /// <summary>
        /// gets all tasks of a desired column
        /// </summary>
        /// <param name="c">column which we want to get its tasks</param>
        /// <returns>list of all the tasks of the desired columns</returns>
        public List<TaskDTO> SelectAllTasks(IColumnDTO c)
        {
            List<TaskDTO> result = Select(c).Cast<TaskDTO>().ToList();
            return result;
        }


        /// <summary>
        /// inserts a task to the Tasks table in the DB
        /// </summary>
        /// <param name="task">task to be persisted</param>
        /// <returns>true if succeeded, false otherwise</returns>
        public bool Insert(TaskDTO task)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({TaskDTO.BoardIDColumnName},{TaskDTO.ID} ,{TaskDTO.CreationTimeColumnName},{TaskDTO.DueDateColumnName},{TaskDTO.TitleColumnName},{TaskDTO.DescriptionColumnName},{TaskDTO.AssigneeColumnName},{TaskDTO.IsDoneColumnName},{TaskDTO.ColumnOrdinalColumnName}) " +
                        $"VALUES (@bIdVal,@idVal,@creationVal,@dueVal,@titleVal,@descriptionVal,@assigneeVal,@doneVal,@colVal);";


                    SQLiteParameter bIdParam = new SQLiteParameter(@"bIdVal", task.BoardId);
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", task.Id);
                    SQLiteParameter creationParam = new SQLiteParameter(@"creationVal", task.CreationTime);
                    SQLiteParameter dueParam = new SQLiteParameter(@"dueVal", task.DueDate);
                    SQLiteParameter titleParam = new SQLiteParameter(@"titleVal", task.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"descriptionVal", task.Description);
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"assigneeVal", task.Assignee);
                    SQLiteParameter doneParam = new SQLiteParameter(@"doneVal", task.IsDone);
                    SQLiteParameter columnOrdinalParam = new SQLiteParameter(@"colVal", task.ColumnOrdinal);

                    command.Parameters.Add(bIdParam);
                    command.Parameters.Add(idParam);
                    command.Parameters.Add(creationParam);
                    command.Parameters.Add(dueParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(assigneeParam);
                    command.Parameters.Add(doneParam);
                    command.Parameters.Add(columnOrdinalParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to insert Task to DB");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }


        /// <summary>
        /// converts a record from the Tasks table to an appropriate TaskDTO
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>task dto which represents this record</returns>
        protected override TaskDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO((long)reader.GetValue(3), DateTime.Parse(reader.GetString(0)), DateTime.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(4), reader.GetString(5), StringToBool(reader.GetString(6)), (long)reader.GetValue(7), (long)reader.GetValue(8));
            return result;
        }

        /// <summary>
        /// a helper method which converts a string from DB which represents a bolean
        /// to a bolean(for the is-done column of the table)
        /// </summary>
        /// <param name="done">"0" or "1" or "True" or "False"</param>
        /// <returns>false or true respectively</returns>
        private bool StringToBool(string done)
        {
            if (done == "1" || done == "True")
                return true;
            if (done == "0" || done == "False")
            {
                return false;
            }
            throw new Exception("Invalid is done field");
        }


        /// <summary>
        /// gets all tasks of a desired column from DB table Tasks
        /// </summary>
        /// <param name="c">the column which we wish to get its tasks</param>
        /// <returns>a list of all tasks related to this column</returns>
        private List<TaskDTO> Select(IColumnDTO c)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where BoardID={c.ID} and ColumnOrdinal={c.ColOrdinal};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                catch (Exception)
                {
                    throw new Exception("Failed to get all column tasks from DB");
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }


        /// <summary>
        /// deletes all tasks that belong to a specific board 
        /// </summary>
        /// <param name="DTOObj">a board which is being deleted which we want to delete its tasks</param>
        /// <returns>true if succeeded, false otherwise</returns>
        public bool DeleteBoard(BoardDTO DTOObj)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where BoardID = {DTOObj.ID}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete all board tasks from DB");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }
    }
}