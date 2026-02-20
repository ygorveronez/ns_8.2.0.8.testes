using Dominio.Entidades.EDI.DOCCOB.v30A;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;

namespace SGT.Mobile
{
    public abstract class WebServiceBase
    {
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

        #region Métodos Protegidos

        protected List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao> ObterLocais(Repositorio.UnitOfWork unitOfWork, string QRCodeLocal)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculo repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo = repositorio.BuscarPorQRCode(QRCodeLocal);

            if (areaVeiculo == null)
                throw new WebServiceException("Área não encontrada");

            if ((areaVeiculo.Posicoes == null) || (areaVeiculo.Posicoes.Count == 0))
                throw new WebServiceException("Área não possui locais cadastrados");

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao> locais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao>();

            foreach (var posicao in areaVeiculo.Posicoes)
            {
                locais.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao()
                {
                    Descricao = posicao.Descricao,
                    QRCode = posicao.QRCode
                });
            }

            return locais.OrderBy(o => o.Descricao).ToList();
        }

        protected Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao ObterLocal(Repositorio.UnitOfWork unitOfWork, string QRCodeLocal)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);

            return repositorio.BuscarPorQRCode(QRCodeLocal);
        }

        protected AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware ObterTipoServico(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracao;

            if (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null)
                configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao;

            return configuracao.TipoServicoMultisoftware;
        }

        protected string ObterURLCliente(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
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

        protected string ObterURLClienteMobile(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            var isHomologacao = (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null);

            if (isHomologacao)
                return usuarioMobileCliente.Cliente.UrlMobileHomologacao;
            else
                return usuarioMobileCliente.Cliente.UrlMobile;
        }


        protected Dominio.Entidades.Usuario ObterMotorista(Repositorio.UnitOfWork unitOfWork, string cpf)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

            return repositorioMotorista.BuscarMotoristaMobilePorCPF(cpf);
        }

        protected void AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(Repositorio.UnitOfWork unitOfWork, string cpf, string versaoApp)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

            repositorioMotorista.AtualizarVersaoAppPorCPFMotorista(cpf, versaoApp);
        }

        protected Dominio.Entidades.Usuario ObterUsuario(Repositorio.UnitOfWork unitOfWork, string cpf)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            return repositorioUsuario.BuscarPorCPF(cpf);
        }

        protected Dominio.Entidades.Usuario ObterUsuarioMotoristaMobile(Repositorio.UnitOfWork unitOfWork, string cpf)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            return repositorioUsuario.BuscarUsuarioMobilePorCPF(cpf);
        }

        protected AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente ObterUsuarioMobileCliente(int codigoUsuario, int codigoEmpresaMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWork);
            var usuarioMobile = repositorioUsuarioMobile.BuscarPorCodigo(codigoUsuario) ?? throw new WebServiceException("O usuário informado é inválido");
            var usuarioMobileCliente = (from usuarioCliente in usuarioMobile.Clientes where usuarioCliente.Cliente.Codigo == codigoEmpresaMultisoftware select usuarioCliente).FirstOrDefault() ?? throw new WebServiceException("O usuário não possui acesso para a empresa informada");

            return usuarioMobileCliente;
        }

        protected AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente ObterUsuarioMobileClientePorCliente(int codigoEmpresaMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repositorioUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWork);
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repositorioUsuarioMobileCliente.BuscarPorCliente(codigoEmpresaMultisoftware);
            return usuarioMobileCliente;
        }

        protected Dominio.Entidades.Veiculo ObterVeiculoPorPlaca(Repositorio.UnitOfWork unitOfWork, string placa)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

            return repositorioVeiculo.BuscarPorPlaca(placa);
        }

        protected Dominio.Entidades.Veiculo ObterVeiculoPorMotorista(Repositorio.UnitOfWork unitOfWork, int codigoMotorista)
        {
            if (codigoMotorista > 0)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                return repositorioVeiculo.BuscarPorMotorista(codigoMotorista);
            }
            return null;
        }

        protected Dominio.Entidades.Veiculo ObterVeiculoPorCargaNoMonitoramento(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(codigoCarga);
                return monitoramento?.Veiculo ?? null;
            }
            return null;
        }

        protected Dominio.Entidades.Veiculo ObterVeiculoPorCargaNoMonitoramentoOuMotorista(Repositorio.UnitOfWork unitOfWork, int codigoCarga, int codigoMotorista)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramento(unitOfWork, codigoCarga);
            if (veiculo == null)
            {
                veiculo = ObterVeiculoPorMotorista(unitOfWork, codigoMotorista);
            }
            return veiculo;
        }

        protected bool CargaEmMonitoramento(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento config = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (config?.ValidarCargaEmMonitoramentoAoReceberPosicao ?? false)
            {
                return repMonitoramento.CargaEstaEmMonitoramento(codigoCarga);
            }
            else
                return true;
        }

        protected Dominio.Entidades.WebService.Integradora ValidarToken(string token)
        {
            return new Dominio.Entidades.WebService.Integradora()
            {
                Ativo = true,
                Descricao = "MultiApp",
                Token = "token"
            };
        }

        private List<int?> DetalharVersao(string versao)
        {
            List<string> versaoExpandida = versao.Split('.').ToList();
            List<int?> versaoDetalhada = new List<int?>();

            foreach (string ver in versaoExpandida)
            {
                if (ver != "*" && int.TryParse(ver, out int intVer))
                    versaoDetalhada.Add(intVer);
                else
                    versaoDetalhada.Add(null);
            }

            return versaoDetalhada;
        }

        protected bool VerificarVersaoMinima(string versaoAparelho, string versaoVerificacao, bool valorPadrao)
        {
            if (string.IsNullOrWhiteSpace(versaoAparelho))
                return valorPadrao;

            List<int?> detalheVersaoAparelho = DetalharVersao(versaoAparelho);
            List<int?> detalheVersaoVerificacao = DetalharVersao(versaoVerificacao);

            if (detalheVersaoAparelho.Count != 3 || detalheVersaoVerificacao.Count != 3)
            {
                Servicos.Log.TratarErro($"Inconsistência no método. versaoAparelho: {versaoAparelho} versaoVerificacao: {versaoVerificacao}", "VerificarVersaoMinima");
                return valorPadrao;
            }

            throw new Exception("Método não finalizado");

            return true;
        }

        protected AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile ValidarSessao(AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            string sessao = headers["Sessao"];
            
            int.TryParse(Startup.appSettingsAD["AppSettings:TempoSessao"]?.ToString(), out int minutos);

            if (minutos == 0)
                minutos = 30;

            if (!string.IsNullOrEmpty(sessao))
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorSessao(sessao, DateTime.Now.AddMinutes(-minutos));

                if (usuarioMobile != null)
                {
                    usuarioMobile.DataSessao = DateTime.Now;
                    repUsuarioMobile.Atualizar(usuarioMobile);
                    return usuarioMobile;
                }
            }

            return null;
        }

        #endregion
    }
}