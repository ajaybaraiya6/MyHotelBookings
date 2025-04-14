using HotelBookings.BusinessCore.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HotelBookings.BusinessCore.Services
{
    public class BookingServices
    {
        /// <summary>
        /// Will give the available count of rooms by the date range, hotel id and room type.
        /// </summary>
        /// <param name="commandString">Command string with parameters. e.g. Availability(H1, 20240901, SGL)</param>
        /// <param name="hotels">List of hotels data</param>
        /// <param name="bookings">List of booking data</param>
        /// <returns>Message with count of available rooms between date ranges supplied.</returns>
        public string Availability(string commandString, List<HotelModel> hotels, List<BookingModel> bookings)
        {
            Match match = Regex.Match(commandString, Constants.AvailabilityRegexPattern);
            if (match.Success)
            {
                try
                {
                    // Separating the parameters from the input command given.
                    string hotelId = match.Groups[1].Value;
                    string dateRange = match.Groups[2].Value;
                    string roomType = match.Groups[3].Value;

                    // Parsing date/date-range for further processing.
                    string[] dates = dateRange.Split('-');
                    DateTime checkInDt = Common.ParseDateFromString(dates[0], "yyyyMMdd");
                    DateTime checkOutDt = default;
                    // If we have a - separated date range then reading it as check out date.
                    if (dates.Count() > 1)
                    {
                        checkOutDt = Common.ParseDateFromString(dates[1], "yyyyMMdd");
                    }

                    HotelModel? hotelModel = null;
                    if (!ValidateHotel(hotels, hotelId, ref hotelModel))
                    {
                        return "\nNo Hotel found for hotel id: " + hotelId;
                    }

                    List<RoomModel> totalHotelRooms = new List<RoomModel>();
                    if (!ValidateRooms(hotelModel, roomType, ref totalHotelRooms))
                    {
                        return $"\nNo such a room {roomType} exist in hotel: " + hotelId;
                    }

                    // From booking data we want valid data as there might have some invalid data and we want only data associated with hotel id only.
                    bookings = FilterCorrectBookingData(hotelId, roomType, bookings, checkInDt, checkOutDt);

                    int availalbeRoomCount = totalHotelRooms.Count - bookings.Count;
                    return "\nTotal available rooms count: " + Convert.ToString(availalbeRoomCount);
                }
                catch (FormatException ex)
                {
                    return "\nInvalid date format: " + ex;
                }
                catch (Exception ex)
                {
                    return "\nException occured: " + ex;
                }
            }
            else
            {
                return "\nUnable to read the commmand! Try again.";
            }
        }

        /// <summary>
        /// Will give the date range with max available room count between those range passed as parameters.
        /// </summary>
        /// <param name="commandString">String command for seach operation. e.g. Search(H1, 365, SGL)</param>
        /// <param name="hotels">List of hotels data</param>
        /// <param name="bookings">List of booking data</param>
        /// <returns>String with comma separated values of date range and available room counts between those range.</returns>
        public string Search(string commandString, List<HotelModel> hotels, List<BookingModel> bookings)
        {
            Match match = Regex.Match(commandString, Constants.SearchRegexPattern);
            if (match.Success)
            {
                try
                {
                    // Separating the parameters from the input command given.
                    string hotelId = match.Groups[1].Value;
                    string daysAheadStr = match.Groups[2].Value;
                    string roomType = match.Groups[3].Value;

                    int daysAhead;
                    if (!int.TryParse(daysAheadStr, out daysAhead))
                    {
                        return $"\n{daysAheadStr} is not a number. It shold be number.";
                    } else if (string.IsNullOrWhiteSpace(daysAheadStr))
                    {
                        return $"\nDays value is incorrect.";
                    }

                    HotelModel? hotelModel = null;
                    if (!ValidateHotel(hotels, hotelId, ref hotelModel))
                    {
                        return "\nNo Hotel found for hotel id: " + hotelId;
                    }

                    List<RoomModel> totalHotelRooms = new List<RoomModel>();
                    if (!ValidateRooms(hotelModel, roomType, ref totalHotelRooms))
                    {
                        return $"\nNo such a room {roomType} exist in hotel: " + hotelId;
                    }

                    List<string> resultList = new List<string>();
                    DateTime today = DateTime.Today;
                    DateTime? blockStart = null;
                    int? previousAvailability = null;

                    // We filter the bookings with hotel id from command parameter and passing checkin, checkout dates as default values to not consider them in our filter.
                    bookings = FilterCorrectBookingData(hotelId, roomType, bookings, default, default);

                    // We iterate for desire days we want to check availability
                    for (int i = 0; i <= daysAhead; i++)
                    {
                        DateTime currentDate = today.AddDays(i);
                        DateTime nextDate = currentDate.AddDays(1);

                        int currentRoomsAvailable = totalHotelRooms.Count;

                        // Will see if there is any bookings on processing date.
                        foreach (BookingModel booking in bookings)
                        {
                            DateTime arrival = Common.ParseDateFromString(booking.Arrival, "yyyyMMdd");
                            DateTime departure = Common.ParseDateFromString(booking.Departure, "yyyyMMdd");

                            if (currentDate < departure && nextDate > arrival)
                            {
                                // Decrease the count so we can compare it with previous day available room count.
                                currentRoomsAvailable--;
                            }
                        }

                        // When on next day if no rooms available then we need to set end date of our range and start new fresh block.
                        if (currentRoomsAvailable <= 0)
                        {
                            if (blockStart.HasValue && previousAvailability > 0)
                            {
                                resultList.Add($"({blockStart:yyyyMMdd}-{currentDate:yyyyMMdd}, {previousAvailability})");
                                blockStart = null;
                                previousAvailability = null;
                            }
                            continue;
                        }

                        if (blockStart == null)
                        {
                            // When we have fresh new date range block started and we have the room available in that block
                            // but yet we are having maximum available dates for this range so will see for next date again.
                            blockStart = currentDate;
                            previousAvailability = currentRoomsAvailable;
                        }
                        else if (previousAvailability != currentRoomsAvailable)
                        {
                            resultList.Add($"({blockStart:yyyyMMdd}-{currentDate:yyyyMMdd}, {previousAvailability})");
                            blockStart = currentDate;
                            previousAvailability = currentRoomsAvailable;
                        }
                    }

                    // Add final block if still open
                    if (blockStart != null && previousAvailability > 0)
                    {
                        resultList.Add($"({blockStart:yyyyMMdd}-{today.AddDays(daysAhead + 1):yyyyMMdd}, {previousAvailability})");
                    }

                    return resultList.Any() ? string.Join(", ", resultList) : string.Empty;
                }
                catch (FormatException ex)
                {
                    return "\nInvalid date format: " + ex;
                }
                catch (Exception ex)
                {
                    return "\nException occured: " + ex;
                }
            }
            else
            {
                return "\nUnable to read the commmand! Try again.";
            }
        }

        /// <summary>
        /// Basic validation of Hotel data if the hotel exist.
        /// </summary>
        /// <param name="hotels">List of hotels</param>
        /// <param name="hotelId">Hotel id to find in available list of hotels</param>
        /// <param name="hotel">ref output variable which will hold if any hotel found</param>
        /// <returns>True if hotel found, false otherwise.</returns>
        private bool ValidateHotel(List<HotelModel> hotels, string hotelId, ref HotelModel hotel)
        {
            hotel = hotels.FirstOrDefault(h => string.Equals(h.Id, hotelId, StringComparison.OrdinalIgnoreCase));
            return hotel != null;
        }

        /// <summary>
        /// Basic validation of rooms data if the type of room exist in hotel.
        /// </summary>
        /// <param name="hotelModel">Hotel data with its rooms</param>
        /// <param name="roomType">Type of room</param>
        /// <param name="totalHotelRooms">ref output variable which will be filled with filtered rooms having room type and hotel id</param>
        /// <returns>True if rooms found in hotel</returns>
        private bool ValidateRooms(HotelModel hotelModel, string roomType, ref List<RoomModel> totalHotelRooms)
        {
            totalHotelRooms = hotelModel
            .Rooms
            .Where(r => string.Equals(r.RoomType, roomType, StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<RoomModel>();
            return totalHotelRooms.Count > 0;
        }

        /// <summary>
        /// Filters data having correct Date formats for arrival and departure for hotel id and room type.
        /// If checkInDt is not provoided that means it will go for search operation otherwise for availability operation.
        /// </summary>
        /// <param name="hotelId">Hotel Id from bookings</param>
        /// <param name="roomType">Room type from bookings</param>
        /// <param name="bookings">List of booking data</param>
        /// <param name="checkInDt">Begin date to search</param>
        /// <param name="checkOutDt">End date to search</param>
        /// <returns>List of bookings information.</returns>
        private List<BookingModel> FilterCorrectBookingData(string hotelId, string roomType, List<BookingModel> bookings, DateTime checkInDt, DateTime checkOutDt)
        {
            return bookings.Where(b =>
                            !string.IsNullOrWhiteSpace(b.Arrival) &&
                            !string.IsNullOrWhiteSpace(b.Departure) &&
                            string.Equals(b.HotelId, hotelId, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(b.RoomType, roomType, StringComparison.OrdinalIgnoreCase) &&
                            Common.ParseDateFromString(b.Arrival, "yyyyMMdd") is DateTime arrival &&
                            Common.ParseDateFromString(b.Departure, "yyyyMMdd") is DateTime departure &&
                            arrival != default &&
                            departure != default && 
                            (checkInDt == default ||
                            (checkInDt != default && checkOutDt != default && (
                                checkInDt == arrival ||
                                (checkInDt > arrival && checkInDt < departure) ||
                                (checkOutDt != default && checkOutDt > arrival)
                                )
                            )
                            )).ToList();
        }
    }
}
