// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Globalization;
using System.Linq;

// TODO: Move to the Utility project
namespace Deduplication.Utility
{
    /// <summary>
    /// Provides extension methods for "int" type.
    /// </summary>
    public static class IntExtensions
    {
        private static readonly string[] Units = { "bytes", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Converts input value to appropriate unit of digital information string (bytes, KB, MB, GB etc.).
        /// </summary>
        /// <param name="input">Value that should be converted.</param>
        /// <returns>Appropriate unit of digital information with appropriate suffix.</returns>
        public static string ToDigitalInfoUnit(this int input)
        {
            if (input < 0)
            {
                throw new ArithmeticException("Can't convert input value. The value should not be less that zero.");
            }

            if (input == 0)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} {1}", input, Units.First());
            }

            float workValue = input;

            foreach (var unit in Units)
            {
                if (workValue > 1024)
                {
                    workValue /= 1024;
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", workValue, unit);
                }
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", input, Units.First());
        }
    }
}
