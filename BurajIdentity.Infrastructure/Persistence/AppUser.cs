using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
	public class AppUser : IdentityUser<int> //IdentityRole generic is string type primary key for DB by default but PostgreSql primary key is int so we used int generic.
	{
		//as an extra to IdentityRole base class parameters we will add Name parameter
		public string Name { get; set; }
	}
}
