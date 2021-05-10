using System;

namespace FastChia.common
{
    public class CommandEventArgs : EventArgs
    {

        public CommandEventArgs(string line)
        {
            this.line = line;
        }

        public string line { get; set; }

    }
}
