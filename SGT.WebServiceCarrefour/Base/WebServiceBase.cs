using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SGT.WebServiceCarrefour
{
    public abstract class WebServiceBase
    {
        #region Propriedades Protegidas

        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente Cliente
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _cliente = ClienteAcesso.Cliente;

                return _cliente;
            }
        }

        private string _webServiceConsultaCTe;
        protected string WebServiceConsultaCTe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_webServiceConsultaCTe))
                    _webServiceConsultaCTe = ClienteAcesso.WebServiceConsultaCTe;

                return _webServiceConsultaCTe;
            }
        }

        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware
        {
            get
            {
                if (_clienteURLAcesso == null)
                    _tipoServicoMultisoftware = ClienteAcesso.TipoServicoMultisoftware;

                return _tipoServicoMultisoftware;
            }
        }

        private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteURLAcesso;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso ClienteAcesso
        {
            get
            {
                if (_clienteURLAcesso == null)
                {
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    _clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.ObterHost);
                    _tipoServicoMultisoftware = _clienteURLAcesso.TipoServicoMultisoftware;
                    _cliente = _clienteURLAcesso.Cliente;
                    unitOfWork.Dispose();
                }

                return _clienteURLAcesso;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        protected Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (_auditado == null)
                {
                    _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                        OrigemAuditado = ObterOrigemAuditado()
                    };
                }

                return _auditado;
            }
        }

        #endregion

        #region Métodos Privados

        private void AuditarIntegracao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao statusRetorno, string nomeMetodo = "")
        {
            OrigemAuditado origem = ObterOrigemAuditado();

            try
            {
                Dominio.Entidades.Auditoria.HistoricoIntegracao historicoIntegracao = new Dominio.Entidades.Auditoria.HistoricoIntegracao()
                {
                    CodigoIntegracao = codigoIntegracao,
                    Data = DateTime.Now,
                    Integradora = Auditado.Integradora,
                    NomeMetodo = nomeMetodo,
                    Origem = origem,
                    Retorno = mensagemRetorno,
                    StatusRetorno = statusRetorno,
                };

                new Repositorio.Auditoria.HistoricoIntegracao(unitOfWork).Inserir(historicoIntegracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Falha ao auditar integração do método {nomeMetodo} do WS {origem.ObterDescricao()}: " + excecao);
            }
        }

        private void SetarIntegradora(Dominio.Entidades.WebService.Integradora integradora, string ipOrigem)
        {
            if (_auditado == null)
            {
                _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                    OrigemAuditado = ObterOrigemAuditado()
                };
            }

            _auditado.Integradora = integradora;
            _auditado.IP = Utilidades.String.Left(ipOrigem, 50);
        }

        #endregion

        #region Métodos Protegidos

        protected void ArmazenarLogParametros(string log)
        {
            Utilidades.IO.FileStorageService.Storage.WriteAllText(ObterCaminhoArquivoLog(), log);
        }

        protected void AuditarRetornoDadosInvalidos(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.DadosInvalidos, nomeMetodo);
        }

        protected void AuditarRetornoDadosInvalidosCNPJTransportador(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.CNPNaoCadastrado, nomeMetodo);
        }

        protected void AuditarRetornoDuplicidadeDaRequisicao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.DuplicidadeDaRequisicao, nomeMetodo);
        }

        protected void AuditarRetornoFalhaGenerica(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.FalhaGenerica, nomeMetodo);
        }

        protected virtual string ObterCaminhoArquivoLog()
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoTempArquivosImportacao"], Guid.NewGuid() + "_integracaoPedidos.txt");
        }

        protected Dominio.Entidades.WebService.Integradora ValidarToken()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                OperationContext context = OperationContext.Current;
                MessageProperties properties = context.IncomingMessageProperties;
                MessageHeaders headers = context.IncomingMessageHeaders;

                if (headers.FindHeader("Token", "Token") == -1)
                    throw new FaultException("Token inválido. Adicione a tag do token no header (cabeçalho) da requisição, conforme exemplo: <Token xmlns='Token'>seu token</Token>");

                try
                {
                    string token = Convert.ToString(headers.GetHeader<string>("Token", "Token"));

                    Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                    Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(token);

                    if (integradora != null && integradora.Ativo)
                    {
                        RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                        string ip = endpoint.Address;

                        SetarIntegradora(integradora, ip);
                        return integradora;
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Token " + token + " inválido.");

                        throw new FaultException("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");
                    }
                }
                catch (Exception)
                {
#if DEBUG
                    return null;
#endif
                    throw;
                }
            }
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract OrigemAuditado ObterOrigemAuditado();

        #endregion
    }
}