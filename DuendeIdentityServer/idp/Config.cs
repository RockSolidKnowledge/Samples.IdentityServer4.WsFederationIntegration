using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace idp;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("scope1"),
        new("scope2")
    ];

    public static IEnumerable<Client> Clients =>
    [
        // relying party
        new()
        {
            ClientId = "rp1",
            AllowedScopes = { "openid", "profile" },
            RedirectUris = { "https://localhost:5001/signin-wsfed" },
            RequireConsent = false,
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        }
    ];
}