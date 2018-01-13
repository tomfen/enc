using System.Collections.Generic;

namespace enc
{
    interface IExperiment
    {
        string Command { get; }
        string Name { get; }
        string Description { get; }

        void Run(Dictionary<string, string> options);
    }
}
