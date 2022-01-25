using Galador.ExcelGrid;
using Galador.ExcelGrid.CellDefinitions;
using Galador.ExcelGrid.Comparers;
using Galador.ExcelGrid.Definitions;
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

        ExcelModel? Model => this.Owner.ItemsSource as ExcelModel;

        public int GetColumnCount() => Model?.ColumnCount ?? 0;
        public int GetRowCount() => Model?.RowCount ?? 0;
        public string GetBindingPath(CellRef cell) => $"[{cell.Column}]"; // TODO: try that in INotifyPropertyChanged
        public object? GetDataContext(CellRef cell)
        {
            var model = Model;
            if (model == null || cell.Row < 0 || cell.Row >= model.RowCount)
                return null;
            var rowIndex = GetItemsSourceIndex(cell.Row);
            return model[rowIndex];
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

            var rowIndex = GetItemsSourceIndex(cell.Row);
            model[rowIndex, cell.Column] = (string)(value ?? "");
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

            var rowIndex = GetItemsSourceIndex(cell.Row);
            return model[rowIndex, cell.Column];
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

        /// <summary>
        /// Converts the collection view index to an items source index.
        /// </summary>
        /// <param name="index">The index in the collection view.</param>
        /// <returns>The index in the items source</returns>
        public int GetItemsSourceIndex(int index)
        {
            var collectionView = this.Owner.CollectionView;
            if (collectionView == null)
            {
                // no collection view, just return the index
                return index;
            }

            // if not using custom sort, and not sorting
            if (this.Owner.CustomSort == null && collectionView.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            // if using custom sort, and not sorting
            if (this.Owner.CustomSort is ISortDescriptionComparer sdc && sdc.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            if (collectionView.IsEmpty)
            {
                // cannot find this in the collection view
                return -1;
            }

            if (index < 0)
                throw new InvalidOperationException("The collection view is probably out of sync. (GetItemsSourceIndex)");

            // get the item at the specified index in the collection view
            // TODO: find a better way to do this
            var counter = 0;
            foreach (var item in this.Owner.CollectionView)
            {
                if (counter++ == index)
                {
                    return Model?.IndexOf((ExcelModel.Row)item) ?? -1;
                }
            }
            throw new InvalidOperationException("The collection view is probably out of sync. (GetItemsSourceIndex)");
        }

        /// <summary>
        /// Converts the items source index to a collection view index.
        /// </summary>
        /// <param name="index">The index in the items source.</param>
        /// <returns>The index in the collection view</returns>
        public int GetCollectionViewIndex(int index)
        {
            if (this.Owner.CollectionView == null)
            {
                return index;
            }

            // if not using custom sort, and not sorting
            if (this.Owner.CustomSort == null && this.Owner.CollectionView.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            // if using custom sort, and not sorting
            if (this.Owner.CustomSort is ISortDescriptionComparer sdc && sdc.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            if (index < 0 || index >= this.Owner.ItemsSource.Count)
            {
                throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
            }

            // get the item at the specified index in the items source
            var item = this.Owner.ItemsSource[index];
            int result = 0;
            foreach (var item2 in this.Owner.CollectionView)
            {
                if (item == item2)
                    return result;
                result++;
            }
            throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
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
                HorizontalAlignment = model?.Alignments[index] ?? HorizontalAlignment.Left,
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

            var rowIndex = GetItemsSourceIndex(index);
            if (rowIndex < 0)
            {
                while (n-- > 0)
                    model.AddRow();
            }
            else
            {
                while (n-- > 0)
                    model.InsertRowAt(rowIndex);
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

            var rowIndex = GetItemsSourceIndex(index);
            while (n-- > 0)
                model.RemoveAt(rowIndex);
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
