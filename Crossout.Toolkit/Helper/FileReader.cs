using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Crossout.Toolkit.Helper
{
    public static class FileReader
    {
        public static string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string readString = sr.ReadToEnd();
            sr.Close();
            return readString;
        }
    }
}
