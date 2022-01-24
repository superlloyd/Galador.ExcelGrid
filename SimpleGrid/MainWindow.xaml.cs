using SimpleGrid.GridModelExtensions;
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

            var model = CreateModel();
            theGrid.GridModel = model;
        }
        GridModel CreateModel()
        {
            var model = new GridModel();
            model.Columns.Add("One");
            model.Columns.Add("Two");
            model.Columns.Add("Three");
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
