﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionMath.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Addition, subtraction and multiplication for all kinds of objects (by reflection to invoke the operators).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Galador.ExcelGrid.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// Addition, subtraction and multiplication for all kinds of objects (by reflection to invoke the operators).
    /// </summary>
    public static class ReflectionMath
    {
        /// <summary>
        /// Tries to parse the specified string.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="s">The arguments.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if parsing successful, <c>false</c> otherwise.
        /// </returns>
        public static bool TryParse(Type type, string s, IFormatProvider provider, out object result)
        {
            try
            {
                var t1 = typeof(string);
                var t2 = provider.GetType();
                var mi =
                    type.GetMethods().FirstOrDefault(m =>
                    {
                        var p = m.GetParameters();
                        return m.Name == "Parse" && p.Length == 2 && p[0].ParameterType.IsAssignableFrom(t1) && p[1].ParameterType.IsAssignableFrom(t2);
                    });
                if (mi == null)
                {
                    result = null;
                    return false;
                }

                result = mi.Invoke(null, parameters: new object[] { s, provider });
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Performs addition with the op_Addition operator. A return value indicates whether the addition succeeded or failed.
        /// </summary>
        /// <param name="o1">The first object.</param>
        /// <param name="o2">The second object.</param>
        /// <param name="result">The sum.</param>
        /// <returns>
        /// True if the addition succeeded.
        /// </returns>
        public static bool TryAdd(object o1, object o2, out object result)
        {
            if (o1 is double && o2 is double)
            {
                result = (double)o1 + (double)o2;
                return true;
            }

            if (o1 is double && o2 is int)
            {
                result = (double)o1 + (int)o2;
                return true;
            }

            if (o1 is int && o2 is double)
            {
                result = (int)o1 + (double)o2;
                return true;
            }

            if (o1 is int && o2 is int)
            {
                result = (int)o1 + (int)o2;
                return true;
            }

            return TryInvoke("op_Addition", o1, o2, out result);
        }

        /// <summary>
        /// Performs multiplication with the op_Multiplication operator. A return value indicates whether the addition succeeded or failed.
        /// </summary>
        /// <param name="o1">The first object.</param>
        /// <param name="o2">The second object.</param>
        /// <param name="result">The product.</param>
        /// <returns>
        /// True if the multiplication succeeded.
        /// </returns>
        public static bool TryMultiply(object o1, object o2, out object result)
        {
            if (o1 is double && o2 is double)
            {
                result = (double)o1 * (double)o2;
                return true;
            }

            if (o1 is int && o2 is int)
            {
                result = (int)o1 * (int)o2;
                return true;
            }

            if (o1 is int && o2 is double)
            {
                result = (int)o1 * (double)o2;
                return true;
            }

            if (o1 is double && o2 is int)
            {
                result = (double)o1 * (int)o2;
                return true;
            }

            // Implementation of the multiply operator for TimeSpan
            if (o1 is TimeSpan && o2 is double)
            {
                double seconds = ((TimeSpan)o1).TotalSeconds * (double)o2;
                result = TimeSpan.FromSeconds(seconds);
                return true;
            }

            return TryInvoke("op_Multiply", o1, o2, out result);
        }

        /// <summary>
        /// Performs subtraction with the op_Subtraction operator. A return value indicates whether the addition succeeded or failed.
        /// </summary>
        /// <param name="o1">The first object.</param>
        /// <param name="o2">The second object.</param>
        /// <param name="result">The difference.</param>
        /// <returns>
        /// True if the subtraction succeeded.
        /// </returns>
        public static bool TrySubtract(object o1, object o2, out object result)
        {
            if (o1 is double && o2 is double)
            {
                result = (double)o1 - (double)o2;
                return true;
            }

            if (o1 is int && o2 is int)
            {
                result = (int)o1 - (int)o2;
                return true;
            }

            return TryInvoke("op_Subtraction", o1, o2, out result);
        }

        /// <summary>
        /// Tries to invoke invoke the specified method.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="o1">The o 1.</param>
        /// <param name="o2">The o 2.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The try invoke.
        /// </returns>
        private static bool TryInvoke(string methodName, object o1, object o2, out object result)
        {
            try
            {
                var t1 = o1.GetType();
                var t2 = o2.GetType();
                var mi =
                    t1.GetMethods().FirstOrDefault(
                        m => m.Name == methodName && m.GetParameters()[1].ParameterType.IsAssignableFrom(t2));
                if (mi == null)
                {
                    result = null;
                    return false;
                }

                result = mi.Invoke(null, new[] { o1, o2 });
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}