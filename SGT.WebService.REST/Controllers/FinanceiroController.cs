using System.Collections.Generic;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.Financeiro;
using Dominio.ObjetosDeValor.WebService.NFe;
using Dominio.ObjetosDeValor.WebService.Rest.Financeiro;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// Endpoint Do Financeiro
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FinanceiroController : BaseService
    {
        #region Construtores
        public FinanceiroController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Buscar Documento Entrada Pendentes
        /// </summary>
        /// <param name="requestDocumento">Parametros necesario</param>
        /// <returns></returns>
        [HttpPost("BuscarDocumentoEntradaPendenteIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> BuscarDocumentoEntradaPendenteIntegracao(RequestDocumentoEntradaPendente requestDocumento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDocumentoEntradaPendenteIntegracao(requestDocumento);
            });
        }

        /// <summary>
        /// Confirmar Integração de Documento Entrada Pendente
        /// </summary>
        /// <param name="protocolos">Protocolos</param>we
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoDocumentoEntrada")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoDocumentoEntrada(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoDocumento(protocolos);
            });
        }


        /// <summary>
        /// Confirmar Integração migo
        /// </summary>
        /// <param name="dados">Dados para confirmar</param>
        /// <returns></returns>
        [HttpPost("ConfirmacaoMIGO")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmacaoMIGO(RequestConfirmacaoMigo dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmacaoMIGO(dados);
            });
        }

        /// <summary>
        /// Confirmar Integração miro
        /// </summary>
        /// <param name="dados">Dados para confirmar</param>
        /// <returns></returns>
        [HttpPost("ConfirmacaoMIRO")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmacaoMIRO(RequestConfirmacaoMigo dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmacaoMIRO(dados);
            });
        }

        /// <summary>
        /// Retorno Integração Miro
        /// </summary>
        /// <param name="dados">Dados para confirmar</param>
        /// <returns></returns>
        [HttpPost("RetornoMIRO")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoMIRO(RetornoMiro dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoMIRO(dados);
            });
        }

        /// <summary>
        /// Retorno Integração Miro
        /// </summary>
        /// <param name="dados">Dados para confirmar</param>
        /// <returns></returns>
        [HttpPost("RetornoDesbloqueioR")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoDesbloqueioR(DesbloqueioR dados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoDesbloqueioR(dados);
            });
        }


        /// <summary>
        /// Confirmar Integração de Titulo Financeiro
        /// </summary>
        /// <param name="protocolos">Protocolos</param>we
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoTituloFinanceiro")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTituloFinanceiro(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoTituloFinanceiro(protocolos);
            });
        }


        /// <summary>
        /// Retorna os Titulos Pendentes de Integração.
        /// </summary>
        /// <param name="quantidade">Quantidade de registro que deseja processar (Opcional). Hoje por pardrão é retornado 50 registros</param>
        /// <returns></returns>
        [HttpPost("BuscarTitulosPentendesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPentendesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarTitulosPentendesIntegracao(quantidade);
            });
        }


        /// <summary>
        /// Retorna os Titulos Pendentes de Integração.
        /// </summary>
        /// <param name="quantidade">Quantidade de registro que deseja processar (Opcional). Hoje por pardrão é retornado 50 registros</param>
        /// <returns></returns>
        [HttpPost("RetornoProvisaoDT")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoProvisaoDT(Dominio.ObjetosDeValor.WebService.Financeiro.RetornoProvisaoDT retornoProvisao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoProvisaoDT(retornoProvisao);
            });
        }

        /// <summary>
        /// Indica Anticipação de Frete Documento
        /// </summary>
        /// <param name="documentoAntecipacao">Documentos de anticipação</param>
        /// <returns></returns>
        [HttpPost("IndicarAntecipacaoFreteDocumento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IndicarAntecipacaoFreteDocumento(Dominio.ObjetosDeValor.WebService.Financeiro.DocumentoAntecipacao documentoAntecipacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IndicarAntecipacaoFreteDocumento(documentoAntecipacao);
            });
        }


        /// <summary>
        /// Envio de arquivo CSV
        /// </summary>
        /// <param name="arquivoCSV">CSV </param>
        /// <returns></returns>
        [HttpPost("ContasPagar")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ContasPagar(IFormFile arquivoCSV)
        {
            using (var ms = new MemoryStream())
            {
                arquivoCSV.CopyTo(ms);
                return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
                {
                    return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ContasPagar(ms, arquivoCSV.FileName);
                });
            }
        }

        /// <summary>
        /// Retorno Cancelamento Provisao
        /// </summary>
        /// <param name="requestDados">Dados request </param>
        /// <returns></returns>
        [HttpPost("RetornoCancelamentoProvisao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoCancelamentoProvisao(RetornoCancelamentoProvisaoRequest requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoCancelamentoProvisao(requestDados);
            });
        }

        /// <summary>
        /// Confirmar Extorno de Provisao
        /// </summary>
        /// <param name="request">Dados request </param>
        /// <returns></returns>
        [HttpPost("ConfirmarEstornoProvisao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarEstornoProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.EstornoProvisaoRequest request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarEstornoProvisao(request);
            });
        }

        /// <summary>
        /// Atualizar Situação da Provisão
        /// </summary>
        /// <param name="requestDados">Dados request</param>
        /// <returns></returns>
        [HttpPost("ConfirmarProvisao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarProvisao(AtualizarSituacaoProvisao requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarProvisao(requestDados);
            });
        }

        /// <summary>
        /// Atualizar Situação do Lote de Escrituração
        /// </summary>
        /// <param name="requestDados">Dados request</param>
        /// <returns></returns>
        [HttpPost("ConfirmarLoteEscrituracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarLoteEscrituracao(RetornoLoteEscrituracaoResult requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarLoteEscrituracao(requestDados);
            });
        }

        /// <summary>
        /// Liberar Pagamento
        /// </summary>
        /// <param name="request">Dados request </param>
        /// <returns></returns>
        [HttpPost("ConfirmarCancelamentoPagamentoCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarCancelamentoPagamentoCarga(RetornoCancelamentoPagamentoCargaRequest requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarCancelamentoPagamentoCarga(requestDados);
            });
        }

        /// <summary>
        /// Liberar Pagamento
        /// </summary>
        /// <param name="request">Dados request </param>
        /// <returns></returns>
        [HttpPost("ConfirmarCancelamentoPagamento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarCancelamentoPagamento(RetornoCancelamentoPagamentoRequest requestDados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarCancelamentoPagamento(requestDados);
            });
        }

        /// <summary>
        /// Liberar Pagamento
        /// </summary>
        /// <param name="request">Dados request </param>
        /// <returns></returns>
        [HttpPost("LiberarPagamento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> LiberarPagamento(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.LiberacaoPagamentoRequest request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).LiberarPagamento(request);
            });
        }

        /// <summary>
        /// Buscar os dados da provisão
        /// </summary>
        /// <param name="protocoloIntegracaoCarga">Protocolo da carga</param>
        /// <param name="dataEmissaoProvisaoInicial">Data inicial da emissão da provisão</param>
        /// <param name="dataEmissaoProvisaoFinal"">Data final da emissão da provisão</param>
        /// <param name="dataCriacaoCargaInicial">Data inicial de criação da carga</param>
        /// <param name="dataCriacaoCargaFinal">Data inicial de criação da carga</param>
        /// <param name="inicioRegistros"´>"Inicio dos registros</param>
        /// <param name="limiteRegistros">Limite de registros</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosProvisao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao>> BuscarDadosProvisao([FromQuery] Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.RequisicaoBuscarDadosProvisao request)
        {

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosProvisao(request);
            });
        }

        /// <summary>
        /// Buscar dados do pagamento
        /// </summary>
        /// <param name="protocoloDocumento">Protocolo do documento</param>
        /// <param name="dataEmissaoDocumentoInicial">Data inicial de emissão do documento</param>
        /// <param name="dataEmissaoDocumentoFinal ">Data final de emissão do documento</param>
        /// <param name="chaveDocumento">Chave do documento</param>
        /// <param name="Início">Inicio dos registros</param>
        /// <param name="Limite">Limite de registros</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosPagamento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento>> BuscarDadosPagamento([FromQuery] Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.RequisicaoBuscarDadosPagamento request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Financeiro.Financeiro(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosPagamento(request);
            });
        }

        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceFinanceiro;
        }

        #endregion
    }
}
