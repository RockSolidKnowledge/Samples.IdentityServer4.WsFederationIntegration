using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Rsk.WsFederation;
using Rsk.WsFederation.Models;

namespace idpWithEf;

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
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        }
    ];

    public static readonly RelyingParty RelyingPartyOverrides = new()
    {
        Realm = "rp1",
        TokenType = WsFederationConstants.TokenTypes.Saml2TokenProfile11
    };
}