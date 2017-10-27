using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using EasyProg.WPF.MVVM.Behaviors.DragDropFiles.Win32;

namespace EasyProg.WPF.MVVM.Behaviors.DragDropFiles
{
	public class InputFileDataObject : System.Windows.IDataObject
	{
		#region Property(s)

		/// <summary>
		/// Holds the <see cref="System.Windows.IDataObject"/> that this class is wrapping
		/// </summary>
		private readonly System.Windows.IDataObject _underlyingDataObject;

		/// <summary>
		/// Holds the <see cref="System.Runtime.InteropServices.ComTypes.IDataObject"/> interface to the <see cref="System.Windows.IDataObject"/> that this class is wrapping.
		/// </summary>
		private System.Runtime.InteropServices.ComTypes.IDataObject _comUnderlyingDataObject;

		/// <summary>
		/// Holds the internal ole <see cref="System.Windows.IDataObject"/> to the <see cref="System.Windows.IDataObject"/> that this class is wrapping.
		/// </summary>
		private System.Windows.IDataObject _oleUnderlyingDataObject;

		/// <summary>
		/// Holds the <see cref="MethodInfo"/> of the "GetDataFromHGLOBLAL" method of the internal ole <see cref="System.Windows.IDataObject"/>.
		/// </summary>
		private MethodInfo getDataFromHGLOBLALMethod;

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="InputFileDataObject"/> class.
		/// </summary>
		/// <param name="underlyingDataObject">The underlying data object to wrap.</param>
		public InputFileDataObject(System.Windows.IDataObject underlyingDataObject)
		{
			//get the underlying dataobject and its ComType IDataObject interface to it
			this._underlyingDataObject = underlyingDataObject;
			this._comUnderlyingDataObject = (System.Runtime.InteropServices.ComTypes.IDataObject)this._underlyingDataObject;

			//get the internal ole dataobject and its GetDataFromHGLOBLAL so it can be called later
			FieldInfo innerDataField = this._underlyingDataObject.GetType().GetField("_innerData", BindingFlags.NonPublic | BindingFlags.Instance);
			this._oleUnderlyingDataObject = (System.Windows.IDataObject)innerDataField.GetValue(this._underlyingDataObject);
			this.getDataFromHGLOBLALMethod = this._oleUnderlyingDataObject.GetType().GetMethod("GetDataFromHGLOBLAL", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		#endregion

		#region IDataObject Members

		/// <summary>
		/// Retrieves the data associated with the specified class type format.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type"></see> representing the format of the data to retrieve. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <returns>
		/// The data associated with the specified format, or null.
		/// </returns>
		public object GetData(Type format)
		{
			return this.GetData(format.FullName);
		}

		/// <summary>
		/// Retrieves the data associated with the specified data format.
		/// </summary>
		/// <param name="format">The format of the data to retrieve. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <returns>
		/// The data associated with the specified format, or null.
		/// </returns>
		public object GetData(string format)
		{
			return this.GetData(format, true);
		}

		/// <summary>
		/// Retrieves the data associated with the specified data format, using a Boolean to determine whether to convert the data to the format.
		/// </summary>
		/// <param name="format">The format of the data to retrieve. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="autoConvert">true to convert the data to the specified format; otherwise, false.</param>
		/// <returns>
		/// The data associated with the specified format, or null.
		/// </returns>
		public object GetData(string format, bool autoConvert)
		{
			//handle the "FileGroupDescriptor" and "FileContents" format request in this class otherwise pass through to underlying IDataObject 
			switch (format)
			{
				case "FileGroupDescriptor":
					//override the default handling of FileGroupDescriptor which returns a
					//MemoryStream and instead return a string array of file names
					IntPtr fileGroupDescriptorAPointer = IntPtr.Zero;
					try
					{
						//use the underlying IDataObject to get the FileGroupDescriptor as a MemoryStream
						MemoryStream fileGroupDescriptorStream = (MemoryStream)this._underlyingDataObject.GetData("FileGroupDescriptor", autoConvert);
						byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
						fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
						fileGroupDescriptorStream.Close();

						//copy the file group descriptor into unmanaged memory 
						fileGroupDescriptorAPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
						Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorAPointer, fileGroupDescriptorBytes.Length);

						//marshal the unmanaged memory to to FILEGROUPDESCRIPTORA struct
						object fileGroupDescriptorObject = Marshal.PtrToStructure(fileGroupDescriptorAPointer, typeof(FILEGROUPDESCRIPTORA));
						FILEGROUPDESCRIPTORA fileGroupDescriptor = (FILEGROUPDESCRIPTORA)fileGroupDescriptorObject;

						//create a new array to store file names in of the number of items in the file group descriptor
						string[] fileNames = new string[fileGroupDescriptor.cItems];

						//get the pointer to the first file descriptor
						IntPtr fileDescriptorPointer = (IntPtr)((int)fileGroupDescriptorAPointer + Marshal.SizeOf(fileGroupDescriptorAPointer));

						//loop for the number of files acording to the file group descriptor
						for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
						{
							//marshal the pointer top the file descriptor as a FILEDESCRIPTORA struct and get the file name
							FILEDESCRIPTORA fileDescriptor = (FILEDESCRIPTORA)Marshal.PtrToStructure(fileDescriptorPointer, typeof(FILEDESCRIPTORA));
							fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

							//move the file descriptor pointer to the next file descriptor
							fileDescriptorPointer = (IntPtr)((int)fileDescriptorPointer + Marshal.SizeOf(fileDescriptor));
						}

						//return the array of filenames
						return fileNames;
					}
					finally
					{
						//free unmanaged memory pointer
						Marshal.FreeHGlobal(fileGroupDescriptorAPointer);
					}

				case "FileGroupDescriptorW":
					//override the default handling of FileGroupDescriptorW which returns a
					//MemoryStream and instead return a string array of file names
					IntPtr fileGroupDescriptorWPointer = IntPtr.Zero;
					try
					{
						//use the underlying IDataObject to get the FileGroupDescriptorW as a MemoryStream
						MemoryStream fileGroupDescriptorStream = (MemoryStream)this._underlyingDataObject.GetData("FileGroupDescriptorW");
						byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
						fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
						fileGroupDescriptorStream.Close();

						//copy the file group descriptor into unmanaged memory
						fileGroupDescriptorWPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
						Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorWPointer, fileGroupDescriptorBytes.Length);

						//marshal the unmanaged memory to to FILEGROUPDESCRIPTORW struct
						object fileGroupDescriptorObject = Marshal.PtrToStructure(fileGroupDescriptorWPointer, typeof(FILEGROUPDESCRIPTORW));
						FILEGROUPDESCRIPTORW fileGroupDescriptor = (FILEGROUPDESCRIPTORW)fileGroupDescriptorObject;

						//create a new array to store file names in of the number of items in the file group descriptor
						string[] fileNames = new string[fileGroupDescriptor.cItems];

						//get the pointer to the first file descriptor
						IntPtr fileDescriptorPointer = (IntPtr)((int)fileGroupDescriptorWPointer + Marshal.SizeOf(fileGroupDescriptorWPointer));

						//loop for the number of files acording to the file group descriptor
						for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
						{
							//marshal the pointer top the file descriptor as a FILEDESCRIPTORW struct and get the file name
							FILEDESCRIPTORW fileDescriptor = (FILEDESCRIPTORW)Marshal.PtrToStructure(fileDescriptorPointer, typeof(FILEDESCRIPTORW));
							fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

							//move the file descriptor pointer to the next file descriptor
							fileDescriptorPointer = (IntPtr)((int)fileDescriptorPointer + Marshal.SizeOf(fileDescriptor));
						}

						//return the array of filenames
						return fileNames;
					}
					finally
					{
						//free unmanaged memory pointer
						Marshal.FreeHGlobal(fileGroupDescriptorWPointer);
					}

				case "FileContents":
					//override the default handling of FileContents which returns the
					//contents of the first file as a memory stream and instead return
					//a array of MemoryStreams containing the data to each file dropped

					//get the array of filenames which lets us know how many file contents exist
					string[] fileContentNames = (string[])this.GetData("FileGroupDescriptor");

					//create a MemoryStream array to store the file contents
					MemoryStream[] fileContents = new MemoryStream[fileContentNames.Length];

					//loop for the number of files acording to the file names
					for (int fileIndex = 0; fileIndex < fileContentNames.Length; fileIndex++)
					{
						//get the data at the file index and store in array
						fileContents[fileIndex] = this.GetData(format, fileIndex);
					}

					//return array of MemoryStreams containing file contents
					return fileContents;
			}

			//use underlying IDataObject to handle getting of data
			return this._underlyingDataObject.GetData(format, autoConvert);
		}

		/// <summary>
		/// Retrieves the data associated with the specified data format at the specified index.
		/// </summary>
		/// <param name="format">The format of the data to retrieve. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="index">The index of the data to retrieve.</param>
		/// <returns>
		/// A <see cref="MemoryStream"/> containing the raw data for the specified data format at the specified index.
		/// </returns>
		public MemoryStream GetData(string format, int index)
		{
			//create a FORMATETC struct to request the data with
			FORMATETC formatetc = new FORMATETC
			{
				cfFormat = (short) DataFormats.GetDataFormat(format).Id,
				dwAspect = DVASPECT.DVASPECT_CONTENT,
				lindex = index,
				ptd = new IntPtr(0),
				tymed = TYMED.TYMED_ISTREAM | TYMED.TYMED_ISTORAGE | TYMED.TYMED_HGLOBAL
			};

			//create STGMEDIUM to output request results into
			STGMEDIUM medium = new STGMEDIUM();

			//using the Com IDataObject interface get the data using the defined FORMATETC
			this._comUnderlyingDataObject.GetData(ref formatetc, out medium);

			//retrieve the data depending on the returned store type
			switch (medium.tymed)
			{
				case TYMED.TYMED_ISTORAGE:
					//to handle a IStorage it needs to be written into a second unmanaged
					//memory mapped storage and then the data can be read from memory into
					//a managed byte and returned as a MemoryStream

					IStorage iStorage = null;
					IStorage iStorage2 = null;
					ILockBytes iLockBytes = null;
					System.Runtime.InteropServices.ComTypes.STATSTG iLockBytesStat;
					try
					{
						//marshal the returned pointer to a IStorage object
						iStorage = (IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);
						Marshal.Release(medium.unionmember);

						//create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
						iLockBytes = Win32.NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true);
						iStorage2 = Win32.NativeMethods.StgCreateDocfileOnILockBytes(iLockBytes, 0x00001012, 0);

						//copy the returned IStorage into the new IStorage
						iStorage.CopyTo(0, null, IntPtr.Zero, iStorage2);
						iLockBytes.Flush();
						iStorage2.Commit(0);

						//get the STATSTG of the ILockBytes to determine how many bytes were written to it
						iLockBytesStat = new System.Runtime.InteropServices.ComTypes.STATSTG();
						iLockBytes.Stat(out iLockBytesStat, 1);
						int iLockBytesSize = (int)iLockBytesStat.cbSize;

						//read the data from the ILockBytes (unmanaged byte array) into a managed byte array
						byte[] iLockBytesContent = new byte[iLockBytesSize];
						iLockBytes.ReadAt(0, iLockBytesContent, iLockBytesContent.Length, null);

						//wrapped the managed byte array into a memory stream and return it
						return new MemoryStream(iLockBytesContent);
					}
					finally
					{
						//release all unmanaged objects
						Marshal.ReleaseComObject(iStorage2);
						Marshal.ReleaseComObject(iLockBytes);
						Marshal.ReleaseComObject(iStorage);
					}

				case TYMED.TYMED_ISTREAM:
					//to handle a IStream it needs to be read into a managed byte and
					//returned as a MemoryStream

					IStream iStream = null;
					System.Runtime.InteropServices.ComTypes.STATSTG iStreamStat;
					try
					{
						//marshal the returned pointer to a IStream object
						iStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
						Marshal.Release(medium.unionmember);

						//get the STATSTG of the IStream to determine how many bytes are in it
						iStreamStat = new System.Runtime.InteropServices.ComTypes.STATSTG();
						iStream.Stat(out iStreamStat, 0);
						int iStreamSize = (int)iStreamStat.cbSize;

						//read the data from the IStream into a managed byte array
						byte[] iStreamContent = new byte[iStreamSize];
						iStream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);

						//wrapped the managed byte array into a memory stream and return it
						return new MemoryStream(iStreamContent);
					}
					finally
					{
						//release all unmanaged objects
						Marshal.ReleaseComObject(iStream);
					}

				case TYMED.TYMED_HGLOBAL:
					//to handle a HGlobal the exisitng "GetDataFromHGLOBLAL" method is invoked via
					//reflection

					return (MemoryStream)this.getDataFromHGLOBLALMethod.Invoke(this._oleUnderlyingDataObject, new object[]
					{
						DataFormats.GetDataFormat(formatetc.cfFormat).Name, medium.unionmember
					});
			}

			return null;
		}

		/// <summary>
		/// Determines whether data stored in this instance is associated with, or can be converted to, the specified format.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type"></see> representing the format for which to check. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <returns>
		/// true if data stored in this instance is associated with, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(Type format)
		{
			return this._underlyingDataObject.GetDataPresent(format);
		}

		/// <summary>
		/// Determines whether data stored in this instance is associated with, or can be converted to, the specified format.
		/// </summary>
		/// <param name="format">The format for which to check. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <returns>
		/// true if data stored in this instance is associated with, or can be converted to, the specified format; otherwise false.
		/// </returns>
		public bool GetDataPresent(string format)
		{
			return this._underlyingDataObject.GetDataPresent(format);
		}

		/// <summary>
		/// Determines whether data stored in this instance is associated with the specified format, using a Boolean value to determine whether to convert the data to the format.
		/// </summary>
		/// <param name="format">The format for which to check. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="autoConvert">true to determine whether data stored in this instance can be converted to the specified format; false to check whether the data is in the specified format.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(string format, bool autoConvert)
		{
			return this._underlyingDataObject.GetDataPresent(format, autoConvert);
		}

		/// <summary>
		/// Returns a list of all formats that data stored in this instance is associated with or can be converted to.
		/// </summary>
		/// <returns>
		/// An array of the names that represents a list of all formats that are supported by the data stored in this object.
		/// </returns>
		public string[] GetFormats()
		{
			return this._underlyingDataObject.GetFormats();
		}

		/// <summary>
		/// Gets a list of all formats that data stored in this instance is associated with or can be converted to, using a Boolean value to determine whether to retrieve all formats that the data can be converted to or only native data formats.
		/// </summary>
		/// <param name="autoConvert">true to retrieve all formats that data stored in this instance is associated with or can be converted to; false to retrieve only native data formats.</param>
		/// <returns>
		/// An array of the names that represents a list of all formats that are supported by the data stored in this object.
		/// </returns>
		public string[] GetFormats(bool autoConvert)
		{
			return this._underlyingDataObject.GetFormats(autoConvert);
		}

		/// <summary>
		/// Stores the specified data in this instance, using the class of the data for the format.
		/// </summary>
		/// <param name="data">The data to store.</param>
		public void SetData(object data)
		{
			this._underlyingDataObject.SetData(data);
		}

		/// <summary>
		/// Stores the specified data and its associated class type in this instance.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type"></see> representing the format associated with the data. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="data">The data to store.</param>
		public void SetData(Type format, object data)
		{
			this._underlyingDataObject.SetData(format, data);
		}

		public void SetData(string format, object data, bool autoConvert)
		{
			SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data and its associated format in this instance.
		/// </summary>
		/// <param name="format">The format associated with the data. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="data">The data to store.</param>
		public void SetData(string format, object data)
		{
			this._underlyingDataObject.SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data and its associated format in this instance, using a Boolean value to specify whether the data can be converted to another format.
		/// </summary>
		/// <param name="format">The format associated with the data. See <see cref="T:System.Windows.DataFormats"></see> for predefined formats.</param>
		/// <param name="autoConvert">true to allow the data to be converted to another format; otherwise, false.</param>
		/// <param name="data">The data to store.</param>
		public void SetData(string format, bool autoConvert, object data)
		{
			this._underlyingDataObject.SetData(format, data, autoConvert);
		}

		#endregion
	}
}