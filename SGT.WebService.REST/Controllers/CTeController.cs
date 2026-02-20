using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.CTe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de CT-e.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CTeController : BaseService
    {
        #region Construtores

        public CTeController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Publicos

        /// <summary>
        /// Salva dados do Mercante.
        /// </summary>
        /// <param name="dadosDoMercante">Dados do Mercantes</param>
        /// <returns></returns>
        [HttpPost("SalvarDadosDoMercante")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarDadosDoMercante(Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarDadosDoMercante(dadosDoMercante);
            });
        }

        /// <summary>
        /// Vincula notas em uma determinada carga.
        /// </summary>
        /// <param name="dadosCtes">Notas e protocolo da carga</param>
        /// <returns></returns>
        [HttpPost("IntegrarDadosCTeAnteriores")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarDadosCTeAnteriores(Dominio.ObjetosDeValor.WebService.CTe.DadosCTes dadosCtes)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IntegrarDadosCTeAnteriores(dadosCtes);
            });
        }

        /// <summary>
        /// Busca de CT-es por protocolo
        /// </summary>
        /// <param name="protocolo">Dados dos protocolos de integração</param>
        /// <param name="tipoDocumentoRetorno">Tipo de documento à ser retornado</param>
        /// <param name="inicio">Início dos registros (paginação)</param>
        /// <param name="limite">Limite dos registros (paginação)</param>
        /// <returns></returns>
        [HttpPost("BuscarCTes")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTes(protocolo, tipoDocumentoRetorno, inicio, limite);
            });
        }

        /// <summary>
        /// Busca de dados da fatura e CT-es por protocolo
        /// </summary>
        /// <param name="protocolo">Dados dos protocolos de integração</param>
        /// <param name="tipoDocumentoRetorno">Tipo de documento à ser retornado</param>
        /// <param name="inicio">Início dos registros (paginação)</param>
        /// <param name="limite">Limite dos registros (paginação)</param>
        /// <returns></returns>
        [HttpPost("BuscarFaturaCTes")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> BuscarFaturaCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarFaturaCTes(protocolo, tipoDocumentoRetorno, inicio, limite);
            });
        }

        /// <summary>
        /// Anulação gerencial de um CT-e
        /// </summary>
        /// <param name="requestAnulacao">Pametros necessarios para realizar a anulução</param>
        /// <returns></returns>
        [HttpPost("RealizarAnulacaoGerencial")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RealizarAnulacaoGerencial(RequestAnulacaoGerencial requestAnulacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RealizarAnulacaoGerencial(requestAnulacao);
            });
        }

        /// <summary>
        /// Busca os CTe-s Por Carga
        /// </summary>
        /// <param name="dadosRequest">Dados Para Buscar Cte Por carga</param>
        /// <returns></returns>
        [HttpPost("BuscarCTesPorCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorCarga(RequestCtePorCarga dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTesPorCarga(dadosRequest, integradora);
            });
        }

        /// <summary>
        /// Buscar Ctes por ocorrencias
        /// </summary>
        /// <param name="dadosRequestCteOcorrencia">Dados para busca dos ctes</param>
        /// <returns></returns>
        [HttpPost("BuscarCTesPorOcorrencia")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorOcorrencia(RequestCtePorOcorrencia dadosRequestCteOcorrencia)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTesPorOcorrencia(dadosRequestCteOcorrencia, integradora);
            });
        }

        /// <summary>
        /// Receberá CT-es Anteriores.
        /// </summary>
        /// <param name="requestCteAnteriores">Dados da Requisição do CT-e</param>
        /// <returns></returns>
        [HttpPost("EnviarCTesAnteriores")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCTesAnteriores(Dominio.ObjetosDeValor.WebService.CTe.RequestCteAnteriores requestCteAnteriores)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).EnviarCTesAnteriores(requestCteAnteriores);
            });
        }

        /// <summary>
        /// Informar Previsão Pagamento CTe.
        /// </summary>
        /// <param name="protocoloCTe">Protocolo do CTe</param>
        /// <param name="dataPrevisaoPagamento">Data previsão do pagamento</param>
        /// <param name="observacao">Observação</param>
        /// <param name="sequenciaParcela"></param>
        /// <returns></returns>
        [HttpPost("InformarPrevisaoPagamentoCTe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarPrevisaoPagamentoCTe(int protocoloCTe, string dataPrevisaoPagamento, string observacao, int sequenciaParcela, Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe informarPrevisaoPagamentoCTe = null)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                if (informarPrevisaoPagamentoCTe == null)
                {
                    informarPrevisaoPagamentoCTe = new Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe()
                    {
                        ProtocoloCTe = protocoloCTe,
                        DataPrevisaoPagamento = dataPrevisaoPagamento,
                        Observacao = observacao,
                        SequenciaParcela = sequenciaParcela
                    };
                }

                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarPrevisaoPagamentoCTe(new System.Collections.Generic.List<InformarPrevisaoPagamentoCTe>() { informarPrevisaoPagamentoCTe });
            });
        }

        /// <summary>
        /// Informar Bloqueio Documento.
        /// </summary>
        /// <param name="informarBloqueioDocumento"></param>
        /// <returns></returns>
        [HttpPost("InformarBloqueioDocumento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarBloqueioDocumento(Dominio.ObjetosDeValor.WebService.CTe.InformarBloqueioDocumento informarBloqueioDocumento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarBloqueioDocumento(informarBloqueioDocumento.ProtocoloCTe, informarBloqueioDocumento.DataBloqueio, informarBloqueioDocumento.Observacao);
            });
        }

        /// <summary>
        /// Informar Desbloqueio Documento.
        /// </summary>
        /// <param name="protocoloCTe">Protocolo do CTe</param>
        /// <param name="dataDesbloqueio">Data do desbloqueio</param>
        /// <param name="observacao">Observação</param>
        /// <returns></returns>
        [HttpPost("InformarDesbloqueioDocumento")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarDesbloqueioDocumento(Dominio.ObjetosDeValor.WebService.CTe.InformarDesbloqueioDocumento informarDesbloqueioDocumento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarDesbloqueioDocumento(informarDesbloqueioDocumento);
            });
        }

        /// <summary>
        /// Confirmar Pagamento do CTe.
        /// </summary>
        /// <param name="protocoloCTe">Protocolo do CTe</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="observacao">Observação</param>
        /// <param name="valorParcelaPaga"></param>
        /// <param name="sequenciaParcelaPaga"></param>
        /// <returns></returns>
        [HttpPost("ConfirmarPagamentoCTe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarPagamentoCTe(int protocoloCTe, string dataPagamento, string observacao, decimal valorParcelaPaga, int sequenciaParcelaPaga, Dominio.ObjetosDeValor.WebService.CTe.ConfirmarPagamentoCTe confirmarPagamentoCTe = null)
        {

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                if (confirmarPagamentoCTe == null)
                {
                    confirmarPagamentoCTe = new Dominio.ObjetosDeValor.WebService.CTe.ConfirmarPagamentoCTe()
                    {
                        ProtocoloCTe = protocoloCTe,
                        DataPagamento = dataPagamento,
                        Observacao = observacao,
                        ValorParcelaPaga = valorParcelaPaga,
                        SequenciaParcelaPaga = sequenciaParcelaPaga,
                    };
                }

                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarPagamentoCTe(confirmarPagamentoCTe);
            });
        }

        [HttpPost("BuscarCTesComplementaresAguardandoIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, string codificarUTF8, int inicio, int limite, string dataInicial, string dataFinal)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTesComplementaresAguardandoIntegracao(tipoDocumentoRetorno, codificarUTF8, inicio, limite, dataInicial, dataFinal);
            });
        }

        [HttpPost("ConfirmarIntegracaoCTeComplementar")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoCTeComplementar(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoCTeComplementar(protocoloCTe);
            });
        }

        [HttpPost("BuscarCTesSubstitutosAguardandoIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesSubstitutosAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, string codificarUTF8, int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTesSubstitutosAguardandoIntegracao(tipoDocumentoRetorno, codificarUTF8, inicio, limite);
            });
        }

        [HttpPost("ConfirmarIntegracaoCTeSubstituto")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoCTeSubstituto(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoCTeComplementar(protocoloCTe);
            });
        }

        /// <summary>
        /// Busca de CT-es por Periodo
        /// </summary>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <param name="tipoDocumentoRetorno">Tipo Documento Retorno</param>
        /// <param name="inicio">Inicio Paginação</param>
        /// <param name="limite">Limite Paginação</param>
        /// <param name="codigoTipoOperacao">Codigo Tipo Operação</param>
        /// <param name="situacao">Situacao</param>
        /// <returns></returns>
        [HttpPost("BuscarCTesPorPeriodo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite, string codigoTipoOperacao, string situacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTesPorPeriodo(dataInicial, dataFinal, tipoDocumentoRetorno, inicio, limite, codigoTipoOperacao, situacao, integradora);
            });
        }

        /// <summary>
        /// EnviarArquivoXMLCTe.
        /// </summary>
        /// <param name="arquivo">arquivo XML CTe</param>
        /// <returns></returns>
        [HttpPost("EnviarArquivoXMLCTe")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public Dominio.ObjetosDeValor.WebService.Retorno<string> EnviarArquivoXMLCTe(IFormFile arquivo)
        {

            if (arquivo != null && arquivo.Length > 0)
            {
                using (Stream ms = arquivo.OpenReadStream())
                {
                    return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
                    {
                        return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).EnviarArquivoXMLCTe(ms);
                    });
                }
            }
            else
            {
                return new Dominio.ObjetosDeValor.WebService.Retorno<string>()
                {
                    Mensagem = "Arquivo não informado ou vazio."
                };
            }
        }

        /// <summary>
        /// Buscar Protocolos CTes Cancelados aguardando confirmação.
        /// </summary>
        /// <param name="inicio">Inicio</param>
        /// <param name="limite">Quantidade máxima de registros</param>
        /// <returns></returns>
        [HttpPost("BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(inicio, limite, integradora);
            });
        }

        /// <summary>
        /// Buscar CTes por Protocolo.
        /// </summary>
        /// <param name="protocoloCTe">Numero do Protocolo</param>
        /// <param name="tipoDocumentoRetorno"> Tipo de Documento (Nenhum = 0, PDF = 1, XML = 2, Todos = 3)
        /// </param>
        /// <returns></returns>
        [HttpPost("BuscarCTePorProtocolo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTePorProtocolo(int protocoloCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarCTePorProtocolo(protocoloCTe, tipoDocumentoRetorno, integradora);
            });
        }

        /// <summary>
        /// Buscar CTes por Protocolo.
        /// </summary>
        /// <param name="protocoloCTe">Numero do Protocolo</param>
        /// <param name="tipoDocumentoRetorno"> Tipo de Documento (Nenhum = 0, PDF = 1, XML = 2, Todos = 3)
        /// </param>
        /// <returns></returns>
        [HttpPost("ConfirmarConsultaCTeCancelado")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarConsultaCTeCancelado(int protocoloCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarConsultaCTeCancelado(protocoloCTe, integradora);
            });
        }

        /// <summary>
        /// Cancelar CTe individualmente.
        /// </summary>
        /// <param name="protocolo">Número do Protocolo da Carga</param>
        /// <param name="chaveCte">Chave do CTe</param>
        /// <param name=justificativa">Justificativa do cancelamento</param>
        /// <returns></returns>
        [HttpPost("CancelarCTeIndividual")]
        public Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> CancelarCTeIndividual(CancelarCTeIndividual cancelarCTeIndividual)
        {
            return ProcessarRequisicaoAsync((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).CancelarCTeIndividualAsync(cancelarCTeIndividual, integradora, cancellationToken);
            });
        }

        ///// <summary>
        ///// Verificar Situacao CTES.
        ///// </summary>
        ///// <param name="chavesCTe">Chave do CTe</param>
        ///// <returns></returns>
        [HttpPost("VerificarSituacaoCTe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.SituacaoCTe>> VerificarSituacaoCTe(Dominio.ObjetosDeValor.Embarcador.CTe.VerificarSituacaoCTe verificarSituacaoCTe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.CTe.CTe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                    .VerificarSituacaoCTe(verificarSituacaoCTe);
            });
        }

        #endregion Métodos Publicos

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceCTe;
        }

        #endregion Métodos Protegidos
    }
}
