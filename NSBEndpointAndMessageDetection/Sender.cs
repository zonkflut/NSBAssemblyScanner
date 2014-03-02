using System.Collections.Generic;

namespace NSBEndpointAndMessageDetection
{
    public class Sender : IInstance
    {
        public Sender()
        {
            Messages = new List<Message>();
        }

        public string AssemblyName { get; set; }

        public string Name { get; set; }

        public IList<Message> Messages { get; set; }
    }
}