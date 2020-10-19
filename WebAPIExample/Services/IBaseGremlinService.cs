using Gremlin.Net.Driver;
using Gremlin.Net.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbGremlinExample
{
    public interface IBaseGremlinService
    {
        Task SeedData();
        Task<ResultSet<TResult>> ExecuteGremlinQuery<TResult>(GremlinQuery query);
    }
}
