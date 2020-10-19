using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbGremlinExample
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            //GremlinClient
            GremlinServer gremlinServer = null;
            GremlinClient gremlinClient = null;
            try
            {
                gremlinServer = new GremlinServer(
                    "localhost", 8901, false, "/dbs/example-db/colls/main", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
                    );

                gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
            }
            catch (Exception e)
            {
                //Log the error
            }
            services.AddSingleton<IGremlinClient>(gremlinClient);


            //CosmosClient
            string connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(connectionString);
            var cosmosClient = cosmosClientBuilder.WithConnectionModeDirect().WithSerializerOptions(new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }).Build();
            services.AddSingleton<CosmosClient>(cosmosClient);

            //Inject services
            services.AddTransient<IBaseGremlinService, BaseGremlinService>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //Seed Data
            var service = serviceProvider.GetService<IBaseGremlinService>();
            service.SeedData().GetAwaiter().GetResult();
        }
    }
}
