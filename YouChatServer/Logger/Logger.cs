using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace YouChatServer
{
    internal class Logger
    {
        //private static readonly string logFileName = "LogFile.txt";
        //private static readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YouChatSever", "LogFile.txt");
        //private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);
        private static StreamWriter logFile;

        private static string logDirectory = Path.Combine("Log", "LogsFolder");


        static Logger()
        {
            Console.WriteLine("f");
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
                //AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
                Application.ApplicationExit += new EventHandler(ApplicationExitHandler);
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging.
                Console.WriteLine($"Failed to create log file: {ex}");
            }
        }
        //private static void ProcessExitHandler(object sender, EventArgs e)
        //{
        //    CloseLogFiles();
        //}

        private static void ApplicationExitHandler(object sender, EventArgs e)
        {
            // Add a "server down" message to the log before closing
            LogInfo("Server down. Application is shutting down...");

            // Perform any necessary cleanup before the application exits
            CloseLogFiles();
        }
        //static Logger()
        //{
        //    string projectDirectory1 = AppDomain.CurrentDomain.BaseDirectory;
        //    string projectDirectory2 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug","");

        //    string logDirectory1 = Path.Combine(projectDirectory1, GeneralLogDirectory);
        //    string logDirectory2 = Path.Combine(projectDirectory2, GeneralLogDirectory);

        //    try
        //    {
        //        Directory.CreateDirectory(logDirectory1);
        //        Directory.CreateDirectory(logDirectory2);

        //        // Generate a unique log file name based on the current date.
        //        string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.txt";

        //        // Create or open the log file for writing (append mode).
        //        string logFilePath1 = Path.Combine(logDirectory1, logFileName);
        //        logFile1 = new StreamWriter(logFilePath1, true);
        //        logFile1.AutoFlush = true; // Automatically flush the buffer after each write.
        //        string logFilePath2 = Path.Combine(logDirectory2, logFileName);
        //        logFile2 = new StreamWriter(logFilePath2, true);
        //        logFile2.AutoFlush = true; // Automatically flush the buffer after each write
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or print the exception details for debugging.
        //        Console.WriteLine($"Failed to create log file: {ex}");
        //    }
        //}

        public static void LogUserLogIn(string message)
        {
            Log("LOG IN", message);

        }
        public static void LogUserLogOut(string message)
        {
            Log("LOG OUT", message);

        }
        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public static void LogError(string message)
        {
            Log("ERROR", message);
        }


        private static void Log(string level, string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            try
            {
                logFile.WriteLine(logEntry);

            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during logging, e.g., file access issues.
                Console.WriteLine($"Failed to log: {ex}");
            }
        }
        public static void CloseLogFiles()
        {
            logFile.Close();
            logFile.Dispose();
        }
    }
}
