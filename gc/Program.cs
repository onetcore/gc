using System;
using System.Collections.Generic;

namespace gc
{
    class Program
    {
        private static readonly IDictionary<string, string> _args = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static void Main(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("--") && i < args.Length - 1)
                    _args[arg] = args[++i];
                else
                    _args[arg] = null;
            }

            Execute();
        }

        private static void Execute()
        {
            var main = new Main(_args);
            main.Run();
        }
    }
}
