using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST
{
    public abstract class BaseService : ControllerBase
    {
        #region Construtores

        protected readonly IConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        public BaseService(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

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
                string host = Conexao.ObterHost(Request, _webHostEnvironment);
                string cacheKey = $"ClienteURLAcesso{host}";

                if (!_memoryCache.TryGetValue(cacheKey, out _clienteURLAcesso))
                {
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment));
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    _clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);

                    // Armazena em cache com tempo de expiração de 60 minutos
                    _memoryCache.Set(cacheKey, _clienteURLAcesso, TimeSpan.FromMinutes(60));

                    _tipoServicoMultisoftware = _clienteURLAcesso.TipoServicoMultisoftware;
                    _cliente = _clienteURLAcesso.Cliente;

                    unitOfWork.Dispose();
                }
                else
                {
                    _tipoServicoMultisoftware = _clienteURLAcesso.TipoServicoMultisoftware;
                    _cliente = _clienteURLAcesso.Cliente;
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

        private async Task ValidarPermissaoAcessoAsync(Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (integradora.TodosWebServicesLiberados)
                return;

            string nomeMetodo = ControllerContext.ActionDescriptor.ActionName;
            Repositorio.WebService.PermissaoWebservice repositorioPermissaoWebservice = new Repositorio.WebService.PermissaoWebservice(unitOfWork, cancellationToken);
            Dominio.Entidades.WebService.PermissaoWebservice permissaoWebservice = await repositorioPermissaoWebservice.BuscarPorIntegradoraNomeMetodoAsync(integradora.Codigo, nomeMetodo);

            if (permissaoWebservice == null)
            {
                Servicos.Log.TratarErro($"Método sem permissão. Integradora: {integradora.Codigo}, Método: {nomeMetodo}");
                throw new WebServiceException($"Não autorizado. Método {nomeMetodo} sem permissão.");
            }

            if ((permissaoWebservice.UltimoReset.AddSeconds(60) > DateTime.Now) && (permissaoWebservice.RequisicoesMinuto > permissaoWebservice.QtdRequisicoes))
            {
                permissaoWebservice.QtdRequisicoes++;
                await repositorioPermissaoWebservice.AtualizarAsync(permissaoWebservice);
            }
            else if (permissaoWebservice.UltimoReset.AddSeconds(60) <= DateTime.Now)
            {
                permissaoWebservice.QtdRequisicoes = 1;
                permissaoWebservice.UltimoReset = DateTime.Now;
                await repositorioPermissaoWebservice.AtualizarAsync(permissaoWebservice);
            }
            else
            {
                Servicos.Log.TratarErro($"Excesso de execuções dentro de um minuto. Integradora: {integradora.Codigo}, Método: {permissaoWebservice.NomeMetodo}, Data último reset: {permissaoWebservice.UltimoReset.ToDateTimeString()}");
                throw new WebServiceException($"Não autorizado. Excesso de execuções dentro de um minuto do método {permissaoWebservice.NomeMetodo}.");
            }
        }

        private async Task<Dominio.Entidades.WebService.Integradora> ValidarTokenAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            string token = ObterToken();

            if (string.IsNullOrWhiteSpace(token))
                throw new WebServiceException("Não autorizado. Token não informado.");

            Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork, cancellationToken);
            Dominio.Entidades.WebService.Integradora integradora = await repositorioIntegradora.BuscarPorTokenAsync(token);

            if (!(integradora?.Ativo ?? false))
                throw new WebServiceException("Não autorizado. Token inválido.");

            await ValidarPermissaoAcessoAsync(integradora, unitOfWork, cancellationToken);

            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            SetarIntegradora(integradora, ip);

            return integradora;
        }

        private string ObterToken()
        {
            string token = (string)Request.Headers["token"] ?? (string)Request.Headers["key"];

            if (!string.IsNullOrWhiteSpace(token))
                return token;

            token = (string)Request.Headers["authorization"];

            if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer"))
                token = token.Substring("Bearer".Length).Trim();

            return token;
        }

        #endregion

        #region Métodos Protegidos

        protected Dominio.ObjetosDeValor.WebService.Retorno<T> ProcessarRequisicao<T>(Func<Dominio.Entidades.WebService.Integradora, Repositorio.UnitOfWork, Dominio.ObjetosDeValor.WebService.Retorno<T>> fnExecute)
        {
            Repositorio.UnitOfWork unitOfWork = null;
            CancellationToken cancellationToken = new CancellationToken();

            try
            {
                unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                Dominio.Entidades.WebService.Integradora integradora = ValidarTokenAsync(unitOfWork, cancellationToken).GetAwaiter().GetResult();

                return fnExecute(integradora, unitOfWork);
            }
            catch (BaseException ex)
            {
                return Dominio.ObjetosDeValor.WebService.Retorno<T>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<T>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
        }

        protected async Task<Dominio.ObjetosDeValor.WebService.Retorno<T>> ProcessarRequisicaoAsync<T>(Func<Dominio.Entidades.WebService.Integradora, Repositorio.UnitOfWork, CancellationToken, Task<Dominio.ObjetosDeValor.WebService.Retorno<T>>> fnExecute)
        {
            Repositorio.UnitOfWork unitOfWork = null;
            CancellationToken cancellationToken = new CancellationToken();

            try
            {
                unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                Dominio.Entidades.WebService.Integradora integradora = await ValidarTokenAsync(unitOfWork, cancellationToken);

                return await fnExecute(integradora, unitOfWork, cancellationToken);
            }
            catch (BaseException ex)
            {
                return Dominio.ObjetosDeValor.WebService.Retorno<T>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<T>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
            finally
            {
                if (unitOfWork != null)
                    await unitOfWork.DisposeAsync();
            }
        }

        protected void ArmazenarLogIntegracao(object entidade)
        {
            try
            {
                string entidadeJson = Newtonsoft.Json.JsonConvert.SerializeObject(entidade);

                Utilidades.IO.FileStorageService.Storage.WriteAllText(ObterCaminhoArquivoLog(), entidadeJson);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Falha ao serializar os dados para gerar o log: " + excecao);
            }
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
            return "";
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract OrigemAuditado ObterOrigemAuditado();

        #endregion
    }
}
