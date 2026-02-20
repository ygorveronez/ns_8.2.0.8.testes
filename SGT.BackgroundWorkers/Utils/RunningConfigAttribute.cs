using System;
using System.Collections.Generic;
using System.Text;

namespace SGT.BackgroundWorkers.Utils
{
    public class RunningConfigAttribute : Attribute
    {
        public int DuracaoPadrao { get; set; }
    }
}
