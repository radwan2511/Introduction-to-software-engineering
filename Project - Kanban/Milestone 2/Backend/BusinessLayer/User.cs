using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.BusinessLayer
{   
    /// <summary>
    /// User class
    /// </summary>
    class User
    {
        public readonly string email;
        private readonly string password;
        /// <summary>
        /// useful constants
        /// </summary>
        private readonly int PASSWORD_MIN_LEN = 4;
        private readonly int PASSWORD_MAX_LEN = 20;
        
        public readonly long id;

        /// <summary>
        /// User constructor
        /// </summary>
        /// <param name="email">a valid email address for the user</param>
        /// <param name="password">a valid password(length: 4-20, contains at least 
        /// one upper letter, one lower and one digit)</param>
        /// <param name="id">user id</param>
        public User(string email, string password,long id)
        {
            if (!IsValidEmail(email))
            {
                throw new Exception("Invalid email");
            }
            if (!ValidatePasswordRules(password))
            {
                throw new Exception("password");
            }
            this.email = email;
            this.password = password;
            this.id = id;
        }

        /// <summary>
        /// checks if the given string is the users' password
        /// </summary>
        /// <param name="password">a string to be checked</param>
        /// <returns>true if the given string is the correct users' password, else false</returns>
        public bool IsCorrectPassword(string password)
        {
            return this.password == password;
        }

        /// <summary>
        /// checks if a given string is a legal email address
        /// </summary>
        /// <param name="email">a string to be checked</param>
        /// <returns>if the string is a legal email address- true, otherwise- false</returns>
        private bool IsValidEmail(string email)
        {
            string regex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (email == null)
            {
                return false;
            }
            return Regex.IsMatch(email, regex);


        }

        /// <summary>
        /// checks a legality of a given password(length 4-20 and at least-one upper letter, one lower letter, one digit)
        /// </summary>
        /// <param name="password">a string to be checked</param>
        /// <returns>true if the given string is a legal password, else- false</returns>
        private bool ValidatePasswordRules(string password)
        {
            bool isValid = true;

            // #1 password length (4 - 20)
            isValid = password.Length >= PASSWORD_MIN_LEN & password.Length <= PASSWORD_MAX_LEN;


            // #2 must contain 1 upper char and 1 lower char and 1 digit
            if (isValid)
            {
                bool atLeastOneDigit = false;
                bool atLeastOneLower = false;
                bool atLeastOneUpper = false;
                for (int i = 0; i < password.Length & !(atLeastOneDigit & atLeastOneLower & atLeastOneUpper); i++)
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
    }
}
