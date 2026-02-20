using System.Globalization;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Sustainsys.Saml2.AspNetCore2;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.OAuth;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Autenticacao;
using DocumentFormat.OpenXml.InkML;
using Grpc.Core;
using Microsoft.AspNetCore.DataProtection;
using Azure.Identity;
using StackExchange.Redis;
using SGT.WebAdmin.Controllers;
using Microsoft.Extensions.Hosting;

namespace SGT.WebAdmin
{
    public partial class Startup
    {
        private static IConfigurationRoot _appSettingsAD;

        public static IConfigurationRoot appSettingsAD
        {
            get
            {
                if (_appSettingsAD == null)
                {
                    // Cria o arquivo appsettingsad.json se não existir
                    string diretorio = AppContext.BaseDirectory;
                    Utilidades.File.CreateAppSettingsADIfNotExist(diretorio);

                    // Carrega a configuração do arquivo appsettingsad.json
                    _appSettingsAD = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                                        .SetBasePath(diretorio)
                                        .AddJsonFile("appsettingsad.json").Build();
                }
                return _appSettingsAD;
            }
        }

        // https://www.c-sharpcorner.com/article/azure-ad-authentication-for-mvc-web-application/
        // Leitura das configurações a partir do appsettings.json
        public static bool FormsAuthentication = appSettingsAD["AppSettings:FormsAuthentication"]?.ToBool() ?? true;

        public static bool AzureAdAuthentication = appSettingsAD["AppSettings:AzureAdAuthentication"]?.ToBool() ?? false;
        public static string AzureAdDisplay = appSettingsAD["AppSettings:AzureAdDisplay"]?.ToString() ?? string.Empty;
        private static string clientId = appSettingsAD["AppSettings:ida:ClientId"]?.ToString() ?? string.Empty;
        private static string adfsDiscoveryDoc = appSettingsAD["AppSettings:ida:ADFSDiscoveryDoc"]?.ToString() ?? string.Empty;
        private static string clientSecret = appSettingsAD["AppSettings:ida:ClientSecret"]?.ToString() ?? string.Empty;
        private static string tenant = appSettingsAD["AppSettings:ida:Tenant"]?.ToString() ?? string.Empty;
        private static string aadInstance = appSettingsAD["AppSettings:ida:AADInstance"]?.ToString() ?? string.Empty;
        private static string postLogoutRedirectUri = appSettingsAD["AppSettings:ida:PostLogoutRedirectUri"]?.ToString() ?? string.Empty;

        private string authority = !string.IsNullOrWhiteSpace(aadInstance) && !string.IsNullOrWhiteSpace(tenant) ? string.Format(CultureInfo.InvariantCulture, aadInstance, tenant) : null;

        // Autenticação login AD local.
        public static bool LoginAD = appSettingsAD["AppSettings:LoginAD"]?.ToBool() ?? false;
        public static string ServidorAD = appSettingsAD["AppSettings:ServidorAD"]?.ToString() ?? string.Empty;

        // Autenticação SAML
        public static bool SamlAuthentication;
        //public static bool SamlAuthentication = appSettingsAD["AppSettings:saml2:Authentication"]?.ToBool() ?? false;
        //public static string SamlDisplay = appSettingsAD["AppSettings:saml2:Display"]?.ToString() ?? string.Empty;
        public static string SamlClientID = appSettingsAD["AppSettings:saml2:ClientId"]?.ToString() ?? string.Empty;
        //public static string SamlEndPoint = appSettingsAD["AppSettings:saml2:EndPoint"]?.ToString() ?? string.Empty;
        //public static string SamlDomain = appSettingsAD["AppSettings:saml2:Domain"]?.ToString() ?? string.Empty;
        //public static string SamlPathCertificate = appSettingsAD["AppSettings:saml2:PathCertificate"]?.ToString() ?? string.Empty;

        public static bool OktaAuthentication = appSettingsAD["AppSettings:okta:Authentication"]?.ToBool() ?? false;
        public static string OktaDisplay = appSettingsAD["AppSettings:okta:Display"]?.ToString() ?? string.Empty;
        public static string OktaEntityId = appSettingsAD["AppSettings:okta:EntityId"]?.ToString() ?? string.Empty;
        public static string OktaCertificado = appSettingsAD["AppSettings:okta:Certificado"]?.ToString() ?? string.Empty;
        public static string OktaMetadataLocation = appSettingsAD["AppSettings:okta:MetadataLocation"]?.ToString() ?? string.Empty;
        public static string OktaReturnUrl = appSettingsAD["AppSettings:okta:ReturnUrl"]?.ToString() ?? string.Empty;
        public static string OktaSingleSignOnServiceUrl = appSettingsAD["AppSettings:okta:SingleSignOnServiceUrl"]?.ToString() ?? string.Empty;
        public static string OktaIssuerUrl = appSettingsAD["AppSettings:okta:IssuerUrl"]?.ToString() ?? string.Empty;
        public static double OktaSessionTime = appSettingsAD["AppSettings:okta:SessionTime"]?.ToDouble() ?? 60;
        public static bool OktaHabilitaPortalTransportador = appSettingsAD["AppSettings:okta:HabilitaPortalTransportador"]?.ToBool() ?? false;
        public static string OktaIssuerUrlPortalTransportador = appSettingsAD["AppSettings:okta:IssuerUrlPortalTransportador"]?.ToString() ?? string.Empty;
        //public static string OktaReturnUrlPortalTransportador = appSettingsAD["AppSettings:okta:ReturnUrlPortalTransportador"]?.ToString() ?? string.Empty;


        public void ConfigureAuth(IServiceCollection services)
        {
            TimeSpan defaultAuthCookieExpirationTime = TimeSpan.FromMinutes(60);

            var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(redisConnection))
                Servicos.Database.ConnectionString.Instance.GetRedisConnectionString("RedisStringConexao");

            var signalRBuilder = services.AddSignalR(o =>
            {
                o.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
                o.KeepAliveInterval = TimeSpan.FromSeconds(60);
                o.HandshakeTimeout = TimeSpan.FromSeconds(30);
                o.EnableDetailedErrors = true;
            })
            .AddNewtonsoftJsonProtocol(o =>
            {
                o.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            if (!string.IsNullOrWhiteSpace(redisConnection))
            {
                signalRBuilder.AddStackExchangeRedis(redisConnection);
            }

#if !DEBUG
            string blobStorageConnectionString = Environment.GetEnvironmentVariable("AZURE_SA_CONNECTION_STRING");
            string blobName = Environment.GetEnvironmentVariable("AZURE_SA_BLOB_NAME");

            if (!string.IsNullOrWhiteSpace(blobStorageConnectionString) && !string.IsNullOrWhiteSpace(blobName))
                services.AddDataProtection().PersistKeysToAzureBlobStorage(blobStorageConnectionString, blobName, "keys.xml");
#endif

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();  //TO-DO - REVER MIGRACAO .NET (Willian) - Razor Pages
            services.AddHttpContextAccessor();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

            if (!AzureAdAuthentication && !OktaAuthentication)
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.Cookie.Name = "SGT.WebAdmin.Auth";
                    options.ExpireTimeSpan = defaultAuthCookieExpirationTime;
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Login";
                });

            if (AzureAdAuthentication)
            {
                if (clientId == string.Empty)
                {
                    clientId = SamlClientID;
                }

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "SGT.WebAdmin.Auth";
                    options.ExpireTimeSpan = defaultAuthCookieExpirationTime;
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Login";
                })
                .AddOpenIdConnect(options =>
                {
                    options.UseTokenLifetime = false;
                    options.ClientId = clientId;
                    options.Authority = authority;
                    options.MetadataAddress = (!string.IsNullOrEmpty(adfsDiscoveryDoc) ? adfsDiscoveryDoc : null);
                    options.ClientSecret = (!string.IsNullOrEmpty(clientSecret) ? clientSecret : null);
                    options.Scope.Add(Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectScope.OpenIdProfile);
                    options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.IdToken;
                    options.SignedOutRedirectUri = postLogoutRedirectUri;
                    //options.SaveTokens = true;
                    //options.CallbackPath = (!string.IsNullOrWhiteSpace(postLogoutRedirectUri) && postLogoutRedirectUri.Contains("/Login")) ? new PathString("/Login") : options.CallbackPath;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = ctxt =>
                        {
                            ctxt.ProtocolMessage.EnableTelemetryParameters = false;
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.Message.Contains("IDX21323"))
                            {
                                context.HandleResponse();
                                context.HttpContext.ChallengeAsync();
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenResponseReceived = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var email = context.Principal!.Claims.First(x => x.Type == "preferred_username").Value;
                            var name = context.Principal.Claims.First(x => x.Type == "name").Value;
                            bool validaGrupoUsuario = false;
                            string groupID = string.Empty;
                            try
                            {
                                bool.TryParse(appSettingsAD["AppSettings:AzureAdValidateUserGroup"], out validaGrupoUsuario);
                                //Ex: 990d436e-5a90-48a5-bc4c-c81ffaf734a5
                                groupID = appSettingsAD["AppSettings:AzureAdUserGroupId"]!;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter configurações do Azure AD: {ex.ToString()}", "CatchNoAction");
                            }
                            int statusCode = 1; // Success
                            if (validaGrupoUsuario)
                            {
                                if (context.Principal.Claims.Any(x => x.Type == "groups"))
                                {
                                    var grupos = context.Principal.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList();
                                    if (!grupos.Any(g => g.ToUpper() == groupID.ToUpper()))
                                        statusCode = 3; // Usuário autenticou no AD, porem não faz parte do grupo permitido de acesso ao sistema.
                                }
                                else
                                    statusCode = 2; // Usuário autenticou no AD, porem o mesmo não faz parte de nenhum grupo.
                            }

                            string chave = string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"));
                            var cripto = Servicos.Criptografia.Criptografar(email + "|" + name, chave);
                            cripto = System.Web.HttpUtility.UrlEncode(cripto);
                            context.Properties!.RedirectUri = "Login/SigninOidc?code=" + cripto + "&status=" + statusCode;
                            return Task.CompletedTask;
                        },
                    };
                });
            }

            if (OktaAuthentication)
            {
                services
                .ConfigureApplicationCookie(options =>
                {
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(OktaSessionTime);
                })
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "SGT.WebAdmin.Auth";
                    options.ExpireTimeSpan = defaultAuthCookieExpirationTime;
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Login";
                })
                .AddSaml2(opt =>
                {
                    // Set up our EntityId, this is our application.
                    opt.SPOptions.EntityId = new EntityId(OktaIssuerUrl);
                    opt.SPOptions.ReturnUrl = new Uri(OktaReturnUrl);
                    opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Never;
                    // Single logout messages should be signed according to the SAML2 standard, so we need
                    // to add a certificate for our app to sign logout messages with to enable logout functionality.
                    //opt.SPOptions.ServiceCertificates.Add(new X509Certificate2("Sustainsys.Saml2.Tests.pfx"));

                    // Add an identity provider.
                    opt.IdentityProviders.Add(new IdentityProvider(
                        // The identityprovider's entity id.
                        new EntityId(OktaEntityId),
                        opt.SPOptions)
                    {
                        // Load config parameters from metadata, using the Entity Id as the metadata address.
                        LoadMetadata = true,
                        SingleSignOnServiceUrl = new Uri(OktaSingleSignOnServiceUrl),
                        MetadataLocation = OktaMetadataLocation,
                        AllowUnsolicitedAuthnResponse = true
                    });
                });
            }
        }

        public void ConfigureSSo(IServiceCollection services)
        {
            services.AddScoped<Dominio.Interfaces.Autenticacao.ISSoConfigService, Servicos.Autenticacao.SSoConfigService>();

            if (!AzureAdAuthentication && !OktaAuthentication)
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "OAuth2";
                })
                .AddOAuth("OAuth2", options =>
                {
                    Controllers.Conexao conexao = services.BuildServiceProvider().GetRequiredService<Controllers.Conexao>();
                    if (!string.IsNullOrWhiteSpace(conexao?.StringConexao ?? string.Empty))
                        conexao.MigrateDatabase(conexao.StringConexao);

                    var provider = services.BuildServiceProvider().GetRequiredService<ISSoConfigService>();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO oAuth2Settings = provider.ObterConfiguracaoSSo(conexao?.StringConexao ?? string.Empty, Dominio.Enumeradores.TipoSso.OAuth2);

                    options.ClientId = oAuth2Settings?.ClientId ?? ".";
                    options.ClientSecret = oAuth2Settings?.ClientSecret ?? ".";
                    options.AuthorizationEndpoint = oAuth2Settings?.UrlAutenticacao ?? "https://accounts.google.com/o/oauth2/v2/auth";
                    options.TokenEndpoint = oAuth2Settings?.UrlAccessToken ?? "https://default-token-endpoint";
                    options.CallbackPath = new PathString("/Login/LoginCallbackOAuth2");

                    options.Scope.Add("openid");
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.SaveTokens = true;

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, "https://openidconnect.googleapis.com/v1/userinfo");
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
                            var response = context.Backchannel.SendAsync(request).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                string resultado = response.Content.ReadAsStringAsync().Result;
                                OAuth2Result user = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuth2Result>(resultado);

                                context.Identity.AddClaim(new System.Security.Claims.Claim("name", user.name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim("email", user.email));

                                context.Properties!.RedirectUri = "CallbackSSo";
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddOpenIdConnect("CyberArk", options =>
                {
                    Controllers.Conexao conexao = services.BuildServiceProvider().GetRequiredService<Controllers.Conexao>();
                    var provider = services.BuildServiceProvider().GetRequiredService<ISSoConfigService>();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSO = provider.ObterConfiguracaoSSo(conexao?.StringConexao ?? string.Empty, Dominio.Enumeradores.TipoSso.CyberArk);

                    options.Authority = configuracaoSSO?.UrlAutenticacao ?? "https://seu-cyberark.com";
                    options.ClientId = configuracaoSSO?.ClientId ?? ".";
                    options.ClientSecret = configuracaoSSO?.ClientSecret ?? ".";
                    options.CallbackPath = "/Login/LoginCallbackOAuth2";
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // Definição dos escopos necessários para obter e-mail e nome
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "email");

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = ctxt =>
                        {
                            ctxt.ProtocolMessage.EnableTelemetryParameters = false;
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.Message.Contains("IDX21323"))
                            {
                                context.HandleResponse();
                                context.HttpContext.ChallengeAsync();
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenResponseReceived = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            string email = context.Principal!.Claims.First(x => x.Type == "preferred_username").Value;
                            string name = context.Principal.Claims.First(x => x.Type == "name").Value;
                            if (string.IsNullOrWhiteSpace(email))
                            {
                                var claimsIdentity = context.Principal.Identity as System.Security.Claims.ClaimsIdentity;
                                email = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                                name = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                            }
                            bool validaGrupoUsuario = false;
                            string groupID = string.Empty;
                            try
                            {
                                bool.TryParse(appSettingsAD["AppSettings:AzureAdValidateUserGroup"], out validaGrupoUsuario);
                                //Ex: 990d436e-5a90-48a5-bc4c-c81ffaf734a5
                                groupID = appSettingsAD["AppSettings:AzureAdUserGroupId"]!;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter configurações do Azure AD (segunda ocorrência): {ex.ToString()}", "CatchNoAction");
                            }
                            int statusCode = 1; // Success
                            if (validaGrupoUsuario)
                            {
                                if (context.Principal.Claims.Any(x => x.Type == "groups"))
                                {
                                    var grupos = context.Principal.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList();
                                    if (!grupos.Any(g => g.ToUpper() == groupID.ToUpper()))
                                        statusCode = 3; // Usuário autenticou no AD, porem não faz parte do grupo permitido de acesso ao sistema.
                                }
                                else
                                    statusCode = 2; // Usuário autenticou no AD, porem o mesmo não faz parte de nenhum grupo.
                            }

                            string chave = string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"));
                            var cripto = Servicos.Criptografia.Criptografar(email + "|" + name, chave);
                            cripto = System.Web.HttpUtility.UrlEncode(cripto);
                            context.Properties!.RedirectUri = "Login/SigninOidc?code=" + cripto + "&status=" + statusCode;
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public static bool AutenticacaoSaml2AtivaArquivoConfiguracao()
        {
            return appSettingsAD["AppSettings:saml2:Authentication"]?.ToBool() ?? false;
        }

        public static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO ConfiguracaoSSoArquivo(Dominio.Enumeradores.TipoSso tipoSso)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSO = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO()
            {
                TipoSSo = tipoSso,
                ClientSecret = string.Empty,
                UrlAccessToken = string.Empty,
                UrlRefreshToken = string.Empty,
                UrlRevokeToken = string.Empty
            };
            configuracaoSSO.Ativo = AutenticacaoSaml2AtivaArquivoConfiguracao();
            configuracaoSSO.Display = appSettingsAD["AppSettings:saml2:Display"]?.ToString() ?? string.Empty;
            configuracaoSSO.ClientId = appSettingsAD["AppSettings:saml2:ClientId"]?.ToString() ?? string.Empty;
            configuracaoSSO.UrlAutenticacao = appSettingsAD["AppSettings:saml2:EndPoint"]?.ToString() ?? string.Empty;
            configuracaoSSO.UrlDominio = appSettingsAD["AppSettings:saml2:Domain"]?.ToString() ?? string.Empty;
            configuracaoSSO.CaminhoArquivoCertificado = appSettingsAD["AppSettings:saml2:PathCertificate"]?.ToString() ?? string.Empty;
            return configuracaoSSO;
        }
    }

    public partial class OAuth2Result
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public string hd { get; set; }

    }
}
