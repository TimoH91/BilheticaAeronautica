using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class RecoverPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
