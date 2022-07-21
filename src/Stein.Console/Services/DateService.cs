using Stein.Models;
using System;

namespace Stein.Services
{
    /// <summary>
    /// Helper methods for interacting with DateTime objects and strings that 
    /// represent dates in ISO8601 format.
    /// </summary>
    public static class DateService
    {
        /// <summary>
        /// Sort the collection using the requested method.
        /// </summary>
        public static void Sort(Collection collection, SortMethod method)
        {
            if (method is SortMethod.LatestDate)
                collection.Items.Sort(LatestDateComparison);
            else if (method is SortMethod.EarliestDate)
                collection.Items.Sort(EarliestDateComparison);
        }

        /// <summary>
        /// Rearrange a string that contains a date based on user preference.
        /// </summary>
        /// <param name="date">The current date string from Item.</param>
        public static string Format(string date)
        {
            DateTime dateTime;
            
            try
            {
                dateTime = DateTime.Parse(date);
            }
            catch (ArgumentNullException)
            {
                return null;
            }

            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;

            string dateConfig = ConfigurationService.Config.DateFormat;

            int yearStart = dateConfig.IndexOf("year");
            string yearResult = dateConfig.Remove(yearStart, 4).Insert(yearStart, dateTime.Year.ToString());

            int monthStart = yearResult.IndexOf("month");
            string monthResult = yearResult.Remove(monthStart, 5).Insert(monthStart, dateTime.Month.ToString());

            int dayStart = monthResult.IndexOf("day");
            string dayResult = monthResult.Remove(dayStart, 3).Insert(dayStart, dateTime.Day.ToString());

            return dayResult;
        }

        /// <summary>
        /// Defines the available sort methods.
        /// </summary>
        public enum SortMethod
        {
            LatestDate,
            EarliestDate
        }

        /// <summary>
        /// Defines the available date formats.
        /// </summary>
        public enum FormatModifier
        {
            Alpha
        }

        private static int LatestDateComparison(Item x, Item y)
        {
            Item itemX = x as Item;
            Item itemY = y as Item;

            if (itemX.Date == null && itemY.Date == null) return 0;

            if (itemX.Date == null) return 1;
            if (itemY.Date == null) return -1;

            DateTime parsedX = DateTime.Parse(itemX.Date);
            DateTime parsedY = DateTime.Parse(itemY.Date);

            return DateTime.Compare(parsedY, parsedX);
        }

        private static int EarliestDateComparison(Item x, Item y)
        {
            Item itemX = x as Item;
            Item itemY = y as Item;

            if (itemX.Date == null && itemY.Date == null) return 0;

            if (itemX.Date == null) return 1;
            if (itemY.Date == null) return -1;

            DateTime parsedX = DateTime.Parse(itemX.Date);
            DateTime parsedY = DateTime.Parse(itemY.Date);

            return DateTime.Compare(parsedX, parsedY);
        }
    }
}
