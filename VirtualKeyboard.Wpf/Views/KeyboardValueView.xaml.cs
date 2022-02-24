using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using VirtualKeyboard.Wpf.ViewModels;

namespace VirtualKeyboard.Wpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy KeyboardValueView.xaml
    /// </summary>
    partial class KeyboardValueView : UserControl
    {
        public KeyboardValueView()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.DataContext is VirtualKeyboardViewModel vm)
            {
                if (e.Key == Key.Back)
                {
                    vm.RemoveCharacter.Execute(null);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Delete)
                {
                    var oldCaretPosition = vm.CaretPosition;
                    vm.CaretPosition++;
                    if (oldCaretPosition != vm.CaretPosition)
                    {
                        vm.RemoveCharacter.Execute(null);
                    }
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Escape && vm.ShowDiscardButton)
                {
                    vm.Discard.Execute(null);
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Enter)
                {
                    vm.Accept.Execute(null);
                    e.Handled = true;
                    return;
                }

                if(!e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.None))
                    return;

                var charString = User32.GetCharFromKey(e.Key)?.ToString();
                if (charString != null)
                {
                    if (vm.Regex == null)
                    {
                        vm.AddCharacter.Execute(charString);
                    }
                    else
                    {
                        if(vm.Regex.IsMatch(TextBox.TextValue + charString))
                            vm.AddCharacter.Execute(charString);
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
