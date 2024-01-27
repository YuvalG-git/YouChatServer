using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing;
using YouChatServer.Encryption;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;
using YouChatServer.ChatHandler;
using static System.Net.Mime.MediaTypeNames;

namespace YouChatServer.UserDetails
{
    /// <summary>
    /// The DataHandler class is responsible for the SQL DataBase functions
    /// </summary>
    internal static class DataHandler
    {
        static string projectFolderPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        static string connectionString;

        static SqlConnection connection = new SqlConnection(SqlRelativePath());
        /// <summary>
        /// Represents the connection string used to connect to the SQL Server database
        /// (represents the database location)
        /// </summary>
        //static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\יובל\source\repos\YouChatServer\YouChatServer\UserDetails\UserDetails.mdf;Integrated Security=True";
        //C:\Users\יובל\source\repos\YouChatServer\YouChatServer\UserList.mdf

        ///// <summary>
        ///// Represents a connection object to a SQL Server database
        ///// </summary>
        //static SqlConnection connection = new SqlConnection(connectionString);

        /// <summary>
        /// Represents a SQL command object which is used to define and execute database commands
        /// </summary>
        static SqlCommand cmd = new SqlCommand();

        private static string SqlRelativePath()
        {
            projectFolderPath = projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length) + "UserDetails\\UserDetails.mdf";
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""" + projectFolderPath + @""";Integrated Security=True";
            return connectionString;
        }
        /// <summary>
        /// The InsertUser method inserts user details into a database table named UsersDetails
        /// It executes an SQL INSERT statement with the provided data and returns the number of affected rows
        /// If an exception occurs, it displays an error message
        /// </summary>
        /// <param name="userdetails"> Represents a string which contains the user details separated by #</param>
        /// <returns>It returns the number of affected rows. If there has been an exception, it returns 0</returns>
        public static int InsertUser(string userdetails)
        {
            try
            {
                string[] data = userdetails.Split('#');
                string Username = data[0];
                string Password = data[1];
                string FirstName = data[2];
                string LastName = data[3];
                string EmailAddress = data[4];
                string City = data[5];
                string[] DateData = data[6].Split('/');
                string Gender = data[7];
                string LastPasswordUpdate = data[8];
                string year = DateData[2];
                string month = DateData[1];
                string day = DateData[0];
                string DateInCurrectOrder = year + "-" + month + "-" + day;
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(Password);
                //bool isTagLineExists = true;
                //string TagLine = "";
                //while (isTagLineExists)
                //{
                //    TagLine = RandomStringCreator.RandomString(6);
                //    if (!TaglineIsExist(TagLine))
                //    {
                //        isTagLineExists = false; //maybe to add a list of failed tagline - will be better to check this list rather then entire database...
                //    }
                //}
                string TagLine = SetTagLine("UserDetails");
                cmd.Connection = connection;
                //string Sql1 = "INSERT INTO UserDetails (Username, Password, FirstName, LastName, EmailAddress, City, BirthDate, Gender, LastPasswordUpdate, ProfilePicture, ProfileStatus) VALUES('" + Username + "','" + Md5Password + "','" + FirstName + "','" + LastName + "','" + EmailAddress + "','" + City + "','" + DateInCurrectOrder + "','" + Gender + "','" + LastPasswordUpdate + "','" + null + "','" + null + "')";
                string Sql1 = "INSERT INTO UserDetails (Username, Password, FirstName, LastName, EmailAddress, City, BirthDate, Gender, LastPasswordUpdate, TagLineId) VALUES('" + Username + "','" + Md5Password + "','" + FirstName + "','" + LastName + "','" + EmailAddress + "','" + City + "','" + DateInCurrectOrder + "','" + Gender + "','" + LastPasswordUpdate + "','" + TagLine + "')";

                //string Sql2 = CreateSqlCommandTextForUserPastPasswordsInserting(Username, Md5Password);
                string Sql2 = "INSERT INTO UserPastPasswords (Username, [Password-1]) VALUES('" + Username + "','" + Md5Password + "')";
                string Sql3 = "INSERT INTO Friends (Username) VALUES('" + Username + "')";

                //will be used for UserVerificationInformation
                //todo - to insert the answers encrypted by MD5
                string Sql4 = "INSERT INTO UserVerificationInformation (Username, TagLineId, [Question-1], [Answer-1], [Question-2], [Answer-2], [Question-3], [Answer-3], [Question-4], [Answer-4], [Question-5], [Answer-5]) VALUES('" + Username + "','" + Md5Password + "','" + FirstName + "','" + LastName + "','" + EmailAddress + "','" + City + "','" + DateInCurrectOrder + "','" + Gender + "','" + LastPasswordUpdate + "','" + TagLine + "')";

                connection.Open();
                cmd.CommandText = Sql1;
                int x = cmd.ExecuteNonQuery();
                cmd.CommandText = Sql2;
                int y = cmd.ExecuteNonQuery();
                cmd.CommandText = Sql3;
                int z = cmd.ExecuteNonQuery();
                connection.Close();
                if ((x > 0) && (y > 0) && (z > 0))
                {
                    return x;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        private static string SetTagLine(string DataBaseTableName)
        {
            bool isTagLineExists = true;
            string TagLine = "";
            while (isTagLineExists)
            {
                TagLine = RandomStringCreator.RandomString(6);
                if (!TaglineIsExist(TagLine, DataBaseTableName))
                {
                    isTagLineExists = false; //maybe to add a list of failed tagline - will be better to check this list rather then entire database...
                }
            }
            return TagLine;
        }
        public static int SetUserOnline(string username)
        {
            //set the user online and set the last seen time to now - not must
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET Online = '" + 1 + "' WHERE Username = '" + username + "'";
                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int SetUserOffline(string username)
        {
            //set the user offline and set the last seen time to now...
            try
            {
                DateTime LastSeenTime = DateTime.Now;
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET Online = '" + 0 + "', LastSeenTime = '" + LastSeenTime + "' WHERE Username = '" + username + "'";
                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }



        private static int NumberOfColumns(string TableName)
        {
            try
            {
                cmd.Connection = connection;
                string Sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'";


                cmd.CommandText = Sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                return c;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        private static string CreateSqlCommandTextForUserPastPasswordsInserting(string Username, string Md5Password)
        {
            string Sql = "INSERT INTO UserPastPasswords (Username, [Password-1]) VALUES('" + Username + "','" + Md5Password; //maybe i dont need this if i set it to nullable so i dont need to insert a value...
            string TableName = "UserPastPasswords";
            int ColumnNumber = NumberOfColumns(TableName);
            for (int i = 3; i < ColumnNumber; i++)
            {
                Sql += "','" + null;
            }
            Sql += "')";
            return Sql;
        }

        //public static int InsertUser(string userdetails)
        //{
        //    try
        //    {
        //        string[] data = userdetails.Split('#');
        //        string[] dateData = data[6].Split('/');
        //        int year = Convert.ToInt32(dateData[2]);
        //        int month = Convert.ToInt32(dateData[1]);
        //        int day = Convert.ToInt32(dateData[0]);
        //        string dateFormat = "yyyy-MM-dd";
        //        string DateInCurrectOrder = dateData[0] + "-" + dateData[1] + "-" + dateData[2];
        //        DateTime birthDate = DateTime.ParseExact(DateInCurrectOrder, dateFormat, CultureInfo.InvariantCulture);
        //        DateTime date = new DateTime(year, month, day);
        //        cmd.Connection = connection;
        //        string sql = "INSERT INTO UserDetails VALUES('" + data[0] + "','" + data[1] + "','" + data[2] + "','" + data[3] + "','" + data[4] + "','" + data[5] + "','" + birthDate + "','" + data[7] + "')";
        //        cmd.CommandText = sql;
        //        connection.Open();
        //        int x = cmd.ExecuteNonQuery();
        //        connection.Close();
        //        return x;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        Console.WriteLine(ex.Message);
        //        return 0;
        //    }
        //}

        /// <summary>
        /// The isExist method checks if a user with the provided username and password exists in the UsersDetails table of the database
        /// The method executes an SQL SELECT statement to count the number of rows matching the username and password
        /// If the count is greater than 0, it shows that the user exists in the database
        /// If an exception occurs, it displays an error message
        /// Otherwise, it returns false. If If an exception occurs, it displays an error message, it displays an error message and returns false.
        /// </summary>
        /// <param name="details">Represents a string which contains the username and password separated by #</param>
        /// <returns>It returns true if there is a user in the database with the same username and password. Otherwise, it returns false (and if an exception occurs)</returns>
        public static bool isExist(string details)
        {
            try
            {
                string[] data = details.Split('#');
                string Username = data[0];
                string Password = data[1];
                string Md5Password = Encryption.MD5.CreateMD5Hash(Password);

                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And Password = '" + Md5Password + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool IsMatchingUsernameAndEmailAddressExist(string details)
        {
            try
            {
                string[] data = details.Split('#');
                string Username = data[0];
                string Email = data[1];
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And EmailAddress = '" + Email + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static int AddFriendRequest(string FriendRequestSenderUsername, string FriendRequestReceiverUsername)
        {
            try
            {
                cmd.Connection = connection;
                string Sql = "INSERT INTO FriendRequest (SenderUsername, ReceiverUsername) VALUES('" + FriendRequestSenderUsername + "','" + FriendRequestReceiverUsername + "')";


                connection.Open();
                cmd.CommandText = Sql;
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static string CheckFriendRequests(string username)
        {
            //StringBuilder FriendRequestSenderUsernames = new StringBuilder();
            //string Usernames = ""; //will return "" if there are no friendrequest so i need to check if this value's length is bigger than 0
            //// if true i need to send the user a message and then he will add those friend requests to his friend request area...
            //try
            //{
            //    cmd.Connection = connection;
            //    string Sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = '" + username + "' And RequestStatus = 'Pending'";
            //    connection.Open();
            //    cmd.CommandText = Sql;
            //    SqlDataReader Reader = cmd.ExecuteReader();
            //    while (Reader.Read())
            //    {
            //        FriendRequestSenderUsernames.Append(Reader["SenderUsername"].ToString());
            //        FriendRequestSenderUsernames.Append("#");

            //    }
            //    if (FriendRequestSenderUsernames.Length > 0)
            //    {
            //        FriendRequestSenderUsernames.Length -= 1;
            //    }

            //    Reader.Close();
            //    connection.Close();
            //    username = FriendRequestSenderUsernames.ToString();
            //    return username;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //    Console.WriteLine(ex.Message);
            //    return Usernames;
            //}
            StringBuilder FriendRequestSenderUsernames = new StringBuilder();
            string Usernames = ""; //will return "" if there are no friendrequest so i need to check if this value's length is bigger than 0
            // if true i need to send the user a message and then he will add those friend requests to his friend request area...
            try
            {
                cmd.Connection = connection;
                string Sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = '" + username + "' And RequestStatus = 'Pending'";
                connection.Open();
                cmd.CommandText = Sql;
                SqlDataReader Reader = cmd.ExecuteReader();
                DateTime requestDate = DateTime.Now;
                while (Reader.Read())
                {
                    FriendRequestSenderUsernames.Append(Reader["SenderUsername"].ToString());
                    FriendRequestSenderUsernames.Append("^");
                    requestDate = Convert.ToDateTime(Reader["RequestDate"]); //maybe to create a list or dictonary..
                    FriendRequestSenderUsernames.Append(requestDate.ToString());


                    FriendRequestSenderUsernames.Append("#");

                }
                if (FriendRequestSenderUsernames.Length > 0)
                {
                    FriendRequestSenderUsernames.Length -= 1;
                }

                Reader.Close();
                connection.Close();
                username = FriendRequestSenderUsernames.ToString();
                return username;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return Usernames;
            }
        }
        public static int HandleFriendRequestStatus(string SenderUsername, string ReceiverUserame, string FriendRequestStatus)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE FriendRequest SET RequestStatus = '" + FriendRequestStatus + "' WHERE SenderUsername = '" + SenderUsername + "' And ReceiverUserame = '" + ReceiverUserame + "'";
                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int AddFriend(string UsernameAdding, string UsernameAdded)
        {
            //needs to change the passwordupdate date and password values in the main table
            // needs to change the password to the pastPasswords table
            try
            {
                string FriendColumn = GetFriendColumnToInsert(UsernameAdding);
                cmd.Connection = connection;
                string sql = "UPDATE Friends SET [" + FriendColumn + "] = '" + UsernameAdded + "' WHERE Username = '" + UsernameAdding + "'";

                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();

                connection.Close();
                //for the pastpasswords table i need to fine the last null value and replace it with a value
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static string GetFriendColumnToInsert(string Username) //will be used in order to find where is the first null value...
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Friends WHERE Username = '" + Username + "'";

                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if (Reader.IsDBNull(i) || Reader[i] == null)
                        {
                            columnName = Reader.GetName(i);
                            connection.Close();
                            Reader.Close();
                            return columnName;
                        }
                    }
                }
                connection.Close();
                Reader.Close();
                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static string GetFriendList(string username)
        {
            StringBuilder Friends = new StringBuilder();
            string friends = ""; //will return "" if there are no friendrequest so i need to check if this value's length is bigger than 0
            // if true i need to send the user a message and then he will add those friend requests to his friend request area...
            try
            {
                cmd.Connection = connection;
                string Sql = "SELECT * FROM Friends WHERE Username = '" + username + "'";
                connection.Open();
                cmd.CommandText = Sql;
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) // 0- id/ 1-username, 2,3,4 - friends names...
                    {
                        Friends.Append(Reader[i].ToString());
                        Friends.Append("#");
                    }
                }
                if (Friends.Length > 0)
                {
                    Friends.Length -= 1;
                }

                Reader.Close();
                connection.Close();
                friends = Friends.ToString();
                return friends;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return friends;
            }
        }
        //public static Dictionary<string, string> GetFriendsProfileInformation(string Friends)
        //{
        //    try
        //    {
        //        string[] FriendName = Friends.Split('#');

        //        Dictionary<string,string> FriendsProfileDetails = new Dictionary<string, string>();
        //        cmd.Connection = connection;
        //        connection.Open();
        //        SqlDataReader Reader;
        //        string ProfileInformation;

        //        string ProfilePicture;
        //        string ProfileStatus;
        //        DateTime LastSeenTime;
        //        bool LastSeenProperty;
        //        bool OnlineProperty;
        //        bool ProfilePictureProperty;
        //        bool StatusProperty;

        //        foreach (string usernameToSearch in FriendName)
        //        {
        //            ProfileInformation = "";
        //            ProfilePicture = "";
        //            ProfileStatus = "";
        //            LastSeenTime = new DateTime();
        //            LastSeenProperty = true;
        //            OnlineProperty = true;
        //            ProfilePictureProperty = true;
        //            StatusProperty = true;
        //            string Sql = "SELECT ProfilePicture, Status, LastSeenTime, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty FROM UserDetails WHERE Username = '" + usernameToSearch + "'";
        //            cmd.CommandText = Sql;
        //            Reader = cmd.ExecuteReader();
        //            while (Reader.Read())
        //            {
        //                ProfilePicture = Reader.GetString(0); //needs to change the profilepicture in database from image to string...
        //                ProfileStatus = Reader.GetString(1);
        //                LastSeenProperty = Reader.GetBoolean(2);
        //                LastSeenTime = Reader.GetDateTime(3);

        //                OnlineProperty = Reader.GetBoolean(4);
        //                ProfilePictureProperty = Reader.GetBoolean(5);
        //                StatusProperty = Reader.GetBoolean(6);


        //                //FriendsProfileInformation.Append(Reader[i].ToString());
        //                //FriendsProfileInformation.Append("#");
        //                // Add the details to the list
        //            }
        //              Reader.Close();
        //              connection.Close();
        //            ProfileInformation = usernameToSearch + "#" + ProfilePicture + "#" + ProfileStatus + "#" + LastSeenTime.ToString("yyyy-MM-dd") + "#" + LastSeenProperty + "#" + OnlineProperty + "#" + ProfilePictureProperty + "#" + StatusProperty;
        //            // Add the list of details to the dictionary
        //            FriendsProfileDetails[usernameToSearch] = ProfileInformation;
        //        }

        //        return FriendsProfileDetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }

        //}

        public static Dictionary<string, string> GetFriendsProfileInformation(string Friends)
        {
            try
            {
                string[] FriendName = Friends.Split('#');

                Dictionary<string, string> FriendsProfileDetails = new Dictionary<string, string>();

                foreach (string usernameToSearch in FriendName)
                {
                    FriendsProfileDetails[usernameToSearch] = GetFriendProfileInformation(usernameToSearch);
                }

                return FriendsProfileDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public static string GetFriendProfileInformation(string FriendName) //will be used when logging in a loop to get all friends info + when creating a new friends and needing his details...
        {
            try
            {
                string ProfilePicture = "";
                string ProfileStatus = "";
                DateTime LastSeenTime = new DateTime();
                bool LastSeenProperty = true;
                bool OnlineProperty = true;
                bool ProfilePictureProperty = true;
                bool StatusProperty = true;
                string Sql = "SELECT ProfilePicture, ProfileStatus, LastSeenTime, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty FROM UserDetails WHERE Username = '" + FriendName + "'";
                cmd.Connection = connection;

                connection.Open();
                cmd.CommandText = Sql;
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ProfilePicture = Reader.GetString(0); //needs to change the profilepicture in database from image to string...
                    ProfileStatus = Reader.GetString(1);
                    LastSeenProperty = Reader.GetBoolean(2);
                    LastSeenTime = Reader.GetDateTime(3);

                    OnlineProperty = Reader.GetBoolean(4);
                    ProfilePictureProperty = Reader.GetBoolean(5);
                    StatusProperty = Reader.GetBoolean(6);


                    //FriendsProfileInformation.Append(Reader[i].ToString());
                    //FriendsProfileInformation.Append("#");
                    // Add the details to the list
                }
                Reader.Close();
                connection.Close();
                string FriendProfileInformation = FriendName + "^" + ProfilePicture + "^" + ProfileStatus + "^" + LastSeenTime.ToString("yyyy-MM-dd") + "^" + LastSeenProperty + "^" + OnlineProperty + "^" + ProfilePictureProperty + "^" + StatusProperty;
                return FriendProfileInformation;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }


        }




        public static bool IsMatchingUsernameAndTagLineIdExist(string details)
        {
            try
            {
                string[] data = details.Split('#');
                string Username = data[0];
                string TagLine = data[1];
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And TagLineId = '" + TagLine + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        /// <summary>
        /// The usernameIsExist method checks if a user with the provided username exists in the UsersDetails table of the database
        /// The method executes an SQL SELECT statement to count the number of rows matching the username
        /// If the count is greater than 0, it returns true indicating the username exists
        /// otherwise, it returns false. If an exception occurs, it displays an error message and returns false.
        /// </summary>
        /// <param name="username">Represents the username</param>
        /// <returns>It returns true if the number of rows, in the SQL database, matching the username is greater than 0. Otherwise, it returns false (and if an exception occurs) </returns>
        public static bool usernameIsExist(string username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + username + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool TaglineIsExist(string Tagline, string DataBaseTableName)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM " + DataBaseTableName + " WHERE TaglineId = '" + Tagline + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// The EmailAddressIsExist method checks if a user with the provided Email Address exists in the UsersDetails table of the database
        /// The method executes an SQL SELECT statement to count the number of rows matching the email address 
        /// If the count is greater than 0, it returns true indicating the  email address  exists
        /// otherwise, it returns false. If an exception occurs, it displays an error message and returns false.
        /// </summary>
        /// <param name="username">Represents the username</param>
        /// <returns>It returns true if the number of rows, in the SQL database, matching the  email address  is greater than 0. Otherwise, it returns false (and if an exception occurs) </returns>
        public static bool EmailAddressIsExist(string emailAddress)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE EmailAddress = '" + emailAddress + "'";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool ProfilePictureIsExist(string Username) //should be asked in the login part
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And (ProfilePicture IS NOT NULL OR ProfilePicture !=  '" + DBNull.Value + "')";
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static int InsertProfilePicture(string Username, string ProfilePictureID)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET ProfilePicture = '" + ProfilePictureID + "' WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int InsertStatus(string Username, string Status)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET ProfileStatus = '" + Status + "' WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public static bool StatusIsExist(string Username)//should be asked in the login part if the profilepictureisexist returns true...
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And (ProfileStatus IS NOT NULL OR ProfileStatus !=  '" + DBNull.Value + "')"; //returns the oppsite for some reason
                cmd.CommandText = sql;
                connection.Open();
                int c = (int)cmd.ExecuteScalar();
                connection.Close();
                if (c > 0)
                    return true; //should be true - but for some reason the cmd returns the opposite...
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static DateTime GetLastPasswordUpdateDate(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT LastPasswordUpdate FROM UserDetails WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();

                object result = cmd.ExecuteScalar();
                DateTime LastPasswordUpdateDate = new DateTime();
                // Check if a result was found
                if (result != null && result != DBNull.Value)
                {
                    LastPasswordUpdateDate = (DateTime)result;
                }
                connection.Close();
                return LastPasswordUpdateDate;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return new DateTime(); //needs to do a check if thats a new datetime with no value
            }
        }

        public static string GetEmailAddress(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT EmailAddress FROM UserDetails WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();

                object result = cmd.ExecuteScalar();
                string emailAddress = "";
                // Check if a result was found
                if (result != null && result != DBNull.Value)
                {
                    emailAddress = (string)result;
                }
                connection.Close();
                return emailAddress;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static string GetProfilePicture(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT ProfilePicture FROM UserDetails WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();

                object result = cmd.ExecuteScalar();
                string profilePicture = "";
                // Check if a result was found
                if (result != null && result != DBNull.Value)
                {
                    profilePicture = (string)result;
                }
                connection.Close();
                return profilePicture;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static string GetUserProfileSettings(string Username)
        {
            cmd.Connection = connection;
            string sql = "SELECT ProfilePicture, ProfileStatus, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty, TextSizeProperty, MessageGapProperty, EnterKeyPressedProperty, TagLineId FROM UserDetails WHERE Username = '" + Username + "'";
            cmd.CommandText = sql;
            connection.Open();
            string ProfilePicture = "";
            string ProfileStatus = "";
            bool LastSeenProperty = true;
            bool OnlineProperty = true;
            bool ProfilePictureProperty = true;
            bool StatusProperty = true;
            int TextSizeProperty = 2;
            int MessageGapProperty = 10;
            bool EnterKeyPressedProperty = false;
            string TagLineId = "";
            SqlDataReader Reader = cmd.ExecuteReader();
            while (Reader.Read())
            {
                // Access the specific columns you selected
                ProfilePicture = Reader.GetString(0); //needs to change the profilepicture in database from image to string...
                ProfileStatus = Reader.GetString(1);
                LastSeenProperty = Reader.GetBoolean(2);
                OnlineProperty = Reader.GetBoolean(3);
                ProfilePictureProperty = Reader.GetBoolean(4);
                StatusProperty = Reader.GetBoolean(5);
                TextSizeProperty = Reader.GetByte(6);
                MessageGapProperty = Reader.GetInt16(7);
                EnterKeyPressedProperty = Reader.GetBoolean(8);
                TagLineId = Reader.GetString(9);

            }
            Reader.Close();
            connection.Close();
            string UserProfileSettings = Username + "#" + ProfilePicture + "#" + ProfileStatus + "#" + LastSeenProperty + "#" + OnlineProperty + "#" + ProfilePictureProperty + "#" + StatusProperty + "#" + TextSizeProperty + "#" + MessageGapProperty + "#" + EnterKeyPressedProperty + "#" + TagLineId;
            return UserProfileSettings;
        }

        public static int SetNewPassword(string Username, string NewPassword)
        {
            //needs to change the passwordupdate date and password values in the main table
            // needs to change the password to the pastPasswords table
            try
            {
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(NewPassword);
                string PasswordRenewalDate = DateTime.Today.ToString("yyyy-MM-dd"); //todo - to decide if the user sends this or to keep it like that...
                string PasswordColumn = GetPasswordColumnToInsert(Username);
                cmd.Connection = connection;
                string sql1 = "UPDATE UserDetails SET Password = '" + Md5Password + "', LastPasswordUpdate = '" + PasswordRenewalDate + "' WHERE Username = '" + Username + "'";
                string sql2 = "UPDATE UserPastPasswords SET [" + PasswordColumn + "] = '" + Md5Password + "' WHERE Username = '" + Username + "'";

                cmd.CommandText = sql1;
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                cmd.CommandText = sql2;
                int y = cmd.ExecuteNonQuery();

                connection.Close();
                //for the pastpasswords table i need to fine the last null value and replace it with a value
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

      

        public static bool PasswordIsExist(string Username, string Password)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(Password);

                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    // Check each column for the desired value
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if ((Value != DBNull.Value) && (Value != null) && (Value.ToString() == Md5Password))
                        {
                            // The value exists in one of the columns
                            Reader.Close();
                            connection.Close();
                            return true;
                        }
                    }
                }
                Reader.Close();
                connection.Close();
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static string GetPasswordColumnToInsert(string Username) //will be used in order to find where is the first null value...
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";

                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if (Reader.IsDBNull(i) || Reader[i] == null)
                        {
                            columnName = Reader.GetName(i);
                            connection.Close();
                            Reader.Close();
                            return columnName;
                        }
                    }
                }
                connection.Close();
                Reader.Close();
                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static bool CheckFullPasswordCapacity(string Username) //to call to this method when i need to decide to add a new column for another password
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                if (Reader.Read()) // Check if a row was found
                {
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i)) // Check if the column is null
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }

                // Close the reader and connection when done
                Reader.Close();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool CheckFullFriendsCapacity(string Username) //to call to this method when i need to decide to add a new column for another password
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Friends WHERE Username = '" + Username + "'";
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                if (Reader.Read()) // Check if a row was found
                {
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i)) // Check if the column is null
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }

                // Close the reader and connection when done
                Reader.Close();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static void AddColumnToUserPastPasswords() //after this i need to add the new password...
        {
            try
            {
                cmd.Connection = connection;
                string TableName = "UserPastPasswords";
                string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();

                string lastColumnName = null;

                while (Reader.Read())
                {
                    lastColumnName = Reader["COLUMN_NAME"].ToString();
                }

                Reader.Close();

                if (lastColumnName != null)
                {
                    string NewColumnName = "";
                    string[] PasswordColumnInformation = lastColumnName.Split('-');
                    string PasswordNumberValueAsString = PasswordColumnInformation[1];
                    int PasswordNumber;

                    if (int.TryParse(PasswordNumberValueAsString, out PasswordNumber))
                    {
                        NewColumnName = "Password-" + (PasswordNumber + 1);
                    }
                    sql = "ALTER TABLE UserPastPasswords ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Handle the case where no columns exist in the table.
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }
        public static void AddColumnToFriends() //after this i need to add the new password...
        {
            try
            {
                cmd.Connection = connection;
                string TableName = "Friends";
                string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();

                string lastColumnName = null;

                while (Reader.Read())
                {
                    lastColumnName = Reader["COLUMN_NAME"].ToString();
                }

                Reader.Close();

                if (lastColumnName != null)
                {
                    string NewColumnName = "";
                    string[] FriendColumnInformation = lastColumnName.Split('-');
                    string FriendNumberValueAsString = FriendColumnInformation[1];
                    int FriendNumber;

                    if (int.TryParse(FriendNumberValueAsString, out FriendNumber))
                    {
                        NewColumnName = "Friend-" + (FriendNumber + 1);
                    }
                    sql = "ALTER TABLE Friends ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Handle the case where no columns exist in the table.
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }
        public static void AddColumnToChats() //after this i need to add the new password...
        {
            try
            {
                cmd.Connection = connection;
                string TableName = "Chats";
                string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();

                string lastColumnName = null;

                while (Reader.Read())
                {
                    lastColumnName = Reader["COLUMN_NAME"].ToString();
                }

                Reader.Close();

                if (lastColumnName != null)
                {
                    string NewColumnName = "";
                    string[] ChatColumnInformation = lastColumnName.Split('-');
                    string ChatNumberValueAsString = ChatColumnInformation[1];
                    int ChatNumber;

                    if (int.TryParse(ChatNumberValueAsString, out ChatNumber))
                    {
                        NewColumnName = "ChatParticipant-" + (ChatNumber + 1);
                    }
                    sql = "ALTER TABLE " + TableName + " ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Handle the case where no columns exist in the table.
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }
        public static int CreateGroupChat(ChatCreator chat) //for when i want to add another particiapnt i need to first give him the group tagline as well becuase there might be couple group with the same name
        {
            try
            {
                string ChatName = chat._chatName;
                List<string> ChatMembers = chat._chatParticipants;
                string FirstChatMember = ChatMembers[0];
                System.Drawing.Image ChatIcon;
                byte[] ChatIconBytes = chat._chatProfilePictureBytes;
                using (MemoryStream ms = new MemoryStream(ChatIconBytes))
                {
                    ChatIcon = System.Drawing.Image.FromStream(ms);
                }
                string ChatTagLine = SetTagLine("UserDetails");
                cmd.Connection = connection;
                string Sql = "INSERT INTO Chats (ChatName, ChatTagLineId, ChatProfilePicture, [ChatParticipant-1]) VALUES('" + ChatName + "','" + ChatTagLine + "','" + ChatIcon + "','" + FirstChatMember + "')";

                //now i need to add the other members...

                connection.Open();
                cmd.CommandText = Sql;
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                bool isNeededToAddColumn = false;
                for (int index = 1; index< ChatMembers.Count; index++)
                {
                    if (isNeededToAddColumn || CheckFullChatsCapacity(ChatName, ChatTagLine))
                    {
                        isNeededToAddColumn = true;
                        AddColumnToChats();
                    }
                    AddNewChatMember(ChatName, ChatTagLine, ChatMembers[index]);
                }
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int AddNewChatMember(string ChatName, string ChatTagLine, string Username)
        {
            try
            {
                string ChatMemberColumn = GetChatMemberColumnToInsert(ChatName, ChatTagLine);
                cmd.Connection = connection;
                string sql = "UPDATE Chats SET [" + ChatMemberColumn + "] = '" + Username + "' WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'";

                cmd.CommandText = sql;
                connection.Open();
                int x = cmd.ExecuteNonQuery();

                connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static string GetChatMemberColumnToInsert(string ChatName, string ChatTagLine) //will be used in order to find where is the first null value...
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Chats WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'";

                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 7; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if (Reader.IsDBNull(i) || Reader[i] == null)
                        {
                            columnName = Reader.GetName(i);
                            connection.Close();
                            Reader.Close();
                            return columnName;
                        }
                    }
                }
                connection.Close();
                Reader.Close();
                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static bool CheckFullChatsCapacity(string ChatName, string ChatTagLine) //to call to this method when i need to decide to add a new column for another password
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Chats WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'"; ;
                cmd.CommandText = sql;
                connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                if (Reader.Read()) // Check if a row was found
                {
                    for (int i = 7; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i)) // Check if the column is null
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }

                // Close the reader and connection when done
                Reader.Close();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
