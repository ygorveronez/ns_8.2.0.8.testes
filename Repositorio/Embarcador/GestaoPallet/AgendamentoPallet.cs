using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPallet
{
    public class AgendamentoPallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>
    {
        #region Construtores

        public AgendamentoPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public AgendamentoPallet(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public int ContarConsultaAgendamentoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa)
        {
            var sql = QueryConsultaAgendamentoPallet(filtrosPesquisa, null, true);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.DadosPesquisaAgendamentoPallet> ConsultaAgendamentoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sql = QueryConsultaAgendamentoPallet(filtrosPesquisa, parametroConsulta, false);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.DadosPesquisaAgendamentoPallet)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.DadosPesquisaAgendamentoPallet>();
        }

        public Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>();

            var result = query.Where(o => o.Carga.Codigo == codigoCarga);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> BuscarPorCargas(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>();

            var result = query.Where(o => codigos.Contains(o.Carga.Codigo));

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>> BuscarPorCargasAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>();

            var result = query.Where(o => codigos.Contains(o.Carga.Codigo));

            return result.ToListAsync(CancellationToken);
        }

        public int ObterProximoNumeroSequencial()
        {
            var consultaAgendamentoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>();
            int? ultimoNumeroSequencial = consultaAgendamentoPallet.Max(o => (int?)o.Sequencia);

            return ultimoNumeroSequencial.HasValue ? (ultimoNumeroSequencial.Value + 1) : 1;
        }

        public int ObterProximaSenhaSequencial()
        {
            var consultaAgendamentoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet>();
            int? ultimaSenhaSequencial = consultaAgendamentoPallet.Max(o => o.SenhaSequencial);

            return ultimaSenhaSequencial.HasValue ? (ultimaSenhaSequencial.Value + 1) : 1;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string QueryConsultaAgendamentoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append(@"
                            SELECT
                                agendamentoPallet.ACP_CODIGO            AS Codigo,
                                carga.CAR_CODIGO_CARGA_EMBARCADOR       AS Carga,
                                agendamentoPallet.ACP_SENHA             AS Senha,
                                agendamentoPallet.ACP_DATA_AGENDAMENTO  AS DataAgendamento,
                                agendamentoPallet.ACP_DATA_CRIACAO      AS DataCriacao,
                                agendamentoPallet.ACP_DATA_ENTREGA      AS DataConfirmada,
                                remetente.CLI_NOME                      AS Remetente,
                                destinatario.CLI_NOME                   AS Destinatario,
                                tipoCarga.TCG_DESCRICAO                 AS TipoCarga,
                                agendamentoPallet.ACP_ETAPA             AS EtapaAgendamentoPallet,
                                agendamentoPallet.ACP_SITUACAO          AS Situacao,
                                carga.CAR_SITUACAO						AS SituacaoCarga
                ");

            sql.Append(" FROM T_AGENDAMENTO_PALLET agendamentoPallet ");

            sql.Append(ObterJoinsAgendamentoPallet);
            sql.Append(ObterFiltrosAgendamentoPallet(filtrosPesquisa, parametroConsulta));

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        private string ObterJoinsAgendamentoPallet => @"
                LEFT JOIN T_CLIENTE remetente ON agendamentoPallet.REM_CODIGO = remetente.CLI_CGCCPF
                LEFT JOIN T_CLIENTE destinatario ON agendamentoPallet.DES_CODIGO = destinatario.CLI_CGCCPF
                LEFT JOIN T_CARGA carga ON agendamentoPallet.CAR_CODIGO = carga.CAR_CODIGO
                LEFT JOIN T_TIPO_DE_CARGA tipoCarga ON agendamentoPallet.TCG_CODIGO = tipoCarga.TCG_CODIGO";

        private string ObterFiltrosAgendamentoPallet(Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" WHERE 1 = 1 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                sql.Append($" AND carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%{filtrosPesquisa.NumeroCarga}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Senha))
                sql.Append($" AND agendamentoPallet.ACP_SENHA LIKE '%{filtrosPesquisa.Senha}%'");

            if (filtrosPesquisa.DataAgendamento != default)
                sql.Append($" AND CAST(agendamentoPallet.ACP_DATA_AGENDAMENTO AS DATE) = '{filtrosPesquisa.DataAgendamento.Date:yyyy-MM-dd}'");

            if (filtrosPesquisa.CodigoDestinatario > 0)
                sql.Append($" AND agendamentoPallet.DES_CODIGO = {filtrosPesquisa.CodigoDestinatario}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                sql.Append($" AND agendamentoPallet.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.CodigoCliente > 0)
                sql.Append($" AND agendamentoPallet.REM_CODIGO = {filtrosPesquisa.CodigoCliente}");

            if (filtrosPesquisa.SituacaoCargaJanelaCarregamento.HasValue)
                sql.Append($" AND agendamentoPallet.ACP_SITUACAO = {(int)filtrosPesquisa.SituacaoCargaJanelaCarregamento}");

            return sql.ToString();
        }


        #endregion Métodos Privados
    }
}
