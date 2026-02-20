using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.CTe;
using Dominio.ObjetosDeValor.WebService.NFS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class NFSController : BaseService
    {
        #region Construtores

        public NFSController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Públicos

        /// <summary>
        /// Pesquisa NFS Por Carga
        /// </summary>
        /// <param name="dadosRequest">Dados Para Buscar NFS Por carga</param>
        /// <returns></returns>
        [HttpPost("BuscarNFSsPorCarga")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorCarga(RequestCtePorCarga dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSsPorCarga(dadosRequest, integradora);
            });
        }

        /// <summary>
        /// Pesquisa NFS Por Ocorrencia
        /// </summary>
        /// <param name="dadosRequest">Dados Para Buscar NFS Por Ocorrencia</param>
        /// <returns></returns>
        [HttpPost("BuscarNFSsPorOcorrencia")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorOcocorrencia(RequestNFSOcorrencia dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSsPorOcocorrencia(dadosRequest, integradora);
            });
        }

        /// <summary>
        /// Obter as pré NFS-e que estão aguardando serem emitidas fora.
        /// </summary>
        /// <param name="dataInicioEmissao">Data inicial para busca por data de emissão da pré NFS-e.</param>
        /// <param name="dataFimEmissao">Data final para busca por data de emissão da pré NFS-e.</param>
        /// <param name="numeroCarga">Número da carga.</param>
        /// <returns></returns>
        [HttpGet("BuscarPreNFSeAguardandoEmissao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe>> BuscarPreNFSeAguardandoEmissao(DateTime dataInicioEmissao, DateTime dataFimEmissao, string numeroCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarPreNFSeAguardandoEmissao(dataInicioEmissao, dataFimEmissao, numeroCarga);
            });
        }

        /// <summary>
        /// Enviar as NFS-e que foram emitidas para vincular com as pré NFS-e que estão aguardando a emissão fora.
        /// </summary>
        /// <param name="notasFiscaisServico">Lista de NFS-e emitidas.</param>
        /// <returns></returns>
        [HttpPost("EnviarNFSeEmitidas")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarNFSeEmitidas(List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NFSe> notasFiscaisServico)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).EnviarNFSeEmitidas(notasFiscaisServico);
            });
        }

        [HttpPost("ConfirmarIntegracaoNFSComplementar")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoNFSComplementar(int protocoloNFS)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoNFSComplementar(protocoloNFS);
            });
        }

        [HttpPost("BuscarNFSesComplementaresAguardandoIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> BuscarNFSesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSesComplementaresAguardandoIntegracao(tipoDocumentoRetorno, inicio, limite);
            });
        }

        /// <summary>
        /// Busca de NFSs por Periodo
        /// </summary>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <param name="tipoDocumentoRetorno">Tipo Documento Retorno</param>
        /// <param name="inicio">Inicio Paginação</param>
        /// <param name="limite">Limite Paginação</param>
        /// <param name="codigoTipoOperacao">Codigo Tipo Operação</param>
        /// <param name="situacao">Situacao</param>
        /// <returns></returns>

        [HttpPost("BuscarNFSsPorPeriodo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite, string codigoTipoOperacao, string situacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSsPorPeriodo(dataInicial, dataFinal, tipoDocumentoRetorno, inicio, limite, codigoTipoOperacao, situacao, integradora);
            });
        }

        /// <summary>
        /// Notas Fiscais aguardando Integração
        /// </summary>
        /// <param name="inicio">Inicio</param>
        /// <param name="limite">Limite de Registros</param>
        /// <returns></returns>
        [HttpPost("BuscarNFSAguardandoIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarNFSAguardandoIntegracao(int inicio, int limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSAguardandoIntegracao(inicio, limite);
            });
        }

        [HttpPost("BuscarNFSPorProtocolo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFSPorProtocolo(int protocoloNFS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarNFSPorProtocolo(protocoloNFS, tipoDocumentoRetorno);
            });
        }

        [HttpPost("ConfirmarIntegracaoNFS")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoNFS(int protocoloNFS)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFS.NFS(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoNFS(protocoloNFS);
            });
        }

    #endregion Métodos Públicos

    #region Métodos Privados

    protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceNFS;
        }

        #endregion Métodos Privados
    }
}
