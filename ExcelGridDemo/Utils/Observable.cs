﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelGridDemo.Utils
{
    public abstract class Observable : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when the specified property has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// True if the property was set.
        /// </returns>
        /// <remarks>This method uses the CallerMemberNameAttribute to determine the property name.</remarks>
        protected bool SetValue<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            // ReSharper disable once RedundantNameQualifier
            if (object.Equals(field, value))
            {
                return false;
            }

            this.VerifyProperty(propertyName);

            //// this.OnPropertyChanging(propertyName, field, value);
            T oldValue = field;
            field = value;
            this.OnPropertyChanged(propertyName, oldValue, value);
            return true;
        }

        /// <summary>
        /// Verifies the property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            var type = this.GetType();

            // Look for a public property with the specified name.
            var propertyInfo = type.GetTypeInfo().GetDeclaredProperty(propertyName);

            Debug.Assert(propertyInfo != null, string.Format(CultureInfo.InvariantCulture, "{0} is not a property of {1}", propertyName, type.FullName));
        }
    }
}
