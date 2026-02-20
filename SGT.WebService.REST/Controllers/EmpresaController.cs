using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de empresa.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EmpresaController : BaseService
    {
        #region Construtores

        public EmpresaController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion


        #region Métodos Públicos

        /// <summary>
        /// Adicionar ou atualizar Transportador.
        /// </summary>
        /// <param name="transportador">Transportador que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("SalvarTransportador")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarTransportador(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarTransportador(transportador);
            });
        }

        /// <summary>
        /// Adicionar ou atualizar Motorista.
        /// </summary>
        /// <param name="motoristaIntegracao">Motorista que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("SalvarMotorista")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarMotorista(motoristaIntegracao, integradora);
            });
        }

        /// <summary>
        /// Adicionar ou atualizar Veiculo.
        /// </summary>
        /// <param name="veiculoIntegracao">Veiculo que será adicionado ou atualizado.</param>
        /// <returns></returns>
        [HttpPost("SalvarVeiculo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarVeiculo(veiculoIntegracao, integradora);
            });
        }

        /// <summary>
        /// Adicionar Situação Colaborador.
        /// </summary>
        /// <param name="situacaoColaboradorIntegracao">Situação Colaborador que será adicionado.</param>
        /// <returns></returns>
        [HttpPost("SalvarSituacaoColaborador")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarSituacaoColaborador(Dominio.ObjetosDeValor.Embarcador.Carga.SituacaoColaboradorIntegracaoWS situacaoColaboradorIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Pessoa.ColaboradorSituacaoLancamento(unitOfWork, Auditado, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarSituacaoColaborador(situacaoColaboradorIntegracao);
            });
        }

        /// <summary>
        /// Busca os Transportadores Pendente de Integração.
        /// </summary>
        /// <param name="quantidade">Quantidade pode informar a quantidade que deseja processar por padrão é (50).(Opcional)</param>
        /// <returns></returns>
        [HttpPost("BuscarTransportadoresPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarTransportadoresPendentesIntegracao(quantidade);
            });
        }

        /// <summary>
        /// Confirma os transportadores Integrados
        /// </summary>
        /// <param name="protocolos">Protocolos dos transportadores integrados.</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoTransportador")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTransportador(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoTransportador(protocolos);
            });
        }

        /// <summary>
        /// Busca os Veiculos Pendente de Integração.
        /// </summary>
        /// <param name="quantidade">Quantidade pode informar a quantidade que deseja processar por padrão é (50).(Opcional)</param>
        /// <returns></returns>
        [HttpPost("BuscarVeiculosPendentesIntegracao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarVeiculosPendentesIntegracao(quantidade);
            });
        }


        /// <summary>
        /// Confirma os Veiculos Integrados
        /// </summary>
        /// <param name="protocolos">Protocolos dos veiculos integrados.</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoVeiculo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoVeiculo(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmarIntegracaoVeiculo(protocolos);
            });
        }

        /// <summary>
        /// Retorna os veículos com rastreadores vinculados no período informado
        /// </summary>
        /// <param name="dataModificacao">Período para obtenção dos dados.</param>
        /// <returns></returns>
        [HttpPost("ObterRastreadoresVeiculosPorData")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores> ObterRastreadoresVeiculosPorData(Dominio.ObjetosDeValor.Embarcador.Veiculos.DataConsultaModificacao dataModificacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterRastreadoresVeiculosPorData(dataModificacao);
            });
        }

        #endregion
        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceEmpresa;
        }
        #endregion
    }
}
