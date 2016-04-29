using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzureFileStorage.Data;
using AzureFileStorage;
using Avbox.Models;

namespace Avbox.Controllers
{
    public class DownloadController : Controller
    {
        private AzureFileInfoContext db = new AzureFileInfoContext();

        // GET: Download
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Download/Details/DirectoryName/1
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public ActionResult Details(string directory, int azureId, string recipient)
        {
            if (!FileRequestValidator.IsValidDirectoryName(directory))
            {
                return new HttpNotFoundResult();
            }

            using (db)
            {
                var azureFileInfo = db.AzureFileInfos.FirstOrDefault(x => x.Id == azureId && x.AzureDirectory == directory);

                if (azureFileInfo == null)
                {
                    return new HttpNotFoundResult();
                }

                var azureUri = FileManager.GetSasUri(new Uri(azureFileInfo.AzureUri));

                return View(new AzureFileDownloadViewModel() 
                { 
                    Directory = azureFileInfo.AzureDirectory,
                    AzureId = azureFileInfo.Id,
                    FileName = azureFileInfo.FileName,
                    FileSize = azureFileInfo.FileSize,
                    Recipient = azureFileInfo.Recipient
                });
            }
        }

        /// <summary>
        /// Downloads the file using a generated url with a SAS token.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public ActionResult GetAzureFile(string directory, int azureId)
        {
            if (!FileRequestValidator.IsValidDirectoryName(directory))
            {
                return new HttpNotFoundResult();
            }

            using (db)
            {
                var azureFileInfo = db.AzureFileInfos.FirstOrDefault(x => x.Id == azureId);

                if (azureFileInfo == null ||
                    !azureFileInfo.AzureUri.Contains(directory))
                {
                    return new HttpNotFoundResult();
                }

                var azureUri = FileManager.GetSasUri(new Uri(azureFileInfo.AzureUri));

                //var uri = FileManager.GetCloudFileSasUri(azurePath, fileName);

                return Redirect(azureUri.ToString());
            }
        }

        /// <summary>
        /// Lists AzureFileInfo from the database. Ability to go to a file and download.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult List()
        {
            return View(db.AzureFileInfos.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
