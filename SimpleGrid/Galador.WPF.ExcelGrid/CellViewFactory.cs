using Galador.Document.Grid;
using Galador.ExcelGrid;
using Galador.ExcelGrid.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Galador.WPF.ExcelGrid
{
    public class CellViewFactory : IControlFactory
    {
        public bool Match(CellDescriptor descriptor)
            => descriptor?.Item is Cell;

        protected static void SetBinding<T>(FrameworkElement target, DependencyProperty property, Cell cell, Expression<Func<Cell, T>> bindingsource, Action<Binding>? init = null)
        {
            var (path, prop) = ParseBindingSource(bindingsource);
            if (prop == null)
                return;
            var binding = new Binding(path)
            {
                Source = cell,
                Mode = prop.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                NotifyOnSourceUpdated = true
            };
            if (init != null)
                init(binding);
            target.SetBinding(property, binding);
        }
        private static (string? path, PropertyInfo? prop) ParseBindingSource<T>(Expression<Func<Cell, T>> bindingsource)
        {
            object? root = null;
            var propertyPath = new List<PropertyInfo>();
            var me = bindingsource.Body as MemberExpression;
            while (me != null && root == null)
            {
                switch (me.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        if (me.Expression is ConstantExpression)
                        {
                            var ce = (ConstantExpression)me.Expression;
                            root = ce.Value;
                        }
                        else
                        {
                            if (me.Member is not PropertyInfo pi)
                                return (null, null);
                            propertyPath.Add(pi);
                        }
                        me = me.Expression as MemberExpression;
                        break;
                    default:
                        return (null, null);
                }
            }
            if (propertyPath.Count == 0)
                return (null, null);
            propertyPath.Reverse();
            return (propertyPath.Select(x => x.Name).Aggregate((s1, s2) => s1 + '.' + s2), propertyPath.Last());
        }

        public FrameworkElement CreateDisplayControl(CellDescriptor d)
        {
            var cell = d.Item as Cell;
            if (cell == null)
                return null!;
            return CreateDisplayControl(cell);
        }

        protected virtual FrameworkElement CreateDisplayControl(Cell cell)
        {
            var text = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(4, 0, 4, 0)
            };

            SetBinding(text, TextBlock.TextProperty, cell, x => x.Text, b => b.Mode = BindingMode.OneWay);
            SetBinding(text, FrameworkElement.HorizontalAlignmentProperty, cell, x => x.HorizontalAlignment, b => b.Mode = BindingMode.OneWay);
            return text;
        }

        public FrameworkElement CreateEditControl(CellDescriptor d)
        {
            var cell = d.Item as Cell;
            if (cell == null)
                return null!;
            return CreateEditControl(cell);
        }
        public virtual FrameworkElement CreateEditControl(Cell cell)
        {
            var text = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(1, 1, 0, 0)
            };
            text.Loaded += (sender, args) =>
            {
                text.CaretIndex = text.Text.Length;
                text.SelectAll();
            };
            SetBinding(text, TextBox.TextProperty, cell, x => x.Text);
            SetBinding(text, Control.HorizontalContentAlignmentProperty, cell, x => x.HorizontalAlignment, b => b.Mode = BindingMode.OneWay);
            return text;
        }
    }
}
