using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class BalanceteGerencial : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial, Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito _repositorioBalanceteGerencial;

        #endregion

        #region Construtores

        public BalanceteGerencial(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioBalanceteGerencial = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioBalanceteGerencial.ConsultarRelatorioBalanceteGerencial(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioBalanceteGerencial.ContarConsultaRelatorioBalanceteGerencial(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/BalanceteGerencial";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();


            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = filtrosPesquisa.CodigoPlanoContaSintetica > 0 ? repPlanoConta.BuscarPorCodigo(filtrosPesquisa.CodigoPlanoContaSintetica) : null;
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CodigosCentroResultado.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigosCentroResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            if (planoConta != null)
                parametros.Add(new Parametro("PlanoContaSintetico", planoConta.Plano + " - " + planoConta.Descricao, true));
            else
                parametros.Add(new Parametro("PlanoContaSintetico", false));

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("TipoConta", filtrosPesquisa.TipoConta?.ObterDescricao()));
            parametros.Add(new Parametro("CentroResultado", centrosResultado.Select(o => o.Descricao)));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoAgendamento")
                return "Agendamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}