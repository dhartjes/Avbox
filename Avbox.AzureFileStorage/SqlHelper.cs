using AzureFileStorage.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;
using System.Data.SqlClient;

namespace AzureFileStorage
{
    public class SqlHelper
    {
        /// <summary>
        /// Tries to add AzureFileInfo record to DB. Creates a link that can be sent to the recipient. 
        /// If a file recipient is specified, the user of the link will be required to enter their email address as authentication.
        /// </summary>
        /// <param name="cloudFile">The cloud file.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <param name="fileRecipient">The file recipient.</param>
        /// <returns></returns>
        internal static bool TryAddAzureFileInfo(CloudFile cloudFile, string directory, string fileName, long fileSize, string fileRecipient)
        {
            try
            {
                // Add FileInfo to Database
                using (var db = new AzureFileInfoContext())
                {
                    var azureFileInfo = new AzureFileInfo()
                    {
                        AzureUri = cloudFile.Uri.ToString(),
                        AzureDirectory = directory,
                        FileName = fileName,
                        FileSize = fileSize,
                        UploadDate = DateTime.Now,
                        Recipient = !String.IsNullOrWhiteSpace(fileRecipient) ? fileRecipient : null
                    };

                    // Generate the azureFileInfo field and save to DB.
                    db.AzureFileInfos.Add(azureFileInfo);
                    db.SaveChanges();

                    // The newly generated Id value will be used to create the Link that the user will click on: Download\[Id]
                    azureFileInfo.GenerateLink();

                    // Commit additional updates to the FileInfo record.
                    db.SaveChanges();

                    return true;
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        internal static bool TryRemoveAzureFileInfo(CloudFile azureFile)
        {
            try
            {
                using (var db = new AzureFileInfoContext())
                {
                    var azureUriFilter = azureFile.Uri.ToString();
                    var azureFileNameFilter = azureFile.Name;

                    var azureFileInfo = db.AzureFileInfos.FirstOrDefault(x => x.FileName == azureFileNameFilter &&
                                                                         x.AzureUri == azureUriFilter);

                    if (azureFileInfo != null)
                    {
                        db.AzureFileInfos.Remove(azureFileInfo);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException se)
            {
                Console.WriteLine(se.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


    }
}
