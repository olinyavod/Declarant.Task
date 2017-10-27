using System;
using System.Windows;

namespace EasyProg.WPF.MVVM.Behaviors
{
    [Serializable]
    public class WindowStateSettings
    {
        public Size WindowSize { get; set; }
        public WindowStartupLocation WindowStartupLocation { get; set; }
        public Point Location { get; set; }
        public WindowState WindowState { get; set; }
    }
}