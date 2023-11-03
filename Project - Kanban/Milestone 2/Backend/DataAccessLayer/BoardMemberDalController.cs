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
    /// BoardMemberDalController class
    /// resposible for the access to the table BoardMembers
    /// which contains all the relations between emails of users to Boards they created/joined
    /// </summary>
    class BoardMemberDalController:DalController
    {
        /// <summary>
        /// BoardMemberDalController constructor
        /// relates the table with BoardMembers table
        /// </summary>
        public BoardMemberDalController():base("BoardMembers")
        {

        }

        /// <summary>
        /// gets all board members of a specific board
        /// </summary>
        /// <param name="b">some board we want to get its members</param>
        /// <returns>the board members list</returns>
        public List<BoardMemberDTO> SelectAllBoardMembers(BoardDTO b)
        {
            List<BoardMemberDTO> result = Select(b).Cast<BoardMemberDTO>().ToList();
            return result;
        }


        /// <summary>
        /// persists a board member in the DB
        /// </summary>
        /// <param name="boardMember">board member to be persisted</param>
        /// <returns>true if succeeded, false otherwise</returns>
        public bool Insert(BoardMemberDTO boardMember)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({BoardMemberDTO.BoardIDColumnName},{BoardMemberDTO.MemberIDColumnName},{BoardMemberDTO.MemberEmailColumnName}) " +
                        $"VALUES (@idVal,@memIDVal,@memberVal);";
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardMember.ID);
                    SQLiteParameter memIdParam = new SQLiteParameter(@"memIDVal", boardMember.MemberID);
                    SQLiteParameter memberParam = new SQLiteParameter(@"memberVal", boardMember.MemberEmail);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(memIdParam);
                    command.Parameters.Add(memberParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw new Exception("Failed to insert board member to DB");
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
        /// converts record from BoardMembers table of the DB to an appropriate Board Member DTO
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>an appropriate Board Member DTO which represents the given record</returns>
        protected override BoardMemberDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardMemberDTO result = new BoardMemberDTO( reader.GetString(0), (long)reader.GetValue(1), (long)reader.GetValue(2));
            return result;
        }


        /// <summary>
        /// gets all the members of a specific board
        /// </summary>
        /// <param name="b">board which we want to get its members</param>
        /// <returns>list of all board members</returns>
        private List<BoardMemberDTO> Select(BoardDTO b)
        {
            List<BoardMemberDTO> results = new List<BoardMemberDTO>();
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
                catch (Exception)
                {
                    throw new Exception("Failed to get board members from DB");
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
        /// deletes all members of a board from DB
        /// </summary>
        /// <param name="DTOObj">board which is being deleted and we want to delete its members</param>
        /// <returns>true if succeede, false if not</returns>
        public bool DeleteBoard(BoardDTO DTOObj)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where BoardID={DTOObj.ID}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete board members of the relevant board from DB");
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
