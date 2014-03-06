using System;
using System.Collections.Generic;
using Neo4jClient;

namespace NSBEndpointAndMessageDetection
{
    public class GraphDbRenderer
    {
        public void RenderClient(IList<IInstance> instances, string neo4JUrl)
        {
            var client = new GraphClient(new Uri(neo4JUrl));
            client.Connect();
                
        }
    }
}