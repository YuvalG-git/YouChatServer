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
using YouChatServer.JsonClasses;
using System.Data;
using YouChatServer.ContactHandler;

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
        public static int InsertUser(string username, string Password, string FirstName, string LastName, string EmailAddress, string CityName, string Gender, string dateOfBirthAsString, string registrationDateAsString, List<string[]> VerificationQuestionsAndAnswers)
        {
            SqlTransaction transaction = null;
            try
            {
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(Password);
                string TagLine = SetTagLine("UserDetails");
                connection.Open();
                transaction = connection.BeginTransaction();
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string Sql1 = "INSERT INTO UserDetails (Username, Password, FirstName, LastName, EmailAddress, City, BirthDate, Gender, LastPasswordUpdate, TagLineId) VALUES(@Username, @Md5Password, @FirstName, @LastName, @EmailAddress, @City, @BirthDate, @Gender, @LastPasswordUpdate, @TagLine)";
                cmd.CommandText = Sql1;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
                cmd.Parameters.AddWithValue("@FirstName", FirstName);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@EmailAddress", EmailAddress);
                cmd.Parameters.AddWithValue("@City", CityName);
                cmd.Parameters.AddWithValue("@BirthDate", dateOfBirthAsString);
                cmd.Parameters.AddWithValue("@Gender", Gender);
                cmd.Parameters.AddWithValue("@LastPasswordUpdate", registrationDateAsString);
                cmd.Parameters.AddWithValue("@TagLine", TagLine);
                int x = cmd.ExecuteNonQuery();

                string Sql2 = "INSERT INTO UserPastPasswords (Username, [Password-1]) VALUES(@Username, @Md5Password)";
                cmd.CommandText = Sql2;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
                int y = cmd.ExecuteNonQuery();


                //string Sql3 = "INSERT INTO Friends (Username) VALUES('" + Username + "')";
                string Sql3 = "INSERT INTO Friends (Username) VALUES(@Username)";
                cmd.CommandText = Sql3;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                int z = cmd.ExecuteNonQuery();

                //will be used for UserVerificationInformation
                //todo - to insert the answers encrypted by MD5
                string questionNumber1 = VerificationQuestionsAndAnswers[0][0];
                string questionNumber2 = VerificationQuestionsAndAnswers[1][0];
                string questionNumber3 = VerificationQuestionsAndAnswers[2][0];
                string questionNumber4 = VerificationQuestionsAndAnswers[3][0];
                string questionNumber5 = VerificationQuestionsAndAnswers[4][0];
                string answerNumber1 = VerificationQuestionsAndAnswers[0][1];
                string answerNumber2 = VerificationQuestionsAndAnswers[1][1];
                string answerNumber3 = VerificationQuestionsAndAnswers[2][1];
                string answerNumber4 = VerificationQuestionsAndAnswers[3][1];
                string answerNumber5 = VerificationQuestionsAndAnswers[4][1];
                string Md5AnswerNumber1 = YouChatServer.Encryption.MD5.CreateMD5Hash(answerNumber1);
                string Md5AnswerNumber2 = YouChatServer.Encryption.MD5.CreateMD5Hash(answerNumber2);
                string Md5AnswerNumber3 = YouChatServer.Encryption.MD5.CreateMD5Hash(answerNumber3);
                string Md5AnswerNumber4 = YouChatServer.Encryption.MD5.CreateMD5Hash(answerNumber4);
                string Md5AnswerNumber5 = YouChatServer.Encryption.MD5.CreateMD5Hash(answerNumber5);

                //string Sql4 = "INSERT INTO UserVerificationInformation (Username, TagLineId, [Question-1], [Answer-1], [Question-2], [Answer-2], [Question-3], [Answer-3], [Question-4], [Answer-4], [Question-5], [Answer-5]) VALUES('" + Username + "','" + TagLine + "','" + questionNumber1 + "','" + Md5AnswerNumber1 + "','" + questionNumber2 + "','" + Md5AnswerNumber2 + "','" + questionNumber3 + "','" + Md5AnswerNumber3 + "','" + questionNumber4 + "','" + Md5AnswerNumber4 + "','" + questionNumber5 + "','" + Md5AnswerNumber5 + "')";
                string Sql4 = "INSERT INTO UserVerificationInformation (Username, TagLineId, [Question-1], [Answer-1], [Question-2], [Answer-2], [Question-3], [Answer-3], [Question-4], [Answer-4], [Question-5], [Answer-5]) VALUES(@Username, @TagLine, @questionNumber1, @Md5AnswerNumber1, @questionNumber2, @Md5AnswerNumber2, @questionNumber3, @Md5AnswerNumber3, @questionNumber4, @Md5AnswerNumber4, @questionNumber5, @Md5AnswerNumber5)";
                cmd.CommandText = Sql4;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@TagLine", TagLine);
                cmd.Parameters.AddWithValue("@questionNumber1", questionNumber1);
                cmd.Parameters.AddWithValue("@Md5AnswerNumber1", Md5AnswerNumber1);
                cmd.Parameters.AddWithValue("@questionNumber2", questionNumber2);
                cmd.Parameters.AddWithValue("@Md5AnswerNumber2", Md5AnswerNumber2);
                cmd.Parameters.AddWithValue("@questionNumber3", questionNumber3);
                cmd.Parameters.AddWithValue("@Md5AnswerNumber3", Md5AnswerNumber3);
                cmd.Parameters.AddWithValue("@questionNumber4", questionNumber4);
                cmd.Parameters.AddWithValue("@Md5AnswerNumber4", Md5AnswerNumber4);
                cmd.Parameters.AddWithValue("@questionNumber5", questionNumber5);
                cmd.Parameters.AddWithValue("@Md5AnswerNumber5", Md5AnswerNumber5);
                int w = cmd.ExecuteNonQuery();

                if ((x > 0) && (y > 0) && (z > 0) && (w > 0))
                {
                    transaction.Commit();
                    return x;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();

                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
        }

        //public static List<Chat> GetChatList()
        //{
        //    List<Chat> groupChats = new List<Chat>();

        //    try
        //    {
        //        cmd.Connection = connection;
        //        string sql = "SELECT * FROM GroupChats";
        //        cmd.CommandText = sql;
        //        connection.Open();

        //        SqlDataReader Reader = cmd.ExecuteReader();
        //        while (Reader.Read())
        //        {
        //            Chat groupChat = new Chat
        //            {
        //                Id = (int)Reader["Id"],
        //                ChatName = Reader["ChatName"].ToString(),
        //                ChatTagLineId = Reader["ChatTagLineId"].ToString()
        //                // Populate other properties as needed
        //            };
        //            groupChats.Add(groupChat);
        //        }
        //        Reader.Close();
        //        connection.Close();
        //        return groupChats;              
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        Console.WriteLine(ex.Message);
        //        return groupChats;
        //    }

        //}
        public static List<ChatHandler.ChatDetails> GetAllChats()
        {
            List<ChatHandler.ChatDetails> allChats = new List<ChatHandler.ChatDetails>();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                List<ChatHandler.ChatDetails> directChats = GetChats("DirectChats", transaction);
                List<ChatHandler.ChatDetails> groupChats = GetChats("GroupChats", transaction);
                // Add the first friend request
                if (directChats == null)
                {
                    throw new Exception("Failed to get Direct Chats.");
                }

                if (groupChats == null)
                {
                    throw new Exception("Failed to get Group Chats.");
                }
                allChats.AddRange(directChats);
                allChats.AddRange(groupChats);
                transaction.Commit();
                return allChats;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                transaction?.Rollback();
                return allChats;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }
        }
        public static List<ChatHandler.ChatDetails> GetChats(string tableName, SqlTransaction transaction)
        {
            SqlDataReader Reader = null;
            List<ChatHandler.ChatDetails> chats = new List<ChatHandler.ChatDetails>();
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string sql = $"SELECT * FROM {tableName}";
                cmd.CommandText = sql;
                int id;
                string chatTagLineId;
                string messageHistory;
                DateTime? lastMessageTime;
                string lastMessageContent;
                List<ChatParticipant> chatParticipants;
                string chatName;
                byte[] chatProfilePicture;
                ChatParticipant chatParticipant;
                string participant;
                string participantProfilePictureId;
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    chatTagLineId = Reader["ChatTagLineId"].ToString();
                    messageHistory = Reader["MessageHistory"].ToString();
                    lastMessageTime = Reader.IsDBNull(Reader.GetOrdinal("LastMessageTime")) ? null : (DateTime?)Reader["LastMessageTime"];
                    lastMessageContent = Reader["LastMessageContent"].ToString();
                    chatParticipants = new List<ChatParticipant>();
                    for (int i = 1; i < Reader.FieldCount; i++)
                    {
                        string columnName = Reader.GetName(i);
                        if (columnName.StartsWith("ChatParticipant-"))
                        {
                            participant = Reader[columnName].ToString();
                            if (!string.IsNullOrEmpty(participant))
                            {
                                participantProfilePictureId = GetProfilePictureId(participant);
                                if (participantProfilePictureId != "")
                                {
                                    chatParticipant = new ChatParticipant(participant, participantProfilePictureId);
                                    chatParticipants.Add(chatParticipant);

                                }
                            }
                        }
                    }
                    ChatHandler.ChatDetails chat = null;
                    if (tableName == "DirectChats")
                    {
                        chat = new DirectChatDetails(chatTagLineId, messageHistory, lastMessageTime, lastMessageContent, chatParticipants);
                    }
                    else if (tableName == "GroupChats")
                    {
                        chatName = Reader["ChatName"].ToString();
                        chatProfilePicture = (byte[])Reader["ChatProfilePicture"];
                        chat = new GroupChatDetails(chatTagLineId, messageHistory, lastMessageTime, lastMessageContent, chatParticipants, chatName, chatProfilePicture);

                    }
                    if (chat != null)
                    {
                        chats.Add(chat);
                    }
                }
                Reader.Close();
                return chats;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
            }
        }
        public static List<ChatParticipant> GetChatParticipants(List<string> chatParticipantNames)
        {
            List<ChatParticipant> chatParticipants = new List<ChatParticipant>();
            ChatParticipant chatParticipant;
            string participantProfilePictureId;
            foreach (string chatParticipantName in chatParticipantNames)
            {
                participantProfilePictureId = GetProfilePicture(chatParticipantName);
                if (participantProfilePictureId != "")
                {
                    chatParticipant = new ChatParticipant(chatParticipantName, participantProfilePictureId);
                    chatParticipants.Add(chatParticipant);
                }
            }
            return chatParticipants;
        }

        private static string GetProfilePictureId(string Username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        string sql = "SELECT ProfilePicture FROM UserDetails WHERE Username = @Username";
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@Username", Username);
                        object result = cmd.ExecuteScalar();
                        string profilePicture = "";
                        if (result != null && result != DBNull.Value)
                        {
                            profilePicture = (string)result;
                        }
                        return profilePicture;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static string[] GetUserVerificationQuestions(string Username)
        {
            string[] questions = new string[5];
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT [Question-1], [Question-2], [Question-3], [Question-4], [Question-5] FROM UserVerificationInformation WHERE Username = '" + Username + "'";
                string sql = "SELECT [Question-1], [Question-2], [Question-3], [Question-4], [Question-5] FROM UserVerificationInformation WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();

                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int questionNumber = i + 1;
                        string question = Reader["Question-" + questionNumber.ToString()] as string;
                        if (!string.IsNullOrEmpty(question))
                        {
                            questions[i] = question;
                        }
                    }
                }
                Reader.Close();
                connection.Close();
                return questions;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return questions;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool CheckUserVerificationInformation(string Username, PersonalVerificationAnswers personalVerificationAnswers)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM UserVerificationInformation WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM UserVerificationInformation WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                int matchingQuestionCounter = 0;
                string Md5AnswerNumber1 = YouChatServer.Encryption.MD5.CreateMD5Hash(personalVerificationAnswers.AnswerNumber1);
                string Md5AnswerNumber2 = YouChatServer.Encryption.MD5.CreateMD5Hash(personalVerificationAnswers.AnswerNumber2);
                string Md5AnswerNumber3 = YouChatServer.Encryption.MD5.CreateMD5Hash(personalVerificationAnswers.AnswerNumber3);


                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        string storedQuestion = Reader["Question-" + i.ToString()] as string;
                        string storedAnswer = Reader["Answer-" + i.ToString()] as string;
                        if (storedQuestion.Equals(personalVerificationAnswers.QuestionNumber1))
                        {
                            matchingQuestionCounter++;
                            if (!storedAnswer.Equals(Md5AnswerNumber1))
                            {
                                return false;
                            }
                        }
                        else if (storedQuestion.Equals(personalVerificationAnswers.QuestionNumber2))
                        {
                            matchingQuestionCounter++;
                            if (!storedAnswer.Equals(Md5AnswerNumber2))
                            {
                                return false;
                            }
                        }
                        else if (storedQuestion.Equals(personalVerificationAnswers.QuestionNumber3))
                        {
                            matchingQuestionCounter++;
                            if (!storedAnswer.Equals(Md5AnswerNumber3))
                            {
                                return false;
                            }
                        }
                    }
                }
                Reader.Close();
                connection.Close();
                if (matchingQuestionCounter != 3)
                    return false;
                return true; // All questions and answers matched
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private static string SetTagLine(string DataBaseTableName, bool HandleConnection = true)
        {
            bool isTagLineExists = true;
            string TagLine = "";
            while (isTagLineExists)
            {
                TagLine = RandomStringCreator.RandomString(6);
                if (!TaglineIsExist(TagLine, DataBaseTableName, HandleConnection))
                {
                    isTagLineExists = false; //maybe to add a list of failed tagline - will be better to check this list rather then entire database...
                }
            }
            return TagLine;
        }
        public static string SetTagLine(string DataBaseTableName1, string DataBaseTableName2, bool HandleConnection = true)
        {
            bool isTagLineExists = true;
            string TagLine = "";
            while (isTagLineExists)
            {
                TagLine = RandomStringCreator.RandomString(6);
                if (!TaglineIsExist(TagLine, DataBaseTableName1, HandleConnection) && !TaglineIsExist(TagLine, DataBaseTableName2, HandleConnection))
                {
                    isTagLineExists = false; //maybe to add a list of failed tagline - will be better to check this list rather then entire database...
                }
            }
            return TagLine;
        }

        public static bool AreFriends(string Username, string FriendUsername)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);

                connection.Open();
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    // Check each column for the desired value
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if ((Value != DBNull.Value) && (Value != null) && (Value.ToString() == FriendUsername))
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int SetUserOnline(string username)
        {
            //set the user online and set the last seen time to now - not must
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE UserDetails SET Online = '" + 1 + "' WHERE Username = '" + username + "'";
                string sql = "UPDATE UserDetails SET Online = @OnlineValue WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@OnlineValue", 1);
                cmd.Parameters.AddWithValue("@Username", username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int SetUserOffline(string username)
        {
            //set the user offline and set the last seen time to now...
            try
            {
                DateTime LastSeenTime = DateTime.Now;
                cmd.Connection = connection;
                //string sql = "UPDATE UserDetails SET Online = '" + 0 + "', LastSeenTime = '" + LastSeenTime + "' WHERE Username = '" + username + "'";
                string sql = "UPDATE UserDetails SET Online = @OnlineValue, LastSeenTime = @LastSeenTime WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@OnlineValue", 0);
                cmd.Parameters.AddWithValue("@LastSeenTime", LastSeenTime);
                cmd.Parameters.AddWithValue("@Username", username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }



        private static int NumberOfColumns(string TableName)
        {
            try
            {
                cmd.Connection = connection;
                //string Sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'";
                string sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@TableName", TableName);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
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
        public static bool isMatchingUsernameAndPasswordExist(string Username, string Password)
        {
            try
            {
                string Md5Password = Encryption.MD5.CreateMD5Hash(Password);

                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And Password = '" + Md5Password + "'";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND Password = @Md5Password";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool IsFriendRequestPending(string SenderUsername, string ReceiverUsername)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM FriendRequest WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = 'Pending'";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@SenderUsername", SenderUsername);
                cmd.Parameters.AddWithValue("@ReceiverUsername", ReceiverUsername);

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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static bool IsMatchingUsernameAndEmailAddressExist(string Username, string EmailAddress)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And EmailAddress = '" + Email + "'";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND EmailAddress = @Email";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Email", EmailAddress);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int AddFriendRequest(string FriendRequestSenderUsername, string FriendRequestReceiverUsername)
        {
            try
            {
                cmd.Connection = connection;
                //string Sql = "INSERT INTO FriendRequest (SenderUsername, ReceiverUsername) VALUES('" + FriendRequestSenderUsername + "','" + FriendRequestReceiverUsername + "')";
                string sql = "INSERT INTO FriendRequest (SenderUsername, ReceiverUsername) VALUES (@SenderUsername, @ReceiverUsername)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@SenderUsername", FriendRequestSenderUsername);
                cmd.Parameters.AddWithValue("@ReceiverUsername", FriendRequestReceiverUsername);


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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        //public static string CheckFriendRequests(string username)
        //{
        //    StringBuilder FriendRequestSenderUsernames = new StringBuilder();
        //    string Usernames = ""; //will return "" if there are no friendrequest so i need to check if this value's length is bigger than 0
        //    // if true i need to send the user a message and then he will add those friend requests to his friend request area...
        //    try
        //    {
        //        cmd.Connection = connection;
        //        //string Sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = '" + username + "' And RequestStatus = 'Pending'";
        //        string sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = @Username AND RequestStatus = 'Pending'";
        //        cmd.CommandText = sql;
        //        cmd.Parameters.Clear(); // Clear previous parameters
        //        cmd.Parameters.AddWithValue("@Username", username);
        //        connection.Open();
        //        SqlDataReader Reader = cmd.ExecuteReader();
        //        DateTime requestDate = DateTime.Now;
        //        while (Reader.Read())
        //        {
        //            FriendRequestSenderUsernames.Append(Reader["SenderUsername"].ToString());
        //            FriendRequestSenderUsernames.Append("^");
        //            requestDate = Convert.ToDateTime(Reader["RequestDate"]); //maybe to create a list or dictonary..
        //            FriendRequestSenderUsernames.Append(requestDate.ToString());


        //            FriendRequestSenderUsernames.Append("#");

        //        }
        //        if (FriendRequestSenderUsernames.Length > 0)
        //        {
        //            FriendRequestSenderUsernames.Length -= 1;
        //        }

        //        Reader.Close();
        //        connection.Close();
        //        username = FriendRequestSenderUsernames.ToString();
        //        return username;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        Console.WriteLine(ex.Message);
        //        return Usernames;
        //    }
        //}
        public static DateTime GetFriendRequestDate(string SenderUsername, string ReceiverUsername, DateTime currentTime)
        {
            DateTime RequestDate = currentTime;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT TOP 1 RequestDate FROM FriendRequest WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = 'Pending' ORDER BY RequestDate DESC";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@SenderUsername", SenderUsername);
                cmd.Parameters.AddWithValue("@ReceiverUsername", ReceiverUsername);
                connection.Open();
                object result = cmd.ExecuteScalar();
                // Check if a result was found
                if (result != null && result != DBNull.Value)
                {
                    RequestDate = (DateTime)result;
                }
                connection.Close();
                return RequestDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return RequestDate;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static List<PastFriendRequest> CheckFriendRequests(string username)
        {
            SqlDataReader Reader = null;
            List<PastFriendRequest> pastFriendRequests = new List<PastFriendRequest>();
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = @Username AND RequestStatus = 'Pending'";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                string friendUsername = "";
                DateTime requestDate = DateTime.Now;
                while (Reader.Read())
                {
                    friendUsername = Reader["SenderUsername"].ToString();
                    requestDate = Convert.ToDateTime(Reader["RequestDate"]); 
                    PastFriendRequest pastFriendRequest = new PastFriendRequest(friendUsername,requestDate);
                    pastFriendRequests.Add(pastFriendRequest);
                }
                Reader.Close();
                connection.Close();
                return pastFriendRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int HandleFriendRequestStatus(string SenderUsername, string ReceiverUsername, string FriendRequestStatus)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE FriendRequest SET RequestStatus = '" + FriendRequestStatus + "' WHERE SenderUsername = '" + SenderUsername + "' And ReceiverUserame = '" + ReceiverUserame + "'";
                string sql = "UPDATE FriendRequest SET RequestStatus = @FriendRequestStatus WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = @CurrentFriendRequestStatus";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@FriendRequestStatus", FriendRequestStatus);
                cmd.Parameters.AddWithValue("@SenderUsername", SenderUsername);
                cmd.Parameters.AddWithValue("@ReceiverUsername", ReceiverUsername);
                cmd.Parameters.AddWithValue("@CurrentFriendRequestStatus", "Pending");

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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        //public static int AddFriend(string UsernameAdding, string UsernameAdded)
        //{
        //    //needs to change the passwordupdate date and password values in the main table
        //    // needs to change the password to the pastPasswords table
        //    try
        //    {
        //        string FriendColumn = GetFriendColumnToInsert(UsernameAdding);
        //        cmd.Connection = connection;
        //        //string sql = "UPDATE Friends SET [" + FriendColumn + "] = '" + UsernameAdded + "' WHERE Username = '" + UsernameAdding + "'";
        //        string sql = "UPDATE Friends SET [" + FriendColumn + "] = @UsernameAdded WHERE Username = @UsernameAdding";
        //        cmd.CommandText = sql;
        //        cmd.Parameters.Clear(); // Clear previous parameters
        //        cmd.Parameters.AddWithValue("@UsernameAdded", UsernameAdded);
        //        cmd.Parameters.AddWithValue("@UsernameAdding", UsernameAdding);
        //        connection.Open();
        //        int x = cmd.ExecuteNonQuery();

        //        connection.Close();
        //        //for the pastpasswords table i need to fine the last null value and replace it with a value
        //        return x;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        Console.WriteLine(ex.Message);
        //        return 0;
        //    }
        //}
        public static int AddFriend(string UsernameAdding, string UsernameAdded, SqlTransaction transaction)
        {
            try
            {
                string FriendColumn = GetFriendColumnToInsert(UsernameAdding, transaction);
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                //string sql = "UPDATE Friends SET [" + FriendColumn + "] = '" + UsernameAdded + "' WHERE Username = '" + UsernameAdding + "'";
                string sql = "UPDATE Friends SET [" + FriendColumn + "] = @UsernameAdded WHERE Username = @UsernameAdding";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@UsernameAdded", UsernameAdded);
                cmd.Parameters.AddWithValue("@UsernameAdding", UsernameAdding);
                int x = cmd.ExecuteNonQuery();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static string GetFriendColumnToInsert(string Username, SqlTransaction transaction) //will be used in order to find where is the first null value...
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;

                //string sql = "SELECT * FROM Friends WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if (Reader.IsDBNull(i) || Reader[i] == null)
                        {
                            columnName = Reader.GetName(i);
                            Reader.Close();
                            return columnName;
                        }
                    }
                }
                Reader.Close();
                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
            finally
            {
                // Ensure the reader is closed even if an exception occurred
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
            }
        }
        public static List<string> GetFriendList(string username)
        {
            SqlDataReader Reader = null;
            StringBuilder Friends = new StringBuilder();
            List<string> friends = new List<string>(); //will return "" if there are no friendrequest so i need to check if this value's length is bigger than 0
            // if true i need to send the user a message and then he will add those friend requests to his friend request area...
            try
            {
                cmd.Connection = connection;
                //string Sql = "SELECT * FROM Friends WHERE Username = '" + username + "'";
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) // 0- id/ 1-username, 2,3,4 - friends names...
                    {
                        friends.Add(Reader[i].ToString());
                        //Friends.Append(Reader[i].ToString());
                        //Friends.Append("#");
                    }
                }
                //if (Friends.Length > 0)
                //{
                //    Friends.Length -= 1;
                //}

                Reader.Close();
                connection.Close();
                //friends = Friends.ToString();
                return friends;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return friends;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
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

        public static Contacts GetFriendsProfileInformation(List<string> FriendNames)
        {
            Contacts contacts = null;
            List<ContactDetails> contactList = new List<ContactDetails>();
            try
            {
                //string[] FriendName = Friends.Split('#');

                //Dictionary<string, string> FriendsProfileDetails = new Dictionary<string, string>();

                //foreach (string usernameToSearch in FriendName)
                //{
                //    FriendsProfileDetails[usernameToSearch] = GetFriendProfileInformation(usernameToSearch);
                //}
                //return FriendsProfileDetails;
                ContactDetails contact;
                foreach (string friendName in FriendNames)
                {
                    if (friendName != "")
                    {
                        contact = GetFriendProfileInformation(friendName);
                        contactList.Add(contact);
                    }
                }
                contacts = new Contacts(contactList);
                return contacts;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return contacts;
            }

        }
        public static ContactDetails GetFriendProfileInformation(string FriendName) //will be used when logging in a loop to get all friends info + when creating a new friends and needing his details...
        {
            ContactDetails contact = null;
            SqlDataReader Reader = null;
            try
            {
                string TagLine = "";
                string ProfilePicture = "";
                string ProfileStatus = "";
                DateTime LastSeenTime = new DateTime();
                bool Online = true;
                //bool LastSeenProperty = true;
                //bool OnlineProperty = true;
                //bool ProfilePictureProperty = true;
                //bool StatusProperty = true;
                cmd.Connection = connection;

                //string Sql = "SELECT ProfilePicture, ProfileStatus, LastSeenTime, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty FROM UserDetails WHERE Username = '" + FriendName + "'";
                string sql = "SELECT TagLineId, ProfilePicture, ProfileStatus, LastSeenTime, Online FROM UserDetails WHERE Username = @FriendName";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@FriendName", FriendName);
                connection.Open();
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    TagLine = Reader.GetString(0); 
                    ProfilePicture = Reader.GetString(1);
                    ProfileStatus = Reader.GetString(2);
                    LastSeenTime = Reader.GetDateTime(3);
                    Online = Reader.GetBoolean(4);


                    //FriendsProfileInformation.Append(Reader[i].ToString());
                    //FriendsProfileInformation.Append("#");
                    // Add the details to the list
                }
                Reader.Close();
                connection.Close();
                contact = new ContactDetails(FriendName, TagLine, ProfilePicture, ProfileStatus, LastSeenTime, Online);
                //string FriendProfileInformation = FriendName + "^" + ProfilePicture + "^" + ProfileStatus + "^" + LastSeenTime.ToString("yyyy-MM-dd") + "^" + LastSeenProperty + "^" + OnlineProperty + "^" + ProfilePictureProperty + "^" + StatusProperty;
                return contact;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return contact;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }




        public static bool IsMatchingUsernameAndTagLineIdExist(string Username, string TagLine)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And TagLineId = '" + TagLine + "'";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND TagLineId = @TagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@TagLine", TagLine);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
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
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + username + "'";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static bool TaglineIsExist(string Tagline, string DataBaseTableName, bool HandleConnection)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM " + DataBaseTableName + " WHERE TaglineId = '" + Tagline + "'";
                string columnName = (DataBaseTableName == "UserDetails") ? "TaglineId" : "ChatTaglineId";
                string sql = $"SELECT COUNT(*) FROM {DataBaseTableName} WHERE {columnName} = @Tagline";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Tagline", Tagline);
                if (HandleConnection)
                {
                    connection.Open();
                }
                int c = (int)cmd.ExecuteScalar();
                if (HandleConnection)
                {
                    connection.Close();
                }
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
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
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE EmailAddress = '" + emailAddress + "'";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE EmailAddress = @EmailAddress";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@EmailAddress", emailAddress);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static bool ProfilePictureIsExist(string Username) //should be asked in the login part
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And (ProfilePicture IS NOT NULL OR ProfilePicture !=  '" + DBNull.Value + "')";
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND (ProfilePicture IS NOT NULL OR ProfilePicture != @DBNullValue)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@DBNullValue", DBNull.Value);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int InsertProfilePicture(string Username, string ProfilePictureID)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE UserDetails SET ProfilePicture = '" + ProfilePictureID + "' WHERE Username = '" + Username + "'";
                string sql = "UPDATE UserDetails SET ProfilePicture = @ProfilePictureID WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ProfilePictureID", ProfilePictureID);
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int InsertStatus(string Username, string Status)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE UserDetails SET ProfileStatus = '" + Status + "' WHERE Username = '" + Username + "'";
                string sql = "UPDATE UserDetails SET ProfileStatus = @Status WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int UpdateChatSettings(string Username, byte TextSizeProperty, short MessageGapProperty, bool EnterKeyPressedProperty)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE UserDetails SET ProfileStatus = '" + Status + "' WHERE Username = '" + Username + "'";
                string sql = "UPDATE UserDetails SET TextSizeProperty = @TextSizeProperty, MessageGapProperty = @MessageGapProperty, EnterKeyPressedProperty = @EnterKeyPressedProperty WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@TextSizeProperty", TextSizeProperty);
                cmd.Parameters.AddWithValue("@MessageGapProperty", MessageGapProperty);
                cmd.Parameters.AddWithValue("@EnterKeyPressedProperty", EnterKeyPressedProperty);
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool StatusIsExist(string Username)//should be asked in the login part if the profilepictureisexist returns true...
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = '" + Username + "' And (ProfileStatus IS NOT NULL OR ProfileStatus !=  '" + DBNull.Value + "')"; //returns the oppsite for some reason
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND (ProfileStatus IS NOT NULL OR ProfileStatus != @DBNullValue)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@DBNullValue", DBNull.Value);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static DateTime GetLastPasswordUpdateDate(string Username)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT LastPasswordUpdate FROM UserDetails WHERE Username = '" + Username + "'";
                string sql = "SELECT LastPasswordUpdate FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static string GetEmailAddress(string Username)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT EmailAddress FROM UserDetails WHERE Username = '" + Username + "'";
                string sql = "SELECT EmailAddress FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static string GetProfilePicture(string Username)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT ProfilePicture FROM UserDetails WHERE Username = '" + Username + "'";
                string sql = "SELECT ProfilePicture FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static JsonClasses.UserDetails GetUserProfileSettings(string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT ProfilePicture, ProfileStatus, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty, TextSizeProperty, MessageGapProperty, EnterKeyPressedProperty, TagLineId FROM UserDetails WHERE Username = '" + Username + "'";
                string sql = "SELECT ProfilePicture, ProfileStatus, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty, TextSizeProperty, MessageGapProperty, EnterKeyPressedProperty, TagLineId FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
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
                Reader = cmd.ExecuteReader();
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
                JsonClasses.UserDetails userDetails = new JsonClasses.UserDetails(Username, ProfilePicture, ProfileStatus, LastSeenProperty, OnlineProperty, ProfilePictureProperty, StatusProperty, TextSizeProperty, MessageGapProperty, EnterKeyPressedProperty, TagLineId);
                return userDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

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
                //string sql1 = "UPDATE UserDetails SET Password = '" + Md5Password + "', LastPasswordUpdate = '" + PasswordRenewalDate + "' WHERE Username = '" + Username + "'";
                //string sql2 = "UPDATE UserPastPasswords SET [" + PasswordColumn + "] = '" + Md5Password + "' WHERE Username = '" + Username + "'";
                string sql1 = "UPDATE UserDetails SET Password = @Md5Password, LastPasswordUpdate = @PasswordRenewalDate WHERE Username = @Username";
                string sql2 = "UPDATE UserPastPasswords SET [" + PasswordColumn + "] = @Md5Password WHERE Username = @Username";

                cmd.CommandText = sql1;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
                cmd.Parameters.AddWithValue("@PasswordRenewalDate", PasswordRenewalDate);
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();

                int x = cmd.ExecuteNonQuery();

                cmd.CommandText = sql2; //to check it really works...
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

      

        public static bool PasswordIsExist(string Username, string Password)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(Password);

                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static string GetPasswordColumnToInsert(string Username) //will be used in order to find where is the first null value...
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";

                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool CheckFullPasswordCapacity(string Username) //to call to this method when i need to decide to add a new column for another password
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM UserPastPasswords WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool CheckFullFriendsCapacity(string Username) //to call to this method when i need to decide to add a new column for another password
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM Friends WHERE Username = '" + Username + "'";
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static void AddColumnToUserPastPasswords() //after this i need to add the new password... //todo - improve duplicate code for add column
        {
            AddColumnToTable("UserPastPasswords");
            //try
            //{
            //    cmd.Connection = connection;
            //    string TableName = "UserPastPasswords";
            //    //string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
            //    string sql = "SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION DESC";
            //    cmd.CommandText = sql;
            //    cmd.Parameters.AddWithValue("@TableName", TableName);
            //    connection.Open();
            //    SqlDataReader Reader = cmd.ExecuteReader();

            //    string lastColumnName = null;

            //    while (Reader.Read())
            //    {
            //        lastColumnName = Reader["COLUMN_NAME"].ToString();
            //    }

            //    Reader.Close();

            //    if (lastColumnName != null)
            //    {
            //        string NewColumnName = "";
            //        string[] PasswordColumnInformation = lastColumnName.Split('-');
            //        string PasswordNumberValueAsString = PasswordColumnInformation[1];
            //        int PasswordNumber;

            //        if (int.TryParse(PasswordNumberValueAsString, out PasswordNumber))
            //        {
            //            NewColumnName = "Password-" + (PasswordNumber + 1);
            //        }
            //        sql = "ALTER TABLE UserPastPasswords ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
            //        cmd.CommandText = sql;
            //        cmd.ExecuteNonQuery();
            //    }
            //    else
            //    {
            //        // Handle the case where no columns exist in the table.
            //    }
            //    connection.Close();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //    Console.WriteLine(ex.Message);
            //}
        }
        public static void AddColumnToFriends() //after this i need to add the new password...
        {
            AddColumnToTable("Friends");

            //try
            //{
            //    cmd.Connection = connection;
            //    string TableName = "Friends";
            //    //string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
            //    string sql = "SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION DESC";
            //    cmd.CommandText = sql;
            //    cmd.Parameters.AddWithValue("@TableName", TableName);
            //    connection.Open();
            //    SqlDataReader Reader = cmd.ExecuteReader();

            //    string lastColumnName = null;

            //    while (Reader.Read())
            //    {
            //        lastColumnName = Reader["COLUMN_NAME"].ToString();
            //    }

            //    Reader.Close();

            //    if (lastColumnName != null)
            //    {
            //        string NewColumnName = "";
            //        string[] FriendColumnInformation = lastColumnName.Split('-');
            //        string FriendNumberValueAsString = FriendColumnInformation[1];
            //        int FriendNumber;

            //        if (int.TryParse(FriendNumberValueAsString, out FriendNumber))
            //        {
            //            NewColumnName = "Friend-" + (FriendNumber + 1);
            //        }
            //        sql = "ALTER TABLE Friends ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
            //        cmd.CommandText = sql;
            //        cmd.ExecuteNonQuery();
            //    }
            //    else
            //    {
            //        // Handle the case where no columns exist in the table.
            //    }
            //    connection.Close();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //    Console.WriteLine(ex.Message);
            //}
        }
        public static void AddColumnToGroupChats() //after this i need to add the new password...
        {
            AddColumnToTable("GroupChats");
            //try
            //{
            //    cmd.Connection = connection;
            //    string TableName = "Chats";
            //    //string sql = $"SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}' ORDER BY ORDINAL_POSITION DESC";
            //    string sql = "SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION DESC";
            //    cmd.CommandText = sql;
            //    cmd.Parameters.AddWithValue("@TableName", TableName);
            //    connection.Open();
            //    SqlDataReader Reader = cmd.ExecuteReader();

            //    string lastColumnName = null;

            //    while (Reader.Read())
            //    {
            //        lastColumnName = Reader["COLUMN_NAME"].ToString();
            //    }

            //    Reader.Close();

            //    if (lastColumnName != null)
            //    {
            //        string NewColumnName = "";
            //        string[] ChatColumnInformation = lastColumnName.Split('-');
            //        string ChatNumberValueAsString = ChatColumnInformation[1];
            //        int ChatNumber;

            //        if (int.TryParse(ChatNumberValueAsString, out ChatNumber))
            //        {
            //            NewColumnName = "ChatParticipant-" + (ChatNumber + 1);
            //        }
            //        sql = "ALTER TABLE " + TableName + " ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
            //        cmd.CommandText = sql;
            //        cmd.ExecuteNonQuery();
            //    }
            //    else
            //    {
            //        // Handle the case where no columns exist in the table.
            //    }
            //    connection.Close();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //    Console.WriteLine(ex.Message);
            //}
        }
        private static void AddColumnToTable(string tableName)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION DESC";

                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@TableName", tableName);
                connection.Open();
                Reader = cmd.ExecuteReader();

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
                    string ChatNumberValueAsString;
                    int columnNumber;
                    if (ChatColumnInformation.Length > 1)
                    {
                        ChatNumberValueAsString = ChatColumnInformation[1];
                    }
                    else
                    {
                        ChatNumberValueAsString = "0";
                    }

                    if (int.TryParse(ChatNumberValueAsString, out columnNumber))
                    {
                        switch (tableName)
                        {
                            case "UserPastPasswords":
                                NewColumnName = $"Password-{columnNumber + 1}";
                                break;
                            case "Friends":
                                NewColumnName = $"Friend-{columnNumber + 1}";
                                break;
                            case "GroupChats":
                                NewColumnName = $"ChatParticipant-{columnNumber + 1}";
                                break;
                                // Add cases for other tables here if needed
                        }
                    }
                    sql = "ALTER TABLE " + tableName + " ADD [" + NewColumnName + "] NVARCHAR(50) NULL";

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int CreateGroupChat(ChatCreator chat, string ChatTagLine) //for when i want to add another particiapnt i need to first give him the group tagline as well becuase there might be couple group with the same name
        {
            try
            {
                string ChatName = chat.ChatName;
                List<string> ChatMembers = chat.ChatParticipants;
                string FirstChatMember = ChatMembers[0];
                System.Drawing.Image ChatIcon;
                byte[] ChatIconBytes = chat.ChatProfilePictureBytes;
                using (MemoryStream ms = new MemoryStream(ChatIconBytes))
                {
                    ChatIcon = System.Drawing.Image.FromStream(ms);
                }
                cmd.Connection = connection;
                //string Sql = "INSERT INTO Chats (ChatName, ChatTagLineId, ChatProfilePicture, [ChatParticipant-1]) VALUES('" + ChatName + "','" + ChatTagLine + "','" + ChatIcon + "','" + FirstChatMember + "')";
                string sql = "INSERT INTO GroupChats (ChatName, ChatTagLineId, ChatProfilePicture, [ChatParticipant-1]) VALUES(@ChatName, @ChatTagLine, @ChatIcon, @FirstChatMember)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                cmd.Parameters.AddWithValue("@ChatIcon", ChatIcon);
                cmd.Parameters.AddWithValue("@FirstChatMember", FirstChatMember);

                //now i need to add the other members...

                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                bool isNeededToAddColumn = false;
                for (int index = 1; index< ChatMembers.Count; index++)
                {
                    if (isNeededToAddColumn || CheckFullChatsCapacity(ChatName, ChatTagLine))
                    {
                        isNeededToAddColumn = true;
                        AddColumnToGroupChats();
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

        }
        public static bool HandleDirectChatCreation(string ChatTagLine, string FriendRequestSenderUsername, string FriendRequestReceiverUsername, string MessageHistoryFilePath)
        {
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                // Add the first friend request
                if (AddFriend(FriendRequestSenderUsername, FriendRequestReceiverUsername, transaction) == 0)
                {
                    throw new Exception("Failed to add friend request.");
                }

                // Add the friend request in reverse
                if (AddFriend(FriendRequestReceiverUsername, FriendRequestSenderUsername, transaction) == 0)
                {
                    throw new Exception("Failed to add friend request in reverse.");
                }

                // Create a direct chat
                if (CreateDirectChat(ChatTagLine, FriendRequestSenderUsername, FriendRequestReceiverUsername, MessageHistoryFilePath, transaction) == 0)
                {
                    throw new Exception("Failed to create direct chat.");
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                transaction?.Rollback();
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }
        }

        public static int CreateDirectChat(string ChatTagLine, string ChatParticipant1, string ChatParticipant2, string MessageHistoryFilePath, SqlTransaction transaction) //for when i want to add another particiapnt i need to first give him the group tagline as well becuase there might be couple group with the same name
        {
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                //string Sql = "INSERT INTO Chats (ChatName, ChatTagLineId, ChatProfilePicture, [ChatParticipant-1]) VALUES('" + ChatName + "','" + ChatTagLine + "','" + ChatIcon + "','" + FirstChatMember + "')";
                string sql = "INSERT INTO DirectChats (ChatTagLineId, MessageHistory, [ChatParticipant-1], [ChatParticipant-2]) VALUES(@ChatTagLine, @MessageHistory, @FirstChatMember, @SecondChatMember)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                cmd.Parameters.AddWithValue("@MessageHistory", MessageHistoryFilePath);
                cmd.Parameters.AddWithValue("@FirstChatMember", ChatParticipant1);
                cmd.Parameters.AddWithValue("@SecondChatMember", ChatParticipant2);

                int x = cmd.ExecuteNonQuery();
                return x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        //public static int CreateDirectChat(string ChatParticipant1, string ChatParticipant2) //for when i want to add another particiapnt i need to first give him the group tagline as well becuase there might be couple group with the same name
        //{
        //    try
        //    {
        //        string ChatTagLine = SetTagLine("GroupChats", "DirectChats");
        //        cmd.Connection = connection;
        //        //string Sql = "INSERT INTO Chats (ChatName, ChatTagLineId, ChatProfilePicture, [ChatParticipant-1]) VALUES('" + ChatName + "','" + ChatTagLine + "','" + ChatIcon + "','" + FirstChatMember + "')";
        //        string sql = "INSERT INTO DirectChats (ChatTagLineId, [ChatParticipant-1], [ChatParticipant-1]) VALUES(@ChatTagLine, @FirstChatMember, @SecondChatMember)";
        //        cmd.CommandText = sql;
        //        cmd.Parameters.Clear(); // Clear previous parameters
        //        cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
        //        cmd.Parameters.AddWithValue("@FirstChatMember", ChatParticipant1);
        //        cmd.Parameters.AddWithValue("@SecondChatMember", ChatParticipant2);

        //        //now i need to add the other members...

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

        public static int HandleGroupChatMessage(string MessageContent, DateTime MessageTime, string ChatId)
        {
            return HandleMessage("GroupChats", MessageContent, MessageTime, ChatId);
        }
        public static int HandleDirectChatMessage(string MessageContent, DateTime MessageTime, string ChatId)
        {
            return HandleMessage("DirectChats", MessageContent, MessageTime, ChatId);
        }
        private static int HandleMessage(string TableName, string MessageContent, DateTime MessageTime, string ChatId)
        {
            try
            {
                cmd.Connection = connection;
                //string sql = "UPDATE Chats SET [" + ChatMemberColumn + "] = '" + Username + "' WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'";
                string sql = "UPDATE " + TableName + " SET LastMessageContent = @LastMessageContent, LastMessageTime = @LastMessageTime WHERE ChatTagLineId = @ChatTagLineId";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@LastMessageContent", MessageContent);
                cmd.Parameters.AddWithValue("@LastMessageTime", MessageTime);
                cmd.Parameters.AddWithValue("@ChatTagLineId", ChatId);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static int AddNewChatMember(string ChatName, string ChatTagLine, string Username)
        {
            try
            {
                string ChatMemberColumn = GetChatMemberColumnToInsert(ChatName, ChatTagLine);
                cmd.Connection = connection;
                //string sql = "UPDATE Chats SET [" + ChatMemberColumn + "] = '" + Username + "' WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'";
                string sql = "UPDATE GroupChats SET [" + ChatMemberColumn + "] = @Username WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
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
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static bool DeleteGroupChatMember(string ChatName, string ChatTagLine, string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                connection.Open();
                Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 7; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        if (!Reader.IsDBNull(i) && Reader.GetFieldType(i) == typeof(string))
                        {
                            columnName = Reader.GetName(i);
                            if (Reader.GetString(i) == Username)
                            {
                                Reader.Close();
                                sql = $"UPDATE GroupChats SET {columnName} = @NullValue WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                                cmd.CommandText = sql;
                                cmd.Parameters.AddWithValue("@NullValue", DBNull.Value);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                                return true;
                            }
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        public static string GetChatMemberColumnToInsert(string ChatName, string ChatTagLine) //will be used in order to find where is the first null value...
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM Chats WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'";
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static bool CheckFullChatsCapacity(string ChatName, string ChatTagLine) //to call to this method when i need to decide to add a new column for another password
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                //string sql = "SELECT * FROM Chats WHERE ChatName = '" + ChatName + "' AND ChatTagLineId = '" + ChatTagLine + "'"; ;
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                connection.Open();
                Reader = cmd.ExecuteReader();
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
            finally
            {
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
    }
}
