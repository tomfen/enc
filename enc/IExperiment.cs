using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enc
{
    interface IExperiment
    {
        string Command { get; }
        string Name { get; }
        string Description { get; }
        string Options { get; }

        void Run(Dictionary<string, string> options);
    }
}
