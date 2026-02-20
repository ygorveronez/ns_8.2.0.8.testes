using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class ControleTacografo : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ControleTacografo>
    {
        public ControleTacografo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ControleTacografo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Frota.ControleTacografo> Consultar(bool consultaAcerto, int codigo, int codigoVeiculo, int codigoMotorista, DateTime dataRecebimentoInicial, DateTime dataRecebimentoFinal, Dominio.Enumeradores.OpcaoSimNaoPesquisa excesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(consultaAcerto, codigo, codigoVeiculo, codigoMotorista, dataRecebimentoInicial, dataRecebimentoFinal, excesso, status);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > 0)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(bool consultaAcerto, int codigo, int codigoVeiculo, int codigoMotorista, DateTime dataRecebimentoInicial, DateTime dataRecebimentoFinal, Dominio.Enumeradores.OpcaoSimNaoPesquisa excesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = Consultar(consultaAcerto, codigo, codigoVeiculo, codigoMotorista, dataRecebimentoInicial, dataRecebimentoFinal, excesso, status);

            return result.Count();
        }

        #endregion


        #region Relatório de Tacógrafo

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo> ConsultarRelatorioTacografo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioTacografo(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo>> ConsultarRelatorioTacografoAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioTacografo(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Tacografo>();
        }

        public int ContarConsultaRelatorioTacografo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioTacografo(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioTacografo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioTacografo(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioTacografo(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioTacografo(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_CONTROLE_TACOGRAFO Tacografo ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

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

        private void SetarSelectRelatorioConsultaRelatorioTacografo(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Tacografo.CTA_CODIGO Codigo, ";
                        //groupBy += "Tacografo.CTA_CODIGO, ";
                    }
                    break;
                case "DataRepasseFormatada":
                case "DataRepasse":
                    if (!select.Contains(" DataRepasse, "))
                    {
                        select += "Tacografo.CTA_DATA_RECEBIMENTO DataRepasse, ";
                        //groupBy += "Tacografo.CTA_DATA_RECEBIMENTO, ";
                    }
                    break;
                case "DataRetornoFormatada":
                case "DataRetorno":
                    if (!select.Contains(" DataRetorno, "))
                    {
                        select += "Tacografo.CTA_DATA_RETORNO DataRetorno, ";
                        //groupBy += "Tacografo.CTA_DATA_RETORNO, ";
                    }
                    break;
                case "HouveExcessoVelocidadeFormatada":
                case "HouveExcessoVelocidade":
                    if (!select.Contains(" HouveExcessoVelocidade, "))
                    {
                        select += "Tacografo.CTA_EXCESSO HouveExcessoVelocidade, ";
                        //groupBy += "Tacografo.CTA_EXCESSO, ";
                    }
                    break;
                case "SituacaoFormatada":
                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += "Tacografo.CTA_SITUACAO Situacao, ";
                        //groupBy += "Tacografo.CTA_SITUACAO, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Tacografo.CTA_OBSERVACAO Observacao, ";
                        //groupBy += "Tacografo.CTA_OBSERVACAO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Tacografo.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Placa, ";
                        //groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        if (!joins.Contains(" Motorista "))
                            joins += " LEFT JOIN T_FUNCIONARIO Funcionario ON Funcionario.FUN_CODIGO = Tacografo.FUN_CODIGO";

                        select += "Funcionario.FUN_NOME Motorista, ";
                        //groupBy += "Funcionario.FUN_NOME, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioTacografo(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioTacografo filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigosVeiculos != null && filtrosPesquisa.CodigosVeiculos.Count > 0)
                where += " and Tacografo.VEI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosVeiculos) + ")";

            if (filtrosPesquisa.CodigosMotoristas != null && filtrosPesquisa.CodigosMotoristas.Count > 0)
                where += " and Tacografo.FUN_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosMotoristas) + ")";

            if (filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0)
                where += " and Tacografo.CTA_SITUACAO in (" + string.Join(", ", filtrosPesquisa.Situacoes) + ")";

            if (filtrosPesquisa.DataInicialRepasse != DateTime.MinValue)
                where += " and CAST(Tacografo.CTA_DATA_RECEBIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicialRepasse.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinalRepasse != DateTime.MinValue)
                where += " and CAST(Tacografo.CTA_DATA_RECEBIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinalRepasse.ToString(pattern) + "'";

            if (filtrosPesquisa.DataInicialRetorno != DateTime.MinValue)
                where += " and CAST(Tacografo.CTA_DATA_RETORNO AS DATE) >= '" + filtrosPesquisa.DataInicialRetorno.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinalRetorno != DateTime.MinValue)
                where += " and CAST(Tacografo.CTA_DATA_RETORNO AS DATE) <= '" + filtrosPesquisa.DataFinalRetorno.ToString(pattern) + "'";

            if (filtrosPesquisa.ExcessoVelocidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where += " and Tacografo.CTA_EXCESSO = 1 ";
            else if (filtrosPesquisa.ExcessoVelocidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where += " and Tacografo.CTA_EXCESSO = 0 ";
        }

        #endregion


        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.ControleTacografo> Consultar(bool consultaAcerto, int codigo, int codigoVeiculo, int codigoMotorista, DateTime dataRecebimentoInicial, DateTime dataRecebimentoFinal, Dominio.Enumeradores.OpcaoSimNaoPesquisa excesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ControleTacografo>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(c => c.Codigo == codigo);

            if (dataRecebimentoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataRecebimento.Date >= dataRecebimentoInicial.Date);

            if (dataRecebimentoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataRecebimento.Date <= dataRecebimentoFinal.Date);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (excesso == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                result = result.Where(o => o.Excesso);
            else if (excesso == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                result = result.Where(o => !o.Excesso);

            if (consultaAcerto)
            {
                var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo>();
                queryAcerto = queryAcerto.Where(c => c.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);
                result = result.Where(o => !queryAcerto.Any(c => c.ControleTacografo == o) && o.Situacao == 1);
            }

            return result;
        }

        #endregion
    }
}
