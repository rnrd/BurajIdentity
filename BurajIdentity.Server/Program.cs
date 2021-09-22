using BurajIdentity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BurajIdentity.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //In order to implement migrations by  CLI commands,
            //We need to run SeedData class just before the program run.
            var host = CreateHostBuilder(args).Build();
            //IServiceScopeFactory will create service in a scope.
            using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //we will send provider of the current scope which we have just created into our EnsureSeedData method located in SeedData class.
                SeedData.EnsureSeedData(scope.ServiceProvider);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //Our identity server will run on 5001 port of localhost.
                    //webBuilder.UseUrls("http://0.0.0.0:5001");
                });
    }
}
