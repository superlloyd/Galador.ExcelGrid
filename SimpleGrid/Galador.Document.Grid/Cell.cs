using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galador.Document.Grid
{
    [Serializable]
    public class Cell : INotifyPropertyChanged, IComparable<Cell>, IComparable, ICloneable, IEquatable<Cell>
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected void OnPropertiesChanged(params string[] names)
        {
            var e = PropertyChanged;
            if (e != null)
                foreach (var name in names)
                    e(this, new PropertyChangedEventArgs(name));
        }

        public virtual int CompareTo(Cell? other)
        {
            if (other is null)
                return 1;
            return string.Compare(Text, other.Text, true);
        }

        public int CompareTo(object? obj)
            => CompareTo(obj as Cell);


        object ICloneable.Clone() => Clone();
        public virtual Cell Clone()
        {
            var result = (Cell)base.MemberwiseClone();
            result.PropertyChanged = null;
            return result;
        }

        public virtual bool Equals(Cell? other)
        {
            if (other == null)
                return false;
            return Text == other.Text
                && HorizontalAlignment == other.HorizontalAlignment;
        }
        public override bool Equals(object? obj) => Equals(obj as Cell);
        public override int GetHashCode() => Text?.GetHashCode() ?? 0;

        public override string? ToString() => Text; // make sure copy / paste work if not serializable

        public virtual bool Apply(Cell? cell)
        {
            if (cell == null)
            {
                Text = null;
            }
            else
            {
                Text = cell.Text;
                HorizontalAlignment = cell.HorizontalAlignment;
            }
            return true;
        }

        public virtual string? Text
        {
            get => mText;
            set
            {
                if (value == Text)
                    return;
                mText = value;
                OnPropertyChanged();
            }
        }
        string? mText;

        public HorizontalAlignment HorizontalAlignment
        {
            get => mHorizontalAlignment;
            set
            {
                if (value == HorizontalAlignment)
                    return;
                mHorizontalAlignment = value;
                OnPropertyChanged();
            }
        }
        HorizontalAlignment mHorizontalAlignment = HorizontalAlignment.Left;
    }
}
