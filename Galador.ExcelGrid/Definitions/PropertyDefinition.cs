// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Provides a base class for column and row definitions in a DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Galador.ExcelGrid.Definitions
{
    using System.Collections;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Provides a base class for column and row definitions in a <see cref="DataGrid" />.
    /// </summary>
    public abstract class PropertyDefinition
    {
        // use by grid
        public string FormatString { get; set; }
        public object Header { get; set; }
        public object Tooltip { get; set; }
        public System.Windows.HorizontalAlignment HorizontalAlignment { get; set; }
        public string PropertyName { get; set; }
        public bool CanSort { get; set; } = true;

        // use by factory
        public IValueConverter Converter { get; set; }
        public CultureInfo ConverterCulture { get; set; }
        public object ConverterParameter { get; set; }

        public bool IsEditable { get; set; }
        public bool IsReadOnly { get; set; }
        public IEnumerable ItemsSource { get; set; }
        public string ItemsSourceProperty { get; set; }
        public string SelectedValuePath { get; set; }
        public string DisplayMemberPath { get; set; }
        public int MaxLength { get; set; } = int.MaxValue;
        public string IsEnabledByProperty { get; set; }
        public object IsEnabledByValue { get; set; }
        public object IsEnabledBySource { get; set; }
        public Brush Background { get; set; }
        public string BackgroundProperty { get; set; }
    }
}