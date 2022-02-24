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
            if(e.Key == Key.Delete || e.Key == Key.Back) return;
            if (this.DataContext is VirtualKeyboardViewModel vm)
            {
                e.Handled = true;
                if (e.Key == Key.Escape && vm.ShowDiscardButton)
                {
                    vm.Discard.Execute(null);
                    return;
                }

                if (e.Key == Key.Enter)
                {
                    vm.Accept.Execute(null);
                    return;
                }

                if (e.Key == Key.Left)
                {
                    vm.CaretPosition--;
                    return;
                }

                if (e.Key == Key.Right)
                {
                    vm.CaretPosition++;
                    return;
                }

                var charString = User32.GetCharFromKey(e.Key)?.ToString();
                if (charString != null)
                {
                    if (vm.Regex == null)
                    {
                        vm.AddCharacter.Execute(charString);
                    }
                    else
                    {
                        if(vm.Regex.IsMatch(TextBox.Text + charString))
                            vm.AddCharacter.Execute(charString);
                    }
                }
            }
        }
    }
}
