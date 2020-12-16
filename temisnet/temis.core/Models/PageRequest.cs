using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temis.Core.Models
{
    public class PageRequest
    {
        protected internal static readonly int DefaultPageLimit = 3;
        protected internal static readonly List<int> PageLimits = new List<int> { 10, 20, 30, 50, 100 };
        public int Number { get; set; }
        public int Limit;
        public int number { get { return number; } }
        public int limit { get { return limit; } }

        public PageRequest() { }

        public bool ValidPageLimit(int limit)
        {
            return PageLimits.Exists(e => e == limit);
        }

        public static PageRequest Of(Nullable<int> number, Nullable<int> limit)
        {
            int n = number.HasValue ? number.Value : 1;
            int l = limit.HasValue ? limit.Value : DefaultPageLimit;
            return Of(n, l);
        }

        public static PageRequest Of(int number, int limit)
        {
            PageRequest pr = new PageRequest();

            pr.Number = number < 1 ? 1 : number;

            if (pr.ValidPageLimit(limit))
            {
                pr.Limit = limit;
            }
            else
            {
                pr.Limit = DefaultPageLimit;
            }

            return pr;

        }
        public static PageRequest First() => Of(1, DefaultPageLimit);
        public static PageRequest First(Nullable<int> limit) => Of(1, limit);

    }

}