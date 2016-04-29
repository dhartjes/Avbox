using AzureFileStorage.Data;
using System;
using System.Data.Entity;

namespace Avbox.RefreshDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDatabase();
        }

        private static void CreateDatabase()
        {
            var context = new AzureFileInfoContext();
            context.Database.Initialize(true);
        }
    }
}
