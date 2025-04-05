using System.ComponentModel.DataAnnotations;

namespace Common.Attributes;
/// <summary>
/// Validates that a string is a valid GUID/UUID
/// </summary>
public class GuidAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        
        if (value is string strValue)
        {
            if (Guid.TryParse(strValue, out _))
            {
                return ValidationResult.Success;
            }
        }
        
        return new ValidationResult("The field must be a valid GUID/UUID.");
    }   
}
