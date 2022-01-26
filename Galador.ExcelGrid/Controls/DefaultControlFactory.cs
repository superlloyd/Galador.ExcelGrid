// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridControlFactory.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Creates display and edit controls for the DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Galador.ExcelGrid.Controls
{
    using Galador.ExcelGrid.Controls;
    using Galador.ExcelGrid.Definitions;
    using Galador.ExcelGrid.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using HorizontalAlignment = System.Windows.HorizontalAlignment;

    public class DefaultControlFactory : IControlFactory
    {
        private readonly Dictionary<Type, IValueConverter> valueConverters = new Dictionary<Type, IValueConverter>();

        /// <summary>
        /// Registers the value converter for the specified type.
        /// </summary>
        /// <param name="forInstancesOf">The type of instances the converter is applied to.</param>
        /// <param name="converter">The converter.</param>
        public void RegisterValueConverter(Type forInstancesOf, IValueConverter converter)
        {
            this.valueConverters[forInstancesOf] = converter;
        }
        public IValueConverter GetValueConverter(CellDescriptor d)
        {
            if (d.PropertyDefinition.Converter != null)
                return d.PropertyDefinition.Converter;

            foreach (var type in this.valueConverters.Keys)
                if (d.PropertyType.IsAssignableFrom(type))
                    return this.valueConverters[type];

            return null;
        }

        public void RegisterFactory(IControlFactory factory)
        {
            if (factory == null || otherFactories.Contains(factory))
                return;
            otherFactories.Add(factory);
        }
        readonly List<IControlFactory> otherFactories = new();

        public bool Match(CellDescriptor descriptor)
            => true;

        public FrameworkElement CreateDisplayControl(CellDescriptor d)
        {
            // loop backward to get latest registration first, assuming they are the most well targeted
            for (int i = otherFactories.Count - 1; i >= 0; i--)
            {
                var factory = otherFactories[i];
                if (factory.Match(d))
                {
                    var view = factory.CreateDisplayControl(d);
                    if (view != null)
                        return view;
                }
            }

            var element = this.CreateDisplayControlOverride(d);
            return element;
        }

        public FrameworkElement CreateEditControl(CellDescriptor d)
        {
            // loop backward to get latest registration first, assuming they are the most well targeted
            for (int i = otherFactories.Count - 1; i >= 0; i--)
            {
                var factory = otherFactories[i];
                if (factory.Match(d))
                    return factory.CreateEditControl(d);
            }

            if (d.PropertyDefinition.IsReadOnly)
            {
                return null;
            }

            var element = this.CreateEditControlOverride(d);

            if (element != null)
            {
                // The edit control should fill the cell
                element.VerticalAlignment = VerticalAlignment.Stretch;
                element.HorizontalAlignment = HorizontalAlignment.Stretch;
            }

            return element;
        }

        /// <summary>
        /// Creates the display control.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>The display control.</returns>
        protected virtual FrameworkElement CreateDisplayControlOverride(CellDescriptor d)
        {
            if (d.PropertyDefinition is TemplateColumnDefinition tcl)
            {
                return this.CreateTemplateControl(d, tcl.CellEditingTemplate);
            }
            else if (d.PropertyType.Is(typeof(bool)))
            {
                return this.CreateCheckBoxControl(d);
            }
            else if (d.PropertyType.Is(typeof(Color)))
            {
                return this.CreateColorPreviewControl(d);
            }
            return this.CreateTextBlockControl(d);
        }

        /// <summary>
        /// Creates the edit control.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// The control.
        /// </returns>
        protected virtual FrameworkElement CreateEditControlOverride(CellDescriptor d)
        {
            if (d.PropertyDefinition is TemplateColumnDefinition tcl)
            {
                return this.CreateTemplateControl(d, tcl.CellEditingTemplate ?? tcl.CellTemplate);
            }
            else if (d.PropertyType.Is(typeof(bool)))
            {
                return null;
            }
            else if (d.PropertyType.Is(typeof(Color)))
            {
                return this.CreateColorPickerControl(d);
            }
            else if (d.PropertyType.Is(typeof(Enum)))
            {
                var enumType = Nullable.GetUnderlyingType(d.PropertyType) ?? d.PropertyType;
                var values = Enum.GetValues(enumType).Cast<object>().ToList();
                if (Nullable.GetUnderlyingType(d.PropertyType) != null)
                    values.Insert(0, null);
                return this.CreateComboBox(d, values);
            }
            else if (d.PropertyDefinition.ItemsSourceProperty != null)
            {
                return this.CreateComboBox(d);
            }
            return this.CreateTextBox(d);
        }

        /// <summary>
        /// Creates a container with background binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <param name="c">The control to add to the container.</param>
        /// <returns>The container element.</returns>
        protected virtual FrameworkElement CreateContainer(CellDescriptor d, FrameworkElement c)
        {
            var binding = GetBackgroundBinding(d);
            if (binding == null)
                return c;

            var container = new Border { Child = c };
            container.SetBinding(Border.BackgroundProperty, binding);
            return container;
        }

        /// <summary>
        /// Creates a binding.
        /// </summary>
        /// <param name="d">The cd.</param>
        /// <returns>
        /// A binding.
        /// </returns>
        protected Binding CreateBinding(CellDescriptor d)
        {
            // two-way binding requires a path...
            var bindingMode = d.PropertyDefinition.IsReadOnly || string.IsNullOrEmpty(d.BindingPath) ? BindingMode.OneWay : BindingMode.TwoWay;

            var formatString = d.PropertyDefinition.FormatString;
            if (formatString != null && !formatString.StartsWith("{"))
            {
                formatString = "{0:" + formatString + "}";
            }

            var binding = new Binding(d.BindingPath)
            {
                Mode = bindingMode,
                Converter = GetValueConverter(d),
                ConverterParameter = d.PropertyDefinition.ConverterParameter,
                StringFormat = formatString,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                NotifyOnSourceUpdated = true
            };

            if (d.PropertyDefinition.ConverterCulture != null)
            {
                binding.ConverterCulture = d.PropertyDefinition.ConverterCulture;
            }

            return binding;
        }

        /// <summary>
        /// Creates the one way binding.
        /// </summary>
        /// <param name="cd">The cell definition.</param>
        /// <returns>
        /// A binding.
        /// </returns>
        protected Binding CreateOneWayBinding(CellDescriptor d)
        {
            var b = this.CreateBinding(d);
            b.Mode = BindingMode.OneWay;
            return b;
        }

        /// <summary>
        /// Creates a check box control with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A CheckBox.
        /// </returns>
        protected virtual FrameworkElement CreateCheckBoxControl(CellDescriptor d)
        {
            if (d.PropertyDefinition.IsReadOnly)
            {
                var cm = new CheckMark
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = d.PropertyDefinition.HorizontalAlignment,
                };
                cm.SetBinding(CheckMark.IsCheckedProperty, this.CreateBinding(d));
                this.SetBackgroundBinding(d, cm);
                return cm;
            }

            var c = new CheckBox
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = d.PropertyDefinition.HorizontalAlignment
            };
            c.SetBinding(ToggleButton.IsCheckedProperty, this.CreateBinding(d));
            this.SetIsEnabledBinding(d, c);
            this.SetBackgroundBinding(d, c);
            return c;
        }

        /// <summary>
        /// Creates a color picker control with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A color picker.
        /// </returns>
        protected virtual FrameworkElement CreateColorPickerControl(CellDescriptor d)
        {
            //var c = new ColorPicker
            //{
            //    VerticalAlignment = VerticalAlignment.Center,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    Focusable = false
            //};
            //c.SetBinding(ColorPicker.SelectedColorProperty, this.CreateBinding(d));
            //this.SetIsEnabledBinding(d, c);
            //this.SetBackgroundBinding(d, c);
            //return c;
            return this.CreateColorPreviewControl(d);
        }

        /// <summary>
        /// Creates a color preview control with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A preview control.
        /// </returns>
        protected virtual FrameworkElement CreateColorPreviewControl(CellDescriptor d)
        {
            var c = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                SnapsToDevicePixels = true,
                Width = 12,
                Height = 12
            };

            // Bind to the data context, since the binding may contain a custom converter
            var contextBinding = this.CreateBinding(d);
            c.SetBinding(FrameworkElement.DataContextProperty, contextBinding);

            this.SetIsEnabledBinding(d, c);

            // Convert the color in the data context to a brush
            var fillBinding = new Binding { Converter = new ColorToBrushConverter() };
            c.SetBinding(Shape.FillProperty, fillBinding);

            // Create a grid for the data context
            var grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(c);

            // Create a container to support background binding
            return this.CreateContainer(d, grid);
        }

        /// <summary>
        /// Creates a combo box with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A ComboBox.
        /// </returns>
        protected virtual FrameworkElement CreateComboBox(CellDescriptor d, List<object> itemsSourceOverride = null)
        {
            var c = new ComboBox
            {
                IsEditable = d.PropertyDefinition.IsEditable,
                Focusable = false, // keep focus on the data grid until the user opens the dropdown
                Margin = new Thickness(1, 1, 0, 0),
                HorizontalContentAlignment = d.PropertyDefinition.HorizontalAlignment,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(3, 0, 3, 0),
                BorderThickness = new Thickness(0)
            };

            if (itemsSourceOverride != null || d.PropertyDefinition.ItemsSource != null)
            {
                c.ItemsSource = itemsSourceOverride ?? d.PropertyDefinition.ItemsSource;
            }
            else
            {
                if (d.PropertyDefinition.ItemsSourceProperty != null)
                {
                    var itemsSourceBinding = new Binding(d.PropertyDefinition.ItemsSourceProperty);
                    c.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);
                }
            }

            c.DropDownOpened += (s, e) =>
            {
                // when the dropdown has been opened by F4 or mouse down
                // it should be possible to change selection by the arrow keys
                // or edit the text (if the property is annotated as editable)
                c.Focusable = true;
            };

            c.DropDownClosed += (s, e) =>
            {
                // when the dropdown has been closed
                // the arrows keys should be used to change cell
                // the (undesired) side effect is also that the text cannot be edited (if the property is annotated as editable)
                c.Focusable = false;

                FocusParentDataGrid(c);
            };

            var binding = this.CreateBinding(d);
            binding.NotifyOnSourceUpdated = true;
            c.SetBinding(d.PropertyDefinition.IsEditable ? ComboBox.TextProperty : Selector.SelectedValueProperty, binding);
            c.SelectedValuePath = d.PropertyDefinition.SelectedValuePath;
            c.DisplayMemberPath = d.PropertyDefinition.DisplayMemberPath;
            this.SetIsEnabledBinding(d, c);
            this.SetBackgroundBinding(d, c);
            return c;
        }

        /// <summary>
        /// Creates a text block control with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A TextBlock.
        /// </returns>
        protected virtual FrameworkElement CreateTextBlockControl(CellDescriptor d)
        {
            var textBlock = new TextBlockEx
            {
                HorizontalAlignment = d.PropertyDefinition.HorizontalAlignment,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(4, 0, 4, 0)
            };

            var binding = this.CreateOneWayBinding(d);

            if (d.PropertyDefinition.ItemsSource != null)
            {
                if (!string.IsNullOrEmpty(d.PropertyDefinition.DisplayMemberPath))
                    binding.Path.Path += "." + d.PropertyDefinition.DisplayMemberPath;
            }

            textBlock.SetBinding(TextBlock.TextProperty, binding);
            this.SetIsEnabledBinding(d, textBlock);

            return this.CreateContainer(d, textBlock);
        }

        /// <summary>
        /// Creates a text box with data binding.
        /// </summary>
        /// <param name="d">The cell definition.</param>
        /// <returns>
        /// A TextBox.
        /// </returns>
        protected virtual FrameworkElement CreateTextBox(CellDescriptor d)
        {
            var c = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = d.PropertyDefinition.HorizontalAlignment,
                MaxLength = d.PropertyDefinition.MaxLength,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(1, 1, 0, 0)
            };

            c.Loaded += (sender, args) =>
            {
                var tb = (TextBox)sender;
                tb.CaretIndex = tb.Text.Length;
                tb.SelectAll();
            };

            var binding = this.CreateBinding(d);
            c.SetBinding(TextBox.TextProperty, binding);
            this.SetIsEnabledBinding(d, c);
            this.SetBackgroundBinding(d, c);

#if DATAGRID_DEBUG_INFO
            c.ToolTip = $"Binding: {binding.Path.Path} {binding.Mode} {binding.Source}\nConverter: {GetValueConverter(d)} {d.PropertyDefinition.ConverterParameter}\nBindingSource: {d.BindingSource}";
#endif
            return c;
        }

        /// <summary>
        /// Creates the template control.
        /// </summary>
        /// <param name="d">The definition.</param>
        /// <param name="template">The data template.</param>
        /// <returns>A content control.</returns>
        protected virtual FrameworkElement CreateTemplateControl(CellDescriptor d, DataTemplate template)
        {
            var content = (FrameworkElement)template.LoadContent();
            var binding = this.CreateBinding(d);
            binding.Mode = BindingMode.OneWay;
            var c = new ContentControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = content
            };
            content.SetBinding(FrameworkElement.DataContextProperty, binding);
            this.SetIsEnabledBinding(d, content);
            this.SetBackgroundBinding(d, c);
            return c;
        }

        /// <summary>
        /// Sets the IsEnabled binding.
        /// </summary>
        /// <param name="cd">The cell definition.</param>
        /// <param name="element">The element.</param>
        protected virtual void SetIsEnabledBinding(CellDescriptor d, FrameworkElement element)
        {
            if (d.PropertyDefinition.IsEnabledByProperty != null)
            {
                element.SetIsEnabledBinding(
                    d.PropertyDefinition.IsEnabledByProperty,
                    d.PropertyDefinition.IsEnabledByValue,
                    d.PropertyDefinition.IsEnabledBySource);
            }
        }

        protected void SetBackgroundBinding(CellDescriptor d, Control c)
        {
            var binding = GetBackgroundBinding(d);
            if (binding != null)
                c.SetBinding(Control.BackgroundProperty, binding);
        }
        protected void SetBackgroundBinding(CellDescriptor d, Border container)
        {
            var binding = GetBackgroundBinding(d);
            if (binding != null)
                container.SetBinding(Border.BackgroundProperty, binding);
        }
        protected virtual Binding GetBackgroundBinding(CellDescriptor d)
        {
            var source = d.PropertyDefinition.Background;
            var path = source == null ? d.PropertyDefinition.BackgroundProperty : "";
            if (path == null)
                return null;
            return new Binding(path) { Source = source };
        }

        /// <summary>
        /// Focuses on the parent data grid.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject" />.</param>
        private static void FocusParentDataGrid(DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null && !(parent is DataGrid))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var u = parent as UIElement;
            u?.Focus();
        }
    }
}
