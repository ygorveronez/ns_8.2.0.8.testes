using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pedido
{
    public class PedidoDevolucao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDevolucao, Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDevolucao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.Pedido _repositorioPedidoDevolucao;

        #endregion

        #region Construtores
        public PedidoDevolucao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedidoDevolucao = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        }

        #endregion

        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDevolucao> ConsultarRegistros(FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioPedidoDevolucao.ConsultarRelatorioPedidoDevolucao(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPedidoDevolucao.ContarConsultaRelatorioPedidoDevolucao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pedidos/PedidoDevolucao";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioMotivo = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repositorioCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = filtrosPesquisa.CodigoPedido > 0 ? repositorioPedido.BuscarPorCodigo(filtrosPesquisa.CodigoPedido) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CodigoCliente > 0 ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoCliente) : null;
            Dominio.Entidades.TipoDeOcorrenciaDeCTe motivo = filtrosPesquisa.CodigoMotivo > 0 ? repositorioMotivo.BuscarPorCodigo(filtrosPesquisa.CodigoMotivo) : null;

            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador ?? string.Empty));
            parametros.Add(new Parametro("Pedido", pedido?.NumeroPedidoEmbarcador ?? string.Empty));
            parametros.Add(new Parametro("TipoDevolucao", filtrosPesquisa.TipoDevolucao != null ? filtrosPesquisa.TipoDevolucao == TipoColetaEntregaDevolucao.Parcial ? TipoColetaEntregaDevolucaoHelper.ObterDescricao(TipoColetaEntregaDevolucao.Parcial) : TipoColetaEntregaDevolucaoHelper.ObterDescricao(TipoColetaEntregaDevolucao.Total) : string.Empty));
            parametros.Add(new Parametro("NumeroNF", filtrosPesquisa.NumeroNF));
            parametros.Add(new Parametro("DataEmissaoNF", filtrosPesquisa.DataEmissaoNFInicial, filtrosPesquisa.DataEmissaoNFFinal));
            parametros.Add(new Parametro("Transportador", empresa?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Cliente", cliente?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Motivo", motivo?.Descricao ?? string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
