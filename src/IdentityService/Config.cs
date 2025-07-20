using Duende.IdentityServer.Models;

namespace IdentityService;

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
            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman",
                AllowedScopes = {"openid", "profile", "auctionApp"}, // open id and profile are here because they're in IdentityResource[].
                //These are the resources that the client will receivce with the token. Using Open Id means there are going to be 2 tokens returned from this request,
                // one for the id and the other one for access. The access token is the one that the client will be using across Resource Server to access
                // an endpoint.
                RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                ClientSecrets = new[] {new Secret("NotASecret".Sha256())},
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
            },
            new Client
            {
                ClientId = "nextApp",
                ClientName = "nextApp",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials, // Access token will be shared across network without involvement of browser.
                RequirePkce = false, // PKCE is a security measure to prevent code interception attacks. It is used in public clients where the client secret cannot be stored securely.
                // If we were building a react native app, we would use PKCE to prevent code interception attacks since the client secret cannot be stored securely.
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                AllowOfflineAccess = true, // This allows the client to request a refresh token.
                AllowedScopes = {"openid", "profile", "auctionApp"}, // open id and profile are here because they're in IdentityResource[].
                AccessTokenLifetime = 3600*24*30, // 30 days
                AlwaysIncludeUserClaimsInIdToken = true, // This will include the user claims in the id token.
            }
        };
}
