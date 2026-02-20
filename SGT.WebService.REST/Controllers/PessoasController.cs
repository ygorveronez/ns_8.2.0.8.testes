using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de Clientes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PessoasController : BaseService
    {
        #region Construtores
        public PessoasController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adicionar ou atualizar Cliente.
        /// </summary>
        /// <param name="clienteIntegracao">Cliente que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("SalvarCliente")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarCliente(clienteIntegracao, Auditado, TipoServicoMultisoftware);
            });
        }


        /// <summary>
        /// Retorna os Clientes que Estiverem Pendentes de Integração.
        /// </summary>
        /// <param name="quantidade">Quantidade de registro que deseja processar (Opcional). Hoje por pardrão é retornado 50 registros</param>
        /// <returns></returns>
        [HttpPost("BuscarClientesPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarClientesPendentesIntegracao(quantidade);
            });
        }

        /// <summary>
        /// Confirma os clientes Integrados
        /// </summary>
        /// <param name="protocolos">Protocolos dos clientes integrados.</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoPessoa")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoPessoa(List<long> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoPessoa(protocolos);
            });
        }

        /// <summary>
        /// Buscar Clientes.
        /// </summary>
        /// <param name="inicio">Número inicial da consulta.</param>
        /// <param name="limite">Número máximo de registros que serão trazidos pela consulta, sendo no máximo de 100 registros.</param>
        /// <param name="consultarApenasAtualizados">Buscar somente clientes atualizados.</param>
        /// <returns></returns>
        [HttpPost("BuscarPessoas")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarPessoas(int inicio, int limite, bool consultarApenasAtualizados)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarPessoas(inicio, limite, consultarApenasAtualizados);
            });
        }

        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServicePessoas;
        }
        #endregion
    }
}
