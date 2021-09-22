using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
	//IdentityDbContext is for users involved with asp.net identity
	public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, int>
	{
      
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
		{
		}

		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//Now we will insert default role of a user to table
			modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User" });
			base.OnModelCreating(modelBuilder);
		}
	}

}
