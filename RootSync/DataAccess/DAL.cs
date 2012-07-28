using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using www.Controllers;
using www.Models;
using rootsync.Business.Models;

namespace www.DataAccess
{
    public class DAL
    {
        //static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ToString());

        /// <summary>
        /// <para>Validates whether the user entered credentials match with those in our database.</para>
        /// <para>LastLogin will NOT be updated.</para>
        /// </summary>
        /// <param name="Username">Users name/email in a string</param>
        /// <param name="Password">Users password in a string</param>
        /// <returns>UserID value from the database</returns>
        public static int UserIsValid(string username, string password) {
            return UserIsValid(username, password, false);
        }

        /// <summary>
        /// <para>Validates whether the user entered credentials match with those in our database.</para>
        /// </summary>
        /// <param name="Username">Users name/email in a string</param>
        /// <param name="Password">Users password in a string</param>
        /// <param name="updateLastLogin">Whether to update the LastLogin column with DateTime.Now.</param>
        /// <returns>UserID value from the database</returns>
        public static Int32 UserIsValid(string Username, string Password, bool updateLastLogin)
        {
            Int32 userID = -1;

            string query = string.Format("SELECT UserID, Password FROM [User] WHERE Username = '{0}'", Username);

            //using(SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ToString())) {
            using (RootSyncContext context = new RootSyncContext()) {

                User usr = context.Users.FirstOrDefault(u => u.Username == Username);
                bool valid = Helper.VerifyHash(Password, "SHA512", usr.Password);
                if (valid) { userID = usr.UserID; }

                if (usr != null && updateLastLogin) {
                    usr.LastLogin = DateTime.Now;
                    context.SaveChanges();
                }

                context.Dispose();
            }

            return userID;
        }

        /// <summary>
        /// Registration of a new account.  This takes in our very basic user registration form and enters it into our database.
        /// </summary>
        /// <param name="First">Users First name as a string.</param>
        /// <param name="Last">Users Last name as a string</param>
        /// <param name="Username">Desired username for this user as a string.</param>
        /// <param name="Password">Users chosen password as a string.</param>
        /// <returns>UserID from the database for this user.</returns>
        public static Int32 RegisterAccount(string First, string Last, string Username, string Password)
        {
            Int32 UserID = -1;
                
            string hash = Helper.ComputeHash(Password, "SHA512", null);

            using (RootSyncContext context = new RootSyncContext()) {
                User usr = new User();
                usr.First = First;
                usr.Last = Last;
                usr.Username = Username;
                usr.Password = hash;
                usr.LastLogin = DateTime.Now;

                context.Users.Add(usr);

                context.SaveChanges();

                UserID = usr.UserID;

                context.Dispose();
            }

            return UserID;
        }


        /// <summary>
        /// Retrieves a users entire profile from the database. 
        /// </summary>
        /// <param name="UserID">Database UserID to be retrieved</param>
        /// <returns>accountModel object with data for this user.</returns>
        public static accountModel retAccount(Int32 UserID)
        {
            accountModel user = null;

            using (RootSyncContext context = new RootSyncContext()) {
                User usr = context.Users.FirstOrDefault(u => u.UserID == UserID);
                if (usr != null) {
                    user = new accountModel();
                    user.First = usr.First;
                    user.Last = usr.Last;
                    user.Username = usr.Username;
                }

                context.Dispose();
            }

            return user;            
        }


        /// <summary>
        /// Recieves a accountModel and updates the data in our database accordingly.
        /// </summary>
        /// <param name="UserID">Database UserID for this user.</param>
        /// <param name="model">accountModel containing the users updated information.</param>
        /// <returns>Returns a true/false if the database update was successful.</returns>
        public static void UpdateAccount(Int32 UserID, accountModel model)
        {
            using (RootSyncContext context = new RootSyncContext()) {
                User usr = context.Users.First(u => u.UserID == UserID);

                usr.First = model.First;
                usr.Last = model.Last;

                context.SaveChanges();

                context.Dispose();
            }
        }
    }
}