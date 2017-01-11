using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemJam
{

    
    public class Logger
    {
        public event NewLogMessageDelegate NewLogMessage;
        public delegate void NewLogMessageDelegate(string message);

        public void LogMessage(string message)
        {
            if (NewLogMessage != null)
                NewLogMessage(message);
        }
    }
}
