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
    using Galador.ExcelGrid.CellDefinitions;
    using System.Windows;

    public interface IDataGridControlFactory2
    {
        FrameworkElement CreateDisplayControl(CellDescriptor d);
        FrameworkElement CreateEditControl(CellDescriptor d);
    }


    /// <summary>
    /// Specifies a control factory for the <see cref="DataGrid" />.
    /// </summary>
    public interface IDataGridControlFactory
    {
        /// <summary>
        /// Creates the display control with data binding.
        /// </summary>
        /// <param name="cellDefinition">The cell definition.</param>
        /// <returns>
        /// The control.
        /// </returns>
        FrameworkElement CreateDisplayControl(CellDefinitions.CellDefinition cellDefinition);

        /// <summary>
        /// Creates the edit control with data binding.
        /// </summary>
        /// <param name="cellDefinition">The cell definition.</param>
        /// <returns>
        /// The control.
        /// </returns>
        FrameworkElement CreateEditControl(CellDefinitions.CellDefinition cellDefinition);
    }
}