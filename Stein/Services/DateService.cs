using Stein.Models;
using System;

namespace Stein.Services
{
    public static class DateService
    {
        public enum SortMethod
        {
            LatestDate,
            EarliestDate
        }

        public static void Sort(Collection collection, SortMethod method)
        {
            // The collections are ISerializable
            //List<Item> items = (List<Item>)collection.Items.Cast<Item>().ToList();

            if (method is SortMethod.LatestDate)
            {
                collection.Items.Sort(LatestDateComparison);
            } 
            else if (method is SortMethod.EarliestDate)
            {
                collection.Items.Sort(EarliestDateComparison);
            }
        }

        public static void LatestDateSort(Collection collection)
        {
            //collection.Items.Sort((a, b) =>
            //{
            //    Item item = 
            //    if (a.Date == null && b.Date == null) return 0;
            //    else if (a.Date == null) return 1;
            //    else if (b.Date == null) return -1;

            //    DateTime parsedA = DateTime.Parse(a.Date);
            //    DateTime parsedB = DateTime.Parse(b.Date);

            //    return DateTime.Compare(parsedB, parsedA);
            //});
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
