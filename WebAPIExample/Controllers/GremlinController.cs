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
        public async Task<dynamic> GetUser()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Has("id", "thomas")
                .OutE().HasLabel("follows").InV().Count()
            .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
            return deserialized;
        }


        [HttpGet("topBookmarks")]
        public async Task<dynamic> getTopBookmarkedProducts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("product")
                .Group<dynamic, dynamic>()
                .By(__.Values<dynamic>("title"))
                .By(__.OutE().HasLabel("is bookmarked by").InV().HasLabel("person").Count())
                .Order(Scope.Local).By(Column.Values, Order.Decr).Limit<dynamic>(Scope.Local, 3)
               
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(queryRes));
            return deserialized;
        }


        [HttpGet("userFollowers")]
        public async Task<dynamic> getUsersAndFollowers()
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
            return deserialized;
        }


        [HttpGet("userProducts")]
        public async Task<dynamic> getTop5UsersWithPublishedProducts()
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
            return deserialized;
        }


        [HttpGet("usersTopBookmarkedProducts")]
        public async Task<dynamic> topUsersTopProducts()
        {
            var g = AnonymousTraversalSource.Traversal();
            GremlinQuery q = g.V()
                .HasLabel("person")
                .Group<dynamic, dynamic>()
                .By(__.Values<dynamic>("firstName"))
                .By(__.OutE().HasLabel("published")
                    .Group<dynamic, dynamic>()
                    .By(__.InV().HasLabel("product"))
                    .By(__.InV().HasLabel("product").OutE().HasLabel("is bookmarked by").Count())
                    .Order(Scope.Local).By(Column.Values, Order.Decr).Limit<dynamic>(Scope.Local, 5)
                    .Unfold<dynamic>()
                    .Project<dynamic>("title", new string[] { "bookmarks" })
                    .By(__.Select<dynamic>(Column.Keys).Unfold<dynamic>().Values<string>("title"))
                    .By(__.Select<dynamic>(Column.Values).Unfold<dynamic>()).Fold()                   
                )
                .ToGremlinQuery();

            var queryRes = await gremlinService.ExecuteGremlinQuery<dynamic>(q);
            var qq = JsonConvert.SerializeObject(queryRes);
            var deserialized = JsonConvert.DeserializeObject<dynamic>(qq);
            return deserialized;
        }
    }
}
