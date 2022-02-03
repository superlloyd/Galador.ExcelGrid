using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galador.Document.Grid
{
    public class CellModel<T> : CellGridModel
        where T : Cell, new()
    {
        protected override Cell CreateCell()
            => new T();

        public new T this[int row, int col] => (T)base[row, col];
    }

    /// <summary>A model that behave very much like a <seealso cref="List{Cell}"/></summary>
    public class CellGridModel : IList<CellGridModel.Row>, IList, INotifyCollectionChanged, INotifyPropertyChanged, ICloneable, IEquatable<CellGridModel>
    {
        List<Row> rows = new List<Row>();

        /// <summary>Create the <seealso cref="Cell"/> that populate this model</summary>
        protected virtual Cell CreateCell() => new Cell();

        public int ColumnCount
        {
            get => columnCount;
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 26 * 27)
                    value = 26 * 27;
                if (value == ColumnCount)
                    return;
                columnCount = value;
                foreach (var row in rows)
                    row.EnsureSize();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColumnCount)));
            }
        }
        int columnCount = 0;

        public int RowCount
        {
            get => rows.Count;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (value > 64_000)
                    value = 64_000;

                if (value < rows.Count)
                {
                    var oldrows = new Row[rows.Count - value];
                    for (int i = 0; i < oldrows.Length; i++)
                        oldrows[i] = rows[i + value];
                    rows.RemoveRange(value, oldrows.Length);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldrows, value));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowCount)));
                }
                else if (value > rows.Count)
                {
                    var newrows = new Row[value - rows.Count];
                    for (int i = 0; i < newrows.Length; i++)
                        newrows[i] = new Row(this);
                    rows.AddRange(newrows);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newrows, rows.Count - newrows.Length));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowCount)));
                }
            }
        }
        int ICollection<Row>.Count => RowCount;
        int ICollection.Count => RowCount;

        public Cell this[int row, int col] => rows[row][col];

        Row IList<Row>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }
        public Row this[int index] => rows[index];

        bool ICollection<Row>.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        void ICollection<Row>.Add(Row item) => throw new NotSupportedException();

        public Row AddRow() => InsertRowAt(RowCount);

        public Row InsertRowAt(int index)
        {
            if (index < 0 || index > rows.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var row = new Row(this);
            rows.Insert(index, row);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object?)row, index));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowCount)));
            return row;
        }

        public void Clear()
        {
            if (rows.Count == 0)
                return;
            var old = rows.ToArray();
            rows.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,old, 0));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowCount)));
        }

        public bool Contains(Row? item) => IndexOf(item) > -1;

        public void CopyTo(Row[] array, int arrayIndex)
        {
            for (int i = 0; i < RowCount && i + arrayIndex < array.Length; i++)
                array[i + arrayIndex] = this[i];
        }

        public IEnumerator<Row> GetEnumerator() => rows.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(Row? item)
        {
            if (item == null)
                return -1;
            for (int i = 0; i < RowCount; i++)
            {
                if (item == this[i])
                    return i;
            }
            return -1;
        }

        void IList<Row>.Insert(int index, Row item) => throw new NotSupportedException();

        public bool Remove(Row? item)
        {
            var index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= RowCount)
                return;
            var row = this[index];
            rows.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object?)row, index));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RowCount)));
        }

        int IList.Add(object? value) => throw new NotSupportedException();

        bool IList.Contains(object? value) => value is Row row && rows.Contains(row);

        int IList.IndexOf(object? value) => value is Row row ? rows.IndexOf(row) : -1;

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        void IList.Remove(object? value) => Remove(value as Row);

        void ICollection.CopyTo(Array? array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                throw new ArgumentException("Array Rank should be 1");
            for (int i = 0; i < RowCount && i + index < array.Length; i++)
                array.SetValue(this[i], i + index);
        }

        public void InsertColumns(int index, int count)
        {
            if (index < 0 || index > ColumnCount || count < 0)
                throw new ArgumentOutOfRangeException("index");
            if (count == 0)
                return;

            ColumnCount += count;
            if (index + count == ColumnCount)
                return;

            foreach (var row in this)
                row.InsertColumns(index, count);
        }
        public void DeleteColumns(int index, int count)
        {
            if (index < 0 || index >= ColumnCount || count < 0 || index + count > ColumnCount || ColumnCount - count < 1)
                throw new ArgumentOutOfRangeException("index");
            if (count == 0)
                return;

            ColumnCount -= count;
            foreach (var row in this)
                row.DeleteColumns(index, count);
        }

        object ICloneable.Clone() => Clone();
        public virtual CellGridModel Clone()
        {
            var result = (CellGridModel)base.MemberwiseClone();
            result.PropertyChanged = null;
            result.CollectionChanged = null;
            result.rows = new List<Row>(this.rows.Capacity);
            foreach (var item in this.rows)
                result.rows.Add(item.Clone(result));
            return result;
        }

        public virtual bool Equals(CellGridModel? model)
        {
            if (model is null)
                return false;
            if (model.RowCount != RowCount || model.ColumnCount != ColumnCount)
                return false;
            for (int i = 0; i < RowCount; i++)
                if (!Equals(this[i], model[i]))
                    return false;
            return true;
        }
        public override bool Equals(object? obj) => Equals(obj as CellGridModel);
        public override int GetHashCode()
        {
            int result = 0;
            foreach (var row in this)
                result ^= row.GetHashCode();
            return result;
        }


        [TypeDescriptionProvider(typeof(CellColumnsProvider))]
        public class Row : IList<Cell>, IList, INotifyPropertyChanged
        {
            readonly CellGridModel grid;
            Cell[]? row;

            internal Row(CellGridModel grid)
            {
                this.grid = grid;
            }
            public CellGridModel Grid => grid;

            internal Row Clone(CellGridModel owner)
            {
                var result = new Row(owner);
                if (row != null)
                {
                    result.row = new Cell[row.Length];
                    for (int i = 0; i < row.Length; i++)
                    {
                        var cell = row[i];
                        if (cell != null)
                            result.row[i] = cell.Clone();
                    }
                }
                return result;
            }
            public override bool Equals(object? obj)
            {
                if (!(obj is Row orow))
                    return false;
                if (this.row == null || orow.row == null)
                    return this.row == orow.row;
                if (row.Length != orow.row.Length)
                    return false;
                for (int i = 0; i < row.Length; i++)
                    if (!Equals(row[i], orow.row[i]))
                        return false;
                return true;
            }
            public override int GetHashCode()
            {
                int result = 0;
                if (row != null)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        var cell = row[i];
                        if (cell == null)
                            continue;
                        result ^= cell.GetHashCode();
                    }
                }
                return result;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            Cell IList<Cell>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            public Cell this[int index] => Get(index, true)!;
            internal void Set(int index, Cell? value)
            {
                if (row is null)
                {
                    if (value is null)
                        return;
                    row = new Cell[Grid.ColumnCount];
                }
                row[index] = value!; 
            }
            internal Cell? Get(int index, bool instantiate)
            {
                if (row is null || row[index] is null)
                {
                    if (instantiate)
                    {
                        if (row == null)
                            row = new Cell[Grid.ColumnCount];
                        var result = Grid.CreateCell() ?? throw new InvalidOperationException("Can't instantiate cell");
                        row[index] = result;
                        return result;
                    }
                    return null;
                }
                return row[index];
            }

            public int Count => grid.ColumnCount;

            bool ICollection<Cell>.IsReadOnly => false;

            bool IList.IsFixedSize => true;

            bool IList.IsReadOnly => false;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this;

            object? IList.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }

            void ICollection<Cell>.Add(Cell? item) => throw new NotSupportedException();

            public void Clear()
            {
                for (int i = 0; i < Count; i++)
                    Set(i, null);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }

            public bool Contains(Cell? item) => IndexOf(item) >= 0;

            public void CopyTo(Cell[] array, int arrayIndex)
            {
                for (int i = 0; i < Count && i + arrayIndex < array.Length; i++)
                    array[i + arrayIndex] = this[i];
            }

            public IEnumerator<Cell> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(Cell? item)
            {
                if (item is null)
                    return -1;
                for (int i = 0; i < Count; i++)
                {
                    if (item == this[i])
                        return i;
                }
                return -1;
            }

            void IList<Cell>.Insert(int index, Cell item) => throw new NotSupportedException();

            bool ICollection<Cell>.Remove(Cell item) => throw new NotSupportedException();

            void IList<Cell>.RemoveAt(int index) => throw new NotSupportedException();

            int IList.Add(object? value) => throw new NotSupportedException();

            bool IList.Contains(object? value) => value is Cell s && Contains(s);

            int IList.IndexOf(object? value) => value is Cell s ? IndexOf(s) : -1;

            void IList.Insert(int index, object? value) => throw new NotSupportedException();

            void IList.Remove(object? value) => throw new NotSupportedException();

            void IList.RemoveAt(int index) => throw new NotSupportedException();

            void ICollection.CopyTo(Array array, int index)
            {
                if (array.Rank != 1)
                    throw new ArgumentException("Array Rank should be 1");
                for (int i = 0; i < Count && i + index < array.Length; i++)
                    array.SetValue(this[i], i + index);
            }
            internal void InsertColumns(int index, int count)
            {
                if (row is null)
                    return;

                var newrow = new Cell[Grid.ColumnCount];
                for (int i = 0; i < newrow.Length; i++)
                {
                    if (i < index)
                    {
                        newrow[i] = row[i];
                    }
                    else if (i < index + count)
                    {
                        // nothing
                    }
                    else
                    {
                        newrow[i] = row[i - count];
                    }
                }
                row = newrow;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
            internal void DeleteColumns(int index, int count)
            {
                if (row is null)
                    return;

                var newrow = new Cell[Grid.ColumnCount];
                for (int i = 0; i < newrow.Length; i++)
                {
                    if (i < index)
                    {
                        newrow[i] = row[i];
                    }
                    else
                    {
                        newrow[i] = row[i + count];
                    }
                }
                row = newrow;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
            internal void EnsureSize()
            {
                if (row is null)
                    return;
                var newrow = new Cell[Grid.ColumnCount];
                for (int i = 0; i < newrow.Length && i < row.Length; i++)
                    newrow[i] = row[i];
                row = newrow;
            }
        }
    }
}
