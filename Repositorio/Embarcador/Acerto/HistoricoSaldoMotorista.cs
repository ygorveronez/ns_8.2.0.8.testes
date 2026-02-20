using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Acerto
{
    public class HistoricoSaldoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista>
    {
        public HistoricoSaldoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public HistoricoSaldoMotorista(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo> RelatorioMotoristaExtratoSaldo(int codigoColaborador, DateTime dataInicial, DateTime dataFinal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioExtratoMotorista(codigoColaborador, dataInicial, dataFinal, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo>> RelatorioMotoristaExtratoSaldoAsync(int codigoColaborador, DateTime dataInicial, DateTime dataFinal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioExtratoMotorista(codigoColaborador, dataInicial, dataFinal, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.MotoristaExtratoSaldo>();
        }

        public int ContarMotoristaExtratoSaldo(int codigoColaborador, DateTime dataInicial, DateTime dataFinal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioExtratoMotorista(codigoColaborador, dataInicial, dataFinal, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista> BuscarPorMotorista(Dominio.Entidades.Usuario entidadeMotorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.HistoricoSaldoMotorista>();
            var result = from obj in query where obj.Motorista == entidadeMotorista select obj;

            return result.ToList();
        }

        private string ObterSelectConsultaRelatorioExtratoMotorista(int codigoColaborador, DateTime dataInicial, DateTime dataFinal, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   groupBySub = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   whereSub = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioExtratoMotorista(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref whereSub, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioExtratoMotorista(ref where, ref whereSub, ref groupBySub, ref groupBy, ref joins, codigoColaborador, dataInicial, dataFinal);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioExtratoMotorista(propAgrupa, 0, ref select, ref whereSub, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += @" FROM T_HISTORICO_SALDO_MOTORISTA H ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            if (groupBySub.Length > 0)
            {
                groupBySub = " GROUP BY " + groupBySub.Substring(0, groupBySub.Length - 2);
                groupBySub = string.Empty;
            }

            if (whereSub.Length > 0 || groupBySub.Length > 0)
            {
                if (query.Contains(" Entrada,") || query.Contains(" Saida,"))
                {
                    query = string.Format(query, whereSub, groupBySub);
                }
                else
                    query = string.Format(query, "", "");
            }
            else
                query = string.Format(query, "", "");

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioExtratoMotorista(string propriedade, int codigoDinamico, ref string select, ref string whereSub, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {

                        select += "HSM_CODIGO Codigo, ";
                        groupBy += "HSM_CODIGO, ";
                    }
                    break;
                case "Data":
                    if (!select.Contains(" Data, "))
                    {

                        select += "HSM_DATA Data, ";
                        groupBy += "HSM_DATA, ";
                    }
                    break;
                case "DataLancamento":
                    if (!select.Contains(" DataLancamento, "))
                    {

                        select += "HSM_DATA_LANCAMENTO DataLancamento, ";
                        groupBy += "HSM_DATA_LANCAMENTO, ";
                    }
                    break;
                case "Entrada":
                    if (!select.Contains(" Entrada, "))
                    {
                        select += "case when HSM_VALOR > 0 then HSM_VALOR else 0 end Entrada, ";
                        groupBy += "HSM_VALOR, ";
                    }
                    break;
                case "Saida":
                    if (!select.Contains(" Saida, "))
                    {
                        select += "case when HSM_VALOR <= 0 then HSM_VALOR else 0 end Saida, ";
                        groupBy += "HSM_VALOR, ";
                    }
                    break;
                case "TipoPagamento":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select += "CASE WHEN HSM_TIPO_PAGAMENTO = 0 THEN 'Nenhum' WHEN HSM_TIPO_PAGAMENTO = 1 THEN 'Diária' ELSE 'Adiantamento' END  TipoPagamento, ";
                        groupBy += "HSM_TIPO_PAGAMENTO, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                        {
                            joins += " JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = H.FUN_CODIGO";
                        }
                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_CODIGO, Operador.FUN_NOME, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        if (!joins.Contains(" Motorista "))
                        {
                            joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = H.FUN_CODIGO_MOTORISTA";
                        }
                        select += "Motorista.FUN_NOME Motorista, ";
                        groupBy += "Motorista.FUN_CODIGO, Motorista.FUN_NOME, ";
                    }
                    break;
                case "NumeroAcerto":
                    if (!select.Contains(" NumeroAcerto, "))
                    {
                        if (!joins.Contains(" Acerto "))
                        {
                            joins += " LEFT OUTER JOIN T_ACERTO_DE_VIAGEM Acerto ON Acerto.ACV_CODIGO = H.ACV_CODIGO";
                        }
                        select += "ISNULL(Acerto.ACV_NUMERO, 0) NumeroAcerto, ";
                        groupBy += "Acerto.ACV_NUMERO, ";
                    }
                    break;
                case "NumeroPagamento":
                    if (!select.Contains(" NumeroPagamento, "))
                    {
                        if (!joins.Contains(" Pagamento "))
                        {
                            joins += " LEFT OUTER JOIN T_PAGAMENTO_MOTORISTA_TMS Pagamento ON Pagamento.PAM_CODIGO = H.PAM_CODIGO";
                        }
                        select += "ISNULL(Pagamento.PAM_NUMERO, 0) NumeroPagamento, ";
                        groupBy += "Pagamento.PAM_NUMERO, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioExtratoMotorista(ref string where, ref string whereSub, ref string groupBySub, ref string groupBy, ref string joins, int codigoColaborador, DateTime dataInicial, DateTime dataFinal)
        {

            if (codigoColaborador > 0)
            {
                if (!joins.Contains(" Motorista "))
                    joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = H.FUN_CODIGO_MOTORISTA";

                where += " AND Motorista.FUN_CODIGO = " + codigoColaborador.ToString();
                groupBySub += "Motorista.FUN_CODIGO, ";
            }

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                where += " AND HSM_DATA_LANCAMENTO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND HSM_DATA_LANCAMENTO <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
                groupBy += "HSM_DATA_LANCAMENTO, ";
            }

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
            {
                where += " AND HSM_DATA_LANCAMENTO >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
                groupBy += "HSM_DATA_LANCAMENTO, ";
            }

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                where += " AND HSM_DATA_LANCAMENTO <= '" + dataFinal.ToString("MM/dd/yyyy") + "' ";

                groupBy += "HSM_DATA_LANCAMENTO, ";
            }
        }
    }
}
