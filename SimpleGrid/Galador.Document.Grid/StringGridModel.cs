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
    public partial class StringGridModel : IList<StringGridModel.Row>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly List<Row> rows = new List<Row>();

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

        public int RowCount => rows.Count;
        int ICollection<Row>.Count => RowCount;
        int ICollection.Count => RowCount;

        public string this[int row, int col]
        {
            get => rows[row][col];
            set => rows[row][col] = value;
        }

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

        [TypeDescriptionProvider(typeof(StringColumnsProvider))]
        public class Row : IList<string>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        {
            readonly StringGridModel grid;
            string[]? row;

            internal Row(StringGridModel grid)
            {
                this.grid = grid;
            }
            public StringGridModel Grid => grid;

            public event NotifyCollectionChangedEventHandler? CollectionChanged;
            public event PropertyChangedEventHandler? PropertyChanged;

            public string this[int index]
            {
                get => Get(index);
                set
                {
                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    var old = this[index];
                    if ((value ?? "") == old)
                        return;

                    Set(index, value);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old, index));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
            internal void Set(int index, string? value)
            {
                if (row is null)
                {
                    if (value is null)
                        return;
                    row = new string[Grid.ColumnCount];
                }
                row[index] = value!;
            }
            internal string Get(int index) => row != null ? row[index] ?? "" : "";

            public int Count => grid.ColumnCount;

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
            internal void InsertColumns(int index, int count)
            {
                if (row is null)
                    return;

                var newrow = new string[Grid.ColumnCount];
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

                var newrow = new string[Grid.ColumnCount];
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
                var newrow = new string[Grid.ColumnCount];
                for (int i = 0; i < newrow.Length && i < row.Length; i++)
                    newrow[i] = row[i];
                row = newrow;
            }
        }
    }
}
