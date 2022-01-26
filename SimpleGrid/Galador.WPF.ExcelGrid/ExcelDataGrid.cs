using Galador.Document.Grid;
using Galador.ExcelGrid;
using Galador.ExcelGrid.Controls;
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
        public ExcelDataGrid()
        {
            var factory = new DefaultControlFactory();
            factory.RegisterFactory(new CellViewFactory());
            ControlFactory = factory;
        }

        protected override IDataGridOperator CreateOperator()
        {
            var list = this.ItemsSource;
            if (list is StringGridModel)
                return new StringGridModelOperator(this);
            if (list is CellGridModel)
                return new CellGridModelOperator(this);
            return base.CreateOperator();
        }
    }
}
