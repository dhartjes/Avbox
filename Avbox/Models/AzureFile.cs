using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Avbox.Models
{
    public class AzureFile
    {
        public string FileName { get; set; }
        public double FileSize { get; set; }
        public string Recipient { get; set; }
        public string AzureUri { get; set; }
    }

    public class AzureFileDownloadViewModel
    {
        public string FileName { get; set; }
        //public string AzureUri { get; set; }
        public long FileSize { get; set; }
        [DisplayName("Secret")]
        public string Directory { get; set; }
        [DisplayName("Id")]
        public int AzureId { get; set; }
        [DisplayName("Your Email")]
        public string Recipient { get; set; }
    }

    public class AzureFileUploadViewModel
    {
        public string FileName { get; set; }
        public string Recipient { get; set; }
    }
}