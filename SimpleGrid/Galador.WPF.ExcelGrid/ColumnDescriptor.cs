using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galador.WPF.ExcelGrid
{
    internal class ColumnDescriptor : PropertyDescriptor
    {
        readonly string name;
        private ColumnDescriptor(int column)
            : base(GetColumnHeader(column), Array.Empty<Attribute>())
        {
            Column = column;
            name = GetColumnHeader(column);
        }
        public static string GetColumnHeader(int column)
        {
            var i1 = column % 26;
            var i2 = column / 26;
            return i2 == 0
                ? $"{(char)('A' + i1)}"
                : $"{(char)('A' + (i2 - 1))}{(char)('A' + i1)}";
        }
        public int Column { get; }

        public static ColumnDescriptor GetColumn(int index)
        {
            if (!_columnDescriptors.TryGetValue(index, out var descriptor))
            {
                descriptor = new ColumnDescriptor(index);
                _columnDescriptors[index] = descriptor;
            }
            return descriptor;
        }
        static Dictionary<int, ColumnDescriptor> _columnDescriptors = new();

        public override string Name => name;
        public override string DisplayName => Name;

        public override Type ComponentType => typeof(ExcelModel.Row);

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(string);

        public override bool CanResetValue(object component) => true;

        public override object? GetValue(object? component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (ExcelModel.Row)component;
            return row[Column];
        }

        public override void ResetValue(object component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (ExcelModel.Row)component;
            row[Column] = "";
        }

        public override void SetValue(object? component, object? value)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (ExcelModel.Row)component;
            row[Column] = (string)(value ?? "");
        }

        public override bool ShouldSerializeValue(object component)
        {
            var value = (string?)GetValue(component);
            return !string.IsNullOrEmpty(value);
        }
    }
}
