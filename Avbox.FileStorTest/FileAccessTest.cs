using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureFileStorage;
using System.Configuration;
using System.IO;

namespace Avbox.FileStorTest
{
    [TestClass]
    public class FileAccessTest
    {
        public static string TestFile = "TestFile.txt";
        //public static string TestUserName = "dhartjes";
        //public static string TestCompanyName = "Winmark-Test";
        //public static Guid SessionGuid = Guid.NewGuid();
        //public static string TestDirectory = "Test2";
        //public static string TestDirectory = Path.Combine(TestCompanyName, TestUserName, SessionGuid.ToString());
        public static string TestDirectory = "KrNYODsZr0RY";
        public static string TestFileText = "Default text string: I am happy.";
        public static int DefaultQuota = 5120;

        [TestMethod]
        public void FileRequestValidatorTest()
        {
            Assert.AreEqual(false, TestFileName("%2e%2e%2fForbiddenFile.txt"), "File name with encoded invalid characters should fail.");
            Assert.AreEqual(false, TestFileName("com1"), "File name cannot be a reserved file name in Windows.");
            Assert.AreEqual(false, TestFileName("Invalid?Chars"), "File name cannot contain invalid character '?'");
            Assert.AreEqual(false, TestFileName("Invalid\\Chars"), "File name cannot contain invalid character '\'");
            Assert.AreEqual(false, TestFileName("Invalid/Chars"), "File name cannot contain invalid character '/'");

            Assert.AreEqual(true, TestFileName("Program%20Files(x86)"), "File name is url decoded. Spaces are allowed");
            Assert.AreEqual(true, TestFileName(TestFile), "TestFile contains an invalid file name.");

            //Assert.AreEqual(false, TestDirectoryName("%2e%2e%2fForbiddenDirectory"), "Url Encoding cannot be used to change navigate directories.");
            //Assert.AreEqual(false, TestDirectoryName(""), "Blank directory name is not permitted.");
            //Assert.AreEqual(false, TestDirectoryName("."), "Root directory shorthand '.' is not valid for a directory name.");
            //Assert.AreEqual(false, TestDirectoryName("/"), "Root directory shorthand '/' is not valid for a directory name.");
            //Assert.AreEqual(false, TestDirectoryName(".."), "Two '.' in a row is not permitted in a directory name.");
            
            //// I intend to give users only single depth access to the file structure in Azure. 
            //// Format for folder structure will be: [avboxShareName]/[companyName]/[userName]/[Session-GUID]/[fileName]
            //Assert.AreEqual(true, TestDirectoryName("Winmark.Test"), "Single '.' can be used in a directory name.");
            //Assert.AreEqual(true, TestDirectoryName("Test1/SubTest1"), "Forwardslashes are acceptable in directory names.");
            //Assert.AreEqual(true, TestDirectoryName("Test1\\SubTest1"), "Backslashes are acceptable in directory names.");
            //Assert.AreEqual(true, TestDirectoryName(TestDirectory), "Invalid directory name in TestDirectory variable.");
        }

        private bool TestDirectoryName(string directoryName)
        {
            return FileRequestValidator.IsValidDirectoryName(directoryName);
        }

        private bool TestFileName(string fileName)
        {
            return FileRequestValidator.IsValidFileName(fileName);
        }

        [TestMethod]
        public void GetClientFileShareTest()
        {
            Assert.IsNotNull(FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]));
        }

        [TestMethod]
        public void GetCloudFileTest()
        {
            var share = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);
            var file = FileManager.GetCloudFile(TestDirectory, TestFile);
            Console.WriteLine(file.DownloadText());
        }

        [TestMethod]
        public void GetCloudFileSasUriTest()
        {
            //var share = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);
            var sasUri = FileManager.GetCloudFileSasUri(TestDirectory, TestFile);
            Console.WriteLine(sasUri);
            Assert.IsNotNull(sasUri);
        }

        [TestMethod]
        public void CheckFileShareCurrentUsageTest()
        {
            var share = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);
            var stats = share.GetStats();

            // Check the current usage stats for the share.
            //throw new Exception(String.Format("Current share usage: {0} GB", stats.Usage.ToString()));

            // Check the current quota for the share. Fetch attributes populates the share's properties with up-to-date data.
            //throw new Exception(String.Format("Current share quota: {0} GB", share.Properties.Quota));

            Assert.AreEqual(DefaultQuota, share.Properties.Quota);

            // Check that at least 10 GB of usage are still available before exceeding quota.
            Assert.IsTrue(share.Properties.Quota > stats.Usage + 10);
        }

        [TestMethod]
        public void UpdateFileShareQuotaTest()
        {
            var share = FileShareManager.GetFileShare(ConfigurationManager.AppSettings["ClientFileShare"]);

            try
            {
                // Set property in memory. Quota has a minimum of 1.
                share.Properties.Quota = 1;

                // Set property must be called to ensure that FetchAttributes doesn't retrieve old data.
                share.Properties.Quota = 1;
                share.SetProperties();
                share.FetchAttributes();
                Assert.IsTrue(share.Properties.Quota < DefaultQuota);

                // Get current stats.
                var stats = share.GetStats();

                // Set to greater than the current usage.
                share.Properties.Quota = 10 + stats.Usage;
                share.SetProperties();
                share.FetchAttributes();
                Assert.IsTrue(share.Properties.Quota > 0);

                // Restore to original default setting.
                share.Properties.Quota = DefaultQuota;
                share.SetProperties();
                share.FetchAttributes();
                Assert.AreEqual(DefaultQuota, share.Properties.Quota);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
