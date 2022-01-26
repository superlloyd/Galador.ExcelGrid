using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galador.Document.Grid
{
    internal class StringColumnsProvider : TypeDescriptionProvider
    {
        static readonly Dictionary<int, StringColumnsPropertyDescriptor> rowDescriptors = new();

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            if (instance is StringGridModel.Row row)
            {
                if (!rowDescriptors.TryGetValue(row.Grid.ColumnCount, out var result))
                {
                    result = new StringColumnsPropertyDescriptor(row.Grid.ColumnCount);
                    rowDescriptors[row.Grid.ColumnCount] = result;
                }
                return result;
            }
            return base.GetExtendedTypeDescriptor(instance);
        }

        private class StringColumnsPropertyDescriptor : CustomTypeDescriptor
        {
            public StringColumnsPropertyDescriptor(int columnCount)
            {
                ColumnCount = columnCount;

                var columns = new PropertyDescriptorCollection(null);
                for (int i = 0; i < columnCount; i++)
                    columns.Add(StringColumnPropertyDescriptor.GetColumn(i));
                Columns = columns;
            }
            public int ColumnCount { get; }
            public PropertyDescriptorCollection Columns { get; }

            public override PropertyDescriptorCollection GetProperties() => Columns;
        }
    }
    internal class StringColumnPropertyDescriptor : PropertyDescriptor
    {
        readonly string name;
        private StringColumnPropertyDescriptor(int column)
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

        public static StringColumnPropertyDescriptor GetColumn(int index)
        {
            if (!_columnDescriptors.TryGetValue(index, out var descriptor))
            {
                descriptor = new StringColumnPropertyDescriptor(index);
                _columnDescriptors[index] = descriptor;
            }
            return descriptor;
        }
        static Dictionary<int, StringColumnPropertyDescriptor> _columnDescriptors = new();

        public override string Name => name;
        public override string DisplayName => Name;

        public override Type ComponentType => typeof(StringGridModel.Row);

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(string);

        public override bool CanResetValue(object component) => true;

        public override object? GetValue(object? component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (StringGridModel.Row)component;
            return row[Column];
        }

        public override void ResetValue(object component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (StringGridModel.Row)component;
            row[Column] = "";
        }

        public override void SetValue(object? component, object? value)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (StringGridModel.Row)component;
            row[Column] = (string)(value ?? "");
        }

        public override bool ShouldSerializeValue(object component)
        {
            var value = (string?)GetValue(component);
            return !string.IsNullOrEmpty(value);
        }
    }
}
