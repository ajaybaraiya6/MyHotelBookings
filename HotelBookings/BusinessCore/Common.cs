using HotelBookings.BusinessCore.Models;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HotelBookings.BusinessCore
{
    public static class Common
    {
        /// <summary>
        /// Checks if the app run command is supplied with correct arguments or not.
        /// </summary>
        /// <param name="args">Arguments with file names</param>
        /// <param name="hotelFile">ref output variable which will hold hotel file name after validation</param>
        /// <param name="bookingFile">ref output variable which will hold booking file name after validation</param>
        /// <returns>True and false based on arguments.</returns>
        public static bool IsValidUserInput(string[] args, ref string hotelFile, ref string bookingFile)
        {
            if (args.Length == 0) { Console.WriteLine(Constants.NoFileName); Console.ReadLine(); return false; }

            // Parse command-line arguments
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--hotels":
                        hotelFile = args[++i];
                        break;
                    case "--bookings":
                        bookingFile = args[++i];
                        break;
                }
            }

            // Validate if the file name is provided
            if (string.IsNullOrWhiteSpace(hotelFile) || string.IsNullOrWhiteSpace(bookingFile))
            {
                Console.WriteLine("File name not supplied!");
                Console.WriteLine("Example: HotelBookings --hotels hotels.json --bookings bookings.json");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generic method to parse json data from file to our type T model.
        /// </summary>
        /// <typeparam name="T">Generic type T, could be any class type model</typeparam>
        /// <param name="fileName">File name from which we want to read the data</param>
        /// <returns>Generic collection of Type T List</returns>
        private static List<T> ParseJsonFromFile<T>(string fileName)
        {
            return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(fileName), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Checks if the file is a JSON file and data contain by this file is proper json data that we can parse.
        /// </summary>
        /// <param name="fileName">File name from which we want to read the data</param>
        /// <returns>True if file and data are valid otherwise false</returns>
        public static bool IsValidFileAndData(string fileName)
        {
            string errorMessage = string.Empty;
            if (Path.GetExtension(fileName).ToLower() != ".json")
            {
                errorMessage = $"{fileName} is an incorrect file type.";
            }
            else
            {
                try
                {
                    string content = File.ReadAllText(fileName);
                    JsonDocument.Parse(content);
                }
                catch (Exception)
                {
                    errorMessage = $"{fileName} do not contain valid JSON data.";
                }
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Console.WriteLine(errorMessage);
                Console.ReadLine();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Common method to fill the models from JSON file data.
        /// </summary>
        /// <param name="hotelFile">Name of hotel file from which we want to get data</param>
        /// <param name="bookingFile">Name of booking file from which we want to get data</param>
        /// <param name="hotels">ref output variable to hold the parse data as a list</param>
        /// <param name="bookings">ref output variable to hold the parse data as a list</param>
        public static void FillDataListFromFile(string hotelFile, string bookingFile, ref List<HotelModel> hotels, ref List<BookingModel> bookings)
        {
            hotels = ParseJsonFromFile<HotelModel>(hotelFile);
            bookings = ParseJsonFromFile<BookingModel>(bookingFile);
        }
    
        /// <summary>
        /// Common method to parse the date from string.
        /// </summary>
        /// <param name="dateStr">Value of date as a string</param>
        /// <param name="dateFormat">DateTime format of present string</param>
        /// <returns>Converted DateTime value from dateStr parameter or default value.</returns>
        public static DateTime ParseDateFromString(string dateStr, string dateFormat)
        {
            if (DateTime.TryParseExact(dateStr.Trim(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// To decide next operation to perform based on command given by user.
        /// </summary>
        /// <param name="commandString">String command name</param>
        /// <returns>Returns Enum of Operation Command based on passed commandString parameter.</returns>
        public static Enum GetOperationMethodFromUser(out string commandString)
        {
            Enum opMethod = OperationCommands.None;
            commandString = string.Empty;

            Console.WriteLine("\nPlease enter operation command to perform desire operation from valid examples like, " + Constants.ValidInputExamples);

            // Case insensitive regex.
            bool validInput = false;
            while (!validInput)
            {
                string? input = Console.ReadLine();
                if (input != null && !string.IsNullOrWhiteSpace(input))
                {
                    validInput = true;
                    commandString = input.Trim();
                    if (IsRegexMatch(commandString, Constants.AvailabilityRegexPattern))
                    {
                        opMethod = OperationCommands.Availability;
                    }
                    else if (IsRegexMatch(commandString, Constants.SearchRegexPattern))
                    {
                        opMethod = OperationCommands.Search;
                    }
                    else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        opMethod = OperationCommands.None;
                    }
                    else
                    {
                        validInput = false;
                    }
                }

                if (!validInput)
                {
                    validInput = false;
                    commandString = string.Empty;
                    Console.WriteLine("\nInvalid command entered. Check examples below." + Constants.ValidInputExamples);
                }
            }
            return opMethod;
        }

        /// <summary>
        /// Common helper method to match the regex pattern.
        /// </summary>
        /// <param name="commandString">Inout to math with our regex pattern</param>
        /// <param name="pattern">Regex pattern for validating our input</param>
        /// <returns>True if pattern matches false otherwise.</returns>
        private static bool IsRegexMatch(string commandString, string pattern)
        {
            MatchCollection mc = Regex.Matches(commandString, pattern);
            return mc.Count == 1;
        }
    }
}
