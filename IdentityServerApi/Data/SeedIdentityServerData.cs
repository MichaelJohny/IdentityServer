using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServerApi.Data
{
    public class SeedIdentityServerData
    {
   
        public static async Task SeedAll(ConfigurationDbContext context)
        {
            await context.Database.MigrateAsync();
            await SeedClients(context);
            await SeedApiResources(context);
            await SeedIdentityResource(context);
            await SeedApiScopes(context);
        }

        private static async Task SeedClients(ConfigurationDbContext context)
        {
            if (! await context.Clients.AnyAsync())
            {
                foreach (var client in Configuration.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
        
        private static async Task SeedIdentityResource(ConfigurationDbContext context)
        {
            if (! await context.IdentityResources.AnyAsync())
            {
                foreach (var resource in Configuration.GetIdentityResource())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
        
        private static async Task SeedApiResources(ConfigurationDbContext context)
        {
           
            if (! await context.ApiResources.AnyAsync())
            {
                foreach (var resource in Configuration.GetApis())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
        
        private static async Task SeedApiScopes(ConfigurationDbContext context)
        {
           
            if (! await context.ApiScopes.AnyAsync())
            {
                foreach (var scope in Configuration.GetApiScopes())
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
        
        
        
        
        
    }
}
