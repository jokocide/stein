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
        /// Defines the available sort methods.
        /// </summary>
        public enum SortMethod
        {
            LatestDate,
            EarliestDate
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
