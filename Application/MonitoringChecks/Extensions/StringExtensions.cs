using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Application.MonitoringChecks.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAll(this string str, IEnumerable<string> values, out IEnumerable<string> missingValues)
        {
            if (string.IsNullOrEmpty(str))
            {
                missingValues = values;
                return false;
            }

            missingValues = values.Where(x => !str.Contains(x));

            return (missingValues.Count() == 0);
        }
    }
}
