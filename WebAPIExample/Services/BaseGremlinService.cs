using Gremlin.Net.Driver;
using Gremlin.Net.Extensions;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbGremlinExample
{
    public class BaseGremlinService : IBaseGremlinService
    {
        protected Container CosmosContainer { get; }
        public IGremlinClient GremlinClient { get; }

        public BaseGremlinService(CosmosClient cosmosClient, IGremlinClient gremlinClient)
        {
            CosmosContainer = cosmosClient.GetDatabase("example-db").GetContainer("main");
            GremlinClient = gremlinClient;
        }

        /// <summary>
        /// Executes queries
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResultSet<TResult>> ExecuteGremlinQuery<TResult>(GremlinQuery query)
        {
            ResultSet<TResult> results = null;
            results = await GremlinClient.SubmitAsync<TResult>(query.ToString(), new Dictionary<string, object>(query.Arguments));

            return results;
        }


        public async Task SeedData()
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
            {
                { "AddVertex 1",    "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44).property('partKey', 'person')" },
                { "AddVertex 2",    "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39).property('partKey', 'person')" },
                { "AddVertex 3",    "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller').property('partKey', 'person')" },
                { "AddVertex 5",    "g.addV('person').property('id', 'haily').property('firstName', 'Hauly').property('lastName', 'Holmes').property('partKey', 'person')" },
                { "AddVertex 6",    "g.addV('person').property('id', 'ana').property('firstName', 'Ana').property('lastName', 'White').property('partKey', 'person')" },
                { "AddVertex 7",    "g.addV('person').property('id', 'james').property('firstName', 'James').property('lastName', 'Charles').property('partKey', 'person')" },

                { "AddVertex 8",    "g.addV('product').property('id', '1').property('link', 'link to product').property('title', 'Book').property('partKey', 'product')" },
                { "AddVertex 9",    "g.addV('product').property('id', '2').property('link', 'link to product').property('title', 'T-Shirt').property('partKey', 'product')" },
                { "AddVertex 10",    "g.addV('product').property('id', '3').property('link', 'link to product').property('title', 'Shopping bag').property('partKey', 'product')" },
                { "AddVertex 11",    "g.addV('product').property('id', '4').property('link', 'link to product').property('title', 'Water bottle').property('partKey', 'product')" },
                
                { "AddEdge 1",      "g.V('thomas').addE('follows').to(g.V('mary'))" },
                { "AddEdge 2",      "g.V('mary').addE('is followed by').to(g.V('thomas'))" },

                { "AddEdge 3",      "g.V('thomas').addE('follows').to(g.V('ben'))" },
                { "AddEdge 4",      "g.V('ben').addE('is followed by').to(g.V('thomas'))" },

                { "AddEdge 5",      "g.V('ben').addE('follows').to(g.V('james'))" },
                { "AddEdge 6",      "g.V('james').addE('is followed by').to(g.V('ben'))" },

                { "AddEdge 7",      "g.V('ben').addE('follows').to(g.V('ana'))" },
                { "AddEdge 8",      "g.V('ana').addE('is followed by').to(g.V('ben'))" },

                { "AddEdge 9",      "g.V('james').addE('follows').to(g.V('haily'))" },
                { "AddEdge 10",      "g.V('haily').addE('is followed by').to(g.V('james'))" },

                { "AddEdge 11",      "g.V('ana').addE('follows').to(g.V('ben'))" },
                { "AddEdge 12",      "g.V('ben').addE('is followed by').to(g.V('ana'))" },

                { "AddEdge 13",      "g.V('ana').addE('published').to(g.V('1'))" },
                { "AddEdge 14",      "g.V('ana').addE('published').to(g.V('2'))" },
                { "AddEdge 15",      "g.V('james').addE('published').to(g.V('3'))" },
                { "AddEdge 16",      "g.V('ben').addE('published').to(g.V('4'))" },

                { "AddEdge 17",      "g.V('1').addE('is published by').to(g.V('ana'))" },
                { "AddEdge 18",      "g.V('2').addE('is published by').to(g.V('ana'))" },
                { "AddEdge 19",      "g.V('3').addE('is published by').to(g.V('james'))" },
                { "AddEdge 20",      "g.V('4').addE('is published by').to(g.V('ben'))" },

                { "AddEdge 21",      "g.V('thomas').addE('bookmarked').to(g.V('1'))" },
                { "AddEdge 22",      "g.V('thomas').addE('bookmarked').to(g.V('2'))" },
                { "AddEdge 23",      "g.V('thomas').addE('bookmarked').to(g.V('3'))" },
                { "AddEdge 24",      "g.V('ben').addE('bookmarked').to(g.V('1'))" },
                { "AddEdge 25",      "g.V('haily').addE('bookmarked').to(g.V('4'))" },
                { "AddEdge 26",      "g.V('ana').addE('bookmarked').to(g.V('4'))" },
                { "AddEdge 27",      "g.V('james').addE('bookmarked').to(g.V('4'))" },
                { "AddEdge 28",      "g.V('james').addE('bookmarked').to(g.V('3'))" },

                { "AddEdge 29",      "g.V('1').addE('is bookmarked by').to(g.V('thomas'))" },
                { "AddEdge 30",      "g.V('2').addE('is bookmarked by').to(g.V('thomas'))" },
                { "AddEdge 31",      "g.V('3').addE('is bookmarked by').to(g.V('thomas'))" },
                { "AddEdge 32",      "g.V('1').addE('is bookmarked by').to(g.V('ben'))" },
                { "AddEdge 33",      "g.V('4').addE('is bookmarked by').to(g.V('haily'))" },
                { "AddEdge 34",      "g.V('4').addE('is bookmarked by').to(g.V('ana'))" },
                { "AddEdge 35",      "g.V('4').addE('is bookmarked by').to(g.V('james'))" },
                { "AddEdge 36",      "g.V('3').addE('is bookmarked by').to(g.V('james'))" },
            };
          
            foreach (var query in gremlinQueries)
            {
                await GremlinClient.SubmitAsync<dynamic>(query.Value);
            }
        }
    }
}

