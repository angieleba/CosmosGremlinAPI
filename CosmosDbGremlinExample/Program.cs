﻿

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CosmosDbGremlinExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();

            startup.ConfigureServices(services);
        }
    }
}
