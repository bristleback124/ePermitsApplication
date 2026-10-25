using System.ComponentModel.DataAnnotations;
using ePermitsApp.Constants;

namespace ePermitsApp.Helpers
{
    /// Peso amount on a permit form: 0 to ValidationLimits.MaxCost. Out-of-range values
    /// fail model validation (400) instead of overflowing the decimal column on save (500).
    public sealed class CostRangeAttribute : RangeAttribute
    {
        public CostRangeAttribute() : base(typeof(decimal), "0", ValidationLimits.MaxCost)
        {
            ParseLimitsInInvariantCulture = true;
            ConvertValueInInvariantCulture = true;
            ErrorMessage = "{0} must be between ₱0 and ₱9,999,999,999,999.99.";
        }
    }

    /// Area in sqm on a permit form: 0 to ValidationLimits.MaxArea.
    public sealed class AreaRangeAttribute : RangeAttribute
    {
        public AreaRangeAttribute() : base(typeof(decimal), "0", ValidationLimits.MaxArea)
        {
            ParseLimitsInInvariantCulture = true;
            ConvertValueInInvariantCulture = true;
            ErrorMessage = "{0} must be between 0 and 999,999,999.9999 sqm.";
        }
    }
}
