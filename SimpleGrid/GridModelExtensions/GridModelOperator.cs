using Galador.ExcelGrid;
using Galador.ExcelGrid.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrid.GridModelExtensions
{
    internal class GridModelOperator : ListListOperator
    {
        public GridModelOperator(DataGrid owner) : base(owner)
        {
        }

        public override int InsertItem(int index)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return -1;

            if (this.Owner.ItemsInRows)
            {
                if (index < 0)
                {
                    model.AddRow();
                    return model.Count - 1;
                }
                model.InsertRowAt(index);
                return index;
            }
            else
            {
                if (index < 0)
                {
                    model.Columns.Add("");
                    return model.Columns.Count - 1;
                }
                model.Columns.Insert(index, "");
                return index;
            }
        }

        public override void DeleteRows(int index, int n)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return;

            while (n-- > 0)
                model.RemoveAt(index);
        }
        public override void DeleteColumns(int index, int n)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return;

            while (n-- > 0)
                model.Columns.RemoveAt(index);
        }
        protected override bool DeleteItem(int index)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return false;

            if (this.Owner.ItemsInColumns)
            {
                DeleteColumns(index, 1);
            }
            else
            {
                DeleteRows(index, 1);
            }
            return true;
        }
    }
}
