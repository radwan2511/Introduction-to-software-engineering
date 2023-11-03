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
    /// UserDalController class
    /// responsible for accessing the table Users of our Kanban systen DB
    /// which stores all the users in the system
    /// </summary>
    class UserDalController : DalController
    {
        /// <summary>
        /// UserDalController constructor
        /// relates the controller to the table Users of the DB
        /// </summary>
        public UserDalController() : base("Users")
        {

        }

        /// <summary>
        /// gets all the persisted users from the DB
        /// </summary>
        /// <returns>list of all the persisted uers</returns>
        public List<UserDTO> SelectAllUsers()
        {
            List<UserDTO> result = Select().Cast<UserDTO>().ToList();
            return result;
        }

        /// <summary>
        /// persists a user in the table Users of the DB
        /// </summary>
        /// <param name="user">user to be persisted</param>
        /// <returns>true if succeeded, otherwise- false</returns>
        public bool Insert(UserDTO user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({UserDTO.IDColumnName},{UserDTO.EmailColumnName},{UserDTO.PasswordColumnName}) " +
                        $"VALUES (@idVal,@emailVal,@passwordVal);";

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", user.ID);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", user.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter(@"passwordVal", user.Password);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Failed to insert user to DB");
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
        /// converts a Users table record to an appropriate User DTO 
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>user dto which represents the given record</returns>
        protected override UserDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO result = new UserDTO(reader.GetString(0), reader.GetString(1), (long)reader.GetValue(2));
            return result;
        }
    }
}

