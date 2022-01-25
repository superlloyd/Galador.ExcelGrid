using Galador.ExcelGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Galador.WPF.ExcelGrid
{
    public class ExcelDataGrid : DataGrid
    {
        protected override IDataGridOperator CreateOperator()
        {
            var list = this.ItemsSource;
            if (list is ExcelModel)
                return new ExcelModelOperator(this);

            return base.CreateOperator();
        }
    }
}
