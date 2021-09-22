using BurajIdentity.Infrastructure.Persistence;
using BurajIdentity.Infrastructure.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BurajIdentity.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		
		//The first one is a IserviceCollection because our extension that returns a service, will be added to services in startup.
		//The second one is IConfiguration due to connection string that we will write in configuration file(appsetting.json).
		//in order to get connection string we neeed a configuration parameter.
		//we will define this class in startup.cs which has already included configuration service so we will not need DI operation.

		public static IServiceCollection AddIdentityServerConfig(this IServiceCollection services, IConfiguration configuration)
		{
			//Settings of identity server service
			services.AddIdentity<AppUser, AppRole>(options =>
			{
				options.User.RequireUniqueEmail = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 0;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireDigit = false;
				options.Password.RequireNonAlphanumeric = false;
				options.User.AllowedUserNameCharacters = "abcçdefghıijklmnoöprsştuüvyzABCÇDEFGHIİJKLMNOÖPRSŞTUÜVYZ0123456789-._@+'#!/^%{}*";
			}).AddEntityFrameworkStores<AppIdentityDbContext>()
			.AddDefaultTokenProviders();



			services.AddIdentityServer(options =>
            {
				options.EmitStaticAudienceClaim = true;
				options.Csp.Level = CspLevel.One; // For old browsers 
				options.Csp.AddDeprecatedHeader = true;
            })
			.AddDeveloperSigningCredential()
			.AddOperationalStore(options => 
		{
				//Connection string is in configuration file(appsetting.json) and its key is DefaultConnection.
				options.ConfigureDbContext = builder => builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BurajIdentity.Infrastructure"));
				options.EnableTokenCleanup = true;
			})
			.AddConfigurationStore(options => 
		{
				options.ConfigureDbContext = builder => builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BurajIdentity.Infrastructure"));
			})
			.AddAspNetIdentity<AppUser>();
						
			return services;
		}

		public static IServiceCollection AddServices<TUser>(this IServiceCollection services) where TUser : IdentityUser<int>, new()
		{
			services.AddTransient<IProfileService, IdentityClaimsProfileService>();
			return services;
		}

		public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, string connectionString)
		{
			
			services.AddDbContext<AppIdentityDbContext>(options => options.UseNpgsql(connectionString));
			services.AddDbContext<AppPersistedGrantDbContext>(options => options.UseNpgsql(connectionString));
			services.AddDbContext<AppConfigurationDbContext>(options => options.UseNpgsql(connectionString));
			return services;
		}

		//Package Manager Console(PMC) migrations commands. we will use -Project BurajIdentity.Infrastructure
		//Because we migrate all contexts in BurajIdentity.Infrastructure class library.
		//Commands:
		//Add-Migration FirstMigration -context AppIdentityDbContext -Project BurajIdentity.Infrastructure
		//Update-Database -context AppIdentityDbContext
		//Add-Migration FirstMigration -context AppPersistedGrantDbContext -Project BurajIdentity.Infrastructure
		//Update-Database -context AppPersistedGrantDbContext
		//Add-Migration FirstMigration -context AppConfigurationDbContext -Project BurajIdentity.Infrastructure
		//Update-Database -context AppConfigurationDbContext
	}
}
