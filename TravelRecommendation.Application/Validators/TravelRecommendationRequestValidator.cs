using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using TravelRecommendation.Application.DTO;
namespace TravelRecommendation.Application.NewFolder
{
    public class TravelRecommendationRequestValidator : AbstractValidator<TravelRecommendationRequest>
    {

        public TravelRecommendationRequestValidator()
        {

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180");

            RuleFor(x => x.DestinationDistrict)
                .NotEmpty()
                .WithMessage("Destination district is required")
                .MaximumLength(100)
                .WithMessage("District name must not exceed 100 characters");

            RuleFor(x => x.TravelDate)
              .Cascade(CascadeMode.Stop) // ✅ Stops at first error
              .NotEmpty()
             .WithMessage("Travel date is required")
             .Must(BeValidFormat)
             .WithMessage("Travel date must be in yyyy-MM-dd format (e.g., 2025-01-02)")
             .Must(BeValidDate)
             .WithMessage("Travel date must be a valid date")
             .Must(BeTodayOrFuture)
             .WithMessage("Travel date cannot be in the past")
             .Must(BeWithinForecastRange)
             .WithMessage("Travel date must be within the next 6 days due to air quality forecast availability limitations");
        }

        // Check if original query string matches yyyy-MM-dd format
        private bool BeValidFormat(string travelDate)
        {
            if (string.IsNullOrWhiteSpace(travelDate)) return false;
            var regex = new Regex(@"^\d{4}-\d{2}-\d{2}$");
            return regex.IsMatch(travelDate);
        }

        private bool BeValidDate(string travelDate)
        {
            return DateTime.TryParseExact(travelDate,  "yyyy-MM-dd",  CultureInfo.InvariantCulture,  DateTimeStyles.None,  out _);
        }

        private bool BeTodayOrFuture(string travelDate)
        {
            if (!DateTime.TryParseExact( travelDate,  "yyyy-MM-dd", CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime date))
            {
                return false;
            }

            return date.Date >= DateTime.Today;
        }

        private bool BeWithinForecastRange(string travelDate)
        {
            if (!DateTime.TryParseExact(   travelDate, "yyyy-MM-dd",  CultureInfo.InvariantCulture,   DateTimeStyles.None,   out DateTime date))
            {
                return false;
            }

            return date.Date <= DateTime.Today.AddDays(5);
        }
    }
}
