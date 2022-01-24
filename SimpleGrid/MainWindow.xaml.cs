using SimpleGrid.Models;
using System;
using System.Collections.Generic;
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

            //theGrid.ItemsSource = new[]
            //{
            //    new string[] { "One", "Two", "Three", },
            //    new string[] { "One", "Two", "Three", },
            //    new string[] { "One", "Two", "Three", },
            //    new string[] { "One", "Two", "Three", },
            //    new string[] { "One", "Two", "Three", },
            //    new string[] { "One", "Two", "Three", },
            //};

            var model = new GridModel();
            theGrid.ItemsSource = model.Rows;
            theGrid.ColumnHeadersSource = model.Columns;
        }
    }
}
