using AzureFileStor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avbox.FileStoreTest
{
    public static class GetFileTest
    {
        public static void Run()
        {
            var share = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);
            if (share == null)
                throw new Exception("Couldn't retrieve file share.");

            var file = FileManager.GetFile(share, "11645-TestFile.zip", "Test");
            if (file == null)
                throw new Exception("Couldn't retrieve file.");

            // Write the contents of the file to the console window.
            Console.WriteLine(file.DownloadTextAsync().Result);
            Console.Read();
        }
    }
}
