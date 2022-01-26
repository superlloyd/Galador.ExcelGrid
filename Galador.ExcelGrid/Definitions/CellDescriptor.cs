namespace Galador.ExcelGrid.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Contains all the data that can be used to create a <see cref="CellDefinition" /> in a <see cref="CellDefinitionFactory" />.
    /// </summary>
    public class CellDescriptor
    {
        public Definitions.PropertyDefinition PropertyDefinition { get; set; }
        public object Item { get; set; }
        public Type PropertyType { get; set; }
        public string BindingPath { get; set; }
        public object BindingSource { get; set; }
    }
}