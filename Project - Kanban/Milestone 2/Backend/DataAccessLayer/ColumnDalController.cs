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
    /// ColumnDalController class
    /// reponsible for accessing the table Columns in the DB
    /// which stores all the board columns in our system
    /// </summary>
    class ColumnDalController:DalController
    {
        /// <summary>
        /// ColumnDalController constructor
        /// relates the controller to the table Columns
        /// </summary>
        public ColumnDalController():base("Columns")
        {

        }

        /// <summary>
        /// gets all columns of a specific board from the DB
        /// </summary>
        /// <param name="b">Board which we wish to get his columns</param>
        /// <returns>list of all board columns</returns>
        public List<ColumnDTO> SelectAllColumns(BoardDTO b)
        {
            List<ColumnDTO> result = Select(b).Cast<ColumnDTO>().ToList();
            return result;
        }


        /// <summary>
        /// inserts a column to the table Columns of the DB
        /// </summary>
        /// <param name="column">column to be persisted</param>
        /// <returns>true if succeeded and false if not</returns>
        public bool Insert(ColumnDTO column)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({ColumnDTO.BoardIDColumnName},{ColumnDTO.LimitColumnName},{ColumnDTO.NameColumnName}) " +
                        $"VALUES (@idVal,@limitVal,@nameVal);";

                     SQLiteParameter idParam = new SQLiteParameter(@"idVal", column.ID);
                    SQLiteParameter limitParam = new SQLiteParameter(@"limitVal", column.Limit);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", column.Name);             

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(limitParam);
                    command.Parameters.Add(nameParam);       
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw new Exception("Failed to insert column to DB");
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
        /// converts a record of table Columns of the DB to an appropriate Column DTO
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>an appropriate column dto which suits to the given record</returns>
        protected override ColumnDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO((long)reader.GetValue(0), reader.GetString(1), (long)reader.GetValue(2));
            return result;
        }


        /// <summary>
        /// gets all columns of a board from DB
        /// </summary>
        /// <param name="b">board which we wish to get its columns</param>
        /// <returns>list of all columns of the given board</returns>
        private List<ColumnDTO> Select(BoardDTO b)
        {
            List<ColumnDTO> results = new List<ColumnDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where BoardID={b.ID};";
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
                catch(Exception)
                {
                    throw new Exception("Failed to get columns of the relevant board from DB");
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
        /// deletes all columns of a given board
        /// </summary>
        /// <param name="DTOObj">board which is being deleted and we wish to remove its columns</param>
        /// <returns>true if succeeded, false if not</returns>
        public bool DeleteBoard(BoardDTO DTOObj)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where {ColumnDTO.BoardIDColumnName}={DTOObj.ID}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete columns of relevant board from DB");
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
