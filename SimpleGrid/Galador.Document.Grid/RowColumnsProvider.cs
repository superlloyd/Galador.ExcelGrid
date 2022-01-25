using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galador.Document.Grid
{
    internal class RowColumnsProvider : TypeDescriptionProvider
    {
        static readonly Dictionary<int, ColumnPropertyDescriptor> rowDescriptors = new();

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            if (instance is ExcelModel.Row row)
            {
                if (!rowDescriptors.TryGetValue(row.Grid.ColumnCount, out var result))
                {
                    result = new ColumnPropertyDescriptor(row.Grid.ColumnCount);
                    rowDescriptors[row.Grid.ColumnCount] = result;
                }
                return result;
            }
            return base.GetExtendedTypeDescriptor(instance);
        }

        private class ColumnPropertyDescriptor : CustomTypeDescriptor
        {
            public ColumnPropertyDescriptor(int columnCount)
            {
                ColumnCount = columnCount;

                var columns = new PropertyDescriptorCollection(null);
                for (int i = 0; i < columnCount; i++)
                    columns.Add(ColumnDescriptor.GetColumn(i));
                Columns = columns;
            }
            public int ColumnCount { get; }
            public PropertyDescriptorCollection Columns { get; }

            public override PropertyDescriptorCollection GetProperties() => Columns;
        }
    }
}
