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

    public class UserService
    {
        private readonly BusinessLayer.UserController controller;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UserService()
        {
            controller = new BusinessLayer.UserController();
            var logRepository= LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Starting Up!");
        }

        /// <summary>
        /// 
        /// Load the data. Must be invoked explicitly by the user when the system starts
        /// 
        /// </summary>
        /// <returns>A response instance</returns>
        public Response LoadData()
        {
            return new Response("Not impleneted yet");
        }

        /// <summary>
        /// Registers a user in the system
        /// </summary>
        /// <param name="email">User's email. Assumed not to be registered yet</param>
        /// <param name="password">User's password</param>
        /// <returns>Response<BoardController> in case of success, otherwise error response<User></User> </BoardController></returns>
        public Response Register(string email, string password)
        {
            try
            {
                BusinessLayer.User u=controller.register(email, password);
                log.Info("User Registered Successfully");
                return Response<BusinessLayer.BoardController>.FromValue(u.GetBoardController());
            }
            catch(Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }
        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response<User> login(string email, string password)
        {
            try
            {
                BusinessLayer.User u = controller.login(email, password);
                log.Info("User logged in successfully");
                return Response<User>.FromValue(new User(email));
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }
        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response<User> logout(string email)
        {
            try
            {
                controller.logOut(email);
                log.Info("User logged out successfully");
                return Response<User>.FromValue(new User(email));
            }
            catch(Exception e)
            {
                log.Debug(e.Message);
                return Response<User>.FromError(e.Message);
            }
        }
        ///<summary>This method checks if a user is logged in.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<returns cref="Response">The response of the action which tells us whether the user is logged in to the system or not</returns>
        public Response<bool> isLoggedIn(string email)
        {
            try
            {
                BusinessLayer.User u=controller.GetUser(email);
                bool logged = u.getLoggedIn();
                if (!logged)
                    log.Debug("the given user isn't logged in");
                return Response<bool>.FromValue(logged);

            }catch(Exception e)
            {
                log.Debug(e.Message);
                return Response<bool>.FromError(e.Message);
            }
        }
    }
}

