using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Validations
{
    public class FutureDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Today;
            }
            return false;
        }
    }
}
