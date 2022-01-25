using Galador.WPF.ExcelGrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var model = CreateModel();
            theGrid.ItemsSource = model;
            this.DataContext = model;

            var source = new CollectionViewSource { Source = model };
            var view = source.View;
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription
            {
                Direction = System.ComponentModel.ListSortDirection.Ascending,
                PropertyName = "A",
            });
            view.Refresh();
            Trace.WriteLine("Sorting....");
            foreach (ExcelModel.Row row in view)
            {
                Trace.WriteLine(row[0]);
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription
            {
                Direction = System.ComponentModel.ListSortDirection.Descending,
                PropertyName = "A",
            });
            view.Refresh();
            Trace.WriteLine("Sorting....");
            foreach (ExcelModel.Row row in view)
            {
                Trace.WriteLine(row[0]);
            }
        }
        ExcelModel CreateModel()
        {
            var model = new ExcelModel();
            model.ColumnCount = 3;
            for (int i = 0; i < 6; i++)
            {
                var r = model.AddRow();
                r[0] = i + " One";
                r[1] = i + " Two";
                r[2] = i + " Three";
            }
            return model;
        }
    }
}
