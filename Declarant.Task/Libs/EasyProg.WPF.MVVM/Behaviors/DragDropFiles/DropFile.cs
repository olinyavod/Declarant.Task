using System.IO;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	class DropFile : IDropFile
	{
		public DropFile(string fileName, Stream stream)
		{
			Name = Path.GetFileNameWithoutExtension(fileName);
			Extension = Path.GetExtension(fileName);
			NameWithExtension = Path.GetFileName(fileName);
			Content = stream;
		}

		public string Name { get; private set; }

		public string Extension { get; private set; }

		public string NameWithExtension { get; private set; }

		public Stream Content { get; private set; }
	}
}