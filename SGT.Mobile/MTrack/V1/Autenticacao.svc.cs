using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.NovoApp.Autenticacao;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Autenticacao" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Autenticacao.svc or Autenticacao.svc.cs at the Solution Explorer and start debugging.
    public class Autenticacao : BaseControllerNovoApp, IAutenticacao
    {
        public ResponseLogin Login(RequestLogin request)
        {
            try
            {
                bool utilizaAppTrizy = Startup.appSettingsAD["AppSettings:UtilizaAppTrizy"]?.ToString() == "SIM";
                bool validarSenhaAutomatica = Startup.appSettingsAD["AppSettings:ValidarSenhaAutomatica"]?.ToString() == "SIM";

                var servico = new Servicos.Mobile.Autenticacao.Autenticacao(adminUnitOfWork);
                string tokenSessao = servico.Login(request.CPF, request.Senha, request.VersaoApp, request.UniqueID, request.OneSignalPlayerId, utilizaAppTrizy, validarSenhaAutomatica, out var usuarioMobile, out var empresas);

                if (empresas == null || empresas.Count == 0)
                {
                    throw new ServicoException("Sua conta não está ligada a nenhum embarcador");
                }

                return new ResponseLogin
                {
                    Codigo = usuarioMobile.Codigo,
                    Nome = usuarioMobile.Nome,
                    Token = tokenSessao,
                    Empresas = (from o in empresas
                                select new Empresa
                                {
                                    Codigo = o.Cliente.Codigo,
                                    Descricao = o.Cliente.RazaoSocial,
                                    UrlEmbarcador = ObterURLCliente(o),
                                    UrlMobile = ObterURLClienteMobile(o),
                                }).ToList()
                };

            }
            catch (BaseException e)
            {
                RetornarErro(e.Message, HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                RetornarErro(ex.Message, HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            return null;
        }

        public ResponseBool Logout()
        {
            try
            {
                LimparSessao();

                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException e)
            {
                RetornarErro(e.Message, HttpStatusCode.Unauthorized);
                return new ResponseBool { Sucesso = false };
            }
            catch (Exception ex)
            {
                RetornarErro(ex.Message, HttpStatusCode.InternalServerError);
                return new ResponseBool { Sucesso = false };
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }
        }

        public ResponseBool TestarSessao(int clienteMultisoftware)
        {
            ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out var unitOfWork, false);

            try
            {
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

        }

        public List<string> ObterMenus(int clienteMultisoftware)
        {
            ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out var unitOfWork);

            try
            {
                List<string> retorno = new List<string>();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();

                if (configuracaoMobile.MenuCarga)
                    retorno.Add("Cargas");
                if (configuracaoMobile.MenuServicos)
                    retorno.Add("Servicos");
                if (configuracaoMobile.MenuOcorrencias)
                    retorno.Add("Ocorrencias");
                if (configuracaoMobile.MenuExtratoViagem)
                    retorno.Add("ExtratoViagem");
                if (configuracaoMobile.MenuPontosParada)
                    retorno.Add("PontosParada");
                if (configuracaoMobile.MenuServicosViagem)
                    retorno.Add("ServicosViagem");
                if (configuracaoMobile.MenuRH)
                    retorno.Add("RH");

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private string ObterURLCliente(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            if (usuarioMobileCliente.Cliente.Codigo == 1)
                return "http://192.168.0.125:83";

            var listaUrlCliente = (
                from urlAcesso in usuarioMobileCliente.Cliente.ClienteURLsAcesso
                where urlAcesso.TipoServicoMultisoftware == usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware && !urlAcesso.URLAcesso.Contains("localhost")
                select urlAcesso
            ).ToList();

            var url = "";

            if (listaUrlCliente.Count == 1)
                url = listaUrlCliente.FirstOrDefault().URLAcesso;
            else if (listaUrlCliente.Count > 1)
            {
                var isHomologacao = (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null);

                url = (from urlCliente in listaUrlCliente where urlCliente.URLHomologacao == isHomologacao select urlCliente).FirstOrDefault()?.URLAcesso;
            }

            bool utilizarHttps = Startup.appSettingsAD["AppSettings:UtilizarHttps"]?.ToBool() ?? false;
            return string.IsNullOrWhiteSpace(url) ? "" : utilizarHttps ? $"https://{url}" : $"http://{url}";
        }

        private string ObterURLClienteMobile(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            var isHomologacao = (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null);

            if (isHomologacao)
                return usuarioMobileCliente.Cliente.UrlMobileHomologacao;
            else
                return usuarioMobileCliente.Cliente.UrlMobile;
        }

        #endregion
    }
}
