using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class UserController
    {
        private Dictionary<string, User> usersByEmail;

        private readonly bool logged;

        /*
        UserController constructor

        input: none
        output: none
         */
        public UserController()
        {
            usersByEmail = new Dictionary<string, User>();
            logged = false;
        }


        /*
        creates a new user and add him to the dictionary.


        input: string email, string password
        output: User (return null if user cant be registered due to invalid email or password)
         */
        public User register(string email,string password)
        {
            User user = null;
            if (usersByEmail.ContainsKey(email))
            {
                throw new Exception("user has already registered");
            }
            if (isValidEmail(email) & validatePasswordRules(password))
            {
                user = new User(email, password);
                usersByEmail.Add(email, user);
                return user;
            }
            else
                throw new Exception("email or password is not valid.");
        }

        /*
        first check if the given email address string is in a valid structure of email address  
        and second check if the current email is used by another user

        input: string email
        output: boolean isValid
         */
        private Boolean isValidEmail(string email)
        {
            string regex= @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (email == null)
            {
                return false;
            }
            return Regex.IsMatch(email, regex);
            

        }

        /*
        check if the given password string is in a valid structure of password

        input: string password
        output: boolean isValid
         */
        private Boolean validatePasswordRules(string password)
        {
            Boolean isValid = true;

            // #1 password length (4 - 20)
            isValid = password.Length >= 4 & password.Length <= 20;


            // #2 must contain 1 upper char and 1 lower char and 1 digit
            if (isValid)
            {
                Boolean atLeastOneDigit = false;
                Boolean atLeastOneLower = false;
                Boolean atLeastOneUpper = false;
                for (int i=0; i<password.Length & !(atLeastOneDigit & atLeastOneLower & atLeastOneUpper); i++)
                {
                    if (!atLeastOneDigit)
                    {
                        atLeastOneDigit = Char.IsDigit(password[i]);
                    }

                    if (!atLeastOneLower)
                    {
                        atLeastOneLower = Char.IsLower(password[i]);
                    }

                    if (!atLeastOneUpper)
                    {
                        atLeastOneUpper = Char.IsUpper(password[i]);
                    }
                }


                isValid = (atLeastOneDigit & atLeastOneLower & atLeastOneUpper);
            }

            return isValid;
        }

        /*
        check if the given email related to an existing user and if so, return the user 

        input: string email
        output: User user (return null if the email is not related to any user)
         */
        public User GetUser(string email)
        {
            if (usersByEmail.ContainsKey(email))
            {
                return usersByEmail[email];
            }
            else
            {
                throw new Exception("email doesn't registerd in the system.");
                //return null;
            }
        }

        /*
        check if the given email related to an existing user and if so, return the user and log him in

        input: string email
        output: User user (return null if the email is not related to any user)
         */
        public User login(string email,string password)
        {

            if (!usersByEmail.ContainsKey(email))
            {
                throw new Exception("there isn't an existing user with the given name");
            }
            if (logged)
            {
                throw new Exception("there's already a logged user");
            }
            User user = GetUser(email);
            user.logIn(password);
            return user;
        }

        /*
        logs out a user

       input: string email
       output: none
        */

        public void logOut(string email)
        {
            try
            {
                if (!usersByEmail.ContainsKey(email))
                {
                    throw new Exception("there isn't an existing user with the given name");
                }
                User user = GetUser(email);
                user.logout();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
