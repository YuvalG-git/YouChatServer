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
    /// The "DataHandler" class provides methods for handling various data-related operations.
    /// </summary>
    /// <remarks>
    /// This class contains static methods for managing user data, such as updating messages, inserting statuses,
    /// handling password management, and creating and managing chats.
    /// </remarks>
    internal static class DataHandler
    {
        #region Private Static Fields

        /// <summary>
        /// The static string "projectFolderPath" represents the path to the project folder.
        /// </summary>
        private static string projectFolderPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// The static string "connectionString" represents the connection string for the database.
        /// </summary>
        private static string connectionString;

        #endregion

        #region Private Static Readonly Fields

        /// <summary>
        /// The static SqlConnection "connection" represents the SQL connection object initialized with the connection string obtained from the relative path.
        /// </summary>
        private static readonly SqlConnection connection = new SqlConnection(SqlRelativePath());

        /// <summary>
        /// The static SqlCommand "cmd" represents the SQL command object.
        /// </summary>
        private static readonly SqlCommand cmd = new SqlCommand();

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The "SqlRelativePath" method constructs a connection string for a LocalDB database file located in a specific path relative to the project's build output directory.
        /// </summary>
        /// <returns>A connection string for the LocalDB database file.</returns>
        /// <remarks>
        /// This method modifies the projectFolderPath to remove the @"bin\Debug" part from the end and appends the path to the UserDetails.mdf file.
        /// It then constructs a connection string using the modified projectFolderPath and returns it for use in database operations.
        /// </remarks>
        private static string SqlRelativePath()
        {
            projectFolderPath = projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length) + "UserDetails\\UserDetails.mdf";
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""" + projectFolderPath + @""";Integrated Security=True";
            return connectionString;
        }

        #endregion

        #region Public Static Multi Tables Methods

        /// <summary>
        /// The "InsertUser" method inserts a new user into the database along with their details, past passwords, friends, and verification information.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="Password">The password of the new user.</param>
        /// <param name="FirstName">The first name of the new user.</param>
        /// <param name="LastName">The last name of the new user.</param>
        /// <param name="EmailAddress">The email address of the new user.</param>
        /// <param name="CityName">The city name of the new user.</param>
        /// <param name="Gender">The gender of the new user.</param>
        /// <param name="dateOfBirthAsString">The date of birth of the new user as a string.</param>
        /// <param name="registrationDateAsString">The registration date of the new user as a string.</param>
        /// <param name="VerificationQuestionsAndAnswers">A list containing arrays of verification questions and answers for the new user.</param>
        /// <returns>The number of rows affected. Returns 0 if the insertion fails.</returns>
        /// <remarks>
        /// This method begins a transaction to ensure data consistency during the insertion process. It first hashes the user's password using MD5 encryption.
        /// The method then generates a unique tag line for the user, which is used to identify the user in the database.
        /// Next, it opens the database connection and begins a transaction.
        /// The method inserts the user's details, including their username, hashed password, first name, last name, email address, city, birth date, gender, and the date of the last password update, into the UserDetails table.
        /// It also adds a record to the UserPastPasswords table, storing the username and hashed password.
        /// Additionally, it inserts a record into the Friends table to initialize the user's friends list.
        /// Finally, the method inserts the user's verification questions and answers into the UserVerificationInformation table.
        /// Each question and its corresponding answer are stored along with the username and a tag line to uniquely identify the user.
        /// If all insertions are successful, the transaction is committed, ensuring that all changes are made permanent in the database.
        /// If any insertion fails, the transaction is rolled back, ensuring that the database remains in a consistent state.
        /// </remarks>
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
                cmd.Parameters.Clear();
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
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
                int y = cmd.ExecuteNonQuery();


                string Sql3 = "INSERT INTO Friends (Username) VALUES(@Username)";
                cmd.CommandText = Sql3;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", username);
                int z = cmd.ExecuteNonQuery();

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
                string Sql4 = "INSERT INTO UserVerificationInformation (Username, TagLineId, [Question-1], [Answer-1], [Question-2], [Answer-2], [Question-3], [Answer-3], [Question-4], [Answer-4], [Question-5], [Answer-5]) VALUES(@Username, @TagLine, @questionNumber1, @Md5AnswerNumber1, @questionNumber2, @Md5AnswerNumber2, @questionNumber3, @Md5AnswerNumber3, @questionNumber4, @Md5AnswerNumber4, @questionNumber5, @Md5AnswerNumber5)";
                cmd.CommandText = Sql4;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "SetNewPassword" method sets a new password for the specified user.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <param name="NewPassword">The new password to set for the user.</param>
        /// <returns>
        /// An integer representing the number of rows affected by the update operation in the database.
        /// </returns>
        /// <remarks>
        /// This method updates the Password and LastPasswordUpdate columns in the UserDetails table with the new password and current date, respectively.
        /// It also updates the corresponding column in the UserPastPasswords table with the new password hash.
        /// </remarks>
        public static int SetNewPassword(string Username, string NewPassword)
        {
            try
            {
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(NewPassword);
                string PasswordRenewalDate = DateTime.Today.ToString("yyyy-MM-dd");
                string PasswordColumn = GetPasswordColumnToInsert(Username);
                cmd.Connection = connection;
                string sql1 = "UPDATE UserDetails SET Password = @Md5Password, LastPasswordUpdate = @PasswordRenewalDate WHERE Username = @Username";
                string sql2 = "UPDATE UserPastPasswords SET [" + PasswordColumn + "] = @Md5Password WHERE Username = @Username";
                cmd.CommandText = sql1;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Md5Password", Md5Password);
                cmd.Parameters.AddWithValue("@PasswordRenewalDate", PasswordRenewalDate);
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                int x = cmd.ExecuteNonQuery();

                cmd.CommandText = sql2;
                int y = cmd.ExecuteNonQuery();

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

        /// <summary>
        /// The "AddColumnToTable" method adds a new column to the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to which the column will be added.</param>
        /// <remarks>
        /// This method retrieves the name of the last column in the specified table and increments the column number to create a new column name. It then alters the table to add the new column with the generated name.
        /// </remarks>
        private static void AddColumnToTable(string tableName)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT TOP 1 COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION DESC";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
                    sql = "ALTER TABLE " + tableName + " ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
                    if (int.TryParse(ChatNumberValueAsString, out columnNumber))
                    {
                        switch (tableName)
                        {
                            case "UserPastPasswords":
                                NewColumnName = $"Password-{columnNumber + 1}";
                                sql = "ALTER TABLE " + tableName + " ADD [" + NewColumnName + "] NVARCHAR(50) NULL";
                                break;
                            case "Friends":
                                NewColumnName = $"Friend-{columnNumber + 1}";
                                sql = "ALTER TABLE " + tableName + " ADD [" + NewColumnName + "] NVARCHAR(30) NULL";
                                break;
                            case "GroupChats":
                                NewColumnName = $"ChatParticipant-{columnNumber + 1}";
                                sql = "ALTER TABLE " + tableName + " ADD [" + NewColumnName + "] NVARCHAR(30) NULL";
                                break;
                        }
                    }
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, tableName, null });
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

        /// <summary>
        /// The "HandleDirectChatCreation" method handles the creation of a direct chat between two users.
        /// </summary>
        /// <param name="ChatTagLine">The tagline of the direct chat.</param>
        /// <param name="FriendRequestSenderUsername">The username of the sender of the friend request.</param>
        /// <param name="FriendRequestReceiverUsername">The username of the receiver of the friend request.</param>
        /// <param name="MessageHistoryFilePath">The file path to the XML file containing the chat's message history.</param>
        /// <returns>Returns true if the direct chat creation was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method adds both users as friends to each other and creates a new direct chat between them. It uses a SQL transaction to ensure that all operations succeed or fail together. If any step fails, the transaction is rolled back, and the method returns false.
        /// </remarks>
        public static bool HandleDirectChatCreation(string ChatTagLine, string FriendRequestSenderUsername, string FriendRequestReceiverUsername, string MessageHistoryFilePath)
        {
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                if (AddFriend(FriendRequestSenderUsername, FriendRequestReceiverUsername, transaction) == 0)
                {
                    throw new Exception("Failed to add first friend.");
                }

                if (AddFriend(FriendRequestReceiverUsername, FriendRequestSenderUsername, transaction) == 0)
                {
                    throw new Exception("Failed to add second friend.");
                }

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

        #endregion

        #region Public Static Chats Methods

        /// <summary>
        /// The "UpdateLastMessageData" method updates the last message data in a specified table.
        /// </summary>
        /// <param name="TableName">The name of the table to update.</param>
        /// <param name="ChatId">The ID of the chat to update.</param>
        /// <param name="LastMessageContent">The content of the last message.</param>
        /// <param name="LastMessageSenderName">The name of the sender of the last message.</param>
        /// <param name="LastMessageTimeAsString">The time of the last message as a string.</param>
        /// <returns>
        /// The number of rows affected. This should be 1 if the update is successful; otherwise, 0.
        /// </returns>
        /// <remarks>
        /// This method updates the LastMessageContent, LastMessageSenderName, and LastMessageTime columns
        /// in the specified table with the provided values for the given ChatId.
        /// </remarks>
        public static int UpdateLastMessageData(string TableName, string ChatId, string LastMessageContent, string LastMessageSenderName, string LastMessageTimeAsString) //needs to send from the client what type of chat is that...
        {
            try
            {
                cmd.Connection = connection;
                string sql = $"UPDATE {TableName} SET LastMessageContent = @LastMessageContent, LastMessageSenderName = @LastMessageSenderName, LastMessageTime = @LastMessageTime WHERE ChatTagLineId = @ChatTagLineId";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@LastMessageContent", LastMessageContent);
                cmd.Parameters.AddWithValue("@LastMessageSenderName", LastMessageSenderName);
                cmd.Parameters.AddWithValue("@LastMessageTime", LastMessageTimeAsString);
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

        /// <summary>
        /// The "AreLastMessageDataIdentical" method checks if the last message data is identical in a specified table.
        /// </summary>
        /// <param name="TableName">The name of the table to check.</param>
        /// <param name="ChatId">The ID of the chat to check.</param>
        /// <param name="LastMessageContent">The content of the last message to compare.</param>
        /// <param name="LastMessageSenderName">The name of the sender of the last message to compare.</param>
        /// <param name="LastMessageTimeAsString">The time of the last message as a string to compare.</param>
        /// <returns>
        /// True if the last message content, sender name, and time are identical to the provided values; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method retrieves the existing last message content, sender name, and time from the specified table
        /// for the given ChatId and compares them with the provided values to determine if they are identical.
        /// </remarks>
        public static bool AreLastMessageDataIdentical(string TableName, string ChatId, string LastMessageContent, string LastMessageSenderName, string LastMessageTimeAsString)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = $"SELECT LastMessageContent, LastMessageSenderName, LastMessageTime FROM {TableName} WHERE ChatTagLineId = @ChatTagLineId";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ChatTagLineId", ChatId);
                connection.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    string existingLastMessageContent = Reader["LastMessageContent"].ToString();
                    string existingLastMessageSenderName = Reader["LastMessageSenderName"].ToString();
                    string existingLastMessageTimeAsString = Reader["LastMessageTime"].ToString();
                    DateTime existingLastMessageTime = DateTime.Parse(existingLastMessageTimeAsString);
                    DateTime newLastMessageTime = DateTime.Parse(LastMessageTimeAsString);
                    return existingLastMessageContent == LastMessageContent &&
                           existingLastMessageSenderName == LastMessageSenderName &&
                           existingLastMessageTime == newLastMessageTime;
                }
                return false; // No existing record found for the given ChatId
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

        /// <summary>
        /// The "GetAllChats" method retrieves all chat information from the database.
        /// </summary>
        /// <returns>A list of all chat information, including direct and group chats.</returns>
        /// <remarks>
        /// This method opens a database connection and begins a transaction to ensure data consistency.
        /// It retrieves all direct and group chats from their respective tables in the database.
        /// If the retrieval of direct or group chats fails, an exception is thrown.
        /// All retrieved chat information is added to a list, which is then returned.
        /// If the retrieval is successful, the transaction is committed, ensuring that all changes are made permanent in the database.
        /// If any part of the retrieval fails, the transaction is rolled back, ensuring that the database remains in a consistent state.
        /// </remarks>
        public static List<ChatInformation> GetAllChats()
        {
            List<ChatInformation> allChats = new List<ChatInformation>();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                List<ChatInformation> directChats = GetChats("DirectChats", transaction);
                List<ChatInformation> groupChats = GetChats("GroupChats", transaction);

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

        /// <summary>
        /// The "GetChats" method retrieves chat information from the specified table in the database.
        /// </summary>
        /// <param name="tableName">The name of the table from which to retrieve chat information.</param>
        /// <param name="transaction">The SQL transaction associated with the database connection.</param>
        /// <returns>A list of chat information retrieved from the specified table.</returns>
        /// <remarks>
        /// This method constructs and executes a SQL query to retrieve chat information from the specified table.
        /// It reads the data from the database reader and constructs chat objects based on the retrieved information.
        /// For each chat, it constructs a list of chat participants and adds them to the chat object.
        /// If the specified table is "DirectChats", it constructs a DirectChatDetails object.
        /// If the specified table is "GroupChats", it constructs a GroupChatDetails object.
        /// The method returns a list of ChatInformation objects, each containing a chat object and its message history.
        /// If an exception occurs during the retrieval process, it is caught, and the method returns null.
        /// </remarks>
        public static List<ChatInformation> GetChats(string tableName, SqlTransaction transaction)
        {
            SqlDataReader Reader = null;
            List<ChatInformation> chats = new List<ChatInformation>();
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string sql = $"SELECT * FROM {tableName}";
                cmd.CommandText = sql;
                string chatTagLineId;
                string messageHistory;
                DateTime? lastMessageTime;
                string lastMessageContent;
                string lastMessageSenderName;
                List<ChatParticipant> chatParticipants;
                string chatName;
                byte[] chatProfilePicture;
                ChatParticipant chatParticipant;
                string participant;
                string participantProfilePictureId;
                ChatInformation chatInformation;
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    chatTagLineId = Reader["ChatTagLineId"].ToString();
                    messageHistory = Reader["MessageHistory"].ToString();
                    lastMessageTime = Reader.IsDBNull(Reader.GetOrdinal("LastMessageTime")) ? null : (DateTime?)Reader["LastMessageTime"];
                    lastMessageContent = Reader["LastMessageContent"].ToString();
                    lastMessageSenderName = Reader["lastMessageSenderName"].ToString();
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
                    ChatDetails chat = null;
                    if (tableName == "DirectChats")
                    {
                        chat = new DirectChatDetails(chatTagLineId, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants);
                    }
                    else if (tableName == "GroupChats")
                    {
                        chatName = Reader["ChatName"].ToString();
                        chatProfilePicture = (byte[])Reader["ChatProfilePicture"];
                        chat = new GroupChatDetails(chatTagLineId, lastMessageTime, lastMessageContent, lastMessageSenderName, chatParticipants, chatName, chatProfilePicture);
                    }
                    if (chat != null)
                    {
                        chatInformation = new ChatInformation(chat, messageHistory);
                        chats.Add(chatInformation);
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

        #endregion

        #region Public Static User Details Methods

        /// <summary>
        /// The "InsertStatus" method updates the profile status of a user in the UserDetails table.
        /// </summary>
        /// <param name="Username">The username of the user whose status will be updated.</param>
        /// <param name="Status">The new status to be set for the user.</param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <remarks>
        /// This method updates the ProfileStatus column of the UserDetails table for the specified user with the provided status.
        /// </remarks>
        public static int InsertStatus(string Username, string Status)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET ProfileStatus = @Status WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "StatusIsExist" method checks if a user's profile status exists in the UserDetails table.
        /// </summary>
        /// <param name="Username">The username of the user whose profile status will be checked.</param>
        /// <returns>
        /// True if the user's profile status exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method checks the ProfileStatus column of the UserDetails table for the specified user to see if it is not null or not equal to DBNull.Value.
        /// </remarks>
        public static bool StatusIsExist(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND (ProfileStatus IS NOT NULL OR ProfileStatus != @DBNullValue)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "GetLastPasswordUpdateDate" method retrieves the date of the last password update for a user.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// A DateTime representing the date of the last password update. Returns DateTime.MinValue if no date is found.
        /// </returns>
        /// <remarks>
        /// This method retrieves the LastPasswordUpdate column value from the UserDetails table for the specified user.
        /// </remarks>
        public static DateTime GetLastPasswordUpdateDate(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT LastPasswordUpdate FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
                return new DateTime();
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
        /// The "GetEmailAddress" method retrieves the email address associated with a username.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// A string representing the email address associated with the username. Returns an empty string if no email address is found.
        /// </returns>
        /// <remarks>
        /// This method retrieves the EmailAddress column value from the UserDetails table for the specified user.
        /// </remarks>
        public static string GetEmailAddress(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT EmailAddress FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                object result = cmd.ExecuteScalar();
                string emailAddress = "";
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

        /// <summary>
        /// The "GetProfilePicture" method retrieves the profile picture associated with a username.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// A string representing the profile picture associated with the username. Returns an empty string if no profile picture is found.
        /// </returns>
        /// <remarks>
        /// This method retrieves the ProfilePicture column value from the UserDetails table for the specified user.
        /// </remarks>
        public static string GetProfilePicture(string Username)
        {
            try
            {
                cmd.Connection = connection;
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

        /// <summary>
        /// The "GetUserProfileSettings" method retrieves the user profile settings for a specified username.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// A UserDetails object containing the profile settings for the specified user.
        /// </returns>
        /// <remarks>
        /// This method retrieves the ProfilePicture, ProfileStatus, and TagLineId from the UserDetails table for the specified user.
        /// </remarks>
        public static JsonClasses.UserDetails GetUserProfileSettings(string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT ProfilePicture, ProfileStatus, TagLineId FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                string ProfilePicture = "";
                string ProfileStatus = "";
                string TagLineId = "";
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    ProfilePicture = Reader.GetString(0);
                    ProfileStatus = Reader.GetString(1);
                    TagLineId = Reader.GetString(2);
                }
                Reader.Close();
                connection.Close();
                JsonClasses.UserDetails userDetails = new JsonClasses.UserDetails(Username, ProfilePicture, ProfileStatus, TagLineId);
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

        /// <summary>
        /// The "EmailAddressIsExist" method checks if an email address exists in the UserDetails table.
        /// </summary>
        /// <param name="emailAddress">The email address to check.</param>
        /// <returns>
        /// True if the email address exists in the UserDetails table; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method queries the UserDetails table to check if a row exists with the specified email address.
        /// If a row is found, it returns true; otherwise, it returns false.
        /// </remarks>
        public static bool EmailAddressIsExist(string emailAddress)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE EmailAddress = @EmailAddress";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "ProfilePictureIsExist" method checks if a profile picture exists for a specified username.
        /// </summary>
        /// <param name="Username">The username to check for a profile picture.</param>
        /// <returns>
        /// True if a profile picture exists for the specified username; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method queries the UserDetails table to check if the ProfilePicture column is not null or DBNull.Value
        /// for the specified username. If a profile picture exists, it returns true; otherwise, it returns false.
        /// </remarks>
        public static bool ProfilePictureIsExist(string Username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND (ProfilePicture IS NOT NULL OR ProfilePicture != @DBNullValue)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "InsertProfilePicture" method inserts a profile picture ID for a specified username.
        /// </summary>
        /// <param name="Username">The username for which to insert the profile picture ID.</param>
        /// <param name="ProfilePictureID">The ID of the profile picture to insert.</param>
        /// <returns>
        /// The number of rows affected. This should be 1 if the insertion is successful; otherwise, 0.
        /// </returns>
        /// <remarks>
        /// This method updates the ProfilePicture column in the UserDetails table with the specified
        /// ProfilePictureID for the given Username.
        /// </remarks>
        public static int InsertProfilePicture(string Username, string ProfilePictureID)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET ProfilePicture = @ProfilePictureID WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "GetFriendList" method retrieves a list of friends for the specified user.
        /// </summary>
        /// <param name="username">The username of the user whose friends are to be retrieved.</param>
        /// <returns>
        /// A list of strings containing the usernames of the friends of the specified user.
        /// </returns>
        /// <remarks>
        /// This method retrieves the usernames of all friends of the specified user from the Friends table.
        /// It returns a list of strings containing the usernames of the friends.
        /// If no friends are found, an empty list is returned.
        /// </remarks>
        public static Contacts GetFriendsProfileInformation(List<string> FriendNames)
        {
            Contacts contacts = null;
            List<ContactDetails> contactList = new List<ContactDetails>();
            try
            {
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

        /// <summary>
        /// The "GetFriendProfileInformation" method retrieves the profile information for a friend.
        /// </summary>
        /// <param name="FriendName">The username of the friend whose profile information is to be retrieved.</param>
        /// <returns>
        /// A ContactDetails object containing the profile information of the friend.
        /// </returns>
        /// <remarks>
        /// This method retrieves the tagline, profile picture, profile status, last seen time, and online status
        /// of the specified friend from the UserDetails table. It returns a ContactDetails object containing
        /// this information. If the friend is not found, the method returns null.
        /// </remarks>
        public static ContactDetails GetFriendProfileInformation(string FriendName)
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
                cmd.Connection = connection;
                string sql = "SELECT TagLineId, ProfilePicture, ProfileStatus, LastSeenTime, Online FROM UserDetails WHERE Username = @FriendName";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
                }
                Reader.Close();
                connection.Close();
                contact = new ContactDetails(FriendName, TagLine, ProfilePicture, ProfileStatus, LastSeenTime, Online);
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

        /// <summary>
        /// The "IsMatchingUsernameAndTagLineIdExist" method checks if a matching username and tagline ID exist in the UserDetails table.
        /// </summary>
        /// <param name="Username">The username to check.</param>
        /// <param name="TagLine">The tagline ID to check.</param>
        /// <returns>
        /// True if a matching username and tagline ID exist; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method queries the UserDetails table to check if a row exists with the specified username
        /// and tagline ID. If a row is found, it returns true; otherwise, it returns false.
        /// </remarks>
        public static bool IsMatchingUsernameAndTagLineIdExist(string Username, string TagLine)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND TagLineId = @TagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
        /// The "UsernameIsExist" method checks if a username exists in the UserDetails table.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>
        /// True if the username exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method queries the UserDetails table to check if a row exists with the specified username.
        /// If a row is found, it returns true; otherwise, it returns false.
        /// </remarks>
        public static bool UsernameIsExist(string username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "GetChatParticipants" method retrieves a list of chat participants based on the provided participant names.
        /// </summary>
        /// <param name="chatParticipantNames">A list of participant names for which to retrieve chat participants.</param>
        /// <returns>A list of ChatParticipant objects representing the chat participants.</returns>
        /// <remarks>
        /// This method iterates through the provided list of participant names.
        /// For each participant name, it retrieves the profile picture ID using the GetProfilePicture method.
        /// If a profile picture ID is found, it creates a new ChatParticipant object and adds it to the list of chat participants.
        /// The method then returns the list of chat participants.
        /// </remarks>
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

        /// <summary>
        /// The "GetProfilePictureId" method retrieves the profile picture ID of a user from the database.
        /// </summary>
        /// <param name="Username">The username of the user whose profile picture ID to retrieve.</param>
        /// <returns>The profile picture ID of the user, or an empty string if the user does not have a profile picture or an error occurs.</returns>
        /// <remarks>
        /// This method uses a SQL query to retrieve the profile picture ID of the user with the specified username from the UserDetails table.
        /// If the query returns a non-null value, the method converts the result to a string and returns it as the profile picture ID.
        /// If the query returns null or DBNull.Value, indicating that the user does not have a profile picture, the method returns an empty string.
        /// If an exception occurs during the database operation, the method catches the exception, displays an error message, and returns an empty string.
        /// </remarks>
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

        /// <summary>
        /// The "SetUserOnline" method sets a user's online status to online in the database.
        /// </summary>
        /// <param name="username">The username of the user whose online status is being set.</param>
        /// <returns>The number of rows affected in the database. Returns 0 if the operation fails.</returns>
        /// <remarks>
        /// This method updates the UserDetails table in the database, setting the Online column to 1 (online) for the specified username.
        /// It then executes the update query and returns the number of rows affected, which should be 1 if the operation is successful.
        /// </remarks>
        public static int SetUserOnline(string username)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET Online = @OnlineValue WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "SetUserOffline" method sets a user's online status to offline in the database and updates the last seen time.
        /// </summary>
        /// <param name="username">The username of the user whose online status is being set to offline.</param>
        /// <param name="currentDateTime">The current date and time.</param>
        /// <returns>The last seen time of the user.</returns>
        /// <remarks>
        /// This method updates the UserDetails table in the database, setting the Online column to 0 (offline) and updating the LastSeenTime column to the current date and time for the specified username.
        /// It then executes the update query and returns the last seen time of the user, which should be the same as the current date and time if the operation is successful.
        /// </remarks>
        public static DateTime SetUserOffline(string username, DateTime currentDateTime)
        {
            try
            {
                DateTime LastSeenTime = DateTime.Now;
                cmd.Connection = connection;
                string sql = "UPDATE UserDetails SET Online = @OnlineValue, LastSeenTime = @LastSeenTime WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OnlineValue", 0);
                cmd.Parameters.AddWithValue("@LastSeenTime", LastSeenTime);
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                return LastSeenTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.Message);
                return currentDateTime;
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
        /// The "IsMatchingUsernameAndEmailAddressExist" method checks if a user with the given username and email address exists.
        /// </summary>
        /// <param name="Username">The username to check.</param>
        /// <param name="EmailAddress">The email address to check.</param>
        /// <returns>True if a user with the given username and email address exists, otherwise false.</returns>
        /// <remarks>
        /// This method queries the UserDetails table to check if there is a user with the specified username and email address.
        /// It returns true if a matching user is found, indicating that a user with the given username and email address exists,
        /// otherwise it returns false.
        /// </remarks>
        public static bool IsMatchingUsernameAndEmailAddressExist(string Username, string EmailAddress)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND EmailAddress = @Email";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "IsMatchingUsernameAndPasswordExist" method checks if a username and password match an entry in the UserDetails table.
        /// </summary>
        /// <param name="Username">The username to check.</param>
        /// <param name="Password">The password to check.</param>
        /// <returns>True if a match is found, otherwise false.</returns>
        /// <remarks>
        /// This method computes the MD5 hash of the provided password and checks if there is a matching entry in the UserDetails table with the same username and password.
        /// It then returns true if a match is found, indicating that the username and password are valid, otherwise it returns false.
        /// </remarks>
        public static bool IsMatchingUsernameAndPasswordExist(string Username, string Password)
        {
            try
            {
                string Md5Password = Encryption.MD5.CreateMD5Hash(Password);

                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM UserDetails WHERE Username = @Username AND Password = @Md5Password";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        #endregion

        #region Public Static User Verification Information Methods

        /// <summary>
        /// The "GetUserVerificationQuestions" method retrieves the verification questions associated with a user from the database.
        /// </summary>
        /// <param name="Username">The username of the user whose verification questions to retrieve.</param>
        /// <returns>An array of strings containing the verification questions, or an array with empty strings if the user does not have any verification questions or an error occurs.</returns>
        /// <remarks>
        /// This method uses a SQL query to retrieve the verification questions for the user with the specified username from the UserVerificationInformation table.
        /// It constructs and executes a parameterized SQL query to ensure safe handling of user input.
        /// If the query returns a non-null value for a question, the method adds the question to the array of questions at the corresponding index.
        /// If the query returns null or an empty string for a question, the method adds an empty string to the array of questions at the corresponding index.
        /// If an exception occurs during the database operation, the method catches the exception, displays an error message, and returns an array of empty strings.
        /// </remarks>
        public static string[] GetUserVerificationQuestions(string Username)
        {
            string[] questions = new string[5];
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT [Question-1], [Question-2], [Question-3], [Question-4], [Question-5] FROM UserVerificationInformation WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "CheckUserVerificationInformation" method checks if the provided personal verification answers match the stored answers for a user in the database.
        /// </summary>
        /// <param name="Username">The username of the user whose verification information to check.</param>
        /// <param name="personalVerificationAnswers">An object containing the personal verification answers provided by the user.</param>
        /// <returns>True if the answers match, false otherwise or if an error occurs.</returns>
        /// <remarks>
        /// This method queries the UserVerificationInformation table in the database to retrieve the stored verification questions and answers for the specified user.
        /// It then compares the provided answers with the stored answers, using MD5 encryption for comparison.
        /// If the method finds a match for each of the provided answers in the stored answers, it returns true.
        /// If any answer does not match or if the method encounters an error during the database operation, it returns false.
        /// The method ensures safe handling of user input by using parameterized SQL queries to prevent SQL injection attacks.
        /// </remarks>
        public static bool CheckUserVerificationInformation(string Username, PersonalVerificationAnswers personalVerificationAnswers)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserVerificationInformation WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        #endregion

        #region Public Static Tag Line Methods

        /// <summary>
        /// The "SetTagLine" method generates a unique tag line for a new user or chat.
        /// </summary>
        /// <param name="DataBaseTableName">The name of the database table for which the tag line is being generated.</param>
        /// <param name="HandleConnection">Optional. Indicates whether to handle the database connection. Default is true.</param>
        /// <returns>A unique tag line as a string.</returns>
        /// <remarks>
        /// This method generates a random string of 6 characters as the tag line.
        /// It then checks if the generated tag line already exists in the specified database table.
        /// If the tag line exists, it generates a new random tag line until a unique one is found.
        /// 
        /// The method provides an option to handle the database connection, allowing for reuse in different scenarios.
        /// </remarks>
        private static string SetTagLine(string DataBaseTableName, bool HandleConnection = true)
        {
            bool isTagLineExists = true;
            string TagLine = "";
            while (isTagLineExists)
            {
                TagLine = RandomStringCreator.RandomString(6);
                if (!TaglineIsExist(TagLine, DataBaseTableName, HandleConnection))
                {
                    isTagLineExists = false;
                }
            }
            return TagLine;
        }

        /// <summary>
        /// The "SetTagLine" method generates a unique tag line for a new user or chat, ensuring it does not already exist in two specified database tables.
        /// </summary>
        /// <param name="DataBaseTableName1">The name of the first database table to check for tag line existence.</param>
        /// <param name="DataBaseTableName2">The name of the second database table to check for tag line existence.</param>
        /// <param name="HandleConnection">Optional. Indicates whether to handle the database connection. Default is true.</param>
        /// <returns>A unique tag line as a string.</returns>
        /// <remarks>
        /// This method generates a random string of 6 characters as the tag line.
        /// It then checks if the generated tag line already exists in either of the specified database tables.
        /// If the tag line exists in either table, it generates a new random tag line until a unique one is found.
        /// The method provides an option to handle the database connection, allowing for reuse in different scenarios.
        /// </remarks>
        public static string SetTagLine(string DataBaseTableName1, string DataBaseTableName2, bool HandleConnection = true)
        {
            bool isTagLineExists = true;
            string TagLine = "";
            while (isTagLineExists)
            {
                TagLine = RandomStringCreator.RandomString(6);
                if (!TaglineIsExist(TagLine, DataBaseTableName1, HandleConnection) && !TaglineIsExist(TagLine, DataBaseTableName2, HandleConnection))
                {
                    isTagLineExists = false;
                }
            }
            return TagLine;
        }

        /// <summary>
        /// The "TaglineIsExist" method checks if a tagline exists in the specified table.
        /// </summary>
        /// <param name="Tagline">The tagline to check.</param>
        /// <param name="DataBaseTableName">The name of the table to check.</param>
        /// <param name="HandleConnection">Specifies whether to handle opening and closing the connection.</param>
        /// <returns>
        /// True if the tagline exists in the specified table; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method queries the specified table to check if a row exists with the specified tagline.
        /// If a row is found, it returns true; otherwise, it returns false.
        /// </remarks>
        public static bool TaglineIsExist(string Tagline, string DataBaseTableName, bool HandleConnection)
        {
            try
            {
                cmd.Connection = connection;
                string columnName = (DataBaseTableName == "UserDetails") ? "TaglineId" : "ChatTaglineId";
                string sql = $"SELECT COUNT(*) FROM {DataBaseTableName} WHERE {columnName} = @Tagline";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        #endregion

        #region Public Static Friends Methods

        /// <summary>
        /// The "CheckFullFriendsCapacity" method checks if the user's friends list has reached its capacity.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// True if the user's friends list has reached its capacity; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method checks if there are any null values in the user's friends list for the given username, indicating that there is space for a new friend.
        /// </remarks>
        public static bool CheckFullFriendsCapacity(string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i))
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }
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

        /// <summary>
        /// The "GetFriendColumnToInsert" method retrieves the name of the column in the Friends table where a new friend should be inserted.
        /// </summary>
        /// <param name="Username">The username of the user for whom the column name is retrieved.</param>
        /// <param name="transaction">The SQL transaction to be used for the database operation.</param>
        /// <returns>
        /// A string containing the name of the column in the Friends table where a new friend should be inserted.
        /// </returns>
        /// <remarks>
        /// This method retrieves the name of the first empty column (starting from the third column, as the first two columns are reserved for id and username) in the Friends table for the specified user.
        /// It uses the provided SQL transaction for the database operation.
        /// If an empty column is found, the method returns the name of that column.
        /// If no empty column is found, the method returns an empty string.
        /// </remarks>
        public static string GetFriendColumnToInsert(string Username, SqlTransaction transaction)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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
                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                }
            }
        }

        /// <summary>
        /// The "GetFriendList" method retrieves the list of friends for a given user.
        /// </summary>
        /// <param name="username">The username for which to retrieve the friend list.</param>
        /// <returns>A list of strings containing the usernames of the user's friends.</returns>
        /// <remarks>
        /// This method retrieves the list of friends for the specified user from the Friends table in the database.
        /// It iterates through the rows returned by the query and adds the usernames of the user's friends to a list.
        /// The method then returns this list of friend usernames.
        /// </remarks>
        public static List<string> GetFriendList(string username)
        {
            SqlDataReader Reader = null;
            StringBuilder Friends = new StringBuilder();
            List<string> friends = new List<string>();
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) // 0-id, 1-username, 2,3,4 - friends names...
                    {
                        friends.Add(Reader[i].ToString());
                    }
                }
                Reader.Close();
                connection.Close();
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

        /// <summary>
        /// The "AddFriend" method adds a new friend to a user's friend list.
        /// </summary>
        /// <param name="UsernameAdding">The username of the user adding a new friend.</param>
        /// <param name="UsernameAdded">The username of the user being added as a friend.</param>
        /// <param name="transaction">The SQL transaction to be used for the database operation.</param>
        /// <returns>
        /// An integer indicating the number of rows affected by the database update operation.
        /// </returns>
        /// <remarks>
        /// This method updates the Friends table by adding the username of the user being added as a friend to the appropriate column,
        /// determined by the "FriendColumn" retrieved using the "GetFriendColumnToInsert" method for the user adding a new friend.
        /// It uses the provided SQL transaction for the database operation.
        /// If the update is successful, the method returns the number of rows affected (1).
        /// If no rows are affected, the method returns 0.
        /// </remarks>
        public static int AddFriend(string UsernameAdding, string UsernameAdded, SqlTransaction transaction)
        {
            try
            {
                string FriendColumn = GetFriendColumnToInsert(UsernameAdding, transaction);
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string sql = "UPDATE Friends SET [" + FriendColumn + "] = @UsernameAdded WHERE Username = @UsernameAdding";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "AreFriends" method checks if two users are friends based on their usernames.
        /// </summary>
        /// <param name="Username">The username of the first user.</param>
        /// <param name="FriendUsername">The username of the second user (potential friend).</param>
        /// <returns>True if the two users are friends, false otherwise.</returns>
        /// <remarks>
        /// This method queries the Friends table in the database to determine if the specified usernames are associated with a friend relationship.
        /// It iterates through the rows corresponding to the first user's username and checks if the second user's username is found in any column beyond the first two (skipping the ID and username columns).
        /// If a match is found, the method returns true, indicating that the users are friends. If no match is found, it returns false.
        /// </remarks>
        public static bool AreFriends(string Username, string FriendUsername)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM Friends WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "AddColumnToFriends" method adds a new column to the Friends table.
        /// </summary>
        /// <remarks>
        /// This method is used to add a new column to the Friends table. It calls the "AddColumnToTable" method with the table name "Friends" to perform the operation.
        /// </remarks>
        public static void AddColumnToFriends()
        {
            AddColumnToTable("Friends");
        }

        #endregion

        #region Public Static Friend Requests Methods

        /// <summary>
        /// The "IsFriendRequestPending" method checks if a friend request from a sender to a receiver is pending.
        /// </summary>
        /// <param name="SenderUsername">The username of the sender.</param>
        /// <param name="ReceiverUsername">The username of the receiver.</param>
        /// <returns>True if there is a pending friend request between the sender and receiver, otherwise false.</returns>
        /// <remarks>
        /// This method checks the FriendRequest table to see if there is a pending friend request from the sender to the receiver.
        /// It returns true if a pending request is found, indicating that the friend request is pending, otherwise it returns false.
        /// </remarks>
        public static bool IsFriendRequestPending(string SenderUsername, string ReceiverUsername)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT COUNT(*) FROM FriendRequest WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = 'Pending'";
                cmd.CommandText = sql;
                cmd.Parameters.Clear(); 
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

        /// <summary>
        /// The "AddFriendRequest" method adds a friend request from one user to another.
        /// </summary>
        /// <param name="FriendRequestSenderUsername">The username of the user sending the friend request.</param>
        /// <param name="FriendRequestReceiverUsername">The username of the user receiving the friend request.</param>
        /// <returns>The number of rows affected. Returns 0 if the insertion fails.</returns>
        /// <remarks>
        /// This method inserts a new record into the FriendRequest table with the specified sender and receiver usernames,
        /// representing a friend request from the sender to the receiver.
        /// If the insertion is successful, it returns the number of rows affected (1), indicating that the friend request was added.
        /// If the insertion fails, it returns 0.
        /// </remarks>
        public static int AddFriendRequest(string FriendRequestSenderUsername, string FriendRequestReceiverUsername)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "INSERT INTO FriendRequest (SenderUsername, ReceiverUsername) VALUES (@SenderUsername, @ReceiverUsername)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "GetFriendRequestDate" method retrieves the date of the most recent friend request between two users.
        /// </summary>
        /// <param name="SenderUsername">The username of the sender of the friend request.</param>
        /// <param name="ReceiverUsername">The username of the receiver of the friend request.</param>
        /// <param name="currentTime">The current date and time used as a fallback if no request date is found.</param>
        /// <returns>
        /// The date of the most recent friend request between the specified sender and receiver.
        /// If no request date is found, the method returns the current date and time.
        /// </returns>
        /// <remarks>
        /// This method retrieves the date of the most recent friend request between the sender and receiver usernames from the FriendRequest table.
        /// It selects the top 1 request date where the sender, receiver, and request status match the specified usernames and 'Pending' status, ordering the results by request date in descending order.
        /// If a request date is found, it is returned. Otherwise, the method returns the current date and time as a fallback value.
        /// </remarks>
        public static DateTime GetFriendRequestDate(string SenderUsername, string ReceiverUsername, DateTime currentTime)
        {
            DateTime RequestDate = currentTime;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT TOP 1 RequestDate FROM FriendRequest WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = 'Pending' ORDER BY RequestDate DESC";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "CheckFriendRequests" method retrieves a list of past friend requests for a specified user.
        /// </summary>
        /// <param name="username">The username of the user whose friend requests are to be retrieved.</param>
        /// <returns>
        /// A list of PastFriendRequest objects representing the past friend requests received by the specified user.
        /// </returns>
        /// <remarks>
        /// This method retrieves past friend requests for the specified user from the FriendRequest table.
        /// It selects the sender username and request date for all pending friend requests where the receiver username matches the specified username.
        /// For each retrieved friend request, a new PastFriendRequest object is created and added to the list of past friend requests.
        /// If no friend requests are found, the method returns null.
        /// </remarks>
        public static List<PastFriendRequest> CheckFriendRequests(string username)
        {
            SqlDataReader Reader = null;
            List<PastFriendRequest> pastFriendRequests = new List<PastFriendRequest>();
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT SenderUsername, RequestDate FROM FriendRequest WHERE ReceiverUsername = @Username AND RequestStatus = 'Pending'";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                string friendUsername = "";
                DateTime requestDate = DateTime.Now;
                while (Reader.Read())
                {
                    friendUsername = Reader["SenderUsername"].ToString();
                    requestDate = Convert.ToDateTime(Reader["RequestDate"]);
                    PastFriendRequest pastFriendRequest = new PastFriendRequest(friendUsername, requestDate);
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

        /// <summary>
        /// The "HandleFriendRequestStatus" method updates the status of a friend request between two users.
        /// </summary>
        /// <param name="SenderUsername">The username of the user who sent the friend request.</param>
        /// <param name="ReceiverUsername">The username of the user who received the friend request.</param>
        /// <param name="FriendRequestStatus">The new status of the friend request (e.g., Accepted, Rejected).</param>
        /// <returns>
        /// An integer indicating the number of rows affected by the update operation.
        /// </returns>
        /// <remarks>
        /// This method updates the status of a friend request in the FriendRequest table.
        /// It sets the RequestStatus column to the specified FriendRequestStatus for the specified SenderUsername and ReceiverUsername,
        /// where the current RequestStatus is 'Pending'.
        /// If the update is successful, the method returns the number of rows affected (1).
        /// If no rows are affected (e.g., the friend request does not exist or the status is already updated), the method returns 0.
        /// </remarks>
        public static int HandleFriendRequestStatus(string SenderUsername, string ReceiverUsername, string FriendRequestStatus)
        {
            try
            {
                cmd.Connection = connection;
                string sql = "UPDATE FriendRequest SET RequestStatus = @FriendRequestStatus WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername AND RequestStatus = @CurrentFriendRequestStatus";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        #endregion

        #region Public Static User Past Passwords Methods

        /// <summary>
        /// The "PasswordIsExist" method checks if the specified password exists in the UserPastPasswords table for the given username.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <param name="Password">The password to check for existence.</param>
        /// <returns>
        /// A boolean value indicating whether the password exists in the UserPastPasswords table for the given username.
        /// </returns>
        /// <remarks>
        /// This method hashes the input password using MD5 and compares it with the hashed passwords stored in the UserPastPasswords table.
        /// If a matching hashed password is found, the method returns true; otherwise, it returns false.
        /// </remarks>
        public static bool PasswordIsExist(string Username, string Password)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
                string Md5Password = YouChatServer.Encryption.MD5.CreateMD5Hash(Password);

                connection.Open();
                Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    for (int i = 2; i < Reader.FieldCount; i++) //0 - id, 1- username
                    {
                        var Value = Reader[i];
                        if ((Value != DBNull.Value) && (Value != null) && (Value.ToString() == Md5Password))
                        {
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

        /// <summary>
        /// The "GetPasswordColumnToInsert" method retrieves the column name in the UserPastPasswords table where a new password should be inserted for the given username.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// The column name in the UserPastPasswords table where a new password should be inserted, or an empty string if no suitable column is found.
        /// </returns>
        /// <remarks>
        /// This method searches for an empty or null value in the UserPastPasswords table for the given username and returns the name of the column where a new password can be inserted.
        /// </remarks>
        public static string GetPasswordColumnToInsert(string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "CheckFullPasswordCapacity" method checks if the user's past passwords table has reached its capacity.
        /// </summary>
        /// <param name="Username">The username of the user.</param>
        /// <returns>
        /// True if the user's past passwords table has reached its capacity; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method checks if there are any null values in the user's past passwords table for the given username, indicating that there is space for a new password.
        /// </remarks>
        public static bool CheckFullPasswordCapacity(string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM UserPastPasswords WHERE Username = @Username";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Username", Username);
                connection.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i))
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }
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

        /// <summary>
        /// The "AddColumnToUserPastPasswords" method adds a new column to the UserPastPasswords table.
        /// </summary>
        /// <remarks>
        /// This method is used to add a new column to the UserPastPasswords table. It calls the "AddColumnToTable" method with the table name "UserPastPasswords" to perform the operation.
        /// </remarks>
        public static void AddColumnToUserPastPasswords()
        {
            AddColumnToTable("UserPastPasswords");
        }

        #endregion

        #region Public Static Group Chats Methods

        /// <summary>
        /// The "CreateGroupChat" method creates a new group chat in the database.
        /// </summary>
        /// <param name="chat">The ChatCreator object containing information about the chat.</param>
        /// <param name="ChatTagLine">The tagline of the chat.</param>
        /// <param name="XmlFilePath">The file path to the XML file containing the chat's message history.</param>
        /// <returns>Returns the number of rows affected in the database.</returns>
        /// <remarks>
        /// This method inserts a new record into the GroupChats table with the specified chat details. It then iterates through the list of chat members (excluding the first member, as it is already added) and adds them to the group chat. If the capacity of the GroupChats table is full, it adds a new column to the table to accommodate more members.
        /// </remarks>
        public static int CreateGroupChat(ChatCreator chat, string ChatTagLine, string XmlFilePath)
        {
            try
            {
                string ChatName = chat.ChatName;
                List<string> ChatMembers = chat.ChatParticipants;
                string FirstChatMember = ChatMembers[0];
                byte[] ChatIconBytes = chat.ChatProfilePictureBytes;
                cmd.Connection = connection;
                string sql = "INSERT INTO GroupChats (ChatName, ChatTagLineId, ChatProfilePicture, MessageHistory, [ChatParticipant-1]) VALUES(@ChatName, @ChatTagLine, @ChatIcon, @MessageHistory, @FirstChatMember)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                cmd.Parameters.AddWithValue("@ChatIcon", ChatIconBytes);
                cmd.Parameters.AddWithValue("@MessageHistory", XmlFilePath);
                cmd.Parameters.AddWithValue("@FirstChatMember", FirstChatMember);
                connection.Open();
                int x = cmd.ExecuteNonQuery();
                connection.Close();
                bool isNeededToAddColumn = false;
                for (int index = 1; index < ChatMembers.Count; index++)
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

        /// <summary>
        /// The "AddNewChatMember" method adds a new member to a group chat.
        /// </summary>
        /// <param name="ChatName">The name of the group chat.</param>
        /// <param name="ChatTagLine">The tagline of the group chat.</param>
        /// <param name="Username">The username of the user to add as a member.</param>
        /// <returns>Returns the number of rows affected. A value of -1 indicates an error.</returns>
        /// <remarks>
        /// This method updates the GroupChats table to add a new member to the specified group chat. It uses the provided ChatName and ChatTagLine to locate the correct chat and updates the corresponding column with the new member's username. If the operation fails, it returns 0.
        /// </remarks>
        public static int AddNewChatMember(string ChatName, string ChatTagLine, string Username)
        {
            try
            {
                string ChatMemberColumn = GetChatMemberColumnToInsert(ChatName, ChatTagLine);
                cmd.Connection = connection;
                string sql = "UPDATE GroupChats SET [" + ChatMemberColumn + "] = @Username WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "DeleteGroupChatMember" method deletes a member from a group chat.
        /// The method isn't in use.
        /// </summary>
        /// <param name="ChatName">The name of the group chat.</param>
        /// <param name="ChatTagLine">The tagline of the group chat.</param>
        /// <param name="Username">The username of the member to delete.</param>
        /// <returns>Returns true if the member was successfully deleted; otherwise, false.</returns>
        /// <remarks>
        /// This method first checks if the specified member exists in the group chat specified by ChatName and ChatTagLine. 
        /// If the member is found, their entry in the GroupChats table is set to null to indicate deletion. 
        /// The method then returns true. If the member is not found, it returns false.
        /// </remarks>
        public static bool DeleteGroupChatMember(string ChatName, string ChatTagLine, string Username)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        /// <summary>
        /// The "GetChatMemberColumnToInsert" method retrieves the name of the column in the GroupChats table where a new chat member should be inserted.
        /// </summary>
        /// <param name="ChatName">The name of the group chat.</param>
        /// <param name="ChatTagLine">The tagline of the group chat.</param>
        /// <returns>The name of the column where a new chat member should be inserted, or an empty string if no column is available.</returns>
        /// <remarks>
        /// This method checks the GroupChats table for the specified group chat using the ChatName and ChatTagLine parameters.
        /// It then looks for an empty or null column in the table (starting from the 8th column) where a new chat member can be inserted.
        /// If such a column is found, its name is returned. If no suitable column is found, an empty string is returned.
        /// </remarks>
        public static string GetChatMemberColumnToInsert(string ChatName, string ChatTagLine)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                connection.Open();
                Reader = cmd.ExecuteReader();
                string columnName = "";
                if (Reader.Read())
                {
                    for (int i = 8; i < Reader.FieldCount; i++) //0 - id, 1- username
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

        /// <summary>
        /// The "CheckFullChatsCapacity" method checks if the group chat has reached its full capacity.
        /// </summary>
        /// <param name="ChatName">The name of the group chat.</param>
        /// <param name="ChatTagLine">The tagline of the group chat.</param>
        /// <returns>True if the group chat has reached its full capacity, otherwise false.</returns>
        /// <remarks>
        /// This method checks the GroupChats table for the specified group chat using the ChatName and ChatTagLine parameters.
        /// It then checks if all the chat member columns (starting from the 8th column) are filled.
        /// If all columns are filled, it means the group chat has reached its full capacity and returns true.
        /// Otherwise, it returns false.
        /// </remarks>
        public static bool CheckFullChatsCapacity(string ChatName, string ChatTagLine)
        {
            SqlDataReader Reader = null;
            try
            {
                cmd.Connection = connection;
                string sql = "SELECT * FROM GroupChats WHERE ChatName = @ChatName AND ChatTagLineId = @ChatTagLine";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ChatName", ChatName);
                cmd.Parameters.AddWithValue("@ChatTagLine", ChatTagLine);
                connection.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.Read())
                {
                    for (int i = 7; i < Reader.FieldCount; i++)
                    {
                        if (Reader.IsDBNull(i))
                        {
                            Reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }
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

        /// <summary>
        /// The "AddColumnToGroupChats" method adds a new column to the GroupChats table.
        /// </summary>
        /// <remarks>
        /// This method is used to add a new column to the GroupChats table. It calls the "AddColumnToTable" method with the table name "GroupChats" to perform the operation.
        /// </remarks>
        public static void AddColumnToGroupChats()
        {
            AddColumnToTable("GroupChats");
        }

        #endregion

        #region Public Static Direct Chats Methods

        /// <summary>
        /// The "CreateDirectChat" method creates a new direct chat between two users.
        /// </summary>
        /// <param name="ChatTagLine">The tagline of the direct chat.</param>
        /// <param name="ChatParticipant1">The username of the first chat participant.</param>
        /// <param name="ChatParticipant2">The username of the second chat participant.</param>
        /// <param name="MessageHistoryFilePath">The file path to the XML file containing the chat's message history.</param>
        /// <param name="transaction">The SQL transaction to be used for the database operation.</param>
        /// <returns>Returns the number of rows affected. A value of -1 indicates an error.</returns>
        /// <remarks>
        /// This method inserts a new record into the DirectChats table with the provided chat details. It is typically used within a transaction to ensure data integrity. If the operation fails, it returns 0.
        /// </remarks>
        public static int CreateDirectChat(string ChatTagLine, string ChatParticipant1, string ChatParticipant2, string MessageHistoryFilePath, SqlTransaction transaction) //for when i want to add another particiapnt i need to first give him the group tagline as well becuase there might be couple group with the same name
        {
            try
            {
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                string sql = "INSERT INTO DirectChats (ChatTagLineId, MessageHistory, [ChatParticipant-1], [ChatParticipant-2]) VALUES(@ChatTagLine, @MessageHistory, @FirstChatMember, @SecondChatMember)";
                cmd.CommandText = sql;
                cmd.Parameters.Clear();
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

        #endregion
    }
}
