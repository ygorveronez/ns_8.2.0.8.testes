using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class ExtratoMotorista : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista, Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.MovimentoFinanceiro _repositorioMovimentoFinanceiro;

        #endregion

        #region Construtores

        public ExtratoMotorista(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoFinanceiro.RelatorioExtratoMotoristaNovo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoFinanceiro.ContarConsultaRelatorioExtratoMotoristaNovo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/ExtratoMotorista";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = filtrosPesquisa.CodigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(filtrosPesquisa.CodigoPlanoConta) : null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = filtrosPesquisa.CodigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(filtrosPesquisa.CodigoTipoMovimento) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", planoConta?.BuscarDescricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoMovimento", filtrosPesquisa.Codigo));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GerarRelatorioAgrupado", false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento?.Descricao));

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

        #endregion 
    }
}
