using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
//using AzureFileStorage.Data;
using System.Data.SqlClient;

namespace AzureFileStorage
{
    public static class FileManager
    {
        #region External Methods

        /// <summary>
        /// Gets the cloud file.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Directory could not be found.
        /// or
        /// File could not be found.
        /// </exception>
        public static CloudFile GetCloudFile(string directoryName, string fileName)
        {
            var targetDirectory = GetCloudDirectory(directoryName);

            if (targetDirectory == null)
            {
                // Get Cloud File requires valid directory
                throw new Exception("Directory could not be found.");
            }

            var cloudFile = targetDirectory.GetFileReference(fileName);

            // Ensure that the file exists.
            if (cloudFile != null && cloudFile.Exists())
            {
                return cloudFile;
            }
            else
            {
                // Get Cloud File requires valid file
                throw new Exception("File could not be found.");
            }
        }

        /// <summary>
        /// Gets the cloud file sas URI.
        /// </summary>
        /// <purpose>
        /// By returning the signed access signature uri, the user can download directly from blob storage rather than through
        /// our web server, but have the security of having a limited time window within which to retrieve the document.
        /// </purpose>
        /// <param name="share">The share.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns></returns>
        public static Uri GetCloudFileSasUri(string directoryName, string fileName)
        {
            var cloudFile = GetCloudFile(directoryName, fileName);

            // Generate Url with Shared Access Signature
            var builder = new UriBuilder(cloudFile.Uri);
            builder.Query = cloudFile.GetSharedAccessSignature(
                new SharedAccessFilePolicy
                {
                    Permissions = SharedAccessFilePermissions.Read,
                    SharedAccessStartTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-5)),
                    SharedAccessExpiryTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(1))
                }).TrimStart('?');

            var signedFileUri = builder.Uri;

            return signedFileUri;
        }

        /// <summary>
        /// Creates the azure file from a text string using the provided filename. Places in a random directory.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileText">The file text.</param>
        /// <param name="fileRecipient">The file recipient.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static bool CreateAzureFileFromText(string fileName, string fileText, string fileRecipient = "")
        {
            //TODO: Replace with code that analyzes the file space of the file to be uploaded.
            var directory = GenerateRandomString();
            var fileSize = 1000;

            var targetDirectory = GetCloudDirectory(directory, true);

            CloudFile cloudFile = null;

            try
            {
                cloudFile = targetDirectory.GetFileReference(fileName);

                cloudFile.Create(fileSize);
                Console.WriteLine("File created");

                cloudFile.UploadText(fileText);
                Console.WriteLine("File text uploaded");
            }
            catch (StorageException se)
            {
                if (se.InnerException != null)
                {
                    var webException = (System.Net.WebException)se.InnerException;

                    var response = (System.Net.HttpWebResponse)webException.Response;

                    if (response.StatusDescription != null)
                    {
                        Console.WriteLine(response.StatusDescription);
                        throw new Exception(response.StatusDescription);
                    }
                }
                throw se;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            // Add FileInfo to Database
            return SqlHelper.TryAddAzureFileInfo(cloudFile, directory, fileName, fileSize, fileRecipient);

        }

        /// <summary>
        /// Creates the azure file from file.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <param name="recipient">The recipient.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool CreateAzureFileFromFile(FileInfo fileInfo, string fileRecipient = "")
        {
            var directory = GenerateRandomString();
            var fileSize = fileInfo.Length;

            var targetDirectory = GetCloudDirectory(directory, true);

            CloudFile cloudFile = null;

            try
            {
                cloudFile = targetDirectory.GetFileReference(fileInfo.Name);

                cloudFile.Create(fileInfo.Length);
                Console.WriteLine("File created");

                cloudFile.UploadFromFile(fileInfo.FullName, FileMode.Open);
                Console.WriteLine("File uploaded");
            }
            catch (StorageException se)
            {
                if (se.InnerException != null)
                {
                    var webException = (System.Net.WebException)se.InnerException;

                    var response = (System.Net.HttpWebResponse)webException.Response;

                    if (response.StatusDescription != null)
                    {
                        Console.WriteLine(response.StatusDescription);
                        throw new Exception(response.StatusDescription);
                    }
                }
                throw se;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return SqlHelper.TryAddAzureFileInfo(cloudFile, directory, fileInfo.Name, fileSize, fileRecipient);

            //try
            //{
            //    // Add FileInfo to Database
            //    using (var db = new AzureFileInfoContext())
            //    {
            //        var azureFileInfo = new AzureFileInfo()
            //        {
            //            AzureUri = cloudFile.Uri.ToString(),
            //            AzureDirectory = directory,
            //            FileName = fileInfo.Name,
            //            FileSize = fileSize,
            //            UploadDate = DateTime.Now
            //        };

            //        // Generate the azureFileInfo field and save to DB.
            //        db.AzureFileInfos.Add(azureFileInfo);
            //        db.SaveChanges();

            //        // The newly generated Id value will be used to create the Link that the user will click on: Download\[Id]
            //        azureFileInfo.GenerateLink();

            //        /* 
            //         * If a file recipient is specified and is a valid email address, the user of the link will be required to 
            //         * enter their email address to identify themselves before downloading.
            //         */
            //        if (!String.IsNullOrWhiteSpace(fileRecipient))
            //        {
            //            azureFileInfo.Recipient = fileRecipient;
            //        }

            //        // Commit additional updates to the FileInfo record.
            //        db.SaveChanges();

            //        return azureFileInfo.Link;
            //    }
            //}
            //catch (SqlException se)
            //{
            //    Console.WriteLine(se.Message);
            //    throw se;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    throw e;
            //} 
        }

        public static Uri GetSasUri(Uri newFileUri)
        {
            var uriParts = newFileUri.ToString().Split('/');

            var fileName = uriParts[uriParts.Length - 1];
            var directoryName = uriParts[uriParts.Length - 2];

            var cloudFile = GetCloudFile(directoryName, fileName);

            var builder = new UriBuilder(cloudFile.Uri);
            builder.Query = cloudFile.GetSharedAccessSignature(
                new SharedAccessFilePolicy
                {
                    Permissions = SharedAccessFilePermissions.Read,
                    SharedAccessStartTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-5)),
                    SharedAccessExpiryTime = new DateTimeOffset(DateTime.UtcNow.AddDays(30))
                }).TrimStart('?');

            var signedFileUri = builder.Uri;

            return signedFileUri;
        }

        /// <summary>
        /// Deletes the expired files in the specified directory. CloudFileDirectories found in the current directory will 
        /// call DeleteExpiredFiles recursively to get any files nested inside of one or more CloudFileDirectories.
        /// </summary>
        /// <param name="daysToRetain">The days to retain.</param>
        /// <param name="currentDirectory">The current directory.</param>
        public static void DeleteExpiredFiles(int daysToRetain, CloudFileDirectory currentDirectory = null)
        {
            bool isRootDirectory = false;

            if (currentDirectory == null)
            {
                // GetCloudDirectory without a parameter returns the root directory.
                currentDirectory = GetCloudDirectory();
                isRootDirectory = true;
            }

            var azureItems = currentDirectory.ListFilesAndDirectories();

            foreach (var azureItem in azureItems)
            {
                if (azureItem.GetType() == typeof(CloudFileDirectory))
                {
                    var azureDirectory = (CloudFileDirectory)azureItem;
                    DeleteExpiredFiles(daysToRetain, azureDirectory);
                }
                else if(azureItem.GetType() == typeof(CloudFile))
                {
                    var azureFile = (CloudFile)azureItem;
                    azureFile.FetchAttributes();
                    Console.WriteLine(azureFile.Properties);

                    var lastModified = azureFile.Properties.LastModified;

                    if (lastModified != null)
                    {
                        var compareResult = DateTimeOffset.Compare((DateTimeOffset)lastModified, DateTimeOffset.Now.AddDays(-(double)daysToRetain));

                        if (compareResult < 0)
                        {
                            // Remove from Avbox database
                            SqlHelper.TryRemoveAzureFileInfo(azureFile);

                            // Remove from Azure Storage.
                            azureFile.Delete();
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot determine age of file. LastModified is null.");
                    }
                }
                else
                {
                    throw new Exception("Unknown file type found. " + azureItem.Uri);
                }
            }

            // If the current directory has no files left and is not the root directory, delete the directory as well.
            if (!isRootDirectory)
            {
                // Directory is not root directory.
                var isDirectoryEmpty = !currentDirectory.ListFilesAndDirectories().Any();

                if (isDirectoryEmpty)
                {
                    currentDirectory.Delete();
                }
            }
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the cloud directory specified as a string value or returns the root directory if no directory is specified.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <param name="createIfNotExists">if set to <c>true</c> [create if not exists].</param>
        /// <returns></returns>
        private static CloudFileDirectory GetCloudDirectory(string directoryName = "", bool createIfNotExists = false)
        {
            var fileShareClient = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);

            // Get a reference to the root directory of the share.
            var rootDirectory = fileShareClient.GetRootDirectoryReference();

            if (String.IsNullOrWhiteSpace(directoryName))
            {
                return rootDirectory;
            }

            var targetDirectory = rootDirectory.GetDirectoryReference(directoryName);
            
            // Ensure that the directory exists.
            if (!targetDirectory.Exists())
            {
                if (createIfNotExists)
                {
                    /*
                    * To ensure that parent directories also exist, split the directoryName and ensure each directory in the 
                    * path is created. 
                    */
                    var pathSplit = targetDirectory.Name.Split('\\');

                    var pathName = "";

                    foreach (var pathPart in pathSplit)
                    {
                        pathName = String.IsNullOrWhiteSpace(pathName) ? pathPart : Path.Combine(pathName, pathPart);

                        var pathDirectory = rootDirectory.GetDirectoryReference(pathName);

                        if (!pathDirectory.Exists())
                            pathDirectory.Create();
                    } 
                }
                else
                {
                    return null;
                }
            }
            return targetDirectory;
        }

        private static string GenerateRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[12];
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Buffer storage.
                byte[] data = new byte[2];

                for (int i = 0; i < stringChars.Length; i++)
                {
                    // Fill the buffer.
                    rng.GetBytes(data);
                    int value = Math.Abs(BitConverter.ToInt16(data, 0) % chars.Length);

                    stringChars[i] = chars[value];
                }
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        #endregion
    }
}
