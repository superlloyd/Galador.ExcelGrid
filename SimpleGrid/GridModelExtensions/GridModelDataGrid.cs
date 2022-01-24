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
        protected override IDataGridOperator CreateOperator()
        {
            var list = this.ItemsSource;
            if (list is GridModel)
                return new GridModelOperator(this);

            return base.CreateOperator();
        }
    }
}
