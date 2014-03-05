using System.Collections.Generic;
using System.Linq;
using NSBEndpointAndMessageDetection;

namespace ScannerTests
{
    public static class InstanceExtensions
    {
        public static IEnumerable<Message> GetMessages(this IList<IInstance> results, string assemblyName, string className, string messageName, string messageOperation)
        {
            return results
                .Where(i => i.AssemblyName.StartsWith(assemblyName))
                .Where(i => i.Name == className)
                .SelectMany(i => i.Messages)
                .Where(m => m.Name == messageName)
                .Where(m => m.Operation == messageOperation);
        }
    }
}