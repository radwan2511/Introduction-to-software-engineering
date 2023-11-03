using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// Class UserDTO
    /// represents a record in Users table(id,email,password)
    /// </summary>
    class UserDTO : DTO
    {
        /// <summary>
        /// Users table(DB) column names:
        /// </summary>
        public const string EmailColumnName = "Email";
        public const string PasswordColumnName = "Password";
        public const string IDColumnName = "ID";
        /// <summary>
        /// user id and its getter
        /// </summary>
        private long _id;
        public long ID { get => _id; }
        /// <summary>
        /// user email and its getter & setter
        /// </summary>
        private string _email;
        public string Email { get => _email; private set { _email = value; _controller.Update(_primaryKeys, _primaryVals, EmailColumnName, value); } }
        /// <summary>
        /// user password and its getter & setter
        /// </summary>
        private string _password;
        public string Password { get => _password; private set { _password = value; _controller.Update(_primaryKeys, _primaryVals, PasswordColumnName, value); } }

        /// <summary>
        /// UserDTO constructor
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <param name="Id">user id</param>
        public UserDTO(string email, string password, long Id) : base(new UserDalController(), new string[] { IDColumnName }, new object[] { Id })
        {
            _id = Id;
            _email = email;
            _password = password;
        }
    }
}
