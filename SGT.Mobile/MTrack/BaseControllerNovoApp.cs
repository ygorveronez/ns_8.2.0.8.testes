using System;
using System.Net;
using System.ServiceModel.Web;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.NovoApp.Comum;

namespace SGT.Mobile.MTrack
{
    public class BaseControllerNovoApp
    {
        protected readonly AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork;

        #region Propriedades

        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _TipoServicoMultisoftware;
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
        {
            get
            {
                if (_ClienteURLAcesso == null)
                    _TipoServicoMultisoftware = ClienteAcesso.TipoServicoMultisoftware;

                return _TipoServicoMultisoftware;
            }
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _ClienteURLAcesso;
        public AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso ClienteAcesso
        {
            get
            {
                if (_ClienteURLAcesso == null)
                {
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

                    try
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                        _ClienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.ObterHost);
                        _TipoServicoMultisoftware = _ClienteURLAcesso.TipoServicoMultisoftware;
                        _Cliente = _ClienteURLAcesso.Cliente;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                    finally {
                        unitOfWork.Dispose();
                    }
                }

                return _ClienteURLAcesso;
            }
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _Cliente;
        public AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                if (_ClienteURLAcesso == null)
                    _Cliente = ClienteAcesso.Cliente;

                return _Cliente;
            }
        }

        #endregion

        #region Métodos Públicos

        public BaseControllerNovoApp()
        {
            adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
        }

        public void LimparSessao()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            string sessao = headers["Sessao"];

            if (string.IsNullOrWhiteSpace(sessao))
                throw new WebServiceException("Sessão não informada");

            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(sessao);

            if (usuarioMobile == null)
                throw new WebServiceException($"Token inválido (Sessão {sessao})");

            usuarioMobile.Sessao = "";
            repUsuarioMobile.Atualizar(usuarioMobile);
        }

        public AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile ValidarSessao(bool naoMatarSessao = false)
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            string sessao = headers["Sessao"];


            int.TryParse(Startup.appSettingsAD["AppSettings:TempoSessao"]?.ToString(), out int minutos);
            bool killSession = Startup.appSettingsAD["AppSettings:KillSessionAfterRequest"]?.ToBool() ?? false;

            if (minutos == 0)
                minutos = 30;

            if (!string.IsNullOrWhiteSpace(sessao))
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(sessao, DateTime.Now.AddMinutes(-minutos));

                if (usuarioMobile != null)
                {
                    if (killSession && !naoMatarSessao)
                        usuarioMobile.Sessao = "";
                    else
                        usuarioMobile.DataSessao = DateTime.Now;

                    repUsuarioMobile.Atualizar(usuarioMobile);
                    return usuarioMobile;
                }
            }

            RetornarErro("Token inválido ou expirado (Sessão " + sessao + ")", HttpStatusCode.Unauthorized);
            return null;
        }

        public AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente ValidarSessaoEObterUsuarioMobileCliente(int clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork, bool conectar = true, bool naoMatarSessao = false)
        {
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(adminUnitOfWork);

                var usuarioMobileCliente = usuarioMobile != null ? repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware) : null;

                if (usuarioMobileCliente == null)
                    RetornarErro("Sua sessão não permite consultar dados deste cliente", HttpStatusCode.Unauthorized);

                if (conectar)
                {
                    unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                }
                else
                    unitOfWork = null;

                return usuarioMobileCliente;
            }
            catch (Exception ex)
            {
                unitOfWork = null;
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoTMS(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            return repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        }

        public void RetornarErro(string mensagem, HttpStatusCode codigo = HttpStatusCode.ExpectationFailed)
        {
            Servicos.Log.TratarErro(mensagem, "RetornoErro");

            throw new WebFaultException<ResponseError>(
                new ResponseError
                {
                    Mensagem = mensagem
                }, codigo);
        }

        #endregion
    }
}