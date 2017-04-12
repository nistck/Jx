using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public static class PathUtils
    {
        public static string NormalizeSlashes(string path)
        {
            string text = path;
            bool isUnix = Environment.OSVersion.Platform == PlatformID.Unix;
            if (isUnix && text != null)
            {
                text = text.Replace('\\', '/');
            }
            return text;
        }

        public static bool IsCorrectFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < fileName.Length; i++)
            {
                char value = fileName[i];
                if (Array.IndexOf<char>(invalidFileNameChars, value) != -1)
                {
                    return false;
                }
            }
            return !(Path.GetExtension(fileName) != "") || !(Path.GetFileNameWithoutExtension(fileName) == "");
        }
    }
}
