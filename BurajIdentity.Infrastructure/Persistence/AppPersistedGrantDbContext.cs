using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
	//PersistedGrantDbContext - used for temporary operational data such as authorization codes, and refresh tokens
	public class AppPersistedGrantDbContext : PersistedGrantDbContext
	{
      

        //initializing the construction
        public AppPersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {

        }

        ///OnModelCreating is one of the most common used method in entity framework
        //It is the method triggered when the table has being created in database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);
		}
	}
}
