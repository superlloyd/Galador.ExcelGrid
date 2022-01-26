using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galador.Document.Grid
{
    internal class CellColumnsProvider : TypeDescriptionProvider
    {
        static readonly Dictionary<int, CellColumnsPropertyDescriptor> rowDescriptors = new();

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            if (instance is CellGridModel.Row row)
            {
                if (!rowDescriptors.TryGetValue(row.Grid.ColumnCount, out var result))
                {
                    result = new CellColumnsPropertyDescriptor(row.Grid.ColumnCount);
                    rowDescriptors[row.Grid.ColumnCount] = result;
                }
                return result;
            }
            return base.GetExtendedTypeDescriptor(instance);
        }

        private class CellColumnsPropertyDescriptor : CustomTypeDescriptor
        {
            public CellColumnsPropertyDescriptor(int columnCount)
            {
                ColumnCount = columnCount;

                var columns = new PropertyDescriptorCollection(null);
                for (int i = 0; i < columnCount; i++)
                    columns.Add(CellColumnPropertyDescriptor.GetColumn(i));
                Columns = columns;
            }
            public int ColumnCount { get; }
            public PropertyDescriptorCollection Columns { get; }

            public override PropertyDescriptorCollection GetProperties() => Columns;
        }
    }
    public class CellColumnPropertyDescriptor : PropertyDescriptor
    {
        readonly string name;
        private CellColumnPropertyDescriptor(int column)
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

        public static CellColumnPropertyDescriptor GetColumn(int index)
        {
            if (!_columnDescriptors.TryGetValue(index, out var descriptor))
            {
                descriptor = new CellColumnPropertyDescriptor(index);
                _columnDescriptors[index] = descriptor;
            }
            return descriptor;
        }
        static Dictionary<int, CellColumnPropertyDescriptor> _columnDescriptors = new();

        public override string Name => name;
        public override string DisplayName => Name;

        public override Type ComponentType => typeof(CellGridModel.Row);

        public override bool IsReadOnly => true;

        public override Type PropertyType => typeof(Cell);

        public override bool CanResetValue(object component) => false;

        public override object? GetValue(object? component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            var row = (CellGridModel.Row)component;
            return row[Column];
        }

        public override void ResetValue(object component)
            => throw new InvalidOperationException();

        public override void SetValue(object? component, object? value)
            => throw new InvalidOperationException();

        public override bool ShouldSerializeValue(object component)
            => false;
    }
}
