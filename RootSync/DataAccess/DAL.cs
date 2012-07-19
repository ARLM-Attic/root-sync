using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using www.Controllers;
using www.Models;

namespace www.DataAccess
{
    public class DAL
    {
        static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ToString());


        /// <summary>
        /// Validates whether the user entered credentials match with those in our database.
        /// </summary>
        /// <param name="Username">Users name/email in a string</param>
        /// <param name="Password">Users password in a string</param>
        /// <returns>UserID value from the database</returns>
        public static Int32 UserIsValid(string Username, string Password)
        {
            Int32 UserID = -1;

            string query = string.Format("SELECT UserID, Password FROM [User] WHERE Username = '{0}'", Username);

            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    sdr.Read();

                    //compare user entered password with the encrypted one in our database
                    bool valid = Helper.VerifyHash(Password, "SHA512", (string)sdr["Password"]);
                    if (valid)
                        UserID = (Int32)sdr["UserID"];

                    sdr.Close();
                }


                if (UserID != -1) //if falid login, Update last login so we can track this
                {
                    query = string.Format("UPDATE [User] SET LastLogin='{0}' WHERE UserId='{1}';", DateTime.Now, UserID);
                    try
                    {
                        cmd = new SqlCommand(query, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                }
            }
            catch { UserID = -1; }
            finally { conn.Close(); }

            return UserID;
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

            string query = string.Format("INSERT INTO [User] (First, Last, Username, Password, LastLogin) OUTPUT INSERTED.UserID VALUES('{0}','{1}','{2}','{3}','{4}');", First, Last, Username, hash, DateTime.Now);

            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                UserID = (Int32)cmd.ExecuteScalar();
            }
            catch (SqlException sex) { UserID = -1; }
            catch (Exception ex) { UserID = -1; }
            finally { conn.Close(); }

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

            string query = string.Format("SELECT * FROM [User] WHERE UserId='{0}';", UserID);


            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    user = new accountModel();
                    sdr.Read();

                    user.First = (string)sdr["First"];
                    user.Last = (string)sdr["Last"];
                    user.Username = (string)sdr["Username"];
                    //user.Password = (string)sdr["Password"];
                }
            }
            catch { user = null; }
            finally { conn.Close(); }

            return user;            
        }


        /// <summary>
        /// Recieves a accountModel and updates the data in our database accordingly.
        /// </summary>
        /// <param name="UserID">Database UserID for this user.</param>
        /// <param name="model">accountModel containing the users updated information.</param>
        /// <returns>Returns a true/false if the database update was successful.</returns>
        public static bool updateAccount(Int32 UserID, accountModel model)
        {
            bool success = false;

            string query = string.Format("UPDATE [User] SET First='{0}',Last='{1}' WHERE UserId='{2}';", model.First, model.Last, UserID);

            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                success = true;
            }
            catch { success = false; }
            finally { conn.Close(); }

            return success;
        }
    }
}