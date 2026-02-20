using System;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace EmissaoCTe.WebAdmin.Class
{
    public class SSOAuthenticationHelper
    {
        #region Métodos Públicos

        public static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno ValidarConfiguracaoSSO()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao))
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno repositorioConfiguracaoSSOInterno = 
                        new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno(unitOfWork);
                    
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = 
                        repositorioConfiguracaoSSOInterno.BuscarConfiguracaoPadrao();

                    if (configuracaoSSOInterno != null && configuracaoSSOInterno.Ativo)
                    {
                        return configuracaoSSOInterno;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return null;
        }

        public static string IniciarFluxoSSO(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno, HttpRequest request)
        {
            try
            {
                if (configuracaoSSOInterno == null || !configuracaoSSOInterno.Ativo)
                {
                    return null;
                }

                string dominioBase = string.Format("{0}://{1}", 
                    request.Url.Scheme, 
                    request.Url.Authority);

                string urlComSubapp = string.Format("{0}://{1}{2}", 
                    request.Url.Scheme, 
                    request.Url.Authority, 
                    request.ApplicationPath.TrimEnd('/'));

                string callbackUrl = $"{urlComSubapp}/LoginSSOCallback.aspx";

                string samlEndpoint = string.Format(configuracaoSSOInterno.UrlAutenticacao, configuracaoSSOInterno.ClientId);

                    App_Start.AuthRequest authRequest = new App_Start.AuthRequest(
                    dominioBase,
                    callbackUrl
                );

                return authRequest.GetRedirectUrl(samlEndpoint);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        public static DadosUsuarioSAML ProcessarRespostaSAML(string samlResponse, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno)
        {
            try
            {
                App_Start.Response response = new App_Start.Response();

                if (string.IsNullOrWhiteSpace(configuracaoSSOInterno.CaminhoArquivoCertificado))
                {
                    return new DadosUsuarioSAML 
                    { 
                        Sucesso = false, 
                        MensagemErro = "Caminho do certificado SAML não configurado." 
                    };
                }

                try
                {
                    string extensao = System.IO.Path.GetExtension(configuracaoSSOInterno.CaminhoArquivoCertificado).ToLower();
                    string certificadoConteudo = Utilidades.IO.FileStorageService.Storage.ReadAllText(configuracaoSSOInterno.CaminhoArquivoCertificado);
                    
                    if (string.IsNullOrWhiteSpace(certificadoConteudo))
                    {
                        return new DadosUsuarioSAML 
                        { 
                            Sucesso = false, 
                            MensagemErro = "Arquivo de certificado está vazio." 
                        };
                    }
                    
                    if (extensao.Equals(".xml"))
                    {
                        response.SetMetadataXml(certificadoConteudo);
                    }
                    else
                    {
                        response.SetCertificateStr(certificadoConteudo);
                    }
                }
                catch (Exception exCert)
                {
                    Servicos.Log.TratarErro(exCert);
                    return new DadosUsuarioSAML 
                    { 
                        Sucesso = false, 
                        MensagemErro = $"Erro ao carregar certificado: {exCert.Message}" 
                    };
                }

                try
                {
                    response.LoadXmlFromBase64(samlResponse);
                }
                catch (Exception exXml)
                {
                    Servicos.Log.TratarErro(exXml);
                    return new DadosUsuarioSAML 
                    { 
                        Sucesso = false, 
                        MensagemErro = $"Erro ao carregar XML SAML: {exXml.Message}" 
                    };
                }

                string erroValidacaoSSO = string.Empty;
                if (!response.IsValid(ref erroValidacaoSSO))
                {
                    Servicos.Log.TratarErro($"Validação SAML falhou: {erroValidacaoSSO}", "sso_saml_processing");
                    return new DadosUsuarioSAML 
                    { 
                        Sucesso = false, 
                        MensagemErro = string.IsNullOrWhiteSpace(erroValidacaoSSO) ? "Resposta SAML inválida" : erroValidacaoSSO
                    };
                }

                string email = response.GetEmail();
                string nameId = response.GetNameID();

                if (string.IsNullOrWhiteSpace(email))
                {
                    email = nameId; // Fallback para NameID se email não estiver presente
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return new DadosUsuarioSAML 
                    { 
                        Sucesso = false, 
                        MensagemErro = "Não foi possível obter o e-mail/login do usuário na resposta SAML." 
                    };
                }

                return new DadosUsuarioSAML
                {
                    Sucesso = true,
                    Email = email,
                    NomeCompleto = $"{response.GetFirstName()} {response.GetLastName()}".Trim(),
                    PrimeiroNome = response.GetFirstName(),
                    UltimoNome = response.GetLastName()
                };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                string mensagemDetalhada = ex.Message;
                if (ex.InnerException != null)
                {
                    mensagemDetalhada += $" - {ex.InnerException.Message}";
                }
                return new DadosUsuarioSAML 
                { 
                    Sucesso = false, 
                    MensagemErro = $"Falha ao processar SAML: {mensagemDetalhada}" 
                };
            }
        }

        public static ResultadoAutenticacaoSSO AutenticarUsuarioSSO(string email, HttpSessionState session)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao))
                {
                    Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                    Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorAmbienteAdmin();

                    if (usuario == null)
                    {
                        return new ResultadoAutenticacaoSSO
                        {
                            Sucesso = false,
                            MensagemErro = "Usuário de serviço SSO não configurado."
                        };
                    }

                    usuario.UltimoAcesso = DateTime.Now;
                    repositorioUsuario.Atualizar(usuario);

                    session["IdEmpresa"] = usuario.Empresa.Codigo;
                    session["IdUsuario"] = usuario.Codigo;
                    session["TentativasLogin"] = 0;

                    CriarLogDeAcessoSSO(unitOfWork, usuario, email);
                    
                    Servicos.Log.TratarErro($"Login SSO: {email} autenticado como {usuario.Nome}", "sso_login");

                    return new ResultadoAutenticacaoSSO
                    {
                        Sucesso = true,
                        Usuario = usuario,
                        EmailAzureAD = email
                    };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new ResultadoAutenticacaoSSO
                {
                    Sucesso = false,
                    MensagemErro = "Ocorreu uma falha ao autenticar o usuário: " + ex.Message
                };
            }
        }

        public static void SetarPermissoesUsuario(Dominio.Entidades.Usuario usuario, System.Web.Caching.Cache cache)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao))
                {
                    string key = string.Concat("CTeUserPages_", usuario.Codigo);
                    
                    if (cache.Get(key) != null)
                        cache.Remove(key);
                    
                    Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                    System.Collections.Generic.List<Dominio.Entidades.PaginaUsuario> paginasUsuario = repPaginaUsuario.BuscarPorUsuario(usuario.Codigo);
                    
                    cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), 
                        System.Web.Caching.Cache.NoSlidingExpiration, 
                        System.Web.Caching.CacheItemPriority.Default, null);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private static void CriarLogDeAcessoSSO(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, string email)
        {
            try
            {
                Repositorio.LogAcesso repositorioLogAcesso = new Repositorio.LogAcesso(unitOfWork);
                Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

                logAcesso.Data = DateTime.Now;
                logAcesso.IPAcesso = ObterIPUsuario();
                logAcesso.Login = email;
                logAcesso.Senha = "*** SSO SAML2 ***";
                logAcesso.SessionID = HttpContext.Current.Session?.SessionID ?? string.Empty;
                logAcesso.Tipo = Dominio.Enumeradores.TipoLogAcesso.Entrada;
                logAcesso.Usuario = usuario;

                repositorioLogAcesso.Inserir(logAcesso);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private static string ObterIPUsuario()
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context != null && context.Request != null)
                {
                    string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = context.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    return ip;
                }
            }
            catch { }
            
            return string.Empty;
        }

        #endregion
    }

    #region Classes Auxiliares

    public class DadosUsuarioSAML
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
        public string Email { get; set; }
        public string NomeCompleto { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
    }

    public class ResultadoAutenticacaoSSO
    {
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; }
        public Dominio.Entidades.Usuario Usuario { get; set; }
        public string EmailAzureAD { get; set; }
    }

    #endregion
}

