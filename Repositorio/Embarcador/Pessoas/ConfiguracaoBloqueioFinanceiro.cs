using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ConfiguracaoBloqueioFinanceiro : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro>
    {
        public ConfiguracaoBloqueioFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro BuscarConfiguracaoBloqueioPadrao(int codigoGrupoPessoa)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro>();
            query = query.Where(c => c.HabilitarRegra && c.GrupoPessoas != null && c.GrupoPessoas.Codigo == codigoGrupoPessoa);
            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigoGrupoPessoa()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro>();
            return query.Where(c => c.HabilitarRegra && c.GrupoPessoas != null).Select(c => c.GrupoPessoas.Codigo).ToList();
        }

        public bool ContemConfiguracaoAtiva()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro>();
            return query.Any(c => c.HabilitarRegra);
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro filtrosPesqusia, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = ConsultarDados(filtrosPesqusia, parametrosConsulta);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro filtrosPesqusia, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = ConsultarDados(filtrosPesqusia, parametrosConsulta);

            return result.Count();
        }

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro> ConsultarDados(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro filtrosPesqusia, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesqusia.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesqusia.Descricao));

            if (filtrosPesqusia.CodigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == filtrosPesqusia.CodigoGrupoPessoa);

            return result;
        }
        #endregion

    }
}
