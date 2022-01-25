﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableCollectionOfDoubleExample.xaml.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Interaction logic for ObservableCollectionOfMassExample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExcelGridDemo
{
    using Galador.ExcelGrid.Controls;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for ObservableCollectionOfMassExample.
    /// </summary>
    public partial class ObservableCollectionOfMassExample
    {
        /// <summary>
        /// The static items.
        /// </summary>
        private static readonly ObservableCollection<Mass> StaticItems = new ObservableCollection<Mass>(new[] { new Mass(32), new Mass(36) });

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionOfMassExample" /> class.
        /// </summary>
        public ObservableCollectionOfMassExample()
        {
            this.InitializeComponent();
            this.DefaultControlFactory.RegisterValueConverter(typeof(Mass), new MassValueConverter());
            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<Mass> Items => StaticItems;

        public DefaultControlFactory DefaultControlFactory { get; } = new DefaultControlFactory();
    }
}