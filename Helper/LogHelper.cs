using Serilog;
using System.IO;
using System.Text;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Console = Colorful.Console;


namespace Helper
{
    public class LogHelper
    {
        public static string? appLogPath => Path.Combine(Environment.CurrentDirectory, "log.txt");
        public static void CreateLogFile(string? filepath)
        {
            string? path = appLogPath;

            if (filepath != null)
            {
                path = filepath;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(path,//文件保存路径
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",//输出日期格式
                rollingInterval: RollingInterval.Day,//日志按日保存
                rollOnFileSizeLimit: true,          // 限制单个文件的最大长度   
                encoding: Encoding.UTF8,            // 文件字符编码     
                retainedFileCountLimit: null         // 最大保存文件数     
                                                     //fileSizeLimitBytes: 10 * 1024       // 最大单个文件长度
                )
                .CreateLogger();
            AddInfoLog("App Start", Color.Green, true, false);
            Console.WriteLine("Log file created at: " + path, Color.Green);
        }
        public static void AddInfoLog(string message, Color? color = null, bool isTimeStamp = false, bool isLogFile = true)
        {
            string logMessage = message;
            if (isTimeStamp)
            {
                logMessage = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            }
            if (color != null)
            {
                Console.WriteLine(logMessage, color);
            }
            else
            {
                Console.WriteLine(logMessage);
            }
            if (isLogFile)
            {
                Log.Information(logMessage);
            }
        }

        public static void AddErrorLog(string message, Color? color = null, bool isTimeStamp = false, bool isLogFile = true)
        {
            string logMessage = message;

            if (isTimeStamp)
            {
                logMessage = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            }

            if (color == null)
            {
                Console.WriteLine(logMessage, Color.Red);
            }
            else
            {
                Console.WriteLine(logMessage, color);
            }

            if (isLogFile)
            {
                Log.Error(logMessage);
            }
        }

        public static void AddWarningLog(string message, Color? color = null, bool isTimeStamp = false, bool isLogFile = true)
        {
            string logMessage = message;

            if (isTimeStamp)
            {
                logMessage = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            }

            if (color == null)
            {
                Console.WriteLine(logMessage, Color.Red);
            }
            else
            {
                Console.WriteLine(logMessage, color);
            }

            if (isLogFile)
            {
                Log.Warning(logMessage);
            }
        }

        public static void AddFatalLog(string message, Color? color = null, bool isTimeStamp = false, bool isLogFile = true)
        {
            string logMessage = message;

            if (isTimeStamp)
            {
                logMessage = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            }

            if (color == null)
            {
                Console.WriteLine(logMessage, Color.Red);
            }
            else
            {
                Console.WriteLine(logMessage, color);
            }

            if (isLogFile)
            {
                Log.Fatal(logMessage);
            }
        }
    }
}

