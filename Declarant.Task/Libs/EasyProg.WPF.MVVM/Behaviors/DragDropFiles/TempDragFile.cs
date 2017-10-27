using System;
using System.IO;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	class TempDragFile : IDragFile, IDisposable
	{
		private readonly IDragFile _innerFile;
		private readonly FileInfo _fileInfo;

		public TempDragFile(IDragFile innerFile)
		{
			_innerFile = innerFile;
			_fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), "Escort_Tmp", innerFile.Name));
			Name = _fileInfo.FullName;
			Length = innerFile.Length;
			CreatedTime = innerFile.CreatedTime;
			if (!_fileInfo.Directory.Exists)
			{
				_fileInfo.Directory.Create();
			}
		}

		public string Name { get; private set; }

		public long Length { get; set; }

		public DateTime CreatedTime { get; set; }
		public bool IsCopied { get; private set; }

		public Stream GetContent()
		{
			if (!IsCopied)
				CopyFrom();
			return _fileInfo.OpenRead();

		}

		public void CopyFrom()
		{
			using (var temp = _fileInfo.OpenWrite())
			{
				using (var s = _innerFile.GetContent())
					s.CopyTo(temp);
			}
			IsCopied = true;
		}

		public void Dispose()
		{
				
		}
	}
}