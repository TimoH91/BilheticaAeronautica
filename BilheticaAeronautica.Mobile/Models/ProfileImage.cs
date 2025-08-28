using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class ProfileImage
    {
        public string? ImageUrl { get; set; }
        public string? ImageFullPath => AppConfig.BlobUrl + ImageUrl;
    }
}
