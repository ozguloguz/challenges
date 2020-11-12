using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OzgulOguz.Challenges
{
    public class Input
    {
        private string currentLine;
        private AutoResetEvent writeWait = new AutoResetEvent(false);
        private AutoResetEvent readWait = new AutoResetEvent(true);
        public bool WriteLog;

        public string ReadLine()
        {
            writeWait.WaitOne();
            readWait.Set();
            return currentLine;
        }

        internal void WriteLine(string line)
        {
            if (WriteLog) Console.Write(line.PadRight(35));
            readWait.WaitOne();
            currentLine = line;
            writeWait.Set();
        }
    }
}
