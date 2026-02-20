using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoGhost : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost>
    {
        public IntegracaoGhost(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost> BuscarIntegracoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost>();

            var result = from obj in query where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.NumeroTentativas < 3 && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj;

            return result.OrderBy(o => o.Codigo).Take(5).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, null, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null, bool somenteContar = false)
        {
            string sql;

            sql = (somenteContar ? "select count(0) " : ObterSqlSelect()) + ObterSqlFrom() + ObterSqlWhere(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterSqlSelect()
        {
            string select =
            @"select 
                Requisicao.ITG_CODIGO Codigo,
                Requisicao.INT_PROBLEMA_INTEGRACAO Retorno,
                Requisicao.INT_DATA_INTEGRACAO DataIntegracao,
                Requisicao.INT_SITUACAO_INTEGRACAO SituacaoIntegracao,
                Requisicao.ITG_TIPO_DESTINO TipoDestino,
                Requisicao.ITG_CHAVE_REQUISICAO Chave
";

            return select;
        }

        private string ObterSqlFrom()
        {
            string select =
            @"from T_INTEGRACAO_GHOST Requisicao
                ";

            return select;
        }

        private string ObterSqlWhere(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd HH:mm";
            string where = " where 1=1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                where += $@"and Requisicao.ITG_CHAVE_REQUISICAO like '%{filtrosPesquisa.Chave}%'";

            if (filtrosPesquisa.DataFinal.HasValue)
                where += $"and Requisicao.INT_DATA_INTEGRACAO <= '{filtrosPesquisa.DataFinal.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.DataInicial.HasValue)
                where += $"and Requisicao.INT_DATA_INTEGRACAO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.SituacaoIntegracao.Count > 0)
                where += $"and Requisicao.INT_SITUACAO_INTEGRACAO in ({string.Join(", ", filtrosPesquisa.SituacaoIntegracao.Select(x => (int)x).ToList())}) ";

            if (filtrosPesquisa.TipoDestino.Count > 0)
                where += $"and Requisicao.ITG_TIPO_DESTINO in ({string.Join(", ", filtrosPesquisa.TipoDestino.Select(x => (int)x).ToList())}) ";

            return where;
        }
    }
}
