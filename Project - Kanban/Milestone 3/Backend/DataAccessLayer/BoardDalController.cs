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
    /// class BoardDalController
    /// manages the access to table Boards in the DB
    /// which stores all the boards in our system
    /// </summary>
    class BoardDalController : DalController
    {
        /// <summary>
        /// BoardDalController constructor
        /// we relate here the controller with the table Boards
        /// </summary>
        public BoardDalController() : base("Boards")
        {

        }

        /// <summary>
        /// gets all boards from the table Boards of the DB
        /// </summary>
        /// <returns>a list of BoardDTOs which represents all the table Boards records</returns>
        public List<BoardDTO> SelectAllBoards()
        {
            List<BoardDTO> result = Select().Cast<BoardDTO>().ToList();
            return result;
        }


        /// <summary>
        /// persists a board in the table Boards in the DB
        /// </summary>
        /// <param name="board">Board to be inserted</param>
        /// <returns>true if succeeded, false otherwise</returns>
        public bool Insert(BoardDTO board)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({BoardDTO.IDColumnName},{BoardDTO.CreatorEmailColumnName},{BoardDTO.BoardNameColumnName}) " +
                        $"VALUES (@idVal,@emailVal,@nameVal);";

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", board.ID);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", board.Email);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", board.BoardName);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(nameParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to insert board to DB");
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
        /// converts a record from Boards table to a suitable BoardDTO object
        /// </summary>
        /// <param name="reader">record from Boards table</param>
        /// <returns>appropriate BoardDTO which represents the record</returns>
        protected override BoardDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardDTO result = new BoardDTO((long)reader.GetValue(2), reader.GetString(1), reader.GetString(0));
            return result;

        }
    }
}
