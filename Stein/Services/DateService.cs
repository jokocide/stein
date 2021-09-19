using Stein.Models;
using System;

namespace Stein.Services
{
    public static class DateService
    {
        public static void LatestDateSort(Collection collection)
        {
            collection.Items.Sort((a, b) =>
            {
                if (a.Date == null && b.Date == null) return 0;
                else if (a.Date == null) return 1;
                else if (b.Date == null) return -1;

                DateTime parsedA = DateTime.Parse(a.Date);
                DateTime parsedB = DateTime.Parse(b.Date);

                return DateTime.Compare(parsedB, parsedA);
            });
        }
    }
}
