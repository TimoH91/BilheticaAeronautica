using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class Token
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("tokenType")]
        public string? TokenType { get; set; }
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
