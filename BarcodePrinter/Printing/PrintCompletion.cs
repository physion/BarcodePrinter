using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BarcodePrinter.Printing
{
    class PrintCompletion
    {
        public IScreen Screen { get; private set; }

        public PrintCompletion(IScreen sender)
        {
            Screen = sender;
        }
    }
}
