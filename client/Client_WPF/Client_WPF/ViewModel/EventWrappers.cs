using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace ViewModel
{

  public static class MouseBehavior
  {
    #region MouseWheel
    public static readonly DependencyProperty MouseWheelCommandProperty =
      DependencyProperty.RegisterAttached("MouseWheelCommand", typeof(ICommand), typeof(MouseBehavior), new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseWheelCommandChanged)));

    private static void MouseWheelCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;

      element.PreviewMouseWheel += new MouseWheelEventHandler(element_MouseWheel);
    }

    static void element_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseWheelCommand(element);

      command.Execute(e);
    }

    public static void SetMouseWheelCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseWheelCommandProperty, value);
    }

    public static ICommand GetMouseWheelCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseWheelCommandProperty);
    }
    #endregion

    #region MouseUp
    public static readonly DependencyProperty MouseUpCommandProperty =
      DependencyProperty.RegisterAttached("MouseUpCommand", typeof(ICommand), typeof(MouseBehavior), new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseUpCommandChanged)));

    private static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;

      element.MouseUp += new MouseButtonEventHandler(element_MouseUp);
    }

    static void element_MouseUp(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseUpCommand(element);

      command.Execute(e);
    }

    public static void SetMouseUpCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseUpCommandProperty, value);
    }

    public static ICommand GetMouseUpCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseUpCommandProperty);
    }
    #endregion

    #region MouseDown
    public static readonly DependencyProperty MouseDownCommandProperty =
      DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseBehavior), new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseDownCommandChanged)));

    private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;

      element.MouseDown += new MouseButtonEventHandler(element_MouseDown);
    }

    static void element_MouseDown(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseDownCommand(element);

      command.Execute(e);
    }

    public static void SetMouseDownCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseDownCommandProperty, value);
    }

    public static ICommand GetMouseDownCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseDownCommandProperty);
    }
    #endregion

    #region MouseAction
    public static readonly DependencyProperty MouseActionCommandProperty =
      DependencyProperty.RegisterAttached("MouseActionCommand", typeof(ICommand), typeof(MouseBehavior), new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseActionCommandChanged)));

    private static void MouseActionCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;

      element.MouseDown += new MouseButtonEventHandler(element_MouseAction);
      element.MouseMove += new MouseEventHandler(element_MouseAction);
      element.MouseUp += new MouseButtonEventHandler(element_MouseAction);
    }

    static void element_MouseAction(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseActionCommand(element);

      command.Execute(e);
    }

    static void element_MouseAction(object sender, MouseEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseActionCommand(element);

      command.Execute(e);
    }

    public static void SetMouseActionCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseActionCommandProperty, value);
    }

    public static ICommand GetMouseActionCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseActionCommandProperty);
    }

    #endregion

    #region MouseMove
    public static readonly DependencyProperty MouseMoveCommandProperty =
      DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseBehavior), new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseMoveCommandChanged)));

    private static void MouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;

      element.MouseMove += new MouseEventHandler(element_MouseMove);
    }

    static void element_MouseMove(object sender, MouseEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;

      ICommand command = GetMouseMoveCommand(element);

      command.Execute(e);
    }

    public static void SetMouseMoveCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseMoveCommandProperty, value);
    }

    public static ICommand GetMouseMoveCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseMoveCommandProperty);
    }
    #endregion
  }
}
