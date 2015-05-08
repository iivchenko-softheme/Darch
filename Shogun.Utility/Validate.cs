// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Shogun.Utility
{
    /// <summary>
    /// Provides parameters/arguments validation functionality.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Throws if input is Null.
        /// </summary>
        /// <param name="input">Object that is under validation.</param>
        /// <param name="parameterName">Name of the <see cref="input"/> variable/parameter. Can't be NULL.</param>
        /// <exception cref="ArgumentNullException"/>
        public static void Null(object input, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            if (input == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
