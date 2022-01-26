using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Galador.ExcelGrid.Definitions
{
    public class DefaultCellContext
    {
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public object BindingSource { get; set; }
        public string BindingPath { get; set; }
        public string FormatString { get; set; }

        public bool IsReadOnly { get; set; }

        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public CultureInfo ConverterCulture { get; set; }

        public object IsEnabledBindingParameter { get; set; }
        public string IsEnabledBindingPath { get; set; }

        public string BackgroundBindingPath { get; set; }
        public object BackgroundBindingSource { get; set; }
    }
    public class TemplateCellContext : DefaultCellContext
    {
        public DataTemplate DisplayTemplate { get; set; }
        public DataTemplate EditTemplate { get; set; }
    }
    public class CheckCellContext : DefaultCellContext
    {
    }
    public class ColorCellContext : DefaultCellContext
    {
    }
    public class SelectorCellContext : DefaultCellContext
    {
        public bool IsEditable { get; set; }
        public IEnumerable ItemsSource { get; set; }
        public string ItemsSourceProperty { get; set; }
        public string SelectedValuePath { get; set; }
        public string DisplayMemberPath { get; set; }
    }
}
