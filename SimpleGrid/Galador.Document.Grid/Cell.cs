using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Galador.Document.Grid
{
    public class Cell : INotifyPropertyChanged, IComparable<Cell>, IComparable
    {
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
    }
}
