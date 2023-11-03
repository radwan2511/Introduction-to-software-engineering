using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// DalController abstract class
    /// responsible of accessing some table in the DB
    /// </summary>
    internal abstract class DalController
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;

        /// <summary>
        /// DalController constructor
        /// relates the controller to the appropriate table in our Kanban system DB
        /// </summary>
        /// <param name="tableName"></param>
        public DalController(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;
        }


        /// <summary>
        /// updates a record in the controller related table(when assigned value is a string)
        /// </summary>
        /// <param name="primaryKeys">primary keys titles in the table which uniquely idetifies each table record</param>
        /// <param name="primaryVals">values which assigned to each primary key in the record we want to update</param>
        /// <param name="attributeName">attribute title which we wish to update</param>
        /// <param name="attributeValue">new value to assign to the given attribute</param>
        /// <returns>true if succeeded to update the record and false if not</returns>
        public bool Update(string[] primaryKeys,object[] primaryVals, string attributeName, string attributeValue)
        {
            string whereCond = makeWhereCond(primaryKeys, primaryVals);
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where "+whereCond
                };
                try
                {

                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw new Exception("Failed to update DB recrord");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }


        /// <summary>
        /// a helper function which completes a where condition in sql query 
        /// </summary>
        /// <param name="primaryKeys">primary keys of the table</param>
        /// <param name="primaryVals">values which assigned to the desired record primary keys</param>
        /// <returns>an appropriate where condition</returns>
        private string makeWhereCond(string[]primaryKeys,object[] primaryVals)
        {
            string whereCond = "";
            if (primaryVals[0] is string)
                whereCond += $"{primaryKeys[0]}='{primaryVals[0].ToString()}'";
            else
                whereCond += $"{primaryKeys[0]}={(long)primaryVals[0]}";
            for (int i = 1; i < primaryKeys.Length; i++)
            {
                if (primaryVals[i] is string)
                    whereCond += $" and {primaryKeys[i]}='{primaryVals[i].ToString()}'";
                else
                    whereCond += $" and {primaryKeys[i]}={(long)primaryVals[i]}";
            }
            return whereCond;
        }

        /// <summary>
        /// updates a record in the controller related table(when assigned value is a long)
        /// </summary>
        /// <param name="primaryKeys">primary keys titles in the table which uniquely idetifies each table record</param>
        /// <param name="primaryVals">values which assigned to each primary key in the record we want to update</param>
        /// <param name="attributeName">attribute title which we wish to update</param>
        /// <param name="attributeValue">new value to assign to the given attribute</param>
        /// <returns>true if succeeded to update the record and false if not</returns>
        public bool Update(string[] primaryKeys, object[] primaryVals, string attributeName, long attributeValue)
        {
            string whereCond = makeWhereCond(primaryKeys, primaryVals);
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where "+whereCond
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw new Exception("Failed to update DB record");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

            }
            return res > 0;
        }


        /// <summary>
        /// gets all records from the table
        /// </summary>
        /// <returns>a list of DTOs which represents all table records</returns>
        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};";
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
                    throw new Exception("Failed to access DB");
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
        /// convert a table record to an appropriate DTO
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        /// <summary>
        /// deletes a record from the table
        /// </summary>
        /// <param name="DTOObj">DTO which represents the desired record in the table</param>
        /// <returns>true if succeeded, false if not</returns>
        public bool Delete(DTO DTOObj)
        {
            int res = -1;
            string[] primaryKeys = DTOObj._primaryKeys;
            object[] primaryVals = DTOObj._primaryVals;
            string whereCond = makeWhereCond(primaryKeys, primaryVals);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where "+whereCond
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete record from DB");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        /// <summary>
        /// deletes all the persisted data in the table
        /// </summary>
        /// <returns>true if succeeded, false if not</returns>
        public bool DeleteAll()
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete all records from DB table");
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
