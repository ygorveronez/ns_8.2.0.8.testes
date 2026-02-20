using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.Colaborador
{
    public class ColaboradorLancamento : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>
    {
        public ColaboradorLancamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ProximoNumeroColaboradorLancamento(int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento> BuscarLancamentosPendentes(int codigoFuncionario)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            var result = query.Where(obj =>
                obj.SituacaoLancamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Cancelado &&
                obj.SituacaoLancamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Finalizado);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Colaborador.Codigo == codigoFuncionario);


            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao> BuscarSituacoesEmExecucao(List<int> motoristas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            var result = from obj in query where motoristas.Contains(obj.Colaborador.Codigo) && obj.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Execucao select obj;
            return result.Select(c => c.ColaboradorSituacao).Distinct().ToList();
        }

        public bool BuscarSeExisteLancamentoEmAndamentoPorColaborador(int codigoColaborador)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            query = query.Where(obj => obj.Colaborador.Codigo == codigoColaborador && obj.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Execucao);

            return query.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento> Consultar(int numero, int codigoEmpresa, string descricao, int codigoColaborador, int codigoSituacaoColaborador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if ((int)situacao > 0)
                result = result.Where(obj => obj.SituacaoLancamento == situacao);

            if (codigoColaborador > 0)
                result = result.Where(obj => obj.Colaborador.Codigo == codigoColaborador);

            if (codigoSituacaoColaborador > 0)
                result = result.Where(obj => obj.ColaboradorSituacao.Codigo == codigoSituacaoColaborador);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int numero, int codigoEmpresa, string descricao, int codigoColaborador, int codigoSituacaoColaborador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if ((int)situacao > 0)
                result = result.Where(obj => obj.SituacaoLancamento == situacao);

            if (codigoColaborador > 0)
                result = result.Where(obj => obj.Colaborador.Codigo == codigoColaborador);

            if (codigoSituacaoColaborador > 0)
                result = result.Where(obj => obj.ColaboradorSituacao.Codigo == codigoSituacaoColaborador);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento BuscarPorCodigoMotorista(int codigomMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            var result = from obj in query where obj.Colaborador.Codigo == codigomMotorista && obj.Colaborador.Tipo == "M" select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento BuscarPorCodigoMotoristaCorrente(int codigomMotorista, DateTime dataBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();

            var situacaoNaoPermitda = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador>() {
               Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Cancelado
            };            
            
            query = query.Where(q => !situacaoNaoPermitda.Contains(q.SituacaoLancamento) &&  q.Colaborador.Codigo == codigomMotorista && q.Colaborador.Tipo == "M");
            if (dataBase != null)
            {
                query = query.Where(q => dataBase.Date <= q.DataFinal.Date && dataBase.Date >= q.DataInicial.Date);
            }
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento BuscarPorCodigoMotoristaProximo(int codigomMotorista, DateTime dataBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>();
            var situacaoNaoPermitda = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador>() {
               Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Execucao
            };
            query = query.Where(q => situacaoNaoPermitda.Contains(q.SituacaoLancamento) && q.Colaborador.Codigo == codigomMotorista && q.Colaborador.Tipo == "M");
            if (dataBase != null)
            {
                query = query.Where(q => q.DataFinal.Date >= dataBase.Date);
            }
            query.OrderBy(q => q.DataInicial);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento ConsultarLancamentoColaboradorPorData(int codigoColaborador, DateTime? data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento>().Where(obj => obj.Colaborador.Codigo == codigoColaborador && obj.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Finalizado);

            var result = query.AsQueryable();

            if (data.HasValue)
                result = result.Where(obj => obj.DataInicial <= data && obj.DataFinal >= data.Value.Date);

            return result.FirstOrDefault();
        }

        #region Relatório de Lançamento de Situação de Colaborador

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.ColaboradorSituacaoLancamento> ConsultarRelatorioColaboradorSituacaoLancamento(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioColaboradorSituacaoLancamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMensal(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.ColaboradorSituacaoLancamento)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.ColaboradorSituacaoLancamento>();
        }

        public int ContarConsultaRelatorioColaboradorSituacaoLancamento(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioColaboradorSituacaoLancamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMensal(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaMensal(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioColaboradorSituacaoLancamento filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDespesaMensal(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioDespesaMensal(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDespesaMensal(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            {
                query += " ColaboradorSituacao.CSI_COR Cor, ";
                query += select.Substring(0, select.Length - 2);
            }

            // FROM
            query += " FROM T_COLABORADOR_LANCAMENTO ColaboradorLancamento ";

            // JOIN
            if (!joins.Contains(" ColaboradorSituacao "))
                joins += " LEFT OUTER JOIN T_COLABORADOR_SITUACAO ColaboradorSituacao ON ColaboradorSituacao.CSI_CODIGO = ColaboradorLancamento.CSI_CODIGO";
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (!groupBy.Contains(" ColaboradorSituacao.CSI_COR, "))
                groupBy += " ColaboradorSituacao.CSI_COR, ";

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

        private void SetarSelectRelatorioConsultaRelatorioDespesaMensal(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "ColaboradorLancamento.CLS_CODIGO Codigo, ";
                        groupBy += "ColaboradorLancamento.CLS_CODIGO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "ColaboradorLancamento.CLS_NUMERO Numero, ";
                        groupBy += "ColaboradorLancamento.CLS_NUMERO, ";
                    }
                    break;
                case "Colaborador":
                    if (!select.Contains(" Colaborador, "))
                    {
                        if (!joins.Contains(" Colaborador "))
                            joins += " LEFT OUTER JOIN T_FUNCIONARIO Colaborador ON Colaborador.FUN_CODIGO = ColaboradorLancamento.FUN_CODIGO";

                        select += "Colaborador.FUN_NOME Colaborador, ";
                        groupBy += "Colaborador.FUN_NOME, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " LEFT OUTER JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = ColaboradorLancamento.FUN_CODIGO_OPERADOR";

                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;
                case "DataLancamento":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select += "ColaboradorLancamento.CLS_DATA_LANCAMENTO DataLancamento, ";
                        groupBy += "ColaboradorLancamento.CLS_DATA_LANCAMENTO, ";
                    }
                    break;
                case "DataInicial":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select += "ColaboradorLancamento.CLS_DATA_INICIAL DataInicial, ";
                        groupBy += "ColaboradorLancamento.CLS_DATA_INICIAL, ";
                    }
                    break;
                case "DataFinal":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select += "ColaboradorLancamento.CLS_DATA_FINAL DataFinal, ";
                        groupBy += "ColaboradorLancamento.CLS_DATA_FINAL, ";
                    }
                    break;
                case "ColaboradorSituacao":
                    if (!select.Contains(" ColaboradorSituacao, "))
                    {
                        if (!joins.Contains(" ColaboradorSituacao "))
                            joins += " LEFT OUTER JOIN T_COLABORADOR_SITUACAO ColaboradorSituacao ON ColaboradorSituacao.CSI_CODIGO = ColaboradorLancamento.CSI_CODIGO";

                        select += "ColaboradorSituacao.CSI_DESCRICAO ColaboradorSituacao, ";
                        groupBy += "ColaboradorSituacao.CSI_DESCRICAO, ";
                    }
                    break;
                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += @"CASE WHEN ColaboradorLancamento.CLS_SITUACAO = 2 THEN 'Cancelado'
                                        WHEN ColaboradorLancamento.CLS_SITUACAO = 3 THEN 'Em Execução'
                                        WHEN ColaboradorLancamento.CLS_SITUACAO = 4 THEN 'Finalizado'
                                    ELSE 'Agendado'
                                    END Situacao, ";
                        groupBy += "ColaboradorLancamento.CLS_SITUACAO, ";
                    }
                    break;
                case "Cargo":
                    if (!select.Contains(" Cargo, "))
                    {
                        if (!joins.Contains(" Cargo "))
                            joins += " LEFT OUTER JOIN T_FUNCIONARIO Cargo ON Cargo.FUN_CODIGO = ColaboradorLancamento.FUN_CODIGO";

                        select += "Cargo.FUN_CARGO Cargo, ";
                        groupBy += "Cargo.FUN_CARGO, ";
                    }
                    break;
                case "CodigoContabil":
                    if (!select.Contains(" CodigoContabil, "))
                    {
                        if (!joins.Contains(" ColaboradorSituacao "))
                            joins += " LEFT OUTER JOIN T_COLABORADOR_SITUACAO ColaboradorSituacao ON ColaboradorSituacao.CSI_CODIGO = ColaboradorLancamento.CSI_CODIGO";

                        select += "ColaboradorSituacao.CSI_CODIGO_CONTABIL CodigoContabil, ";
                        groupBy += "ColaboradorSituacao.CSI_CODIGO_CONTABIL, ";
                    }
                    break;

                case "FrotaVinculada":
                    if (!select.Contains(" FrotaVinculada, "))
                    {
                        select += @"SUBSTRING((
			                            SELECT ', ' +
				                            Veiculo.VEI_PLACA + 
				                            CASE 
					                            WHEN TRIM(ISNULL(Veiculo.VEI_NUMERO_FROTA,'')) <> '' 
						                            THEN ' (' + Veiculo.VEI_NUMERO_FROTA + ')' 
					                            ELSE '' 
				                            END
			                            FROM T_VEICULO_MOTORISTA VeiMotorista 
				                            LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VeiMotorista.VEI_CODIGO
			                            WHERE VeiMotorista.FUN_CODIGO = ColaboradorLancamento.FUN_CODIGO
			                            FOR XML PATH('')), 
	                                3, 1000) FrotaVinculada, ";
                        groupBy += "ColaboradorLancamento.FUN_CODIGO, ";
                    }
                    break;


                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDespesaMensal(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioColaboradorSituacaoLancamento filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.Empresa > 0)
                where += " AND ColaboradorLancamento.EMP_CODIGO = " + filtrosPesquisa.Empresa.ToString();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where += " AND ColaboradorLancamento.CLS_DATA_INICIAL >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where += " AND ColaboradorLancamento.CLS_DATA_FINAL <= '" + filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern) + "'";

            if (filtrosPesquisa.Colaborador > 0)
                where += " AND ColaboradorLancamento.FUN_CODIGO = " + filtrosPesquisa.Colaborador;

            if (filtrosPesquisa.Situacao.Count() > 0)
                where += " AND ColaboradorLancamento.CLS_SITUACAO IN (" + string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ToString("D"))) + ")";

            if (filtrosPesquisa.SituacaoColaborador.Count > 0)
            {
                if (!joins.Contains(" ColaboradorSituacao "))
                    joins += " LEFT OUTER JOIN T_COLABORADOR_SITUACAO ColaboradorSituacao ON ColaboradorSituacao.CSI_CODIGO = ColaboradorLancamento.CSI_CODIGO";

                where += " AND ColaboradorSituacao.CSI_SITUACAO_COLABORADOR IN (" + string.Join(", ", filtrosPesquisa.SituacaoColaborador.Select(o => o.ToString("D"))) + ")";
            }

            if (filtrosPesquisa.Veiculo > 0)
            {
                if (!joins.Contains(" VeiculoMotorista "))
                    joins += " LEFT OUTER JOIN T_VEICULO_MOTORISTA VeiculoMotorista ON VeiculoMotorista.FUN_CODIGO = ColaboradorLancamento.FUN_CODIGO ";

                where += $" AND VeiculoMotorista.VEI_CODIGO = {filtrosPesquisa.Veiculo} ";
            }

        }

        #endregion
    }
}
