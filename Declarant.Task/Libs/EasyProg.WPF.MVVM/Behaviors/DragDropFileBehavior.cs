using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using EasyProg.WPF.MVVM.Behaviors.DragDropFiles;

namespace EasyProg.WPF.MVVM.Behaviors
{
	public class DragDropFileBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register(
			"DropCommand", typeof (ICommand), typeof (DragDropFileBehavior), new PropertyMetadata(default(ICommand)));

		public ICommand DropCommand
		{
			get { return (ICommand) GetValue(DropCommandProperty); }
			set { SetValue(DropCommandProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.AllowDrop = AllowDrop;
			AssociatedObject.Drop += AssociatedObjectOnDrop;
			AssociatedObject.DragEnter += AssociatedObjectOnDragEnter;
			AssociatedObject.DragOver += AssociatedObjectOnDragOver;
			AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
			AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
			AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
			AssociatedObject.QueryContinueDrag += AssociatedObjectOnQueryContinueDrag;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.Drop -= AssociatedObjectOnDrop;
			AssociatedObject.DragEnter -= AssociatedObjectOnDragEnter;
			AssociatedObject.DragOver -= AssociatedObjectOnDragOver;
			AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
			AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
			AssociatedObject.MouseLeftButtonDown -= AssociatedObjectOnMouseLeftButtonDown;
		}

		private void AssociatedObjectOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if(e.EscapePressed)
				return;
			if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) == 0)
			{
				_dragDataObject.DragEnd();
			}
		}

		private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			_startDrag = false;
		}

		private OutputFileDataObject _dragDataObject;

		private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
		{
			var p = _startDragPoint - e.GetPosition(AssociatedObject);
			if (_startDrag && Math.Abs(p.Length) > 10)
			{
				_startDrag = false;
				var tempFiles = ToTempFiles(DragFiles).ToArray();
				try
				{
					_dragDataObject = new OutputFileDataObject(tempFiles);
					var dragged = DragDrop.DoDragDrop(AssociatedObject, _dragDataObject, DragDropEffects.Copy);
				}
				catch
				{

				}
				finally
				{
					foreach(var f in tempFiles)
						f.Dispose();
				}

			}
		}

		IEnumerable<TempDragFile> ToTempFiles(IEnumerable<IDragFile> files)
		{
			foreach (var f in files)
			{
				yield return new TempDragFile(f);
			}
		}

		private bool _startDrag;
		private Point _startDragPoint;

		private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_startDrag = AllowDrag && DragFiles != null && DragFiles.Any();
			_startDragPoint = e.GetPosition(AssociatedObject);
		}


		

		private void AssociatedObjectOnDragOver(object sender, DragEventArgs e)
		{
			e.Effects = OnCanDrop(e) ? DragDropEffects.Copy : DragDropEffects.None;
			e.Handled = true;
		}

		private bool OnCanDrop(DragEventArgs e)
		{
			var fs = e.Data.GetFormats();
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var names = (string[]) e.Data.GetData(DataFormats.FileDrop);
				var dropFiles = (names ?? new string[0]).Select(i => new DropFile(i, Stream.Null)).ToList();
				var result = DropCommand != null && DropCommand.CanExecute(dropFiles);
				foreach (var file in dropFiles)
				{
					file.Content.Dispose();
				}
				return result;
			}
			else if (e.Data.GetDataPresent("FileGroupDescriptorW"))
			{
				var data = new InputFileDataObject(e.Data);
				var names = (string[]) data.GetData("FileGroupDescriptorW");
				var dropFiles = (names ?? new string[0]).Select(i => new DropFile(i, Stream.Null)).ToList();
				var result = DropCommand != null && DropCommand.CanExecute(dropFiles);
				foreach (var file in dropFiles)
				{
					file.Content.Dispose();
				}
				return result;
			}
			else
			{
				return false;
			}
		}

		private void AssociatedObjectOnDragEnter(object sender, DragEventArgs e)
		{
			e.Effects = OnCanDrop(e) ? DragDropEffects.Copy : DragDropEffects.None;
			e.Handled = true;
		}

		private IEnumerable<IDropFile> GetFiles(IEnumerable<string> files)
		{
			foreach (var path in files)
			{
				var info = new FileInfo(path);
				yield return new DropFile(info.Name, info.OpenRead());
			}
		}

		private IEnumerable<IDropFile> GetFiles(InputFileDataObject data)
		{
			var names = (string[]) data.GetData("FileGroupDescriptorW");
			var streams = (MemoryStream[]) data.GetData("FileContents");
			for (int i = 0; names != null && streams != null && i < names.Length; i++)
			{
				yield return new DropFile(names[i], streams[i]);
			}
		}

		private void AssociatedObjectOnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var names = (string[])e.Data.GetData(DataFormats.FileDrop);
				DropCommand.Execute(GetFiles(names));
				e.Handled = true;
			}
			else if (e.Data.GetDataPresent("FileGroupDescriptorW"))
			{
				var data = new InputFileDataObject(e.Data);
				DropCommand.Execute(GetFiles(data));
				e.Handled = true;
			}
			
		}

		public static readonly DependencyProperty AllowDropProperty = DependencyProperty.Register(
			"AllowDrop", typeof (bool), typeof (DragDropFileBehavior), new PropertyMetadata(true, AllowDropPropertyChangedCallback));

		private static void AllowDropPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (DragDropFileBehavior) sender;
			if (control.AssociatedObject != null)
			{
				control.AssociatedObject.AllowDrop = (bool) e.NewValue;
			}
		}


		public bool AllowDrop
		{
			get { return (bool) GetValue(AllowDropProperty); }
			set { SetValue(AllowDropProperty, value); }
		}

		public static readonly DependencyProperty AllowDragProperty = DependencyProperty.Register(
			"AllowDrag", typeof (bool), typeof (DragDropFileBehavior), new PropertyMetadata(true));

		public bool AllowDrag
		{
			get { return (bool) GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}

		public static readonly DependencyProperty DragFilesProperty = DependencyProperty.Register(
			"DragFiles", typeof (IEnumerable<IDragFile>), typeof (DragDropFileBehavior), new PropertyMetadata(default(IEnumerable<IDragFile>)));

		public IEnumerable<IDragFile> DragFiles
		{
			get { return (IEnumerable<IDragFile>) GetValue(DragFilesProperty); }
			set { SetValue(DragFilesProperty, value); }
		}
	}
}