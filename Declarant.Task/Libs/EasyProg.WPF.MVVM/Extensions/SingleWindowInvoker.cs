using System;
using System.Windows;

namespace EasyProg.WPF.MVVM.Extensions
{
    public class SingleWindowInvoker<TWindow>
        where TWindow : Window, new()
    {
        private TWindow _currentWindow;

        public TWindow GetInstance()
        {
            if (_currentWindow == null)
            {
                _currentWindow = new TWindow();

                _currentWindow.Closed += CurrentWindowOnClosed;
            }

            return _currentWindow;
        }

        private void CurrentWindowOnClosed(object sender, EventArgs eventArgs)
        {
            _currentWindow.Closed -= CurrentWindowOnClosed;
            _currentWindow = null;
        }
    }
}
