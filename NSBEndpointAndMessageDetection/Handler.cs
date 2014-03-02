using System.Collections.Generic;

namespace NSBEndpointAndMessageDetection
{
    public class Handler : IInstance
    {
        public Handler()
        {
            Messages = new List<Message>();
        }

        public string AssemblyName { get; set; }

        public string Name { get; set; }

        public IList<Message> Messages { get; set; }
    }
}