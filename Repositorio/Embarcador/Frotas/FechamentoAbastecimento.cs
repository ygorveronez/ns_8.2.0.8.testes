using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frotas
{
    public class FechamentoAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento>
    {
        public FechamentoAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool AbastecimentosComTituloQuitado(int codigo)
        {
            var queryFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            queryFechamento = queryFechamento.Where(c => c.FechamentoAbastecimento.Codigo == codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where queryFechamento.Any(c => obj.Abastecimento == c) && obj.StatusTitulo == StatusTitulo.Quitada select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Abastecimento> ConsultarPorFechamento(int codigo, string situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.FechamentoAbastecimento.Codigo == codigo select obj;

            if (!string.IsNullOrWhiteSpace(situacao) && !situacao.Equals("T"))
                result = result.Where(o => o.Situacao.Equals(situacao));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultarPorFechamento(int codigo, string situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.FechamentoAbastecimento.Codigo == codigo select obj;

            if (!string.IsNullOrWhiteSpace(situacao) && !situacao.Equals("T"))
                result = result.Where(o => o.Situacao.Equals(situacao));

            return result.Count();
        }

        public bool PossuiQuilometragemZeradaPorFechamento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.FechamentoAbastecimento.Codigo == codigoFechamento && o.Kilometragem <= 0 && o.Horimetro <= 0);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento> ConsultarFechamentos(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento> result = Consultar(filtrosPesquisa);

            result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultarFechamentos(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public decimal BuscarValorTotalAbastecimentoPorFechamento(int codigoFechamento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.FechamentoAbastecimento.Codigo == codigoFechamento);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == codigoEquipamento);

            if (posto > 0)
                query = query.Where(o => o.Posto.CPF_CNPJ == posto);

            if (!string.IsNullOrWhiteSpace(situacao) && situacao != "T")
                query = query.Where(o => o.Situacao == situacao);

            if (codigoProduto > 0)
                query = query.Where(o => o.Produto.Codigo == codigoProduto);

            return query.Sum(o => (decimal?)(o.Litros * o.ValorUnitario)) ?? 0m;
        }

        public decimal BuscarLitrosAbastecimentoPorFechamento(int codigoFechamento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.FechamentoAbastecimento.Codigo == codigoFechamento);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == codigoEquipamento);

            if (posto > 0)
                query = query.Where(o => o.Posto.CPF_CNPJ == posto);

            if (!string.IsNullOrWhiteSpace(situacao) && situacao != "T")
                query = query.Where(o => o.Situacao == situacao);
            
            if (codigoProduto > 0)
                query = query.Where(o => o.Produto.Codigo == codigoProduto);

            return query.Sum(o => (decimal?)o.Litros) ?? 0m;
        }

        public decimal BuscarKMAbastecimentoPorFechamento(int codigoFechamento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.FechamentoAbastecimento.Codigo == codigoFechamento);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == codigoEquipamento);

            if (posto > 0)
                query = query.Where(o => o.Posto.CPF_CNPJ == posto);

            if (!string.IsNullOrWhiteSpace(situacao) && situacao != "T")
                query = query.Where(o => o.Situacao == situacao);

            if (codigoProduto > 0)
                query = query.Where(o => o.Produto.Codigo == codigoProduto);

            return query.Sum(o => (decimal?)o.Kilometragem) ?? 0m;
        }

        public int BuscarHorimetroAbastecimentoPorFechamento(int codigoFechamento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.FechamentoAbastecimento.Codigo == codigoFechamento);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == codigoEquipamento);

            if (posto > 0)
                query = query.Where(o => o.Posto.CPF_CNPJ == posto);

            if (!string.IsNullOrWhiteSpace(situacao) && situacao != "T")
                query = query.Where(o => o.Situacao == situacao);

            if (codigoProduto > 0)
                query = query.Where(o => o.Produto.Codigo == codigoProduto);

            return query.Sum(o => (int?)o.Horimetro) ?? 0;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento> ConsultarAbastecimentosPorFechamento(int codigoFechamento, int codigoAbastecimento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao = "", string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarAbastecimentosPorFechamento(codigoFechamento, codigoAbastecimento, situacao, codigoVeiculo, codigoEquipamento, posto, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento>();
        }

        public int ContarAbastecimentosPorFechamento(int codigoFechamento, int codigoAbastecimento, int codigoVeiculo, int codigoEquipamento, double posto, string situacao)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarAbastecimentosPorFechamento(codigoFechamento, codigoAbastecimento, situacao, codigoVeiculo, codigoEquipamento, posto, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento> Consultar(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicio.Value.Date >= filtrosPesquisa.DataInicio.Date && o.DataInicio.Value.Date < filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.DataFim.Value.Date >= filtrosPesquisa.DataFim.Date && o.DataFim.Value.Date < filtrosPesquisa.DataFim.Date);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Todas)
                result = result.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
                var resultAbastecimento = from obj in queryAbastecimento where obj.FechamentoAbastecimento != null && obj.Veiculo != null && obj.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo select obj.FechamentoAbastecimento.Codigo;

                result = result.Where(o => resultAbastecimento.Contains(o.Codigo));
            }

            if (filtrosPesquisa.CodigoEquipamento > 0)
                result = result.Where(obj => obj.Equipamento.Codigo == filtrosPesquisa.CodigoEquipamento);

            return result;
        }

        private string QueryConsultarAbastecimentosPorFechamento(int codigoFechamento, int codigoAbastecimento, string situacao, int codigoVeiculo, int codigoEquipamento, double posto, bool somenteContarNumeroRegistros, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select
                        A.ABA_CODIGO Codigo,
                        C.CLI_NOME Posto,
                        V.VEI_PLACA Placa,
                        A.ABA_TIPO TipoAbastecimento,
                        A.ABA_DATA Data,
                        A.ABA_KM Km,
                        A.ABA_LITROS Litros,
                        ISNULL(ABA_VALOR_UN, 0) ValorUnitario, 
                        (ISNULL(ABA_VALOR_UN, 0) * ISNULL(A.ABA_LITROS, 0)) ValorTotal,
                        (A.ABA_KM - CASE
							WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
								ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO ORDER BY ABA_KM DESC), A.ABA_KM)
							ELSE
								ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO ORDER BY ABA_KM DESC), A.ABA_KM)
						END) KMTotal,
                        ((A.ABA_KM - CASE
							WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
								ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO ORDER BY ABA_KM DESC), A.ABA_KM)
							ELSE
								ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO ORDER BY ABA_KM DESC), A.ABA_KM)
						END) / CASE WHEN A.ABA_LITROS <= 0 THEN 1 ELSE A.ABA_LITROS END) Media,
                        E.EQP_DESCRICAO Equipamento,
                        A.ABA_HORIMETRO Horimetro,

                        (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO ORDER BY ABA_HORIMETRO DESC), A.ABA_HORIMETRO)) HorimetroTotal,

                        (CASE WHEN (SELECT count(*) FROM T_ABASTECIMENTO AA WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO) > 0 and A.ABA_LITROS > 0 THEN
                            CASE WHEN (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                    WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO ORDER BY AA.ABA_HORIMETRO DESC), 0)) <= 0 THEN 0
                            ELSE (A.ABA_LITROS / (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                            WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO ORDER BY AA.ABA_HORIMETRO DESC), 0))) END
                        ELSE 0 END) MediaHorimetro,

                        case when A.EQP_CODIGO is null then
							(ISNULL((SELECT Count(*) FROM T_ABASTECIMENTO AA
								WHERE AA.ABA_KM = A.ABA_KM AND AA.VEI_CODIGO = A.VEI_CODIGO AND AA.ABA_LITROS = A.ABA_LITROS
								GROUP BY AA.ABA_KM, AA.VEI_CODIGO, AA.ABA_LITROS HAVING Count(*) > 1), 0))
						else 
							(ISNULL((SELECT Count(*) FROM T_ABASTECIMENTO AA
								WHERE AA.ABA_HORIMETRO = A.ABA_HORIMETRO AND AA.VEI_CODIGO = A.VEI_CODIGO AND AA.ABA_LITROS = A.ABA_LITROS 
								GROUP BY ABA_HORIMETRO, AA.VEI_CODIGO, AA.ABA_LITROS HAVING Count(*) > 1), 0)) 
						END ContagemDuplicado";

            sql += @"   from T_ABASTECIMENTO A
                        join T_FECHAMENTO_ABASTECIMENTO FechamentoAbastecimento on FechamentoAbastecimento.FAB_CODIGO = A.FAB_CODIGO
                        left outer join T_VEICULO V on V.VEI_CODIGO = A.VEI_CODIGO
                        left outer join T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                        left outer join T_EQUIPAMENTO E ON E.EQP_CODIGO = A.EQP_CODIGO
                        left outer join T_PRODUTO P ON P.PRO_CODIGO = A.PRO_CODIGO
                        where 1 = 1";

            if (codigoFechamento > 0)
                sql += " and FechamentoAbastecimento.FAB_CODIGO = " + codigoFechamento;

            if (codigoAbastecimento > 0)
                sql += " and A.ABA_CODIGO = " + codigoAbastecimento;

            if (codigoVeiculo > 0)
                sql += " and A.VEI_CODIGO = " + codigoVeiculo;

            if (codigoEquipamento > 0)
                sql += " and A.EQP_CODIGO = " + codigoEquipamento;

            if (posto > 0)
                sql += " and A.CLI_CGCCPF = " + posto;

            if (!string.IsNullOrWhiteSpace(situacao) && !situacao.Equals("T"))
                sql += $" and A.ABA_SITUACAO = '{ situacao }'";

            if (!somenteContarNumeroRegistros)
            {
                if (!string.IsNullOrWhiteSpace(propOrdenacao))
                    sql += $" order by {propOrdenacao} {dirOrdenacao}";

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql += $" offset {inicioRegistros} rows fetch next {maximoRegistros} rows only;";
            }

            return sql;
        }

        #endregion
    }
}
