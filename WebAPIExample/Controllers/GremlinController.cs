using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbGremlinExample;
using Gremlin.Net.Extensions;
using Gremlin.Net.Process.Traversal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPIExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GremlinController : ControllerBase
    {
        private readonly IBaseGremlinService gremlinService;

        public GremlinController(IBaseGremlinService gremlinService)
        {
            this.gremlinService = gremlinService;
        }


        [HttpPost("seedData")]
        public async Task SeedData()
        {
            await this.gremlinService.SeedData();
        }


        [HttpGet("follows")]
        public async Task GetUser()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Has("id", "thomas")
                .OutE().HasLabel("follows").InV().Count()
            .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }


        [HttpGet("topBookmarks")]
        public async Task getTopBookmarkedProducts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("product")
                .Group<dynamic, dynamic>()
                .By()
                .By(__.OutE().HasLabel("is bookmarked by").InV().HasLabel("person").Count())
                .Order(Scope.Local).By(Column.Values, Order.Decr)
               
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }


        [HttpGet("userFollowers")]
        public async Task getUsersAndFollowers()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Group<dynamic, dynamic>()
                .By()
                .By(__.OutE().HasLabel("is followed by").InV().HasLabel("person").Count())
                .Order(Scope.Local).By(Column.Values, Order.Decr)

                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }


        [HttpGet("userProducts")]
        public async Task getTop5UsersWithPublishedProducts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Group<dynamic, dynamic>()
                .By()
                .By(__.OutE().HasLabel("published").InV().HasLabel("product").Count())
                .Order(Scope.Local).By(Column.Values, Order.Decr)
                .Limit<int>(Scope.Local, 5)
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }


        [HttpGet("topUsersTopProducts")]
        public async Task topUsersTopProducts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Group<dynamic, dynamic>()
                .By()
                .By(__.OutE().HasLabel("published").InV().HasLabel("product")
                    .Project<dynamic>("title", "bookmarkers")
                    .By(__.Values<dynamic>("title"))
                    .By(__.OutE().HasLabel("is bookmarked by").InV().HasLabel("person").Count())
                    )
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }


        [HttpGet("accountsThatFollowEachOther")]
        public async Task getAccounts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()             
                .HasLabel("person")
                .Group<dynamic, dynamic>()
                .By()
                .By(__.OutE().HasLabel("is followed by").InV().HasLabel("person").Count())
                .Order(Scope.Local).By(Column.Values, Order.Decr)
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
        }





















        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
