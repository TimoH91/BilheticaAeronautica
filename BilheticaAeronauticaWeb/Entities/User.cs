using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace BilheticaAeronauticaWeb.Entities
{
    public class User : IdentityUser
    {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Role { get; set; }

            [DisplayName("Photo")]
            public Guid ImageId { get; set; }

            public string ImageFullPath => ImageId == Guid.Empty
                ? "~/images/noimage.jpg"
            : $"~/images/airplanes/{ImageId}.jpg";

    }
}
