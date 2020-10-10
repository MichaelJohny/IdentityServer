using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServerApi
{
    public class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResource() =>
            new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma"
                    }
                }
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiOne")
                {
                    Scopes = new List<string>() {"ApiOne"}
                },
                new ApiResource("ApiTwo")
                {
                    Scopes = new List<string>() {"ApiTwo"}
                }
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client()
                {
                    ClientId = "client_id",
                    ClientSecrets = {new Secret("client_secret".ToSha256())},
                    AllowedGrantTypes = {GrantType.ClientCredentials},

                    AllowedScopes = {"ApiOne"}
                },
                new Client()
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = {new Secret("client_secret_mvc".ToSha256())},
                    AllowedGrantTypes = {GrantType.AuthorizationCode},
                    RedirectUris = {"http://localhost:5050/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5050/signin-oidc"},
                    AllowedCorsOrigins = {""},
                    AllowedScopes =
                    {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        "rc.scope",
                    },
                    // put all the claims in the id token
                    // AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                }
            };

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("ApiOne", "Read Access to API #1"),
                new ApiScope("ApiTwo", "Read Access to API #2"),
            };
        }
    }
}