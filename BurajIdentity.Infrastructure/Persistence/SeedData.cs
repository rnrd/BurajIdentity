using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider provider)
        {
            //The IServiceProvider is responsible for resolving instances of types at runtime, as required by the application. 
            //These instances can be injected into other services resolved from the same dependency injection container.
            //In order to get service it has two main methods GetService  and GetRequiredService.
            //GetRequiredService throws exception in the runtime if the service it looks for is null. GetService does not throw exception.

            var configuration = provider.GetRequiredService<IConfiguration>();
            //Get contexts before runtime and migrate any database changes on startup (includes initial db creation)
            provider.GetRequiredService<AppIdentityDbContext>().Database.Migrate();
            provider.GetRequiredService<AppPersistedGrantDbContext>().Database.Migrate();
            provider.GetRequiredService<AppConfigurationDbContext>().Database.Migrate();
            var context = provider.GetRequiredService<AppConfigurationDbContext>();

            if (!context.ApiResources.Any())
            {
                //Get values of ApiSource in configuration file(appsettings.json)
                var apiResources = new List<ApiResource>();
                configuration.GetSection("IdentityServer:ApiResources").Bind(apiResources);
                //and add all apiResources to db
                foreach (var apiResource in apiResources)
                    context.ApiResources.Add(apiResource.ToEntity());
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                //Get values of ApiSource in configuration file(appsettings.json)
                var apiScopes = new List<ApiScope>();
                configuration.GetSection("IdentityServer:ApiScopes").Bind(apiScopes);
                //and add all apiResources to db
                foreach (var apiScope in apiScopes)
                    context.ApiScopes.Add(apiScope.ToEntity());
                    context.SaveChanges();
            }

            if (!context.Clients.Any())
            {
                var clients = new List<Client>();
                //Get values of Clients in configuration file(appsettings.json)
                configuration.GetSection("IdentityServer:Clients").Bind(clients);
                //and add all clients to db.
                foreach (var client in clients)
                    context.Clients.Add(client.ToEntity());
                    context.SaveChanges();
            }
  
            if (!context.IdentityResources.Any())
            {
                //Get values of Clients in configuration file(appsettings.json)
                var identityResources = new List<IdentityResource>();
                configuration.GetSection("IdentityServer:IdentityResources").Bind(identityResources);
                //and add all identity resources to db.
                foreach (var identityResource in identityResources)
                    context.IdentityResources.Add(identityResource.ToEntity());
                    context.SaveChanges();
            }


        }
    }
}
