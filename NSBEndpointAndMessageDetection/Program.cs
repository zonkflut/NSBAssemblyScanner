using System;
using System.Linq;

namespace NSBEndpointAndMessageDetection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var scanner = new NsbAssemblyScanner();

            var results = scanner.Scan(args[0]);
                
            if (results.Any())
            {
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            else
            {
                Console.WriteLine("No NServiceBus Directives found");
            }
        }
    }
}
