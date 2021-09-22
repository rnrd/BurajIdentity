using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
	//ConfigurationDbContext - used for configuration data such as clients, resources, and scopes.
	public class AppConfigurationDbContext : ConfigurationDbContext
	{
		
		public AppConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
		{

		}
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);
		}
	}
}
