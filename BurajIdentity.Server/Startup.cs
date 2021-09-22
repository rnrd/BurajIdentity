using BurajIdentity.Infrastructure.Extensions;
using BurajIdentity.Infrastructure.Persistence;
using BurajIdentity.Server.Models;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BurajIdentity.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //CORS policy
            services
                .AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            //Cookie Configuration
            services.ConfigureApplicationCookie(options =>
           {
               options.Cookie.Name = "Cookie";
               options.LoginPath = "/Account/Login";
               options.LogoutPath = "/Account/Logout";
               //options.Cookie.IsEssential = true;
               options.Cookie.SameSite = SameSiteMode.None;
               options.Cookie.SecurePolicy = CookieSecurePolicy.None;
               

           });

            

            //calling methods of ServiceCollectionExtensions class.
            services
                .AddDatabaseConfiguration(Configuration.GetConnectionString("DefaultConnection"))
                .AddIdentityServerConfig(Configuration)
                .AddServices<AppUser>();

           
            //Dependency injection
            services.AddScoped<LoginViewModel>();
            services.AddScoped<RegisterModel>();

            // Cookie policy stuff
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = (SameSiteMode)(-1);

                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });


            services.AddControllersWithViews();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });


            //in order to get jwt token for password and client_credentials grant types:
            //a GET request should be sent to http://localhost:46834/connect/token

        }

        //----------------------------------------------------------------------------------------
        //Dealing with same site issues
        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = (SameSiteMode)(-1);
                }
            }
        }

        private bool DisallowsSameSiteNone(string userAgent)
        {
            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking
            // stack.
            if (userAgent.Contains("CPU iPhone OS 12") ||
            userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. 
            // This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }
        //----------------------------------------------------------------------------------------------------------

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });


            app.UseCors("AllowAll");

            

            
            //using identity server middleware
            app.UseIdentityServer();

            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();


        }
    }
}
