using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims.ToList();
            var _accessToken = new JwtSecurityTokenHandler().ReadToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecrets(accessToken);
            await RefreshAccessToken();
            return View();
        }

        public IActionResult Logout() => SignOut("Cookie", "oidc");
        public async Task<string> GetSecrets(string accessToken)
        {
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var response = await apiClient.GetAsync("http://localhost:5001/secret");

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("http://localhost:5000");


            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "client_id_mvc",
                ClientSecret = "client_secret_mvc"
            });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie");
            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);

            var accessTokenDifferent = !accessToken.Equals(tokenResponse.AccessToken);
            var idTokenDifferent = !idToken.Equals(tokenResponse.IdentityToken);
            var refreshTokenDifferent = !refreshToken.Equals(tokenResponse.RefreshToken);
            

            var a = 1;
        }
    }
}