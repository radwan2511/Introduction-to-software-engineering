using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// User controller class
    /// contains all the users in our kanban system
    /// </summary>
    class UserController
    {
        /// <summary>
        /// a dictionary which matches email to registered user
        /// </summary>
        private readonly Dictionary<string, User> Users;
        /// <summary>
        /// contains the connected user at the moment, if exists
        /// </summary>
        private readonly IList<string> ConnectedUser;
        private readonly UserDalController userDalController;

        /// <summary>
        /// User controller constructor
        /// </summary>
        /// <param name="empty">An empty list which is created in the service layer and is joint with the
        /// board controller. This list will hold the current connected user</param>
        public UserController(IList<string> empty)
        {
            Users = new Dictionary<string, User>();
           /* the idea: both user-controller and board-controller will hold
            joint list which contains the email of the connected User
            to the board-controller can know whether a user is logged in or not
            without needing aggregating other classes */
            if (empty == null || empty.Count != 0)
            {
                throw new Exception("we expect the list to be empty");
            }
            ConnectedUser = empty;
            userDalController = new UserDalController();
        }

        /// <summary>
        /// Registers a user to the system
        /// </summary>
        /// <param name="email">email of the user, should be a valid email</param>
        /// <param name="password">password of the user, should be legal(length 4-20 and contains
        /// at least one upper, one lower and one digit</param>
        public void Register(string email, string password)
        {
            if (Users.ContainsKey(email))
            {
                throw new Exception("user has already registered");
            }
            User registered = new User(email, password,Users.Keys.Count+1);
            Users.Add(email, registered);
            userDalController.Insert(new UserDTO(email, password,Users.Keys.Count));
        }

        /// <summary>
        /// Logs in a user into the system
        /// </summary>
        /// <param name="email">an email address, should be registered</param>
        /// <param name="password">a password, should be correct password of the specified user</param>
        /// <returns>the specified user(if succeeded to log in)</returns>
        public User Login(string email, string password)
        {

            if (!Users.ContainsKey(email))
            {
                throw new Exception("there isn't an existing user with the given name");
            }
            if (ConnectedUser.Count > 0)
            {
                throw new Exception("there's already a logged user");
            }
            User user = Users[email];
            if (!user.IsCorrectPassword(password))
            {
                throw new Exception("wrong password");
            }
            ConnectedUser.Add(email);
            return user;
        }

        /// <summary>
        /// logs out a user from the system
        /// </summary>
        /// <param name="email">user email, should be logged in</param>
        public void LogOut(string email)
        {
            try
            {
                if (!Users.ContainsKey(email))
                {
                    throw new Exception("there isn't an existing user with the given name");
                }
                if (!ConnectedUser.Contains(email))
                {
                    throw new Exception("the given user isn't logged in");
                }
                ConnectedUser.Remove(email);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// deletes all users data from the DB
        /// </summary>
        public void Delete()
        {
            userDalController.DeleteAll();
        }


        /// <summary>
        /// loads all users data from the DB
        /// </summary>
        public void LoadData()
        {
            IList<UserDTO> users = userDalController.SelectAllUsers();
            foreach(UserDTO u in users)
            {
                this.Users.Add(u.Email, new User(u.Email, u.Password,u.ID));
            }
        }
    }
}
