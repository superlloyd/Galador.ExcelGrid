// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundBindingSourceExample.xaml.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Interaction logic for BackgroundBindingSourceExample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExcelGridDemo
{
    using Galador.ExcelGrid.Controls;
    using Galador.ExcelGrid.Definitions;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Data;
    using System.Windows.Media;
 
    /// <summary>
    /// Interaction logic for BackgroundBindingSourceExample.
    /// </summary>
    public partial class BackgroundBindingSourceExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundBindingSourceExample" /> class.
        /// </summary>
        public BackgroundBindingSourceExample()
        {
            this.InitializeComponent();
            this.ItemsSource = new[] { 11d, 0, 0, 0, 22, 0, 0, 0, 33 };
            this.BackgroundSource = new[]
                                  {
                                      Brushes.LightBlue, Brushes.LightGray, Brushes.LightGray, Brushes.LightGray,
                                      Brushes.LightBlue, Brushes.LightGray, Brushes.LightGray, Brushes.LightGray,
                                      Brushes.LightBlue,
                                  };
            this.ControlFactory = new CustomCellDefinitionFactory(this.BackgroundSource);
            this.DataContext = this;
        }

        public CustomCellDefinitionFactory ControlFactory { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public double[] ItemsSource { get; }

        public Brush[] BackgroundSource { get; }

        public class CustomCellDefinitionFactory : DefaultControlFactory
        {
            private readonly IList backgroundSource;

            public CustomCellDefinitionFactory(IList backgroundSource)
            {
                this.backgroundSource = backgroundSource;
            }

            protected override Binding GetBackgroundBinding(CellDescriptor d)
            {
                var source = this.backgroundSource;
                var path = d.BindingPath;
                if (path == null)
                    return null;
                return new Binding(path) { Source = source };
            }
        }
    }
}