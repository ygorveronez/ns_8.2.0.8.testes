using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ModeloDadosController : BaseService
    {
        #region Construtores

        public ModeloDadosController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Públicos

        /// <summary>
        /// Obter os dados de atendimentos.
        /// </summary>
        /// <param name="dataInicioCriacao">Data inicial para busca por data de criação do atendimento.</param>
        /// <param name="dataFimCriacao">Data final para busca por data de criação do atendimento.</param>
        /// <param name="numeroOcorrencia">Número do atendimento.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosAtendimentos")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento>> BuscarDadosAtendimentos(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroOcorrencia)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosAtendimentos(dataInicioCriacao, dataFimCriacao, numeroOcorrencia);
            });
        }

        /// <summary>
        /// Obter os dados das cargas faturadas.
        /// </summary>
        /// <param name="dataAtualizacaoInicial">Data inicial para busca por data de criação da carga.</param>
        /// <param name="dataAtualizacaoFinal">Data final para busca por data de criação da carga.</param>
        /// <param name="numeroCarga">Número da carga.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosCargas")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Carga>> BuscarDadosCargas(DateTime dataAtualizacaoInicial, DateTime dataAtualizacaoFinal, string numeroCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosCargas(dataAtualizacaoInicial, dataAtualizacaoFinal, numeroCarga);
            });
        }

        /// <summary>
        /// Obter os dados das entregas por cargas faturadas.
        /// </summary>
        /// <param name="dataAtualizacaoInicial">Data inicial para busca por data de criação da carga.</param>
        /// <param name="dataAtualizacaoFinal">Data final para busca por data de criação da carga.</param>
        /// <param name="numeroCarga">Número da carga.</param>
        /// <param name="numeroPedido">Número do pedido.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosEntregas")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega>> BuscarDadosEntregas(DateTime dataAtualizacaoInicial, DateTime dataAtualizacaoFinal, string numeroCarga, string numeroPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosEntregas(dataAtualizacaoInicial, dataAtualizacaoFinal, numeroCarga, numeroPedido);
            });
        }

        /// <summary>
        /// Obter os dados de monitoramento por carga.
        /// </summary>
        /// <param name="dataCriacaoCargaInicial">Data inicial para busca por data de criação da carga.</param>
        /// <param name="dataCriacaoCargaFinal">Data final para busca por data de criação da carga.</param>
        /// <param name="numeroCarga">Número da carga.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosMonitoramento")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento>>> BuscarDadosMonitoramentoAsync(DateTime? dataCriacaoCargaInicial, DateTime? dataCriacaoCargaFinal, string numeroCarga)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosMonitoramentoAsync(dataCriacaoCargaInicial, dataCriacaoCargaFinal, numeroCarga, cancellationToken);
            });
        }

        /// <summary>
        /// Obter os dados de ocorrências.
        /// </summary>
        /// <param name="dataInicioCriacao">Data inicial para busca por data de criação da ocorrência.</param>
        /// <param name="dataFimCriacao">Data final para busca por data de criação da ocorrência.</param>
        /// <param name="numeroOcorrencia">Número da ocorrência.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosOcorrencias")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia>> BuscarDadosOcorrencias(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroOcorrencia)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosOcorrencias(dataInicioCriacao, dataFimCriacao, numeroOcorrencia);
            });
        }

        /// <summary>
        /// Obter os dados de pedidos.
        /// </summary>
        /// <param name="dataInicioCriacao">Data inicial para busca por data de criação do pedido.</param>
        /// <param name="dataFimCriacao">Data final para busca por data de criação do pedido.</param>
        /// <param name="numeroPedido">Número do pedido.</param>
        /// <returns></returns>
        [HttpGet("BuscarDadosPedidos")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido>> BuscarDadosPedidos(DateTime dataInicioCriacao, DateTime dataFimCriacao, string numeroPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ModeloDados.ModeloDados(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarDadosPedidos(dataInicioCriacao, dataFimCriacao, numeroPedido);
            });
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceModeloDados;
        }

        #endregion Métodos Privados
    }
}
