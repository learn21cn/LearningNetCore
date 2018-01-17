using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using N7.NewMvcClient.Models;

namespace N7.NewMvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //需要先使用nuget安装IdentityModel
        [Authorize]
        public async Task RefreshTokensAsync()
        {
            //获取Authorization Server的信息
            var authorizationServerInfo = await DiscoveryClient.GetAsync("http://localhost:5000/");
            var client = new TokenClient(authorizationServerInfo.TokenEndpoint, "mvc_code", "secret");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var response = await client.RequestRefreshTokenAsync(refreshToken);
            //需要找到原来的identity token, 因为它相当于是cookie中存储的主键
            var identityToken = await HttpContext.GetTokenAsync("identity_token");
            //然后设置一下过期时间.
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }

            };
            //取当前用户的authentication信息
            var authenticationInfo = await HttpContext.AuthenticateAsync("Cookies");
            //存储新的tokens
            authenticationInfo.Properties.StoreTokens(tokens);
            //重登录, 把当前用户信息的Principal和Properties传进去. 这就会更新客户端的Cookies, 用户也就保持登陆并且刷新了tokens
            await HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);

        }
        
        [Authorize]
        public async Task<IActionResult> GetIdentity()
        {
            //正式环境中应该在401之后，调用这个方法
            await RefreshTokensAsync();

            //通过HttpContext获得access token
            var token = await HttpContext.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                //在请求的Authorization Header加上Bearer Token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = await client.GetStringAsync("http://localhost:5001/api/default");
                // var json = JArray.Parse(content).ToString();
                return Ok(new { value = content });
            }
        }


        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            //这里需要确保同时登出本地应用(MvcClient)的Cookies和OpenId Connect(去Identity Server清除单点登录的Session).
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }
    }
}
