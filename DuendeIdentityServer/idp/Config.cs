using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace idp;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // relying party
            new Client
            {
                ClientId = "rp1",
                AllowedScopes = { "openid", "profile" },
                RedirectUris = { "https://localhost:5001/signin-wsfed" },
                RequireConsent = false,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
            }
        };
}