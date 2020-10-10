using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServerApi
{
    public class Startup
    {
        public Startup(IConfiguration configurations)
        {
            Configurations = configurations;
        }

        public IConfiguration Configurations { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(cfg => cfg.UseSqlServer(Configurations.GetConnectionString("DefaultConnection")));  
            // services.AddDbContext<AppDbContext>(cfg => cfg.UseInMemoryDatabase("Memory"));

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.SignIn.RequireConfirmedEmail = false;
                }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            var assembly = typeof(Startup).Assembly.GetName().Name;
            
            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
//                .AddInMemoryApiResources(Configuration.GetApis())
//                .AddInMemoryClients(Configuration.GetClients())
//                .AddInMemoryApiScopes(Configuration.GetApiScopes())
//                .AddInMemoryIdentityResources(Configuration.GetIdentityResource())
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(Configurations.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(assembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(Configurations.GetConnectionString("DefaultConnection"),
                            sql => sql.MigrationsAssembly(assembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddDeveloperSigningCredential();
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}