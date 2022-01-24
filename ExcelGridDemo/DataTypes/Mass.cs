﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mass.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExcelGridDemo
{
    using System;
    using System.Text.RegularExpressions;

    public struct Mass : IComparable<Mass>, IComparable
    {
        public static Mass Kilogram = new Mass(1);
        public static Mass Gram = new Mass(1e-3);
        public static Mass Tonne = new Mass(1000);
        public static Mass Pound = new Mass(0.45359237);

        private static readonly Regex ParseExpression = new Regex(@"^\s*(?<value>[\d\.\,]+)*\s*(?<unit>.*)\s*$");

        private readonly double value;

        public Mass(double value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return this.value + " kg";
        }

        public int CompareTo(Mass that)
        {
            return this.value.CompareTo(that.value);
        }

        public int CompareTo(object obj)
        {
            if (obj is Mass)
            {
                return this.CompareTo((Mass)obj);
            }

            throw new ArgumentException("Object is not of type Mass");
        }

        public static Mass operator +(Mass x, Mass y)
        {
            return new Mass(x.value + y.value);
        }

        public static Mass operator -(Mass x, Mass y)
        {
            return new Mass(x.value - y.value);
        }

        public static Mass operator *(double x, Mass y)
        {
            return new Mass(x * y.value);
        }

        public static Mass operator *(Mass x, double y)
        {
            return new Mass(x.value * y);
        }

        public static Mass operator /(Mass x, double y)
        {
            return new Mass(x.value / y);
        }

        public static Mass Parse(string s, IFormatProvider formatProvider)
        {
            var m = ParseExpression.Match(s);
            if (!m.Success)
            {
                throw new FormatException();
            }

            var value = double.Parse(m.Groups["value"].Value, formatProvider);
            var unit = m.Groups["unit"].Value;
            switch (unit)
            {
                case "tonne":
                case "t":
                    return value * Tonne;
                case "lb":
                    return value * Pound;
                case "g":
                    return value * Gram;
                case "kg":
                case "":
                    return value * Kilogram;
                default:
                    throw new FormatException();
            }
        }
    }
}
