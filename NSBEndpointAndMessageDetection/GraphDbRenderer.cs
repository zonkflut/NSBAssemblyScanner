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
            //var query = client
            //    .Cypher
            //    .Start(new { root = client.RootNode })
            //    .Match("root-[:HAS_BOOK]->book")
            //    .Where((Book bk) => bk.Pages > 5)
            //    .Return(book => book.As<Book>());
    
            //client.Cypher
            //    .CreateUnique()
        }
    }
}