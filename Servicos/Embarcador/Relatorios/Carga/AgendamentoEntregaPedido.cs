using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class AgendamentoEntregaPedido : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaPedido _repositorioCargaPedido;

        #endregion

        #region Construtores

        public AgendamentoEntregaPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaPedido.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaPedido.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/AgendamentoEntregaPedido";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CodigoCliente > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoCliente) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoAgendamento", filtrosPesquisa.DataAgendamentoInicial, filtrosPesquisa.DataAgendamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoCarregamento", filtrosPesquisa.DataCarregamentoInicial, filtrosPesquisa.DataCarregamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoPrevisaoEntrega", filtrosPesquisa.DataPrevisaoEntregaInicial, filtrosPesquisa.DataPrevisaoEntregaFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoCriacaoPedido", filtrosPesquisa.DataCriacaoPedidoInicial, filtrosPesquisa.DataCriacaoPedidoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoSugestaoEntrega", filtrosPesquisa.DataInicialSugestaoEntrega, filtrosPesquisa.DataFinalSugestaoEntrega));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiNotaFiscalVinculada", filtrosPesquisa.PossuiDataSugestaoEntrega));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiDataSugestaoEntrega", filtrosPesquisa.PossuiDataSugestaoEntrega));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiDataTerminoCarregamento", filtrosPesquisa.PossuiDataTerminoCarregamento));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAgendamento", filtrosPesquisa.SituacaoAgendamento?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", cliente?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.Carga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirCargasAgrupadas", filtrosPesquisa.ExibirCargasAgrupadas ? "Sim" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataSugestaoEntregaFormatada")
                return "DataSugestaoEntrega";

            else if (propriedadeOrdenarOuAgrupar == "DataCarregamentoInicialFormatada")
                return "DataCarregamentoInicial";

            else if (propriedadeOrdenarOuAgrupar == "DataCarregamentoFinalFormatada")
                return "DataCarregamentoFinal";

            else if (propriedadeOrdenarOuAgrupar == "DataAgendamentoFormatada")
                return "DataAgendamento";

            else if (propriedadeOrdenarOuAgrupar == "SituacaoCargaFormatada")
                return "SituacaoCarga";

            else if (propriedadeOrdenarOuAgrupar == "DataPrevisaoEntregaFormatada")
                return "DataPrevisaoEntrega";

            else if (propriedadeOrdenarOuAgrupar == "DataCriacaoPedidoFormatada")
                return "DataCriacaoPedido";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}