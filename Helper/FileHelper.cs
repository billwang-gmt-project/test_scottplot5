using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class FileHelper
    {
        public static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                // Try to open the file for exclusive access
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // If an IOException is thrown, the file is locked
                return true;
            }
            finally
            {
                // Make sure to close the stream if it was opened
                stream?.Close();
            }

            // If no exception was thrown, the file is not locked
            return false;
        }
    }
}
