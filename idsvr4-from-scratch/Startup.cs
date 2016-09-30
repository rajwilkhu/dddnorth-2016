using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services.InMemory;
using Microsoft.AspNetCore.Identity;
using IdentityModel;
using System.Security.Claims;

namespace idvr4_from_scratch
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                    .AddInMemoryStores()
                    .AddInMemoryClients(Clients.Get())
                    .AddInMemoryScopes(Scopes.Get())
                    .AddInMemoryUsers(Users.Get())
                    .SetTemporarySigningCredential();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            // app.Run(async (context) =>
            // {
            //     await context.Response.WriteAsync("Hello World!");
            // });
        }
    }

    internal static class Clients {
        public static IEnumerable<Client> Get() {
            return new List<Client> {
                new Client {
                    ClientId = "oauthClient",
                    ClientName = "Example Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> {
                        new Secret("superSecretPassword".Sha256())},
                    AllowedScopes = new List<string> {"someAPI"}
                },
                new Client {
                    ClientId = "openIdConnectClient",
                    ClientName = "Implicit Client Application",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.Email.Name,
                        StandardScopes.Roles.Name,
                        "someAPI"
                    },
                    RedirectUris = new List<string> {"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris = new List<string> {"http://localhost:5001"}
                }
            };
        }
    }

    internal static class Scopes {
        public static IEnumerable<Scope> Get() {
            return new List<Scope> {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Roles,
                StandardScopes.OfflineAccess,
                new Scope {
                    Name = "customAPI",
                    DisplayName = "Custom API",
                    Description = "Custom API scope",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim> {
                        new ScopeClaim(JwtClaimTypes.Role)
                    },
                    ScopeSecrets =  new List<Secret> {
                        new Secret("scopeSecret".Sha256())
                    }
                }
            };
        }
    }

    internal static class Users {
        public static List<InMemoryUser> Get() {
            return new List<InMemoryUser> {
                new InMemoryUser {
                    Subject = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "raj",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "rajwilkhu@gmail.com"),
                        new Claim(JwtClaimTypes.Role, "Administrator")
                    }
                }
            };
        }
    }
}
