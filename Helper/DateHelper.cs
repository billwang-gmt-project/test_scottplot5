using System.Reflection;
using static Helper.DateHelper;

// Apply the custom attribute to your assembly
[assembly: BuildDate("2024-05-20")]


namespace Helper
{
    public class DateHelper
    {

        // Custom attribute to hold build date
        [AttributeUsage(AttributeTargets.Assembly)]
        public class BuildDateAttribute : Attribute
        {
            public DateTime BuildDate { get; }

            public BuildDateAttribute(string buildDate)
            {
                BuildDate = DateTime.Parse(buildDate);
            }
        }

        // Get date time string in format yyyy-MM-dd HH:mm:ss
        public static string GetDateTimeString() => DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

        public static string GetTimeString => $"[{DateTime.Now:HH:mm:ss}]";

        // Get the build date of the assembly
        // Note: Not work for WPF publish mode
        public static DateTime GetBuildDate()
        {
            // Get the executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the custom attribute
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();

            if (attribute != null)
            {
                return attribute.BuildDate;
            }
            else
            {
                // Attribute not found, return default
                return DateTime.MinValue;
            }
        }
    }
}
