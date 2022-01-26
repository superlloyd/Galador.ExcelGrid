// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataGridControlFactory.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Specifies a control factory for the DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Galador.ExcelGrid
{
    using Galador.ExcelGrid.Definitions;
    using System.Windows;

    public interface IControlFactory
    {
        bool Match(CellDescriptor descriptor);
        FrameworkElement CreateDisplayControl(CellDescriptor d);
        FrameworkElement CreateEditControl(CellDescriptor d);
    }

}