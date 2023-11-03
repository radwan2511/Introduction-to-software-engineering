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
    /// <summary>
    /// UserService class
    /// serves "missions" related to Users
    /// </summary>
    public class UserService
    {
        private readonly BusinessLayer.UserController controller;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// UserService constructor
        /// </summary>
        /// <param name="empty">an empty list, will hold later the connected user</param>
        public UserService(IList<string> empty)
        {
            controller = new BusinessLayer.UserController(empty);
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Starting Up!");
        }

        ///<summary>This method loads the users data from the persistance.
        ///         You should call this function when the program starts. </summary>
        public Response LoadData()
        {
            try
            {
                controller.LoadData();
                log.Info("Loaded users data successfully");
                return Response<string>.FromValue("Loaded users data successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }
        ///<summary>Removes all persistent  users data.</summary>
        public Response DeleteData()
        {
            try
            {
                controller.Delete();
                log.Info("Deleted users data successfully");
                return Response<string>.FromValue("Deleted users data successfully");
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userEmail">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string userEmail, string password)
        {
            try
            {
                controller.Register(userEmail, password);
                log.Info("User Registered Successfully");
                return Response<User>.FromValue(new User(userEmail));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="userEmail">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string userEmail, string password)
        {
            try
            {
                BusinessLayer.User u = controller.Login(userEmail, password);
                log.Info("User logged in successfully");
                return Response<User>.FromValue(new User(userEmail));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }

        /// <summary>        
        /// Log out an logged-in user. 
        /// </summary>
        /// <param name="userEmail">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string userEmail)
        {
            try
            {
                controller.LogOut(userEmail);
                log.Info("User logged out successfully");
                return Response<User>.FromValue(new User(userEmail));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }
    }
}

