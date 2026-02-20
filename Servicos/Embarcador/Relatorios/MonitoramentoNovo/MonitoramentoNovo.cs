using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.MonitoramentoNovo
{
    public class MonitoramentoNovo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.Monitoramento _repositorioMonitoramentoNovo;

        #endregion

        #region Construtores

        public MonitoramentoNovo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMonitoramentoNovo = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {

            return _repositorioMonitoramentoNovo.ConsultarMonitoramento(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMonitoramentoNovo.ContarConsultaView(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Monitoramento/MonitoramentoNovo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
