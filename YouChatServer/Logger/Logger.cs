using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace YouChatServer
{
    /// <summary>
    /// The "Logger" class provides logging functionality for various events in the application.
    /// </summary>
    internal class Logger
    {
        #region Private Static Fields

        /// <summary>
        /// The StreamWriter object "logFile" is used for writing log messages to a file.
        /// </summary>
        private static StreamWriter logFile;

        #endregion

        #region Private Static Readonly Fields

        /// <summary>
        /// The string constant 'logDirectory' represents the directory path where log files are stored.
        /// </summary>
        private static readonly string logDirectory = Path.Combine("Log", "LogsFolder");

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static constructor for the <see cref="Logger"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the logger by creating a log directory and file.
        /// It sets up the log file for writing, including creating a unique log file name based on the current date.
        /// </remarks>
        static Logger()
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            logDirectory = Path.Combine(projectDirectory, logDirectory);

            try
            {
                Directory.CreateDirectory(logDirectory);

                // Generate a unique log file name based on the current date.
                string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";

                // Create or open the log file for writing (append mode).
                string logFilePath = Path.Combine(logDirectory, logFileName);
                logFile = new StreamWriter(logFilePath, true);
                logFile.AutoFlush = true; // Automatically flush the buffer after each write.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create log file: {ex}");
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// The "LogUserLogIn" method logs a user login event with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "LOG IN".
        /// </remarks>
        public static void LogUserLogIn(string message)
        {
            Log("LOG IN", message);
        }
        /// <summary>
        /// The "LogUserLogOut" method logs a user logout event with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "LOG OUT".
        /// </remarks>
        public static void LogUserLogOut(string message)
        {
            Log("LOG OUT", message);
        }
        /// <summary>
        /// The "LogUserBanStart" method logs the start of a user ban with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "BAN".
        /// </remarks>
        public static void LogUserBanStart(string message)
        {
            Log("BAN", message);
        }

        /// <summary>
        /// The "LogUserBanOver" method logs the end of a user ban with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "BAN OVER".
        /// </remarks>
        public static void LogUserBanOver(string message)
        {
            Log("BAN OVER", message);
        }

        /// <summary>
        /// The "LogUserDOSProtection" method logs a message related to DOS protection with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "DOS PROTECTION".
        /// </remarks>
        public static void LogUserDOSProtection(string message)
        {
            Log("DOS PROTECTION", message);
        }

        /// <summary>
        /// The "LogUserSocketConnection" method logs a message related to socket connection with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "SOCKET CONNECTION".
        /// </remarks>
        public static void LogUserSocketConnection(string message)
        {
            Log("SOCKET CONNECTION", message);
        }

        /// <summary>
        /// The "LogInfo" method logs an informational message with the specified message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method calls the Log method with the log level set to "INFO".
        /// </remarks>
        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The "Log" method logs a message with the specified log level.
        /// </summary>
        /// <param name="level">The log level (e.g., INFO, LOG IN, LOG OUT).</param>
        /// <param name="message">The message to be logged.</param>
        /// <remarks>
        /// This method creates a log entry with the current timestamp, log level, and the provided message.
        /// It then attempts to write the log entry to the log file. If an exception occurs during logging (e.g., file access issues), it is caught and displayed in the console.
        /// </remarks>
        private static void Log(string level, string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            try
            {
                logFile.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log: {ex}");
            }
        }

        #endregion
    }
}
