using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.GestaoPatio
{
    public class ControleVisita : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita>
    {
        public ControleVisita(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita BuscarVisitaAbertaCPF(string cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita>();
            var result = from obj in query where obj.CPF == cpf && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Aberto select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita>();
            var result = from obj in query select obj;

            var resultNumero = result.Select(o => o.Numero);

            int maiorNumero = 0;
            if (resultNumero.Count() > 0)
                maiorNumero = resultNumero.Max();

            return maiorNumero + 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita> _Consultar(string cpf, string nome, string empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(cpf))
                result = result.Where(o => o.CPF.Contains(cpf));

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => o.Empresa.Contains(empresa));

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Aberto)
                result = result.Where(o => o.Situacao == situacao);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Fechado)
                result = result.Where(o => o.Situacao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita> Consultar(string cpf, string nome, string empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(cpf, nome, empresa, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string cpf, string nome, string empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita situacao)
        {
            var result = _Consultar(cpf, nome, empresa, situacao);

            return result.Count();
        }

        #region Relatório de Controle de Visita

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita> ConsultarRelatorioControleVisita(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSelectConsultaRelatorioControleVisita(filtrosPesquisa, false, propriedades, parametrosConsulta);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita>();
        }

        public int ContarConsultaRelatorioControleVisita(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioControleVisita(filtrosPesquisa, true, propriedades);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioControleVisita(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioControleVisita(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, ref where, count);

            SetarWhereRelatorioConsultaRelatorioControleVisita(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                {
                    SetarSelectRelatorioConsultaRelatorioControleVisita(parametrosConsulta.PropriedadeAgrupar, 0, ref select, ref groupBy, ref joins, ref where, count);

                    if (select.Contains(parametrosConsulta.PropriedadeAgrupar))
                        orderBy = parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                {
                    if (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta.PropriedadeAgrupar && select.Contains(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeOrdenar != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_CONTROLE_VISITA Controle ";

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
            if (!count && parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros.ToString() + " ROWS FETCH NEXT " + parametrosConsulta.LimiteRegistros.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioControleVisita(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, ref string where, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Controle.COV_CODIGO Codigo, ";
                        groupBy += "Controle.COV_CODIGO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "Controle.CTP_NUMERO Numero, ";
                        groupBy += "Controle.CTP_NUMERO, ";
                    }
                    break;
                case "DescricaoEntrada":
                    if (!select.Contains(" Entrada, "))
                    {
                        select += "Controle.COV_DATA_HORA_ENTRADA Entrada, ";
                        groupBy += "Controle.COV_DATA_HORA_ENTRADA, ";
                    }
                    break;
                case "DescricaoPrevisaoSaida":
                    if (!select.Contains(" PrevisaoSaida, "))
                    {
                        select += "Controle.COV_DATA_HORA_PREVISAO_SAIDA PrevisaoSaida, ";
                        groupBy += "Controle.COV_DATA_HORA_PREVISAO_SAIDA, ";
                    }
                    break;
                case "DescricaoSaida":
                    if (!select.Contains(" Saida, "))
                    {
                        select += "Controle.COV_DATA_HORA_SAIDA Saida, ";
                        groupBy += "Controle.COV_DATA_HORA_SAIDA, ";
                    }
                    break;
                case "CPF":
                    if (!select.Contains(" CPF, "))
                    {
                        select += "Controle.COV_CPF CPF, ";
                        groupBy += "Controle.COV_CPF, ";
                    }
                    break;
                case "Nome":
                    if (!select.Contains(" Nome, "))
                    {
                        select += "Controle.COV_NOME Nome, ";
                        groupBy += "Controle.COV_NOME, ";
                    }
                    break;
                case "DescricaoNascimento":
                    if (!select.Contains(" Nascimento, "))
                    {
                        select += "Controle.COV_DATA_NASCIMENTO Nascimento, ";
                        groupBy += "Controle.COV_DATA_NASCIMENTO, ";
                    }
                    break;
                case "Identidade":
                    if (!select.Contains(" Identidade, "))
                    {
                        select += "Controle.COV_IDENTIDADE Identidade, ";
                        groupBy += "Controle.COV_IDENTIDADE, ";
                    }
                    break;
                case "OrgaoEmissor":
                    if (!select.Contains(" OrgaoEmissor, "))
                    {
                        select += "Controle.COV_ORGAO_EMISSOR OrgaoEmissor, ";
                        groupBy += "Controle.COV_ORGAO_EMISSOR, ";
                    }
                    break;
                case "Estado":
                    if (!select.Contains(" Estado, "))
                    {
                        if (!joins.Contains(" Estado "))
                            joins += " LEFT OUTER JOIN T_UF Estado ON Estado.UF_SIGLA = Controle.UF_SIGLA";

                        select += "Estado.UF_SIGLA Estado, ";
                        groupBy += "Estado.UF_SIGLA, ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select += "Controle.COV_EMPRESA Empresa, ";
                        groupBy += "Controle.COV_EMPRESA, ";
                    }
                    break;
                case "Setor":
                    if (!select.Contains(" Setor, "))
                    {
                        if (!joins.Contains(" Setor "))
                            joins += " JOIN T_SETOR Setor ON Setor.SET_CODIGO = Controle.SET_CODIGO";

                        select += "Setor.SET_DESCRICAO Setor, ";
                        groupBy += "Setor.SET_DESCRICAO, ";
                    }
                    break;
                case "Autorizador":
                    if (!select.Contains(" Autorizador, "))
                    {
                        if (!joins.Contains(" Autorizador "))
                            joins += " JOIN T_FUNCIONARIO Autorizador ON Autorizador.FUN_CODIGO = Controle.FUN_AUTORIZADOR";

                        select += "Autorizador.FUN_NOME Autorizador, ";
                        groupBy += "Autorizador.FUN_NOME, ";
                    }
                    break;
                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select += "Controle.COV_PLACA_VEICULO PlacaVeiculo, ";
                        groupBy += "Controle.COV_PLACA_VEICULO, ";
                    }
                    break;
                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo, "))
                    {
                        select += "Controle.COV_MODELO_VEICULO ModeloVeiculo, ";
                        groupBy += "Controle.COV_MODELO_VEICULO, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Controle.COV_OBSERVACAO Observacao, ";
                        groupBy += "Controle.COV_OBSERVACAO, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioControleVisita(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd HH:mm";

            if (filtrosPesquisa.DataInicialEntrada != DateTime.MinValue)
                where += " AND Controle.COV_DATA_HORA_ENTRADA >= '" + filtrosPesquisa.DataInicialEntrada.Date.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalEntrada != DateTime.MinValue)
                where += " AND Controle.COV_DATA_HORA_ENTRADA <= '" + filtrosPesquisa.DataFinalEntrada.Date.ToString(pattern) + "'";

            if (filtrosPesquisa.DataInicialSaida != DateTime.MinValue)
                where += " AND Controle.COV_DATA_HORA_SAIDA >= '" + filtrosPesquisa.DataInicialEntrada.Date.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalSaida != DateTime.MinValue)
                where += " AND Controle.COV_DATA_HORA_SAIDA <= '" + filtrosPesquisa.DataFinalSaida.Date.ToString(pattern) + "'";

            if (filtrosPesquisa.Setor > 0)
                where += " AND Controle.SET_CODIGO = " + filtrosPesquisa.Setor.ToString();

            if (filtrosPesquisa.Autorizador > 0)
                where += " AND Controle.FUN_AUTORIZADOR = " + filtrosPesquisa.Autorizador.ToString();

            if (!string.IsNullOrEmpty(filtrosPesquisa.CPF.ToString()))
                where += " AND Controle.COV_CPF LIKE '%" + filtrosPesquisa.CPF.ToString() + "%'";

        }

        #endregion
    }
}
