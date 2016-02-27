using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Controls;

namespace ViewModel
{
  #region converters
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public class ArithmeticConverter : MarkupExtension, IValueConverter
  {
    private const string ArithmeticParseExpression = "([+\\-*/]{1,1})\\s{0,}(\\-?[\\d\\.]+)";
    private Regex arithmeticRegex = new Regex(ArithmeticParseExpression); 
    
    private static ArithmeticConverter _converter;
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (_converter == null)
      {
        _converter = new ArithmeticConverter();
      }
      return _converter;
    }

    #region IValueConverter Members

    object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {

      if (value is double && parameter != null)
      {
        string param = parameter.ToString();

        if (param.Length > 0)
        {
          Match match = arithmeticRegex.Match(param);
          if (match != null && match.Groups.Count == 3)
          {
            string operation = match.Groups[1].Value.Trim();
            string numericValue = match.Groups[2].Value;

            double number = 0;
            if (double.TryParse(numericValue, out number)) // this should always succeed or our regex is broken
            {
              double valueAsDouble = (double)value;
              double returnValue = 0;

              switch (operation)
              {
                case "+":
                  returnValue = valueAsDouble + number;
                  break;

                case "-":
                  returnValue = valueAsDouble - number;
                  break;

                case "*":
                  returnValue = valueAsDouble * number;
                  break;

                case "/":
                  returnValue = valueAsDouble / number;
                  break;
              }

              return returnValue;
            }
          }
        }
      }

      return null;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion
  }
  #endregion

  public static class Attachable // HINT: Not sure if working
  {
    public static DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(Attachable), new UIPropertyMetadata(false, OnIsFocusedChanged));

    public static bool GetIsFocused(DependencyObject dependencyObject)
    {
      return (bool)dependencyObject.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject dependencyObject, bool value)
    {
      dependencyObject.SetValue(IsFocusedProperty, value);
    }

    public static void OnIsFocusedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      UIElement element = dependencyObject as UIElement; 
      bool newValue = (bool)dependencyPropertyChangedEventArgs.NewValue;
      bool oldValue = (bool)dependencyPropertyChangedEventArgs.OldValue;
      if (newValue && !oldValue && !element.IsFocused) element.Focus();
    }
  }
  public static class SizeObserver
  {
    public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
        "Observe",
        typeof(bool),
        typeof(SizeObserver),
        new FrameworkPropertyMetadata(OnObserveChanged));

    public static readonly DependencyProperty ObservedWidthProperty = DependencyProperty.RegisterAttached(
        "ObservedWidth",
        typeof(double),
        typeof(SizeObserver));

    public static readonly DependencyProperty ObservedHeightProperty = DependencyProperty.RegisterAttached(
        "ObservedHeight",
        typeof(double),
        typeof(SizeObserver));

    public static bool GetObserve(FrameworkElement frameworkElement)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      return (bool)frameworkElement.GetValue(ObserveProperty);
    }

    public static void SetObserve(FrameworkElement frameworkElement, bool observe)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      frameworkElement.SetValue(ObserveProperty, observe);
    }

    public static double GetObservedWidth(FrameworkElement frameworkElement)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      return (double)frameworkElement.GetValue(ObservedWidthProperty);
    }

    public static void SetObservedWidth(FrameworkElement frameworkElement, double observedWidth)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      frameworkElement.SetValue(ObservedWidthProperty, observedWidth);
    }

    public static double GetObservedHeight(FrameworkElement frameworkElement)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      return (double)frameworkElement.GetValue(ObservedHeightProperty);
    }

    public static void SetObservedHeight(FrameworkElement frameworkElement, double observedHeight)
    {
      //frameworkElement.AssertNotNull("frameworkElement");
      frameworkElement.SetValue(ObservedHeightProperty, observedHeight);
    }

    private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = (FrameworkElement)dependencyObject;

      if ((bool)e.NewValue)
      {
        frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
        UpdateObservedSizesForFrameworkElement(frameworkElement);
      }
      else
      {
        frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
      }
    }

    private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateObservedSizesForFrameworkElement((FrameworkElement)sender);
    }

    private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
    {
      // WPF 4.0 onwards
      frameworkElement.SetCurrentValue(ObservedWidthProperty, frameworkElement.ActualWidth);
      frameworkElement.SetCurrentValue(ObservedHeightProperty, frameworkElement.ActualHeight);

      // WPF 3.5 and prior
      ////SetObservedWidth(frameworkElement, frameworkElement.ActualWidth);
      ////SetObservedHeight(frameworkElement, frameworkElement.ActualHeight);
    }
  }
}
