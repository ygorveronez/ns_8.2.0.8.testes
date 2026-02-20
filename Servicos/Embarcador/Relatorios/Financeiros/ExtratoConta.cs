using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class ExtratoConta : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta, Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.MovimentoFinanceiro _repositorioMovimentoFinanceiro;

        #endregion

        #region Construtores

        public ExtratoConta(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoFinanceiro.RelatorioExtratoConta(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoFinanceiro.ContarRelatorioExtratoConta(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/ExtratoConta";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(_unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> planoContasAnalitica = filtrosPesquisa.CodigosPlanoContaAnalitica.Count > 0 ? repPlanoConta.BuscarPorCodigos(filtrosPesquisa.CodigosPlanoContaAnalitica) : new List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            List<Dominio.Entidades.Cliente> pessoas = filtrosPesquisa.CnpjPessoa.Count > 0 ? repPessoa.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = filtrosPesquisa.CodigosGrupoPessoa.Count > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigosGrupoPessoa) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaSintetica = filtrosPesquisa.CodigoPlanoContaSintetica > 0 ? repPlanoConta.BuscarPorCodigo(filtrosPesquisa.CodigoPlanoContaSintetica) : null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaContrapartida = filtrosPesquisa.CodigoPlanoContaContrapartida > 0 ? repPlanoConta.BuscarPorCodigo(filtrosPesquisa.CodigoPlanoContaContrapartida) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", planoContasAnalitica.Select(o => o.BuscarDescricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBaseInicial", filtrosPesquisa.DataBaseInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBaseFinal", filtrosPesquisa.DataBaseFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", gruposPessoas.Select(o => o.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoas.Select(o => o.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoMovimento", filtrosPesquisa.CodigoMovimento));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", filtrosPesquisa.CentroResultado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroDocumento", filtrosPesquisa.NumeroDocumento));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDebitoCredito", filtrosPesquisa.TipoDebitoCredito.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoContaSintetica", planoContaSintetica != null ? planoContaSintetica?.Descricao + " (" + planoContaSintetica?.Plano + ")" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoContaContrapartida", planoContaContrapartida != null ? planoContaContrapartida?.Descricao + " (" + planoContaContrapartida?.Plano + ")" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MoedaCotacaoBancoCentral", filtrosPesquisa.MoedaCotacaoBancoCentral.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            if (propriedadeOrdenarOuAgrupar == "DataBaseFormatada")
                return "Data_Base";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}