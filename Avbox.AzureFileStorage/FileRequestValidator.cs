using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureFileStorage
{
    public static class FileRequestValidator
    {
        #region privateMethods

        /// <summary>
        /// Reserved file names in Windows.
        /// </summary>
        private static string[] _reserved = new string[]
        {
	        "con",
            "prn",
            "aux",
            "nul",
            "com1",
            "com2",
            "com3",
            "com4",
            "com5",
            "com6",
            "com7",
            "com8",
            "com9",
            "lpt1",
            "lpt2",
            "lpt3",
            "lpt4",
            "lpt5",
            "lpt6",
            "lpt7",
            "lpt8",
            "lpt9",
            "clock$"
        };

        /// <summary>
        /// Determine if the path file name is reserved.
        /// </summary>
        private static bool IsReservedFileName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path); // Extension doesn't matter
            fileName = fileName.ToLower(); // Case-insensitive
            foreach (string reservedName in _reserved)
            {
                if (reservedName == fileName)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsDirectoryTraversal(string directoryName)
        {
            if (directoryName == "." || 
                directoryName == "/" ||
                directoryName.Contains(".."))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region publicMethods

        public static bool IsValidDirectoryName(string directoryName)
        {
            // Must have a valid, non-null directory name. Azure shares similar restrictions on directory names with Windows.
            if (String.IsNullOrWhiteSpace(directoryName))
            {
                Console.WriteLine("Directory name cannot be null or white space.");
                return false;
            }

            if (!directoryName.IsNormalized())
            {
                directoryName = directoryName.Normalize();
            }

            directoryName = HttpUtility.UrlDecode(directoryName);

            if (ContainsDirectoryTraversal(directoryName))
            {
                Console.WriteLine("Directory name cannot be used to navigate the directory.");
                return false;
            }

            try
            {
                Path.GetFullPath(directoryName);
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Directory name is invalid.");
                return false;
            }
        }

        public static bool IsValidFileName(string fileName)
        {
            // Must have a valid, non-null file name. Azure shares similar restrictions on file names with Windows.
            if (String.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("File name cannot be null or white space.");
                return false;
            }

            if (!fileName.IsNormalized())
            {
                fileName = fileName.Normalize();
            }

            fileName = HttpUtility.UrlDecode(fileName);

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Console.WriteLine("Invalid characters used in file name.");
                return false;
            }

            if (IsReservedFileName(fileName))
            {
                Console.WriteLine("File name is a reserved Windows name.");
                return false;
            }
            // Else there are no invalid fileName chars.
            return true;
        }

        #endregion
    }
}
