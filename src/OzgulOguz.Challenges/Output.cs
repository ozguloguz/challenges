using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OzgulOguz.Challenges
{
    public class Output
    {
        private string currentLine;
        private AutoResetEvent writeWait = new AutoResetEvent(false);
        private AutoResetEvent readWait = new AutoResetEvent(true);
        public bool WriteLog;

        internal string ReadLine()
        {
            writeWait.WaitOne();
            string returnValue = currentLine;
            readWait.Set();
            return returnValue;
        }

        public void WriteLine(string line)
        {
            if (WriteLog) Console.Write(line.PadRight(10));
            readWait.WaitOne();
            currentLine = line;
            writeWait.Set();
        }
    }
}
