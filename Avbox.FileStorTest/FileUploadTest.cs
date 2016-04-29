using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureFileStorage;
using System.Configuration;
using System.IO;

namespace Avbox.FileStorTest
{
    [TestClass]
    public class FileUploadTest
    {
        public static string TestFile = "TestFile.txt";
        public static string TestFileText = "Default text string: I am happy.";


        [TestMethod]
        public void CreateFileFromTextTest()
        {
            var newFileUri = FileManager.CreateAzureFileFromText(TestFile, TestFileText);
            Console.WriteLine(newFileUri);
            Assert.IsNotNull(newFileUri);
            Assert.AreNotEqual("", newFileUri);
        }

        [TestMethod]
        public void CreateAzureFilesFromFilesTest()
        {
            Exception e = null;

            try
            {
                var testFilesDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"TestFiles\"));
                var testFiles = testFilesDirectory.GetFiles();

                foreach (var testFile in testFiles)
                {
                    Assert.IsTrue(testFile.Exists);
                    Assert.IsTrue(FileManager.CreateAzureFileFromFile(testFile));

                }
            }
            catch (Exception newException)
            {
                e = newException;
            }

            Assert.IsNull(e);
        }
    }
}
