using Galador.Document.Grid;
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

            //var model = CreateStringModel();
            var model = CreateCellModel();
            theGrid.ItemsSource = model;
            this.DataContext = model;
        }

        CellGridModel CreateCellModel()
        {
            var model = new CellGridModel();
            model.ColumnCount = 3;
            for (int i = 0; i < 6; i++)
            {
                var r = model.AddRow();
                r[0].Text = i + " One";
                r[1].Text = i + " Two";
                r[2].Text = i + " Three";
            }
            return model;
        }
        StringGridModel CreateStringModel()
        {
            var model = new StringGridModel();
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

        private void DoSaveCheck(object sender, RoutedEventArgs e)
        {
            var model = DataContext as StringGridModel;
            if (model is null)
                return;
            var s1 = model!.ToCsv();
            var model2 = StringGridModel.FromCsv(s1);
            var s2 = model2.ToCsv();
            var model3 = new StringGridModel();
            model3.InitializeFromCsv(s2);
            Debug.Assert(s1 == s2);
            Debug.Assert(model.RowCount == model2.RowCount);
            Debug.Assert(model.ColumnCount == model2.ColumnCount);
            for (int i = 0; i < model.RowCount; i++)
                for (int j = 0; j < model.ColumnCount; j++)
                    Debug.Assert(model[i, j] == model2[i, j]);
        }
    }
}
