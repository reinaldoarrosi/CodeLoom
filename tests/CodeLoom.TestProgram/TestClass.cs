using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeLoom.TestProgram
{
    public class TestClass
    {
        private Dictionary<int, string> _indexerValues = new Dictionary<int, string>();

        public string StoredString { get; set; }

        public string this[int index]
        {
            get { return _indexerValues[index]; }
            set { if (value == null) _indexerValues.Remove(index); else _indexerValues[index] = value; }
        }

        public void StoreString(string str)
        {
            StoredString = str;
        }

        public int DivideInt(int a, int b, ref int remainder)
        {
            var result = a / b;
            remainder = a % b;

            return result;
        }
    }
}
