using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class ProcessamentoFinalizacaoColetaEntregaEmLote : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>
    {
        public ProcessamentoFinalizacaoColetaEntregaEmLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();
            var result = query.Where(obj => obj.CodigoCarga == codigoCarga);
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosCargasAgurdandoProcessamento()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();
            var result = query.Where(obj => obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.PendenteFinalizacao ||
            (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.FalhaNaFinalizacao && obj.Tentativas <= 2)).Select(obj => obj.CodigoCarga);
            return result.ToList();
        }

        public IList<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote> ConsultarFiltroCarrossel(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string querySelect = @"SELECT PFC_CODIGO as Codigo,
                                 CAR_CODIGO as CodigoCarga,
                                 PFC_DATA_PROCESSAMENTO as DataProcessamento,
                                 PFC_SITUACAO as Situacao,
                                 PFC_TENTATIVAS as Tentativas,
                                 PFC_DESCRICAO as Descricao";

            string queryFrom = " FROM T_PROCESSAMENTO_FINALIZACAO_COLETA_ENTREGA_EM_LOTE";

            StringBuilder where = new StringBuilder();

            if (filtrosPesquisa.Situacao != null)
            {
                where.Append($" WHERE PFC_SITUACAO = {(int)filtrosPesquisa.Situacao}");
            }

            var result = querySelect + queryFrom + where.ToString();

            var consultaFiltroCarrossel = this.SessionNHiBernate.CreateSQLQuery(result);
            consultaFiltroCarrossel.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote)));
            return consultaFiltroCarrossel.SetTimeout(600).List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();
        }

        public Task<IList<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(false, filtrosPesquisa, parametroConsulta);
            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote)));
            return result.SetTimeout(600).ListAsync<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote>();
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(true, filtrosPesquisa, parametrosConsulta);
            return result.SetTimeout(600).UniqueResultAsync<int>();
        }

        private NHibernate.IQuery Consultar(bool somenteContar, Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaFinalizacaoColetaEmLoteProcessamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContar)
                sql.Append(@"select distinct(count(0) over ())");
            else
            {
                sql.Append(@"SELECT PFC_CODIGO as Codigo,
                                 CAR_CODIGO as CodigoCarga,
                                 PFC_DATA_PROCESSAMENTO as DataProcessamento,
                                 PFC_SITUACAO as Situacao,
                                 PFC_TENTATIVAS as Tentativas,
                                 PFC_DESCRICAO as Descricao");
            }
            sql.Append(" FROM T_PROCESSAMENTO_FINALIZACAO_COLETA_ENTREGA_EM_LOTE");
            sql.Append(" WHERE 1=1 ");

            if (filtrosPesquisa.Situacao != null)
                sql.Append($" AND PFC_SITUACAO = {(int)filtrosPesquisa.Situacao}");

            if (filtrosPesquisa.DataInicialProcessamento.HasValue)
                sql.Append($" AND PFC_DATA_PROCESSAMENTO >= {filtrosPesquisa.DataInicialProcessamento}");

            if (filtrosPesquisa.DataFinalProcessamento.HasValue)
                sql.Append($" AND PFC_DATA_PROCESSAMENTO <= {filtrosPesquisa.DataFinalProcessamento}");

            if (filtrosPesquisa.CodigosCarga.Count > 0)
                sql.Append($" AND CAR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosCarga)})");

            if (!somenteContar)
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return SessionNHiBernate.CreateSQLQuery(sql.ToString());
        }

        #endregion
    }
}
