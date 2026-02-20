using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class MonitoramentoPosicaoFrotaRastreamento : RelatorioBase<FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento, Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.Monitoramento _repMonitoramentoPosicaoFrotaRastreamento;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;

        #endregion

        #region Construtores

        public MonitoramentoPosicaoFrotaRastreamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repMonitoramentoPosicaoFrotaRastreamento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
            _configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarPrimeiroRegistro();
        }

        #endregion        
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ConsultarRegistros(FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repMonitoramentoPosicaoFrotaRastreamento.ConsultarRelatorioPosicaoFrotaRastreamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta, _configuracaoTMS);
        }

        protected override int ContarRegistros(FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repMonitoramentoPosicaoFrotaRastreamento.ContarRelatorioPosicaoFrotaRastreamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/MonitoramentoPosicaoFrotaRastreamento";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return new List<Parametro>();
        }
        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return string.Empty;
        }
    }
}
