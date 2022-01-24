using Galador.ExcelGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGrid.GridModelExtensions
{
    public class GridModelDataGrid : DataGrid
    {
        public GridModel GridModel
        {
            get { return (GridModel)GetValue(GridModelProperty); }
            set { SetValue(GridModelProperty, value); }
        }

        public static readonly DependencyProperty GridModelProperty = DependencyProperty.Register(
            "GridModel", 
            typeof(GridModel), 
            typeof(GridModelDataGrid), 
            new PropertyMetadata(null, (o, e) => ((GridModelDataGrid)o).OnGridModelChanged((GridModel)e.NewValue)));

        private void OnGridModelChanged(GridModel newValue)
        {
            if (newValue != null)
            {
                ItemsSource = newValue;
                ColumnHeadersSource = newValue.Columns;
            }
            else
            {
                ColumnHeadersSource = null;
                ItemsSource = null;
            }
        }

        protected override IDataGridOperator CreateOperator()
        {
            var list = this.ItemsSource;
            if (list is GridModel)
                return new GridModelOperator(this);

            return base.CreateOperator();
        }
    }
}
