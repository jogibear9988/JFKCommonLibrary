﻿using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JFKCommonLibrary.WPF
{
    public class ResolveElementName : TargetedTriggerAction<FrameworkElement>
    {
        private static DependencyObject GetElementBasedOnName(DependencyObject startPoint, string elementName)
        {
            DependencyObject returnValue = null;
            if (startPoint != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(startPoint);
                if (parent != null)
                {
                    FrameworkElement fe = parent as FrameworkElement;
                    if (fe != null)
                    {
                        if (fe.Name == elementName)
                        {
                            returnValue = fe;
                        }
                        else
                        {
                            returnValue = GetElementBasedOnName(fe, elementName);
                        }
                    }
                    else
                    {
                        returnValue = GetElementBasedOnName(fe, elementName);
                    }
                }
            }
            return returnValue;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.Target.Loaded += this.Target_Loaded;
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.Target.Loaded -= this.Target_Loaded;
        }


        private void Target_Loaded(object sender, RoutedEventArgs e)
        {


            if (this.Target != null)
            {
                var fields = this.Target.GetType().GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.FlattenHierarchy |
                    System.Reflection.BindingFlags.Static);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof (DependencyProperty) &&
                        (field.Name == this.PropertyName ||
                         field.Name == string.Concat(this.PropertyName, "Property")))
                    {
                        DependencyProperty dp = field.GetValue(this.Target) as DependencyProperty;
                        var binding = this.Target.GetBindingExpression(dp);
                        string elementName = binding.ParentBinding.ElementName;
                        if (!string.IsNullOrEmpty(elementName))
                        {
                            DependencyObject boundToElement = GetElementBasedOnName(this.Target, elementName);
                            if (boundToElement != null)
                            {
                                Binding newBinding = new Binding();
                                Binding oldBinding = binding.ParentBinding;
                                newBinding.BindsDirectlyToSource = oldBinding.BindsDirectlyToSource;
                                newBinding.Converter = oldBinding.Converter;
                                newBinding.ConverterCulture = oldBinding.ConverterCulture;
                                newBinding.ConverterParameter = oldBinding.ConverterParameter;
                                newBinding.FallbackValue = oldBinding.FallbackValue;
                                newBinding.Mode = oldBinding.Mode;
                                newBinding.NotifyOnValidationError = oldBinding.NotifyOnValidationError;
                                newBinding.Path = oldBinding.Path;
                                newBinding.Source = boundToElement;
                                newBinding.StringFormat = oldBinding.StringFormat;
                                newBinding.TargetNullValue = oldBinding.TargetNullValue;
                                newBinding.UpdateSourceTrigger = oldBinding.UpdateSourceTrigger;
                                newBinding.ValidatesOnDataErrors = oldBinding.ValidatesOnDataErrors;
                                newBinding.ValidatesOnExceptions = oldBinding.ValidatesOnExceptions;
                                if (this.Target is ComboBox)
                                {
                                    ComboBox combo = this.Target as ComboBox;
                                    combo.SetBinding(dp, newBinding);
                                    /*combo.SetBinding(ComboBox.SelectedValueProperty,
                                                     combo.GetBindingExpression(ComboBox.SelectedValueProperty).
                                                         ParentBinding);*/
                                }
                                else
                                {
                                    this.Target.SetBinding(dp, newBinding);
                                }

                            }
                        }
                    }
                }
            }
        }

        public string PropertyName
        {
            get { return (string) this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }


        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof (string), typeof (ResolveElementName),
                                        new PropertyMetadata(string.Empty));


        protected override void Invoke(object parameter)
        {
            // do nothing
        }
    }
}
