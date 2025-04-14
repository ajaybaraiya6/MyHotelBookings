using HotelBookings.BusinessCore;
using HotelBookings.BusinessCore.Models;
using HotelBookings.BusinessCore.Services;

namespace HotelBookings
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Declarations 
            string? hotelFile = null;
            string? bookingFile = null;
            List<HotelModel>? hotels =  null;
            List<BookingModel>? bookings = null;
            string output = string.Empty;

            // Validate the app run arguments.
            if (Common.IsValidUserInput(args, ref hotelFile, ref bookingFile))
            {
                // Validate the present file with JSON data.
                if(!Common.IsValidFileAndData(hotelFile)) return;
                if (!Common.IsValidFileAndData(bookingFile)) return;

                // Read the JSON from file and fill it to our list to process further.
                Common.FillDataListFromFile(hotelFile, bookingFile, ref hotels, ref bookings);

                // Decide which operation we will perform based on user input.
                string commandString;
                Enum opMethod = Common.GetOperationMethodFromUser(out commandString);
                BookingServices bookingServices = new BookingServices();

                // Process the function according to command.
                switch (opMethod)
                {
                    case OperationCommands.Availability:
                        // Process availability command.
                        output = bookingServices.Availability(commandString, hotels, bookings);
                        break;
                    case OperationCommands.Search:
                        // Process search command.
                        output = bookingServices.Search(commandString, hotels, bookings);
                        break;
                    default:
                        break;
                }

                // Show the result to the user.
                if(!string.IsNullOrWhiteSpace(output)) Console.WriteLine("\nYour results: " + output);
            }

            Console.WriteLine("\nPress any key to exit");
            
            // Wait for the user to hit any key before exit so they can see the output.
            Console.ReadLine();
        }
    }
}
