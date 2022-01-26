using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Galador.Document.Grid
{
    public class Cell : INotifyPropertyChanged
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
