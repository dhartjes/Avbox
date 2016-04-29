using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFileStorage
{
    public static class FileShareManager
    {
        public static CloudFileShare GetFileShare(string shareName)
        {
            var fileClient = CloudFileClientFactory.GetCloudFileClient();

            // Get a reference to the file share we created in the storageAccount.
            var share = fileClient.GetShareReference(shareName);

            // Ensure that the share exists.
            if (share.Exists())
            {
                return share;
            }
            else
            {
                throw new Exception("A file share with that name could not be found.");
            }
        }
    }
}
