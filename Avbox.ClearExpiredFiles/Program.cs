using AzureFileStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avbox.ClearExpiredFiles
{
    public class Program
    {
        static void Main(string[] args)
        {
            var daysToRetain = default(int);

            try
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["ExpiredAfterXDays"], out daysToRetain))
                {
                    throw new Exception("Configuration setting 'ExpireAfterXDays' must be an integer value.");
                }

                FileManager.DeleteExpiredFiles(daysToRetain);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
