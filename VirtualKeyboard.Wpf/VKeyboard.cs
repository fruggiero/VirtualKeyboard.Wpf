using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualKeyboard.Wpf.Behaviors;
using VirtualKeyboard.Wpf.Controls;
using VirtualKeyboard.Wpf.ViewModels;
using VirtualKeyboard.Wpf.Views;

namespace VirtualKeyboard.Wpf
{
    public static class VKeyboard
    {
        private const string _keyboardValueName = "KeyboardValueContent";
        private const string _keyboardName = "KeyboardContent";

        private static Type _hostType = typeof(DefaultKeyboardHost);

        private static TaskCompletionSource<Result> _tcs;
        private static Window _windowHost;

        public static bool ShowDiscardButton { get; set; }

        public static void Config(Type hostType)
        {
            if (hostType.IsSubclassOf(typeof(Window))) _hostType = hostType;
            else throw new ArgumentException();
        }

        public static void Listen<T>(Expression<Func<T, string>> property) where T: UIElement
        {
            EventManager.RegisterClassHandler(typeof(T), UIElement.PreviewMouseLeftButtonDownEvent, (RoutedEventHandler)(async (s, e) =>
            {
                if (s is AdvancedTextBox) return;

                // Check if element is a focusable
                IInputElement inputElement = s as IInputElement;
                bool oldFocusable = false;
                if (inputElement != null)
                {
                    oldFocusable = inputElement.Focusable;
                    inputElement.Focusable = false;
                }

                var memberSelectorExpression = property.Body as MemberExpression;
                if (memberSelectorExpression == null) return;
                var prop = memberSelectorExpression.Member as PropertyInfo;
                if (prop == null) return;
                var initValue = (string)prop.GetValue(s);
                var kind = ((DependencyObject)s).GetValue(KeyboardType.KeyboardTypeProperty);
                Types.KeyboardType type = kind == null ? Types.KeyboardType.Alphabet : (Types.KeyboardType)kind;
                var format = ((DependencyObject)s).GetValue(FormatBehavior.FormatProperty);
                var regex = ((DependencyObject)s).GetValue(FormatBehavior.RegexProperty);
                Result value;
                char? passwordChar = null;
                if (s is PasswordBox p) passwordChar = p.PasswordChar;
                if (regex != null)
                {
                    value = await OpenAsyncInternal(initValue, type, (string)regex, passwordChar:passwordChar);
                }
                else if (format != null)
                {
                    value = await OpenAsyncInternal(initValue, type, format:(Format)format, passwordChar:passwordChar);
                }
                else
                {
                    value = await OpenAsyncInternal(initValue, type, passwordChar:passwordChar);
                }

                if (value != null)
                {
                    if (inputElement != null)
                    {
                        var txtArgs = new TextCompositionEventArgs(
                            Keyboard.PrimaryDevice,
                            new TextComposition(InputManager.Current, inputElement, value.KeyboardText))
                        {
                            RoutedEvent = UIElement.PreviewTextInputEvent,
                            Source = inputElement
                        };
                        inputElement.RaiseEvent(txtArgs);
                    }

                    prop.SetValue(s, value.KeyboardText, null);

                    // Set caret index
                    switch (s)
                    {
                        case TextBox txt:
                            txt.CaretIndex = value.CaretPosition;
                            break;
                    }
                }

                // Set focus
                if (inputElement != null)
                {
                    inputElement.Focusable = oldFocusable;
                    if (oldFocusable)
                    {
                        inputElement.Focus();
                    }
                }
            }));
        }

        private static Task<Result> OpenAsyncInternal(string initialValue = "",
            Types.KeyboardType type = Types.KeyboardType.Alphabet,
            string regex = null,
            Format? format = null,
            char? passwordChar = null)
        {
            if (_windowHost != null) throw new InvalidOperationException();

            _tcs = new TaskCompletionSource<Result>();
            _windowHost = (Window)Activator.CreateInstance(_hostType);
            var viewModel = new VirtualKeyboardViewModel(initialValue, type, regex, format)
            {
                ShowDiscardButton = ShowDiscardButton
            };
            _windowHost.DataContext = viewModel;
            var keyboardValueView = new KeyboardValueView();
            ((ContentControl)_windowHost.FindName(_keyboardValueName)).Content = keyboardValueView;
            if (passwordChar != null)
            {
                keyboardValueView.TextBox.PasswordChar = passwordChar;
            }
            ((ContentControl)_windowHost.FindName(_keyboardName)).Content = new VirtualKeyboardView();
            void handler(object s, CancelEventArgs a)
            {
                ((Window)s).Closing -= handler;

                if (IsAccepted())
                {
                    var result = GetResult();
                    _tcs?.SetResult(result);
                }
                else
                {
                    _tcs.SetResult(null);
                }
                
                _windowHost = null;
                _tcs = null;
            }

            _windowHost.Closing += handler;

            _windowHost.Owner = Application.Current.MainWindow;
            _windowHost.Show();
            return _tcs.Task;
        }

        public static async Task<string> OpenAsync(string initialValue = "", 
            Types.KeyboardType type = Types.KeyboardType.Alphabet, 
            string regex = null, 
            Format? format = null, 
            char? passwordChar = null)
        {
            var res = await OpenAsyncInternal(initialValue, type, regex, format, passwordChar);
            return res.KeyboardText;
        }

        public static void Close()
        {
            if (_windowHost == null) throw new InvalidOperationException();
            
            _windowHost.Close();
        }

        private static bool IsAccepted()
        {
            var viewModel = (VirtualKeyboardViewModel)_windowHost.DataContext;
            return viewModel.Accepted;
        }

        private static Result GetResult()
        {
            var viewModel = (VirtualKeyboardViewModel)_windowHost.DataContext;
            return new Result(viewModel.KeyboardText, viewModel.CaretPosition);
        }

        private class Result
        {
            public string KeyboardText { get; }
            public int CaretPosition { get; }

            public Result(string text, int pos)
            {
                KeyboardText = text;
                CaretPosition = pos;
            }
        }
    }
}
