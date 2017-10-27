using System;
using System.IO;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	public interface IDragFile
	{
		string Name { get; }

		long Length { get; }

		DateTime CreatedTime { get; }

		Stream GetContent();
	}
}