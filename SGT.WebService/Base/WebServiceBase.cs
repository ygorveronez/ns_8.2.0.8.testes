using CoreWCF;
using CoreWCF.Channels;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;

namespace SGT.WebService
{
    public abstract class WebServiceBase(IServiceProvider serviceProvider)
    {
        protected readonly IServiceProvider _serviceProvider = serviceProvider;
        private IHttpContextAccessor httpContextAcessor => serviceProvider.GetRequiredService<IHttpContextAccessor>();

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
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    _clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.createInstance(_serviceProvider).ObterHost);
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

        private void AuditarIntegracao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao, StatusRetornoRequisicao statusRetorno, string nomeMetodo = "")
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

        private void ValidarPermissaoAcesso(Dominio.Entidades.WebService.Integradora integradora, string nomeMetodo, Repositorio.UnitOfWork unitOfWork)
        {
            if (integradora.TodosWebServicesLiberados)
                return;

            Repositorio.WebService.PermissaoWebservice repositorioPermissaoWebservice = new Repositorio.WebService.PermissaoWebservice(unitOfWork);
            Dominio.Entidades.WebService.PermissaoWebservice permissaoWebservice = repositorioPermissaoWebservice.BuscarPorIntegradoraNomeMetodoAsync(integradora.Codigo, nomeMetodo).GetAwaiter().GetResult();

            if (permissaoWebservice == null)
            {
                Servicos.Log.TratarErro($"Método sem permissão. Integradora: {integradora.Codigo}, Método: {nomeMetodo}");
                throw new FaultException($"Método {nomeMetodo} sem permissão.");
            }

            if ((permissaoWebservice.UltimoReset.AddSeconds(60) > DateTime.Now) && (permissaoWebservice.RequisicoesMinuto > permissaoWebservice.QtdRequisicoes))
            {
                permissaoWebservice.QtdRequisicoes++;
                repositorioPermissaoWebservice.Atualizar(permissaoWebservice);
            }
            else if (permissaoWebservice.UltimoReset.AddSeconds(60) <= DateTime.Now)
            {
                permissaoWebservice.QtdRequisicoes = 1;
                permissaoWebservice.UltimoReset = DateTime.Now;
                repositorioPermissaoWebservice.Atualizar(permissaoWebservice);
            }
            else
            {
                Servicos.Log.TratarErro($"Excesso de execuções dentro de um minuto. Integradora: {integradora.Codigo}, Método: {permissaoWebservice.NomeMetodo}, Data último reset: {permissaoWebservice.UltimoReset.ToDateTimeString()}");
                throw new FaultException($"Excesso de execuções dentro de um minuto do método {permissaoWebservice.NomeMetodo}");
            }
        }

        #endregion

        #region Métodos Protegidos

        protected Retorno<T> ProcessarRequisicao<T>(Func<Dominio.Entidades.WebService.Integradora, Repositorio.UnitOfWork, Retorno<T>> fnExecute, T defaultValue = default(T))
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                    return fnExecute(integradora, unitOfWork);

                }
                catch (FaultException excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    return Retorno<T>.CriarRetornoExcecao(excecao.Message, defaultValue);
                }
                catch (BaseException excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    return Retorno<T>.CriarRetornoExcecao(excecao.Message, defaultValue);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    return Retorno<T>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.", defaultValue);
                }
                finally
                {
                    if (unitOfWork != null)
                        unitOfWork.Dispose();
                }
            }
        }

        protected async Task<Retorno<T>> ProcessarRequisicaoAsync<T>(Func<Dominio.Entidades.WebService.Integradora, Repositorio.UnitOfWork, Task<Retorno<T>>> fnExecute, T defaultValue = default(T))
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                return await fnExecute(integradora, unitOfWork);
            }
            catch (FaultException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<T>.CriarRetornoExcecao(excecao.Message, defaultValue);
            }
            catch (BaseException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<T>.CriarRetornoExcecao(excecao.Message, defaultValue);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<T>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.", defaultValue);
            }
            finally
            {
                if (unitOfWork != null)
                    await unitOfWork.DisposeAsync();
            }
        }

        protected void ArmazenarLogIntegracao(object entidade, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                string entidadeJson = Newtonsoft.Json.JsonConvert.SerializeObject(entidade);

                Utilidades.IO.FileStorageService.Storage.WriteAllText(ObterCaminhoArquivoLog(unitOfWork), entidadeJson);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Falha ao serializar os dados para gerar o log: " + excecao);
            }
        }

        protected void AuditarRetornoDadosInvalidos(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, StatusRetornoRequisicao.DadosInvalidos, nomeMetodo);
        }

        protected void AuditarRetornoDadosInvalidosCNPJTransportador(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, StatusRetornoRequisicao.CNPNaoCadastrado, nomeMetodo);
        }

        protected void AuditarRetornoDuplicidadeDaRequisicao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, StatusRetornoRequisicao.DuplicidadeDaRequisicao, nomeMetodo);
        }

        protected void AuditarRetornoFalhaGenerica(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, StatusRetornoRequisicao.FalhaGenerica, nomeMetodo);
        }

        protected virtual string ObterCaminhoArquivoLog(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid() + "_integracaoPedidos.txt");
        }

        /// <summary>
        /// Valida o token enviado no header da requisição
        /// </summary>
        /// <param name="nomeMetodo">Deve ser o nome do método que está sendo chamado</param>
        /// <param name="carregarClientes">Deve ser true se os clientes da integração devem ser carregados (integradora.Clientes)</param>
        /// <returns>Retorna a entidade Integradora correspondente ao token</returns>
        /// <exception cref="FaultException">Exceção lançada em caso de falha na validação do token</exception>
        protected Dominio.Entidades.WebService.Integradora ValidarToken(string nomeMetodo = "", bool carregarClientes = false)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
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

                    if (!(integradora?.Ativo ?? false))
                    {
                        Servicos.Log.TratarErro($"Token {token} inválido.");
                        throw new FaultException("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");
                    }

                    if (integradora.TipoAutenticacao == TipoAutenticacao.UsuarioESenha)
                    {
                        integradora.DataExpiracao = DateTime.Now.AddMinutes(integradora.TempoExpiracao);
                        repIntegracadora.Atualizar(integradora);
                    }

                    if (nomeMetodo == "")
                        nomeMetodo = headers.Action.Split('/')[headers.Action.Split('/').Length - 1];

                    ValidarPermissaoAcesso(integradora, nomeMetodo, unitOfWork);

                    SetarIntegradora(integradora, httpContextAcessor.HttpContext.Request.Host.ToString());

                    if (carregarClientes && integradora.Clientes != null)
                    {
                        NHibernate.NHibernateUtil.Initialize(integradora.Clientes);
                    }

                    return integradora;
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