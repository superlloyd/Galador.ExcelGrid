using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrid.GridModelExtensions
{
    public partial class GridModel : IList<GridModel.Row>, IList, INotifyCollectionChanged
    {
        readonly List<Row> rows = new List<Row>();

        public GridModel()
        {
            ObserveColumns();
        }

        void ObserveColumns()
        {
            Columns.CollectionChanged += (o, e) => 
            {
                var N = Columns.Count;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            int n = e.NewItems!.Count;
                            foreach (var row in rows)
                            {
                                for (int i = N - 1; i >= e.NewStartingIndex + n; i--)
                                    row.Set(i, row.Get(i - n));
                                for (int i = 0; i < n; i++)
                                    row.Set(i + e.NewStartingIndex, "");
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            int n = e.OldItems!.Count;
                            foreach (var row in rows)
                            {
                                for (int i = e.OldStartingIndex; i < N - n; i++)
                                    row.Set(i, row.Get(i + n));
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        throw new NotImplementedException();
                    // nothing
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                    default:
                        break;
                }
            };
        }

        public ObservableCollection<string> Columns { get; } = new ObservableCollection<string>();

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

        public int Count => rows.Count;

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

        void ICollection<Row>.Add(Row item) => throw new NotSupportedException();

        public Row AddRow() => InsertRowAt(Count);

        public Row InsertRowAt(int index)
        {
            if (index < 0 || index > rows.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var row = new Row(this);
            rows.Insert(index, row);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object?)row, index));
            return row;
        }

        public void Clear()
        {
            if (rows.Count == 0)
                return;
            var old = rows.ToArray();
            rows.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,old, 0));
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
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object?)row, index));
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

        public class Row : IList<string>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        {
            readonly GridModel grid;
            readonly Dictionary<int, string> row = new();

            internal Row(GridModel grid)
            {
                this.grid = grid;
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged;
            public event PropertyChangedEventHandler? PropertyChanged;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
            internal void Set(int index, string? value) => row[index] = value ?? string.Empty;
            internal string Get(int index) => row.TryGetValue(index, out var result) ? result : "";

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
