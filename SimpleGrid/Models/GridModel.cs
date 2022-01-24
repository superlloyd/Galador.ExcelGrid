using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrid.Models
{
    public class GridModel
    {
        public GridModel()
        {
            Rows = new RowCollection(this);

            Columns.Add("One");
            Columns.Add("Two");
            Columns.Add("Three");
            for (int i = 0; i < 6; i++)
            {
                var r = Rows.Add();
                r[0] = "One";
                r[1] = "Two";
                r[2] = "Three";
            }
        }

        public ObservableCollection<string> Columns { get; } = new ObservableCollection<string>();
        public RowCollection Rows { get; }

        public string this[int row, int col]
        {
            get => Rows[row][col];
            set => Rows[row][col] = value;
        }
        public Row this[int row] => Rows[row];

        public class RowCollection : IList<Row>, IList, INotifyCollectionChanged
        {
            readonly GridModel grid;
            readonly List<Row> rows = new List<Row>();

            internal RowCollection(GridModel grid)
            {
                this.grid = grid;
            }

            Row IList<Row>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }
            public Row this[int index] => rows[index];

            public int Count => rows.Count;

            bool ICollection<Row>.IsReadOnly => false;

            bool IList.IsFixedSize => true;

            bool IList.IsReadOnly => false;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this;

            object? IList.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException();
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged;

            void ICollection<Row>.Add(Row item) => throw new NotSupportedException();

            public Row Add() => InsertAt(Count);

            public Row InsertAt(int index)
            {
                if (index < 0 || index > rows.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                var row = new Row(grid);
                rows.Insert(index, row);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, row, index));
                return row;
            }

            public void Clear()
            {
                if (rows.Count == 0)
                    return;
                var old = rows.ToArray();
                rows.Clear();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old, 0));
            }

            public bool Contains(Row? item) => IndexOf(item) > -1;

            public void CopyTo(Row[] array, int arrayIndex)
            {
                for (int i = 0; i < Count && i + arrayIndex < array.Length; i++)
                    array[i + arrayIndex] = this[i];
            }

            public IEnumerator<Row> GetEnumerator() => rows.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(Row? item)
            {
                if (item == null)
                    return -1;
                for (int i = 0; i < Count; i++)
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
                if (index < 0 || index >= Count)
                    return;
                var row = this[index];
                rows.RemoveAt(index);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, row, index));
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
                for (int i = 0; i < Count && i + index < array.Length; i++)
                    array.SetValue(this[i], i + index);
            }
        }

        public class Row : IList<string>, IList, INotifyCollectionChanged
        {
            readonly GridModel grid;
            readonly Dictionary<int, string> row = new();

            internal Row(GridModel grid)
            {
                this.grid = grid;
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged;

            public string this[int index]
            {
                get
                {
                    row.TryGetValue(index, out var result);
                    return result ?? "";
                }
                set
                {
                    if (index < 0 || index > Count) // allow for 1 extra column
                        throw new ArgumentOutOfRangeException(nameof(index));

                    var old = this[index];
                    if ((value ?? "") == old)
                        return;
                    row[index] = value ?? "";
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old, index));
                }
            }

            public int Count => grid.Columns.Count;

            bool ICollection<string>.IsReadOnly => false;

            bool IList.IsFixedSize => true;

            bool IList.IsReadOnly => false;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this;

            object? IList.this[int index]
            {
                get => this[index];
                set => this[index] = value?.ToString() ?? "";
            }

            void ICollection<string>.Add(string item) => throw new NotSupportedException();

            public void Clear()
            {
                for (int i = 0; i < Count; i++)
                    this[i] = string.Empty;
            }

            public bool Contains(string item) => IndexOf(item) >= 0;

            public void CopyTo(string[] array, int arrayIndex)
            {
                for (int i = 0; i < Count && i + arrayIndex < array.Length; i++)
                    array[i + arrayIndex] = this[i];
            }

            public IEnumerator<string> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(string item)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (item == this[i])
                        return i;
                }
                return -1;
            }

            void IList<string>.Insert(int index, string item) => throw new NotSupportedException();

            bool ICollection<string>.Remove(string item) => throw new NotSupportedException();

            void IList<string>.RemoveAt(int index) => throw new NotSupportedException();

            int IList.Add(object? value) => throw new NotSupportedException();

            bool IList.Contains(object? value) => value is string s && Contains(s);

            int IList.IndexOf(object? value) => value is string s ? IndexOf(s) : -1;

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
        }
    }
}
