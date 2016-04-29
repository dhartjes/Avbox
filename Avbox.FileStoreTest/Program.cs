using AzureFileStor;
using System;
using System.Configuration;
using System.Text;

namespace Avbox.FileStoreTest
{
    class Program
    {
        private enum exitcode
        {
            success = 0,
            fail = 1
        }

        static int Main(string[] args)
        {
            try
            {
                GetFileTest.Run();
                return (int)exitcode.success;
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to complete request. Exiting...");
                Console.Read();
                return (int)exitcode.fail;
            }
        }
    }
}
