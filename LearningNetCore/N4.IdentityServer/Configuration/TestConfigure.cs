using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace N4.IdentityServer.Configuration
{
    public class TestConfigure
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("testapi", "测试应用接口")
                {
                    UserClaims = new [] { "email" }
                }

            };
        }

        //为了使用这些openid connect scopes, 需要设置这些identity resoruces
        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "testapi",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "testapi" }
                },
                new Client
                {
                    //要与客户端指定的名称一致
                    ClientId="mvc_implicit",
                    ClientName="MVC Client",
                    //使用Implicit flow时, 首先会重定向到Authorization Server, 然后登陆, 
                    //然后Identity Server需要知道是否可以重定向回到网站, 
                    //如果不指定重定向返回的地址的话, Session有可能就会被劫持
                    AllowedGrantTypes=GrantTypes.Implicit,
                    //登陆成功之后重定向的网址, 这个网址在MvcClient里, 
                    //openid connect中间件使用这个地址就会知道如何处理从authorization server返回的response. 
                    //这个地址将会在openid connect中间件设置合适的cookies, 以确保配置的正确性.
                    RedirectUris ={  "http://localhost:5002/signin-oidc" },
                    //登出之后重定向的网址. 
                    //有可能发生的情况是, 你登出网站的时候, 会重定向到Authorization Server, 并允许从Authorization Server也进行登出动作.
                    PostLogoutRedirectUris ={ "http://localhost:5002/signout-callback-oidc"},
                    
                    AllowedScopes =new List<string>
                    {
                        // 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息)
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "testapi"
                    },
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    //要与客户端指定的名称一致
                    ClientId="mvc_code",
                    ClientName="New MVC Client",                  
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    //登陆成功之后重定向的网址, 这个网址在MvcClient里, 
                    //openid connect中间件使用这个地址就会知道如何处理从authorization server返回的response. 
                    //这个地址将会在openid connect中间件设置合适的cookies, 以确保配置的正确性.
                    RedirectUris ={  "http://localhost:5003/signin-oidc" },
                    //登出之后重定向的网址. 
                    //有可能发生的情况是, 你登出网站的时候, 会重定向到Authorization Server, 并允许从Authorization Server也进行登出动作.
                    PostLogoutRedirectUris ={ "http://localhost:5003/signout-callback-oidc"},

                    AllowedScopes =new List<string>
                    {
                        // 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息)
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "testapi"
                    },
                    //我们还需要获取Refresh Token, 网站可以使用token来和api进行交互, 而不需要用户登陆到网站上                    
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true

                }             


            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "test@yeah.net",
                    Password = "password",
                    Claims = new [] { new Claim("email", "test@yeah.net") }
                }
            };
        }


    }
}
