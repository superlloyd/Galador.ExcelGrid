using Galador.ExcelGrid;
using Galador.ExcelGrid.Comparers;
using Galador.ExcelGrid.Definitions;
using Galador.ExcelGrid.Helpers;
using Galador.ExcelGrid.Operators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galador.WPF.ExcelGrid
{
    internal class ExcelModelOperator : IDataGridOperator
    {
        public ExcelModelOperator(DataGrid owner)
        {
            Owner = owner;
        }
        public DataGrid Owner { get; }

        public GridLength DefaultColumnWidth { get; set; } = new GridLength(1, GridUnitType.Star);
        public HorizontalAlignment DefaultHorizontalAlignment { get; set; } = HorizontalAlignment.Left;

        ExcelModel? Model => this.Owner.ItemsSource as ExcelModel;

        public int GetColumnCount() => Model?.ColumnCount ?? 0;
        public int GetRowCount() => Model?.RowCount ?? 0;
        public string GetBindingPath(CellRef cell) => $"[{cell.Column}]";
        public object? GetDataContext(CellRef cell)
        {
            var model = Model;
            if (model == null || cell.Row < 0 || cell.Row >= model.RowCount)
                return null;
            return model[Owner.FindSourceIndex(cell.Row)];
        }

        public bool TrySetCellValue(CellRef cell, object value)
        {
            if (value is not null && value is not string)
                return false;

            var model = Model;
            if (model == null)
                return false;

            if (cell.Row < 0 || cell.Row >= model.RowCount)
                return false;
            if (cell.Column < 0 || cell.Column >= model.RowCount)
                return false;

            model[Owner.FindSourceIndex(cell.Row), cell.Column] = (string)(value ?? "");
            return true;
        }
        public object? GetCellValue(CellRef cell)
        {
            var model = Model;
            if (model == null)
                return null;

            if (cell.Row < 0 || cell.Row >= model.RowCount)
                return null;
            if (cell.Column < 0 || cell.Column >= model.RowCount)
                return null;

            return model[Owner.FindSourceIndex(cell.Row), cell.Column];
        }

        public Type GetPropertyType(CellRef cell) => typeof(string);
        public object? GetItem(CellRef cell) => GetCellValue(cell);

        public void UpdatePropertyDefinitions()
        {
            var model = Model;
            if (model == null)
                return;

            var collection = this.Owner.ColumnDefinitions;
            while (collection.Count < model.ColumnCount)
                collection.Add(CreateColumnAt(collection.Count));
            while (collection.Count > model.ColumnCount)
                collection.RemoveAt(collection.Count - 1);
        }

        public CellDescriptor CreateCellDescriptor(CellRef cell)
        {
            var d = new CellDescriptor
            {
                PropertyDefinition = this.Owner.ColumnDefinitions[cell.Column],
                Item = this.GetItem(cell),
                Descriptor = ColumnDescriptor.GetColumn(cell.Column),
                PropertyType = typeof(string),
                BindingPath = this.GetBindingPath(cell),
                BindingSource = this.GetDataContext(cell)
            };
            return d;
        }

        public bool CanSort(int index) => true;

        public void AutoGenerateColumns()
        {
            var model = Model;
            if (model == null)
                return;

            this.Owner.ColumnDefinitions.Clear();
            for (int i = 0; i < model.ColumnCount; i++)
                this.Owner.ColumnDefinitions.Add(CreateColumnAt(i));
        }
        private ColumnDefinition CreateColumnAt(int index)
        {
            var model = Model;
            var header = ColumnDescriptor.GetColumnHeader(index);
            var cd = new ColumnDefinition
            {
                Header = header,
                PropertyName = header,
                HorizontalAlignment = DefaultHorizontalAlignment,
                Width = this.DefaultColumnWidth
            };
            return cd;
        }

        public int InsertItem(int index)
        {
            var model = Model;
            if (model == null)
                return -1;

            if (this.Owner.ItemsInRows)
            {
                InsertRows(index, 1);
                if (index < 0)
                    return model.RowCount - 1;
                return index;
            }
            else
            {
                InsertColumns(index, 1);
                if (index < 0)
                    return model.ColumnCount - 1;
                return index;
            }
        }
        public bool CanInsertRows() => true;
        public void InsertRows(int index, int n)
        {
            var model = Model;
            if (model == null)
                return;

            if (index < 0)
            {
                while (n-- > 0)
                    model.AddRow();
            }
            else
            {
                index = Owner.FindSourceIndex(index);
                while (n-- > 0)
                    model.InsertRowAt(index);
            }
        }
        public bool CanInsertColumns() => true;
        public void InsertColumns(int index, int n)
        {
            var model = Model;
            if (model == null)
                return;

            model.InsertColumns(index, n);
        }

        protected bool DeleteItem(int index)
        {
            var model = Model;
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
        public bool CanDeleteRows() => true;
        public void DeleteRows(int index, int n)
        {
            var model = Model;
            if (model == null)
                return;

            for (int i = n - 1; i >= 0; i--)
            {
                var j = Owner.FindSourceIndex(index + i);
                model.RemoveAt(j);
            }
        }
        public bool CanDeleteColumns() => true;
        public void DeleteColumns(int index, int n)
        {
            var model = Model;
            if (model == null)
                return;

            model.DeleteColumns(index, n);
        }
    }
}
