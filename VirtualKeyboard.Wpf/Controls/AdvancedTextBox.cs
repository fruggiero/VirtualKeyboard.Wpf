using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VirtualKeyboard.Wpf.Controls
{
    class AdvancedTextBox : TextBox
    {
        public char? PasswordChar
        {
            get => (char?)GetValue(PasswordCharProperty);
            set => SetValue(PasswordCharProperty, value);
        }

        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register("PasswordChar", typeof(char?), typeof(AdvancedTextBox), 
                new PropertyMetadata(null));

        public int CaretPosition
        {
            get { return (int)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaretPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.Register("CaretPosition", typeof(int), typeof(AdvancedTextBox), 
                new PropertyMetadata(0, OnCaretPositionChange));

        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(string), typeof(AdvancedTextBox), 
                new PropertyMetadata("", OnSelectedTextChange));

        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextValueProperty =
            DependencyProperty.Register("TextValue", typeof(string), typeof(AdvancedTextBox), 
                new PropertyMetadata("", OnTextValueChange));

        public AdvancedTextBox()
        {
            SelectionChanged += AdvancedTextBox_SelectionChanged;
            TextChanged += AdvancedTextBox_OnTextChanged;
            Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new Action(delegate() { 
                        Focus();
                    }));
            };
            CommandManager.AddPreviewExecutedHandler(this, PreviewExecuted);
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut  || 
                e.Command == ApplicationCommands.Paste)
            {
                if(PasswordChar != null) e.Handled = true;
            }
        }

        private static void AdvancedTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (AdvancedTextBox)sender;
            textBox.SetValue(TextValueProperty, textBox.Text);
        }

        private static void AdvancedTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var s = (AdvancedTextBox)sender;
            s.SelectionChanged -= AdvancedTextBox_SelectionChanged;
            var oldSelectionLength = s.SelectionLength;
            var oldSelectionStart = s.SelectionStart;

            s.SetValue(SelectedValueProperty, s.SelectedText);
            s.SetValue(CaretPositionProperty, s.CaretIndex);

            s.SelectionStart = oldSelectionStart;
            s.SelectionLength = oldSelectionLength;
            s.SelectionChanged += AdvancedTextBox_SelectionChanged;
        }

        private static void OnCaretPositionChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            int? value = e.NewValue as int?;
            var s = (AdvancedTextBox)sender;
            s.SelectionChanged -= AdvancedTextBox_SelectionChanged;
            ((TextBox)sender).CaretIndex = value ?? 0;
            s.SelectionChanged += AdvancedTextBox_SelectionChanged;
        }

        private static void OnSelectedTextChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string value = e.NewValue as string;
            ((TextBox)sender).SelectedText = value ?? "";
        }

        private static void OnTextValueChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (AdvancedTextBox)sender;
            int caretPosition = s.CaretPosition;
            string value = e.NewValue as string;
            s.SelectionChanged -= AdvancedTextBox_SelectionChanged;
            s.TextChanged -= AdvancedTextBox_OnTextChanged;
            s.Text = s.PasswordChar != null ? string.Join(string.Empty, Enumerable.Repeat(s.PasswordChar, value.Length)) : value;
            s.TextChanged += AdvancedTextBox_OnTextChanged;
            s.CaretIndex = caretPosition <= value.Length ? caretPosition : value.Length;
            s.SelectionChanged += AdvancedTextBox_SelectionChanged;
        }
    }
}
