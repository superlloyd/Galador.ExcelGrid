// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsEnabledBindingSourceExample.xaml.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Interaction logic for IsEnabledBindingSourceExample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExcelGridDemo
{
    using Galador.ExcelGrid;
    using Galador.ExcelGrid.Controls;
    using Galador.ExcelGrid.Definitions;
    using System.Collections;
    using System.Windows;

    /// <summary>
    /// Interaction logic for IsEnabledBindingSourceExample.
    /// </summary>
    public partial class IsEnabledBindingSourceExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsEnabledBindingSourceExample" /> class.
        /// </summary>
        public IsEnabledBindingSourceExample()
        {
            this.InitializeComponent();
            this.ItemsSource = new[] { 11d, 0, 0, 0, 22, 0, 0, 0, 33 };
            this.IsItemEnabled = new string[] { "yes", null, null, null, "yes", null, null, null, "yes" };
            this.ControlFactory = new CustomCellDefinitionFactory(this.IsItemEnabled);
            this.DataContext = this;
        }

        public IControlFactory ControlFactory { get; }

        public class CustomCellDefinitionFactory : DefaultControlFactory
        {
            private readonly IList isItemEnabledSource;

            public CustomCellDefinitionFactory(IList isItemEnabledSource)
            {
                this.isItemEnabledSource = isItemEnabledSource;
            }

            protected override void SetIsEnabledBinding(CellDescriptor d, FrameworkElement element)
            {
                d.PropertyDefinition.IsEnabledBySource = this.isItemEnabledSource;
                d.PropertyDefinition.IsEnabledByValue = "yes";
                d.PropertyDefinition.IsEnabledByProperty = d.BindingPath;
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public double[] ItemsSource { get; }


        public string[] IsItemEnabled { get; }
    }
}