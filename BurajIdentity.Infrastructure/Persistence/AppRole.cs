using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Persistence
{
    public class AppRole : IdentityRole<int> //IdentityRole generic is string type primary key for DB by default but PostgreSql primary key is int so we used int generic.
    {
    }
}
