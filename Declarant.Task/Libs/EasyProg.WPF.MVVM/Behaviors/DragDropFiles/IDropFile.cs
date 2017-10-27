using System.IO;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	public interface IDropFile
	{
		string Name { get; }

		string Extension { get; }

		string NameWithExtension { get; }

		Stream Content { get; }
	}
}