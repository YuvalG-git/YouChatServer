using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace YouChatServer.UserDetails
{
    internal class DataCreator
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
        private static string databaseName = "UserDetails2";

        private static string SqlRelativePath()
        {
            projectFolderPath = projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length) + "UserDetails2\\UserDetails2.mdf";
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""" + projectFolderPath + @""";Integrated Security=True";
            return connectionString;
        }
        static void CreateDataBase()
        {

            // Specify the database name


            // Create a connection to the master database for initial setup
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the database already exists
                if (DoesDatabaseExist(connection, databaseName))
                {
                    Console.WriteLine("Database already exists.");
                }
                else
                {
                    // Create the database
                    CreateDatabase(connection, databaseName);
                    Console.WriteLine("Database created successfully.");
                }

                // Switch the connection string to use the new database
                //builder.InitialCatalog = databaseName;
                connection.ChangeDatabase(databaseName);

                // Create tables
                CreatesTables(connection);

                Console.WriteLine("Tables created successfully.");
            }
        }

        static bool DoesDatabaseExist(SqlConnection connection, string databaseName)
        {
            // Check if the database exists
            using (SqlCommand command = new SqlCommand($"SELECT DB_ID('{databaseName}')", connection))
            {
                return (command.ExecuteScalar() != null);
            }
        }

        static void CreateDatabase(SqlConnection connection, string databaseName)
        {
            // Create the database
            using (SqlCommand command = new SqlCommand($"CREATE DATABASE {databaseName}", connection))
            {
                command.ExecuteNonQuery();
            }
        }

        //static void CreateTable(SqlConnection connection, string tableName, string columns)
        //{
        //    // Create the table
        //    using (SqlCommand command = new SqlCommand($"CREATE TABLE {tableName} ({columns})", connection))
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //}
        private static void CreatesTables(SqlConnection connection)
        {
            CreateTable_Chats(connection);
            CreateTable_FriendRequest(connection);
            CreateTable_Friends(connection);
            CreateTable_UserDetails(connection);
            CreateTable_UserPastPasswords(connection);
            CreateTable_UserVerificationInformation(connection);
        }
        private static void CreateTable_Chats(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE [dbo].[Chats] (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [ChatName] NVARCHAR(50) NOT NULL,
                        [ChatTagLineId] NCHAR(6) NOT NULL,
                        [ChatProfilePicture] IMAGE NOT NULL,
                        [MessageHistory] XML NULL,
                        [LastMessageTime] DATETIME NULL,
                        [LastMessageContent] NVARCHAR(MAX) NULL,
                        [ChatParticipant-1] NVARCHAR(50) NOT NULL,
                        PRIMARY KEY CLUSTERED([Id] ASC)
                    );";
            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table created successfully.");
            }  
        }
        private static void CreateTable_FriendRequest(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE [dbo].[FriendRequest] (
                        [RequestId] INT IDENTITY (1, 1) NOT NULL,
                        [SenderUsername] NVARCHAR (50) NOT NULL,
                        [ReceiverUsername] NVARCHAR (50) NOT NULL,
                        [RequestDate] DATETIME DEFAULT (getdate()) NOT NULL,
                        [RequestStatus] NVARCHAR (50) DEFAULT ('Pending') NOT NULL,
                        PRIMARY KEY CLUSTERED ([RequestId] ASC)
                    );";
            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }
        private static void CreateTable_Friends(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE [dbo].[Friends] (
                        [Id] INT IDENTITY (1, 1) NOT NULL,
                        [UserName] NVARCHAR (50) NOT NULL,
                        PRIMARY KEY CLUSTERED ([Id] ASC)
                    );";
            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }
        private static void CreateTable_UserDetails(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE [dbo].[UserDetails] (
                        [Id] INT IDENTITY (1, 1) NOT NULL,
                        [Username] NVARCHAR (30) NOT NULL,
                        [Password] NVARCHAR (50) NOT NULL,
                        [FirstName] NVARCHAR (30) NOT NULL,
                        [LastName]  NVARCHAR (30) NOT NULL,
                        [EmailAddress] NVARCHAR (30) NOT NULL,
                        [City] NVARCHAR (40) NOT NULL,
                        [BirthDate] DATE NOT NULL,
                        [Gender] NVARCHAR (50) NOT NULL,
                        [LastPasswordUpdate] DATE NOT NULL,
                        [ProfilePicture] NVARCHAR (50) NULL,
                        [ProfileStatus] NVARCHAR (50) NULL,
                        [LastSeenProperty] BIT DEFAULT ((1)) NOT NULL,
                        [OnlineProperty] BIT DEFAULT ((1)) NOT NULL,
                        [ProfilePictureProperty] BIT DEFAULT ((1)) NOT NULL,
                        [StatusProperty] BIT DEFAULT ((1)) NOT NULL,
                        [TextSizeProperty] TINYINT DEFAULT ((2)) NOT NULL,
                        [MessageGapProperty] SMALLINT DEFAULT ((10)) NOT NULL,
                        [EnterKeyPressedProperty] BIT DEFAULT ((0)) NOT NULL,
                        [TagLineId] NCHAR (6) NOT NULL,
                        [LastSeenTime] DATETIME DEFAULT (getdate()) NOT NULL,
                        [Online] BIT DEFAULT ((1)) NOT NULL,
                        PRIMARY KEY CLUSTERED ([Id] ASC)
                    );";

            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }
        private static void CreateTable_UserPastPasswords(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE [dbo].[UserPastPasswords] (
                        [Id] INT IDENTITY (1, 1) NOT NULL,
                        [Username]   NVARCHAR (50) NOT NULL,
                        [Password-1] NVARCHAR (50) NOT NULL,
                        PRIMARY KEY CLUSTERED ([Id] ASC)
                    );";

            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }
        private static void CreateTable_UserVerificationInformation(SqlConnection connection)
        {
            string cmd = @"
                    CREATE TABLE[dbo].[UserVerificationInformation] (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Username] NVARCHAR(50)  NOT NULL,
                        [TagLineId] NCHAR(6) NOT NULL,
                        [Question - 1] NVARCHAR(100) NOT NULL,
                        [Answer - 1] NVARCHAR(100) NOT NULL,
                        [Question - 2] NVARCHAR(100) NOT NULL,
                        [Answer - 2] NVARCHAR(100) NOT NULL,
                        [Question - 3] NVARCHAR(100) NOT NULL,
                        [Answer - 3] NVARCHAR(100) NOT NULL,
                        [Question - 4] NVARCHAR(100) NOT NULL,
                        [Answer - 4] NVARCHAR(100) NOT NULL,
                        [Question - 5] NVARCHAR(100) NOT NULL,
                        [Answer - 5] NVARCHAR(100) NOT NULL,
                        PRIMARY KEY CLUSTERED([Id] ASC)
                    );";

            using (SqlCommand command = new SqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Table created successfully.");
            }
        }
    }
}
