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
    public static class CloudFileClientFactory
    {
        public static CloudFileClient GetCloudFileClient(){
            // Uses Microsoft Azure Configuration Manager Nuget Package.
            // Get the storage account using the connection string from app settings.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a CloudFileClient object for credentialed access to File storage.
             return storageAccount.CreateCloudFileClient();
        }
    }
}
