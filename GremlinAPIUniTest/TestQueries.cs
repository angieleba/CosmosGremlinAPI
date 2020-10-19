using CosmosDbGremlinExample;
using Gremlin.Net.Driver;
using Gremlin.Net.Extensions;
using Gremlin.Net.Process.Traversal;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GremlinAPIUniTest
{
    [TestClass]
    public class TestQueries
    {
        private readonly IGremlinClient gremlinClient;

        public TestQueries()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IGremlinClient, GremlinClient>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IGremlinClient>();
            this.gremlinClient = service;
        }

        [TestMethod]
        public async Task TestRelationQuery()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Has("fistName", "robin")
                .OutE().HasLabel("knows").InV().ToGremlinQuery();

            //var queryRes = await baseGremlinService.ExecuteGremlinQuery<dynamic>(q);
            //var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
            await gremlinClient.SubmitAsync(q.ToString(), new Dictionary<string, object>(q.Arguments));

            Assert.AreEqual("", q.ToString(), "Query result is not as expected.");
        }
    }
}
