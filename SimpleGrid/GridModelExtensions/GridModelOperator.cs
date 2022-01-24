using Galador.ExcelGrid;
using Galador.ExcelGrid.Definitions;
using Galador.ExcelGrid.Operators;
using System;
using System.Collections;
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

        public override void AutoGenerateColumns()
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
            {
                base.AutoGenerateColumns();
            }
            else
            {
                this.Owner.ColumnDefinitions.Clear();
                foreach (var cd in this.GenerateColumnDefinitions(model.Columns))
                {
                    this.Owner.ColumnDefinitions.Add(cd);
                    this.Owner.PropertyDefinitions.Add(cd);
                }
            }
        }
        protected override IEnumerable<ColumnDefinition> GenerateColumnDefinitions(IList list)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
            {
                foreach (var column in base.GenerateColumnDefinitions(list))
                    yield return column;
            }
            else
            {
                foreach (var column in model.Columns)
                    yield return new ColumnDefinition
                    {
                        Header = column,
                        HorizontalAlignment = this.DefaultHorizontalAlignment,
                        Width = this.DefaultColumnWidth
                    };
            }
        }
        public override int GetColumnCount()
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return base.GetColumnCount();
            return model.Columns.Count;
        }

        public override int InsertItem(int index)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return -1;

            if (this.Owner.ItemsInRows)
            {
                InsertRows(index, 1);
                if (index < 0)
                    return model.Count - 1;
                return index;
            }
            else
            {
                InsertColumns(index, 1);
                if (index < 0)
                    return model.Columns.Count - 1;
                return index;
            }
        }
        public override void InsertRows(int index, int n)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return;

            if (index < 0)
            {
                while (n-- > 0)
                    model.AddRow();
            }
            else
            {
                while (n-- > 0)
                    model.InsertRowAt(index);
            }
        }
        public override void InsertColumns(int index, int n)
        {
            var model = this.Owner.ItemsSource as GridModel;
            if (model == null)
                return;

            if (index < 0)
            {
                while (n-- > 0)
                    model.Columns.Add("");
            }
            else
            {
                while (n-- > 0)
                    model.Columns.Insert(index, "");
            }

            this.Owner.ColumnHeadersSource = null;
            this.Owner.ColumnHeadersSource = model.Columns;
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
    }
}
