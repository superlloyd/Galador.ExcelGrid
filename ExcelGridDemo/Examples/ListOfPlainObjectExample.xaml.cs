// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListOfObjectExample.xaml.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Interaction logic for ListOfObjectExample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExcelGridDemo
{
    using Galador.ExcelGrid.Controls;
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ListOfObjectExample.
    /// </summary>
    public partial class ListOfPlainObjectExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOfPlainObjectExample" /> class.
        /// </summary>
        public ListOfPlainObjectExample()
        {
            this.InitializeComponent();

            this.DefaultControlFactory.RegisterValueConverter(typeof(Mass), new MassValueConverter());

            this.ItemsSource = new List<PlainOldObject>
                                {
                                    new PlainOldObject
                                        {
                                            Boolean = true,
                                            DateTime = DateTime.Now,
                                            Color = Colors.Blue,
                                            Double = Math.PI,
                                            Fruit = Fruit.Apple,
                                            Integer = 7,
                                            Selector = null,
                                            String = "Hello"
                                        },
                                    new PlainOldObject
                                        {
                                            Boolean = true,
                                            DateTime = DateTime.Now.AddDays(-1),
                                            Color = Colors.Red,
                                            Double = Math.PI * 2,
                                            Fruit = Fruit.Banana,
                                            Integer = 7,
                                            Selector = null,
                                            String = "World"
                                        }
                                };

            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IList<PlainOldObject> ItemsSource { get; set; }

        public DefaultControlFactory DefaultControlFactory { get; } = new DefaultControlFactory();
    }
}