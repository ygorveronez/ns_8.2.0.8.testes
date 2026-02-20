using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeComponentesFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>
    {
        public CargaCTeComponentesFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCTeComponentesFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTes(IEnumerable<int> codigosCargaCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();

            query = query.Where(o => codigosCargaCTes.Contains(o.CargaCTe.Codigo));

            return query.Fetch(o => o.ComponenteFrete).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTesEComponente(List<int> codigosCargaCTe, int componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where codigosCargaCTe.Contains(obj.CargaCTe.Codigo) && obj.ComponenteFrete.Codigo == componente select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTesEComponentes(List<int> codigosCargaCTe, List<int> componentes)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            //query = from obj in query where codigosCargaCTe.Contains(obj.CargaCTe.Codigo) && componentes.Contains(obj.ComponenteFrete.Codigo) select obj;

            //return query.ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();

            int take = 1000;
            int start = 0;
            while (start < codigosCargaCTe?.Count)
            {
                List<int> tmp = codigosCargaCTe.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
                var filter = from obj in query
                             where tmp.Contains(obj.CargaCTe.Codigo) 
                                   && componentes.Contains(obj.ComponenteFrete.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(o => o.CargaCTe)
                                      .ThenFetch(o => o.PreCTe)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete BuscarPorCargaCTeEComponente(int codigoCargaCTe, int componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.ComponenteFrete.Codigo == componente select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete BuscarPorCargaCTeETipoComponente(int codigoCargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.TipoComponenteFrete == tipoComponente select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorSemComposicaoFreteLiquidoCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTeComICMS(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;
            return result.ToList();
        }

        public decimal BuscarTotalCargaPorCompomente(int cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == cargaCTe && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> BuscarPorCargaCTeQueGeraMovimentacao(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.ComponenteFrete.GerarMovimentoAutomatico select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigoComponenteFreteQueGeraMovimentacaoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && obj.CargaCTe.CTe.Status == statusCTe && obj.ComponenteFrete.GerarMovimentoAutomatico && obj.CargaCTe.CargaCTeComplementoInfo == null select obj.ComponenteFrete.Codigo;
            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigoComponenteFreteQueGeraMovimentacaoPorCarga(int codigoCarga, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && statusCTe.Contains(obj.CargaCTe.CTe.Status) && obj.ComponenteFrete.GerarMovimentoAutomatico select obj.ComponenteFrete.Codigo;
            return result.Distinct().ToList();
        }

        public decimal BuscarValorComponentePorCargaEModeloDocumento(int codigoCarga, int codigoComponenteFrete, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();

            query = query.Where(obj => obj.CargaCTe.Carga.Codigo == codigoCarga && obj.CargaCTe.CTe.Status == statusCTe && obj.ComponenteFrete.Codigo == codigoComponenteFrete && obj.CargaCTe.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && obj.CargaCTe.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CargaCTe.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CargaCTe.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CargaCTe.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal BuscarValorComponentePorCargaEModeloDocumento(int codigoCarga, int codigoComponenteFrete, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();

            query = query.Where(obj => obj.CargaCTe.Carga.Codigo == codigoCarga && statusCTe.Contains(obj.CargaCTe.CTe.Status) && obj.ComponenteFrete.Codigo == codigoComponenteFrete && obj.CargaCTe.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && obj.CargaCTe.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CargaCTe.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CargaCTe.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CargaCTe.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CargaCTe.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal BuscarValorComponentePorCargaEModeloDocumento(int codigoCarga, int codigoComponenteFrete, int codigoModeloDocumento, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && statusCTe.Contains(obj.CargaCTe.CTe.Status) && obj.ComponenteFrete.Codigo == codigoComponenteFrete && obj.CargaCTe.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento select obj.ValorComponente;
            return result.Sum();
        }

        public decimal BuscarValorComponentePorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && obj.TipoComponenteFrete == tipoComponente && obj.CargaCTe.CargaCTeComplementoInfo == null select obj;
            return result.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal BuscarValorCompenenteFretePorCTeEComponenteFrete(List<int> codigosCTes, int codigoComponenteFrete)
        {
            var cargaCTeComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>()
                .Where(c => codigosCTes.Contains(c.CargaCTe.Codigo) && c.ComponenteFrete.Codigo == codigoComponenteFrete);

            return cargaCTeComponenteFrete.Sum(c => (decimal?)c.ValorComponente) ?? 0m;
        }

        #endregion

        #region Relatório de Componentes de Frete do CTe

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe> ConsultarRelatorioComponenteFreteCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioComponenteFreteCTe(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe>();
        }
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe>> ConsultarRelatorioComponenteFreteCTeAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioComponenteFreteCTe(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe.ComponenteFreteCTe>();
        }

        public int ContarConsultaRelatorioComponenteFreteCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioComponenteFreteCTe(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0));

            return query.SetTimeout(600).UniqueResult<int>();
        }
        public async Task<int> ContarConsultaRelatorioComponenteFreteCTeAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioComponenteFreteCTe(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0));

            return await query.SetTimeout(600).UniqueResultAsync<int>();
        }

        private string ObterSelectConsultaRelatorioComponenteFreteCTe(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            StringBuilder select = new StringBuilder(),
                   groupBy = new StringBuilder(),
                   joins = new StringBuilder(),
                   where = new StringBuilder(),
                   orderBy = new StringBuilder();


            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioComponenteFreteCTe(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaComponenteFreteCTe(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioComponenteFreteCTe(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy.Append(propAgrupa).Append(" ").Append(dirAgrupa);
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy.Append((orderBy.Length > 0 ? ", " : string.Empty)).Append(propOrdena).Append(" ").Append(dirOrdena);
                }
            }

            // SELECT
            StringBuilder query = new StringBuilder("SELECT ");

            if (count)
                query.Append("DISTINCT(COUNT(0) OVER())");
            else if (select.Length > 0)
                query.Append(select.Remove(select.Length - 2, 2));

            // FROM
            query.Append(" FROM T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete INNER JOIN T_COMPONENTE_FRETE ComponenteFrete ON CargaCTeComponenteFrete.CFR_CODIGO = ComponenteFrete.CFR_CODIGO ");

            // JOIN
            query.Append(joins);

            // WHERE
            query.Append(" WHERE 1 = 1").Append(where);

            // GROUP BY
            if (groupBy.Length > 0)
                query.Append(" GROUP BY " + groupBy.Remove(groupBy.Length - 2, 2));

            // ORDER BY
            if (orderBy.Length > 0)
                query.Append(" ORDER BY " + orderBy);
            else if (!count)
                query.Append(" ORDER BY 1 ASC");

            // LIMIT
            if (!count && limite > 0)
                query.Append(" OFFSET ").Append(inicio.ToString()).Append(" ROWS FETCH NEXT ").Append(limite.ToString()).Append(" ROWS ONLY");

            return query.ToString();
        }

        private void SetarSelectRelatorioComponenteFreteCTe(string propriedade, int codigoDinamico, ref StringBuilder select, ref StringBuilder groupBy, ref StringBuilder joins, bool count)
        {
            switch (propriedade)
            {
                case "ComponenteFrete":
                    if (!select.Contains(" ComponenteFrete,"))
                    {
                        select.Append("ComponenteFrete.CFR_DESCRICAO ComponenteFrete, ");
                        groupBy.Append("ComponenteFrete.CFR_DESCRICAO, ");
                    }
                    break;
                case "ValorComponenteFrete":
                    if (!select.Contains(" ValorComponenteFrete,"))
                        select.Append("SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) ValorComponenteFrete, ");
                    break;
                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoasTomador.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoasTomador.GRP_DESCRICAO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                        if (!joins.Contains(" Tomador "))
                            joins.Append("INNER JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");

                        if (!joins.Contains(" GrupoPessoasTomador "))
                            joins.Append("INNER JOIN T_GRUPO_PESSOAS GrupoPessoasTomador ON GrupoPessoasTomador.GRP_CODIGO = Tomador.GRP_CODIGO ");
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" Carga "))
                            joins.Append("INNER JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
                    }
                    break;
                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe,"))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                    }
                    break;
                case "SerieCTe":
                    if (!select.Contains(" SerieCTe,"))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                        if (!joins.Contains(" Serie "))
                            joins.Append("INNER JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = CTe.CON_SERIE ");
                    }
                    break;
                case "ModeloDocumento":
                    if (!select.Contains(" ModeloDocumento,"))
                    {
                        select.Append("ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, ");
                        groupBy.Append("ModeloDocumento.MOD_ABREVIACAO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                        if (!joins.Contains(" ModeloDocumento "))
                            joins.Append("INNER JOIN T_MODDOCFISCAL ModeloDocumento ON ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC ");
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa,"))
                    {
                        select.Append("Empresa.EMP_RAZAO Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                        if (!joins.Contains(" Empresa "))
                            joins.Append("INNER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO ");
                    }
                    break;

                case "ModeloVeicularCarga":
                    if (!select.Contains(" ModeloVeicularCarga, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" Carga "))
                            joins.Append("INNER JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");

                        if (!joins.Contains(" ModeloVeicularCarga "))
                            joins.Append("LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
                    }
                    break;

                case "NumeroFrotasVeiculos":
                    if (!select.Contains(" NumeroFrotasVeiculos, "))
                    {
                        select.Append("( ");
                        select.Append("    (select _veiculo.VEI_NUMERO_FROTA from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + _veiculo.VEI_NUMERO_FROTA ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS _veiculovinculadocarga ");
                        select.Append("          join T_VEICULO _veiculo on _veiculovinculadocarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("         where _veiculovinculadocarga.CAR_CODIGO = Carga.CAR_CODIGO and _veiculo.VEI_NUMERO_FROTA is not null and _veiculo.VEI_NUMERO_FROTA <> '' ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") NumeroFrotasVeiculos, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" Carga "))
                            joins.Append("INNER JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("CTe.CON_PESO Peso, ");
                        groupBy.Append("CTe.CON_PESO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                    }
                    break;

                case "DataEmissaoCTeFormatada":
                    if (!select.Contains(" DataEmissaoCTe, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                    }
                    break;

                case "DestinatarioCTe":
                    if (!select.Contains(" DestinatarioCTe, "))
                    {
                        select.Append("DestinatarioCTe.PCT_NOME DestinatarioCTe, ");
                        groupBy.Append("DestinatarioCTe.PCT_NOME, ");

                        if (!joins.Contains(" CargaCTe "))
                            joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                        if (!joins.Contains(" CTe "))
                            joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                        if (!joins.Contains(" DestinatarioCTe "))
                            joins.Append("LEFT JOIN T_CTE_PARTICIPANTE DestinatarioCTe ON DestinatarioCTe.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaComponenteFreteCTe(ref StringBuilder where, ref StringBuilder groupBy, ref StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroRelatorioComponenteFreteCTe filtrosPesquisa)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue ||
                filtrosPesquisa.DataFinalEmissao != DateTime.MinValue ||
                filtrosPesquisa.DataInicialAutorizacao != DateTime.MinValue ||
                filtrosPesquisa.DataFinalAutorizacao != DateTime.MinValue)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                if (!joins.Contains(" CTe "))
                    joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATAHORAEMISSAO >= '").Append(filtrosPesquisa.DataInicialEmissao.ToString(datePattern)).Append("'");

                if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATAHORAEMISSAO < '").Append(filtrosPesquisa.DataFinalEmissao.AddDays(1).ToString(datePattern)).Append("'");

                if (filtrosPesquisa.DataInicialAutorizacao != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATA_AUTORIZACAO >= '").Append(filtrosPesquisa.DataInicialAutorizacao.ToString(datePattern)).Append("'");

                if (filtrosPesquisa.DataFinalAutorizacao != DateTime.MinValue)
                    where.Append(" AND CTe.CON_DATA_AUTORIZACAO < '").Append(filtrosPesquisa.DataFinalAutorizacao.AddDays(1).ToString(datePattern)).Append("'");
            }

            if (filtrosPesquisa.Carga > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                where.Append(" AND CargaCTe.CAR_CODIGO = ").Append(filtrosPesquisa.Carga);
            }

            if (filtrosPesquisa.ComponenteFrete != null && filtrosPesquisa.ComponenteFrete.Count > 0)
                where.Append(" AND CargaCTeComponenteFrete.CFR_CODIGO IN (").Append(string.Join(",", filtrosPesquisa.ComponenteFrete)).Append(")");

            if (filtrosPesquisa.CTe > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                where.Append(" AND CargaCTe.CON_CODIGO = ").Append(filtrosPesquisa.CTe);
            }

            if (filtrosPesquisa.Empresa > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                if (!joins.Contains(" CTe "))
                    joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                where.Append(" AND CTe.EMP_CODIGO = ").Append(filtrosPesquisa.Empresa);
            }

            if (filtrosPesquisa.GrupoPessoas != null && filtrosPesquisa.GrupoPessoas.Count > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                if (!joins.Contains(" CTe "))
                    joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                if (!joins.Contains(" Tomador "))
                    joins.Append("INNER JOIN T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");

                where.Append(" AND Tomador.GRP_CODIGO IN (").Append(string.Join(",", filtrosPesquisa.GrupoPessoas)).Append(")");
            }

            if (filtrosPesquisa.ModeloDocumento != null && filtrosPesquisa.ModeloDocumento.Count > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                if (!joins.Contains(" CTe "))
                    joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                where.Append(" AND CTe.CON_MODELODOC IN (").Append(string.Join(",", filtrosPesquisa.ModeloDocumento)).Append(")");
            }

            if (filtrosPesquisa.StatusCTe != null && filtrosPesquisa.StatusCTe.Count > 0)
            {
                if (!joins.Contains(" CargaCTe "))
                    joins.Append("INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO ");

                if (!joins.Contains(" CTe "))
                    joins.Append("INNER JOIN T_CTE CTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");

                where.Append(" AND CTe.CON_STATUS IN (").Append(string.Join(",", filtrosPesquisa.StatusCTe.Select(o => "'" + o + "'"))).Append(")");
            }
        }

        #endregion
    }
}
