using System.Collections.Generic;

namespace NSBEndpointAndMessageDetection
{
    public interface IInstance
    {
        string AssemblyName { get; set; }

        string Name { get; set; }

        IList<Message> Messages { get; set; }
    }
}