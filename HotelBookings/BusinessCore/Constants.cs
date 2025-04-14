
namespace HotelBookings.BusinessCore
{
    public static class Constants
    {
        public const string NoFileName = "File name not supplied!";
        public const string ValidInputExamples = "\nAvailability(H1, 20240901, SGL)\nAvailability(H1, 20240901-20240903, DBL)\nSearch(H1, 365, SGL)\nType 'exit' to terminate the process.";
        public const string AvailabilityRegexPattern = @"^(?i)Availability\s*\(\s*(\w+)\s*,\s*(\d{8}(?:-\d{8})?)\s*,\s*(\w+)\s*\)\s*$";
        public const string SearchRegexPattern = @"^(?i)Search\s*\(\s*(\w+)\s*,\s*(\d+)\s*,\s*(\w+)\s*\)\s*$";
    }
}
