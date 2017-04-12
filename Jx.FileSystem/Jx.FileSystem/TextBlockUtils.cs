using System;
using System.IO;
namespace Jx.FileSystem
{
	public static class TextBlockUtils
	{
		public static TextBlock LoadFromVirtualFile(string path, out string errorString, out bool fileNotFound)
		{
			errorString = null;
			fileNotFound = false;
			TextBlock result;
			try
			{
				using (Stream stream = VirtualFile.Open(path))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						string arg;
						TextBlock textBlock = TextBlock.Parse(streamReader.ReadToEnd(), out arg);
						if (textBlock == null)
						{
							errorString = string.Format("Parsing text block failed \"{0}\" ({1}).", path, arg);
						}
						result = textBlock;
					}
				}
			}
			catch (FileNotFoundException)
			{
				errorString = string.Format("Reading file failed \"{0}\".", path);
				fileNotFound = true;
				result = null;
			}
			catch (Exception)
			{
				errorString = string.Format("Reading file failed \"{0}\".", path);
				result = null;
			}
			return result;
		}

		public static TextBlock LoadFromVirtualFile(string path, out string errorString)
		{
			bool flag;
			return LoadFromVirtualFile(path, out errorString, out flag);
		}

		public static TextBlock LoadFromVirtualFile(string path)
		{
			string text;
			TextBlock textBlock = LoadFromVirtualFile(path, out text);
			if (textBlock == null)
			{
				Log.Error(text);
			}
			return textBlock;
		}

		public static TextBlock LoadFromRealFile(string path, out string errorString, out bool fileNotFound)
		{
			errorString = null;
			fileNotFound = false;
			TextBlock result;
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader streamReader = new StreamReader(fileStream))
					{
						string arg;
						TextBlock textBlock = TextBlock.Parse(streamReader.ReadToEnd(), out arg);
						if (textBlock == null)
						{
							errorString = string.Format("Parsing text block failed \"{0}\" ({1}).", path, arg);
						}
						result = textBlock;
					}
				}
			}
			catch (FileNotFoundException)
			{
				errorString = string.Format("Reading file failed \"{0}\".", path);
				fileNotFound = true;
				result = null;
			}
			catch (Exception)
			{
				errorString = string.Format("Reading file failed \"{0}\".", path);
				result = null;
			}
			return result;
		}

		public static TextBlock LoadFromRealFile(string path, out string errorString)
		{
			bool flag;
			return LoadFromRealFile(path, out errorString, out flag);
		}

		public static TextBlock LoadFromRealFile(string path)
		{
			string text;
			TextBlock textBlock = LoadFromRealFile(path, out text);
			if (textBlock == null)
			{
				Log.Error(text);
			}
			return textBlock;
		}
	}
}
