using System.ComponentModel;
using System.Windows;
using DevExpress.Mvvm.UI;
using EasyProg.WPF.MVVM.Services;
using TSettings;

namespace EasyProg.WPF.MVVM.Behaviors
{
    public class SaveStateWindowBehavior : WindowAwareServiceBase
    {
        protected override void OnActualWindowChanged(Window oldWindow)
        {
            if (oldWindow != null)
            {
                oldWindow.Closing -= ActualWindow_OnClosing;
            }
            if (ActualWindow != null)
            {
                ActualWindow.Closing += ActualWindow_OnClosing;
                RestoreWindowState();
            }
        }

        private void ActualWindow_OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var settings = this.Resolve<Settings>();
            settings.Set(SettingsKey, new WindowStateSettings
            {
                WindowSize = new Size(ActualWindow.Width, ActualWindow.Height),
                Location = new Point(ActualWindow.Left, ActualWindow.Top),
                WindowStartupLocation = ActualWindow.WindowStartupLocation,
                WindowState = ActualWindow.WindowState
            });
            settings.Save();
        }

        private void RestoreWindowState()
        {
            var windowStateSettings = this.Resolve<Settings>().Get(SettingsKey, new WindowStateSettings
            {
                WindowSize = DefaultSize,
                Location = DefaultLocation,
                WindowStartupLocation = DefaultWindowStartupLocation,
                WindowState = DefaultWindowState
            });
			ActualWindow.BeginInit();
            ActualWindow.Height = windowStateSettings.WindowSize.Height;
            ActualWindow.Width = windowStateSettings.WindowSize.Width;
            ActualWindow.WindowStartupLocation = windowStateSettings.WindowStartupLocation;
			ActualWindow.WindowState = windowStateSettings.WindowState;
	        if (ActualWindow.WindowStartupLocation == WindowStartupLocation.Manual)
	        {
				ActualWindow.Left = windowStateSettings.Location.X;
				ActualWindow.Top = windowStateSettings.Location.Y;
			}
			ActualWindow.EndInit();
        }

        public static readonly DependencyProperty SettingsKeyProperty = DependencyProperty.Register(
            "SettingsKey", typeof(string), typeof(SaveStateWindowBehavior), new PropertyMetadata(default(string)));

        public string SettingsKey
        {
            get { return (string) GetValue(SettingsKeyProperty); }
            set { SetValue(SettingsKeyProperty, value); }
        }

        public static readonly DependencyProperty DefaultSizeProperty = DependencyProperty.Register(
            "DefaultSize", typeof(Size), typeof(SaveStateWindowBehavior), new PropertyMetadata(default(Size)));

        public Size DefaultSize
        {
            get { return (Size) GetValue(DefaultSizeProperty); }
            set { SetValue(DefaultSizeProperty, value); }
        }

        public static readonly DependencyProperty DefaultLocationProperty = DependencyProperty.Register(
            "DefaultLocation", typeof(Point), typeof(SaveStateWindowBehavior), new PropertyMetadata(default(Point)));

        public Point DefaultLocation
        {
            get { return (Point) GetValue(DefaultLocationProperty); }
            set { SetValue(DefaultLocationProperty, value); }
        }

        public static readonly DependencyProperty DefaultWindowStateProperty = DependencyProperty.Register(
            "DefaultWindowState", typeof(WindowState), typeof(SaveStateWindowBehavior), new PropertyMetadata(default(WindowState)));

        public WindowState DefaultWindowState
        {
            get { return (WindowState) GetValue(DefaultWindowStateProperty); }
            set { SetValue(DefaultWindowStateProperty, value); }
        }

        public static readonly DependencyProperty DefaultWindowStartupLocationProperty = DependencyProperty.Register(
            "DefaultWindowStartupLocation", typeof(WindowStartupLocation), typeof(SaveStateWindowBehavior), new PropertyMetadata(default(WindowStartupLocation)));

        public WindowStartupLocation DefaultWindowStartupLocation
        {
            get { return (WindowStartupLocation) GetValue(DefaultWindowStartupLocationProperty); }
            set { SetValue(DefaultWindowStartupLocationProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
		}

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
