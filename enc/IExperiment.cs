using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enc
{
    interface IExperiment
    {
        string getCmd();

        void run();
    }
}
