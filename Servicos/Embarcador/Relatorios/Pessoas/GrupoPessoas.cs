using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pessoas
{
    public class GrupoPessoas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioGrupoPessoas, Dominio.Relatorios.Embarcador.DataSource.Pessoas.GrupoPessoas>
    {
        #region Atributos
        
        private readonly Repositorio.Embarcador.Pessoas.GrupoPessoas _repositorioGrupoPessoas;

        #endregion

        #region Construtores

        public GrupoPessoas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.GrupoPessoas> ConsultarRegistros(FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioGrupoPessoas.ConsultarRelatorioGrupoPessoas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioGrupoPessoas.ContarConsultaRelatorioGrupoPessoas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pessoas/GrupoPessoas";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("TipoGrupo", string.Join(", ", filtrosPesquisa.TipoGrupo.Select(o => o.ToString()))));
            parametros.Add(new Parametro("Situacao", SituacaoAtivoPesquisaHelper.ObterDescricao(filtrosPesquisa.Ativo)));
            parametros.Add(new Parametro("Bloqueado", filtrosPesquisa.Bloqueado != 9 ? filtrosPesquisa.Bloqueado == 1 ? "Sim" : "Não" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Descricao") && propriedadeOrdenarOuAgrupar != "Descricao")
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
