using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class MonitoramentoControleEntrega : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega, Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoControleEntregas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.Monitoramento _repositorioMonitoramento;

        #endregion

        #region Construtores

        public MonitoramentoControleEntrega(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoControleEntregas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMonitoramento.ConsultarRelatorioMonitoramentoControleEntrega(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMonitoramento.ContarConsultarRelatorioMonitoramentoControleEntrega(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/MonitoramentoControleEntrega";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioMonitoramentoControleEntrega filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataMonitoramentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataMonitoramentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNotaFiscal", filtrosPesquisa.NumeroNotaFiscal > 0 ? filtrosPesquisa.NumeroNotaFiscal.ToString() : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", !string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador) ? filtrosPesquisa.CodigoCargaEmbarcador : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", !string.IsNullOrEmpty(filtrosPesquisa.NumeroPedido) ? filtrosPesquisa.NumeroPedido : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
