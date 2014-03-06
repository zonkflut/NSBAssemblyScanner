using System;
using System.Collections;
using System.Linq;

namespace NSBEndpointAndMessageDetection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var results = new NsbAssemblyScanner().Scan(args[0]);
                if (results.Any())
                    new GraphDbRenderer().RenderClient(results, "http://localhost:7474/db/data");
                else
                    Console.WriteLine("No NServiceBus Directives found");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error Occurred. Ending Scanner.\r\n{0}", e);
            }
        }
    }
}
