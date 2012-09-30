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
        public static Guid UserIsValid(string username, string password) {
            return UserIsValid(username, password, false);
        }

        /// <summary>
        /// <para>Validates whether the user entered credentials match with those in our database.</para>
        /// </summary>
        /// <param name="Username">Users name/email in a string</param>
        /// <param name="Password">Users password in a string</param>
        /// <param name="updateLastLogin">Whether to update the LastLogin column with DateTime.Now.</param>
        /// <returns>UserID value from the database</returns>
        public static Guid UserIsValid(string Username, string Password, bool updateLastLogin)
        {
            Guid guid = Guid.Empty;

            string query = string.Format("SELECT UserID, Password FROM [User] WHERE Username = '{0}'", Username);

            //using(SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ToString())) {
            using (RootSyncContext context = new RootSyncContext()) {

                User usr = context.Users.FirstOrDefault(u => u.Username == Username);
                bool valid = Helper.VerifyHash(Password, "SHA512", usr.Password);
                if (valid) { guid = usr.guid; }

                if (usr != null && updateLastLogin) {
                    usr.LastLogin = DateTime.Now;
                    usr.numLogins++;
                    context.SaveChanges();
                }
            }

            return guid;
        }

        /// <summary>
        /// Registration of a new account.  This takes in our very basic user registration form and enters it into our database.
        /// </summary>
        /// <param name="First">Users First name as a string.</param>
        /// <param name="Last">Users Last name as a string</param>
        /// <param name="Username">Desired username for this user as a string.</param>
        /// <param name="Password">Users chosen password as a string.</param>
        /// <returns>UserID from the database for this user.</returns>
        public static Guid RegisterAccount(string First, string Last, string Username, string Password)
        {
                
            string hash = Helper.ComputeHash(Password, "SHA512", null);

            using (RootSyncContext context = new RootSyncContext()) {
                User usr = new User();
                usr.guid = Guid.NewGuid();
                usr.First = First;
                usr.Last = Last;
                usr.Username = Username;
                usr.Password = hash;
                usr.LastLogin = DateTime.Now;
                usr.FirstLogin = DateTime.Now;

                context.Users.Add(usr);
                context.SaveChanges();
                return usr.guid;
            }
        }

        /// <summary>
        /// Recieves a accountModel and updates the data in our database accordingly.
        /// </summary>
        /// <param name="UserID">Database UserID for this user.</param>
        /// <param name="model">accountModel containing the users updated information.</param>
        /// <returns>Returns a true/false if the database update was successful.</returns>
        public static void UpdateAccount(Guid guid, accountModel model)
        {
            using (RootSyncContext context = new RootSyncContext()) {
                User usr = context.Users.First(u => u.guid == guid);

                usr.First = model.First;
                usr.Last = model.Last;

                context.SaveChanges();
            }
        }
    }
}