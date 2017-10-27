using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Windows.Forms;
using EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	class OutputFileDataObject : System.Windows.Forms.DataObject, System.Runtime.InteropServices.ComTypes.IDataObject, System.Windows.IDataObject
    {
        private static readonly TYMED[] ALLOWED_TYMEDS =
            new TYMED[] { 
                TYMED.TYMED_HGLOBAL,
                TYMED.TYMED_ISTREAM, 
                TYMED.TYMED_ENHMF,
                TYMED.TYMED_MFPICT,
                TYMED.TYMED_GDI};

        private TempDragFile[] m_SelectedItems;
        private Int32 m_lindex;

        public OutputFileDataObject(TempDragFile[] selectedItems)
        {
            m_SelectedItems = selectedItems;
	        var names = m_SelectedItems.Select(i => i.Name).ToArray();
			SetData(NativeMethods.CFSTR_FILEDROP, names);
			SetData(NativeMethods.CFSTR_FILENAME, names);
			SetData(NativeMethods.CFSTR_FILENAMEW, names);
			SetData(NativeMethods.CFSTR_FILEDESCRIPTORW, GetFileDescriptorW(selectedItems));
			//SetData(NativeMethods.CFSTR_FILEDESCRIPTOR, GetFileDescriptor(selectedItems));
			SetData(NativeMethods.CFSTR_FILECONTENTS, null);
        }

        public override object GetData(string format, bool autoConvert)
        {
	        if (new[]
	        {
		        NativeMethods.CFSTR_FILENAME,
		        NativeMethods.CFSTR_FILENAMEW,
		        NativeMethods.CFSTR_FILEDROP
	        }.Contains(format) && _dragEnd)
	        {
		        var list = new List<string>();
		        foreach (var item in m_SelectedItems)
		        {
			        if(!item.IsCopied)
						item.CopyFrom();
					list.Add(item.Name);
		        }
				SetData(format, list.ToArray());
	        }
            if (String.Compare(format, NativeMethods.CFSTR_FILECONTENTS, StringComparison.OrdinalIgnoreCase) == 0)
            {
                base.SetData(NativeMethods.CFSTR_FILECONTENTS, GetFileContents(m_SelectedItems, m_lindex));
            }
            else if (String.Compare(format, NativeMethods.CFSTR_PERFORMEDDROPEFFECT, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //TODO: Cleanup routines after paste has been performed
            }
            return base.GetData(format, autoConvert);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref System.Runtime.InteropServices.ComTypes.FORMATETC formatetc, out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
        {
            if (formatetc.cfFormat == (Int16)DataFormats.GetFormat(NativeMethods.CFSTR_FILECONTENTS).Id)
                m_lindex = formatetc.lindex;

            medium = new System.Runtime.InteropServices.ComTypes.STGMEDIUM();
            if (GetTymedUseable(formatetc.tymed))
            {
                if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != TYMED.TYMED_NULL)
                {
                    medium.tymed = TYMED.TYMED_HGLOBAL;
                    medium.unionmember = NativeMethods.GlobalAlloc(NativeMethods.GHND | NativeMethods.GMEM_DDESHARE, 1);
                    if (medium.unionmember == IntPtr.Zero)
                    {
                        throw new OutOfMemoryException();
                    }
                    try
                    {
                        ((System.Runtime.InteropServices.ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
                        return;
                    }
                    catch
                    {
                        NativeMethods.GlobalFree(new HandleRef((STGMEDIUM)medium, medium.unionmember));
                        medium.unionmember = IntPtr.Zero;
                        throw;
                    }
                }
                medium.tymed = formatetc.tymed;
                ((System.Runtime.InteropServices.ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
            }
            else
            {
                Marshal.ThrowExceptionForHR(NativeMethods.DV_E_TYMED);
            }
        }

        private static Boolean GetTymedUseable(TYMED tymed)
        {
            for (Int32 i = 0; i < ALLOWED_TYMEDS.Length; i++)
            {
                if ((tymed & ALLOWED_TYMEDS[i]) != TYMED.TYMED_NULL)
                {
                    return true;
                }
            }
            return false;
        }

        private MemoryStream GetFileDescriptor(IDragFile[] SelectedItems)
        {
            MemoryStream FileDescriptorMemoryStream = new MemoryStream();
            // Write out the FILEGROUPDESCRIPTOR.cItems value
            FileDescriptorMemoryStream.Write(BitConverter.GetBytes(SelectedItems.Length), 0, sizeof(UInt32));

            FILEDESCRIPTOR FileDescriptor = new FILEDESCRIPTOR();
            foreach (var si in SelectedItems)
            {
                FileDescriptor.cFileName = si.Name;
                Int64 FileWriteTimeUtc = si.CreatedTime.ToFileTimeUtc();
                FileDescriptor.ftLastWriteTime.dwHighDateTime = (Int32)(FileWriteTimeUtc >> 32);
                FileDescriptor.ftLastWriteTime.dwLowDateTime = (Int32)(FileWriteTimeUtc & 0xFFFFFFFF);
                FileDescriptor.nFileSizeHigh = (UInt32)(si.Length >> 32);
                FileDescriptor.nFileSizeLow = (UInt32)(si.Length & 0xFFFFFFFF);
                FileDescriptor.dwFlags = NativeMethods.FD_WRITESTIME | NativeMethods.FD_FILESIZE | NativeMethods.FD_PROGRESSUI;

                // Marshal the FileDescriptor structure into a byte array and write it to the MemoryStream.
                Int32 FileDescriptorSize = Marshal.SizeOf(FileDescriptor);
                IntPtr FileDescriptorPointer = Marshal.AllocHGlobal(FileDescriptorSize);
                Marshal.StructureToPtr(FileDescriptor, FileDescriptorPointer, true);
                Byte[] FileDescriptorByteArray = new Byte[FileDescriptorSize];
                Marshal.Copy(FileDescriptorPointer, FileDescriptorByteArray, 0, FileDescriptorSize);
                Marshal.FreeHGlobal(FileDescriptorPointer);
                FileDescriptorMemoryStream.Write(FileDescriptorByteArray, 0, FileDescriptorByteArray.Length);
            }
            return FileDescriptorMemoryStream;
        }

		private MemoryStream GetFileDescriptorW(IDragFile[] SelectedItems)
		{
			MemoryStream FileDescriptorMemoryStream = new MemoryStream();
			// Write out the FILEGROUPDESCRIPTOR.cItems value
			FileDescriptorMemoryStream.Write(BitConverter.GetBytes(SelectedItems.Length), 0, sizeof(UInt32));

			FILEDESCRIPTORW FileDescriptor = new FILEDESCRIPTORW();
			foreach (var si in SelectedItems)
			{
				FileDescriptor.cFileName = si.Name;
				Int64 FileWriteTimeUtc = si.CreatedTime.ToFileTimeUtc();
				FileDescriptor.ftLastWriteTime.dwHighDateTime = (Int32)(FileWriteTimeUtc >> 32);
				FileDescriptor.ftLastWriteTime.dwLowDateTime = (Int32)(FileWriteTimeUtc & 0xFFFFFFFF);
				FileDescriptor.nFileSizeHigh = (UInt32)(si.Length >> 32);
				FileDescriptor.nFileSizeLow = (UInt32)(si.Length & 0xFFFFFFFF);
				FileDescriptor.dwFlags = NativeMethods.FD_WRITESTIME | NativeMethods.FD_FILESIZE | NativeMethods.FD_PROGRESSUI;

				// Marshal the FileDescriptor structure into a byte array and write it to the MemoryStream.
				Int32 FileDescriptorSize = Marshal.SizeOf(FileDescriptor);
				IntPtr FileDescriptorPointer = Marshal.AllocHGlobal(FileDescriptorSize);
				Marshal.StructureToPtr(FileDescriptor, FileDescriptorPointer, true);
				Byte[] FileDescriptorByteArray = new Byte[FileDescriptorSize];
				Marshal.Copy(FileDescriptorPointer, FileDescriptorByteArray, 0, FileDescriptorSize);
				Marshal.FreeHGlobal(FileDescriptorPointer);
				FileDescriptorMemoryStream.Write(FileDescriptorByteArray, 0, FileDescriptorByteArray.Length);
			}
			return FileDescriptorMemoryStream;
		}

        private MemoryStream GetFileContents(IDragFile[] SelectedItems, Int32 FileNumber)
        {
            MemoryStream FileContentMemoryStream = null;
            if (SelectedItems != null && FileNumber < SelectedItems.Length)
            {
                FileContentMemoryStream = new MemoryStream();
                var si = SelectedItems[FileNumber];
	            using (var stream = si.GetContent())
	            {
		            stream.CopyTo(FileContentMemoryStream);
	            }

            }
            return FileContentMemoryStream;
        }

	    public void SetData(string format, object data, bool autoConvert)
		{
			SetData(format, autoConvert, data);
		}

		private bool _dragEnd;

		public void DragEnd()
		{
			_dragEnd = true;
		}
    }
}
