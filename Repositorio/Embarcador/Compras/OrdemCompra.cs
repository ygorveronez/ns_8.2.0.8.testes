using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class OrdemCompra : RepositorioBase<Dominio.Entidades.Embarcador.Compras.OrdemCompra>
    {
        public OrdemCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            var resultNumero = result.Select(o => o.Numero);

            int maiorNumero = 0;
            if (resultNumero.Count() > 0)
                maiorNumero = resultNumero.Max();

            return maiorNumero + 1;
        }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(o => o.Fornecedor)
                .Fetch(o => o.Veiculo)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> BuscarPorCotacao(int codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
            var result = from obj in query where obj.CotacaoCompra.Codigo == codigoCotacao select obj;
            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Compras.OrdemCompra BuscarPorNumero(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();
            var result = from obj in query where obj.Numero == numero select obj;
            return result.FirstOrDefault();
        }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.OrdemCompra> _Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaOrdemCompra filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

            var result = from obj in query select obj;
            
            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.NumeroCotacao > 0)
                result = result.Where(o => o.CotacaoCompra.Numero == filtrosPesquisa.NumeroCotacao);

            if (filtrosPesquisa.DataGeracaoInicio != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= filtrosPesquisa.DataGeracaoInicio);

            if (filtrosPesquisa.DataGeracaoFim != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= filtrosPesquisa.DataGeracaoFim);

            if (filtrosPesquisa.DataRetornoInicio != DateTime.MinValue)
                result = result.Where(o => o.DataPrevisaoRetorno.Date >= filtrosPesquisa.DataRetornoInicio);

            if (filtrosPesquisa.DataRetornoFim != DateTime.MinValue)
                result = result.Where(o => o.DataPrevisaoRetorno.Date <= filtrosPesquisa.DataRetornoFim);

            if (filtrosPesquisa.Numero > 0)
                result = result.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Operador > 0)
                result = result.Where(o => o.Usuario.Codigo == filtrosPesquisa.Operador);

            if (filtrosPesquisa.Produto > 0)
                result = result.Where(o => o.Mercadorias.Any(m => m.Produto.Codigo == filtrosPesquisa.Produto));

            if (filtrosPesquisa.Fornecedor > 0)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == filtrosPesquisa.Fornecedor);

            if (filtrosPesquisa.Transportador > 0)
                result = result.Where(o => o.Transportador.CPF_CNPJ == filtrosPesquisa.Transportador);

            if (filtrosPesquisa.Situacao.HasValue)
                result = result.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.Veiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == filtrosPesquisa.Veiculo);

            if (filtrosPesquisa.NumeroRequisicao > 0)
            {
                var queryRequisicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao>();
                result = result.Where(o => queryRequisicao.Any(p => p.OrdemCompra.Codigo == o.Codigo && p.Requisicao.Numero == filtrosPesquisa.NumeroRequisicao));
            }

            return result;
        }

        #endregion

        #region Relatorio Ordem Compra

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra> ConsultarRelatorioOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioOrdemCompra(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioOrdemCompra>();
        }

        public int ContarConsultaRelatorioOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa = "", string dirAgrupa = "", string propOrdena = "", string dirOrdena = "")
        {
            string sql = ObterSelectConsultaRelatorioOrdemCompra(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string with = string.Empty,
                   select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioOrdemCompra(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, ref with, count);

            SetarWhereRelatorioConsultaRelatorioOrdemCompra(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioOrdemCompra(propAgrupa, 0, ref select, ref groupBy, ref joins, ref with, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            string query = "";
            //WITH
            if(!count && with.Length > 0)
                query += "WITH " + with + " ";

            // SELECT
            query += "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_ORDEM_COMPRA_MERCADORIA Mercadoria ";

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

        private void SetarSelectRelatorioConsultaRelatorioOrdemCompra(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, ref string with, bool count)
        {
            switch (propriedade)
            {

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_NUMERO Numero, ";
                        groupBy += "OrdemCompra.ORC_NUMERO, ";
                    }
                    break;
                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" Fornecedor "))
                            joins += " JOIN T_CLIENTE Fornecedor ON OrdemCompra.CLI_FORNECEDOR = Fornecedor.CLI_CGCCPF";

                        select += "Fornecedor.CLI_NOME Fornecedor, ";
                        groupBy += "Fornecedor.CLI_NOME, ";
                    }
                    break;
                case "DataGeracaoInicioFormatada":
                    if (!select.Contains(" DataGeracaoInicio, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_DATA DataGeracaoInicio, ";
                        groupBy += "OrdemCompra.ORC_DATA, ";
                    }
                    break;
                case "DataGeracaoFimFormatada":
                    if (!select.Contains(" DataGeracaoFim, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_DATA DataGeracaoFim, ";
                        groupBy += "OrdemCompra.ORC_DATA, ";
                    }
                    break;
                case "DataPrevisaoInicioFormatada":
                    if (!select.Contains(" DataPrevisaoInicio, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_DATA_PREVISAO DataPrevisaoInicio, ";
                        groupBy += "OrdemCompra.ORC_DATA_PREVISAO, ";
                    }
                    break;
                case "DataPrevisaoFimFormatada":
                    if (!select.Contains(" DataPrevisaoFim, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_DATA_PREVISAO DataPrevisaoFim, ";
                        groupBy += "OrdemCompra.ORC_DATA_PREVISAO, ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" Transportador "))
                            joins += " LEFT JOIN T_CLIENTE Transportador ON OrdemCompra.CLI_TRANSPORTADOR = Transportador.CLI_CGCCPF";

                        select += "Transportador.CLI_NOME Transportador, ";
                        groupBy += "Transportador.CLI_NOME, ";
                    }
                    break;
                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_SITUACAO Situacao, ";
                        groupBy += "OrdemCompra.ORC_SITUACAO, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!joins.Contains(" Produto "))
                            joins += " JOIN T_PRODUTO Produto ON Mercadoria.PRO_CODIGO = Produto.PRO_CODIGO";

                        select += "Produto.PRO_DESCRICAO Produto, ";
                        groupBy += "Produto.PRO_DESCRICAO, ";
                    }
                    break;
                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        select += "SUM(Mercadoria.OCM_QUANTIDADE) Quantidade, ";
                    }
                    break;
                case "ValorUnitario":
                    if (!select.Contains(" ValorUnitario, "))
                    {
                        select += "SUM(Mercadoria.OCM_VALOR_UNITARIO) ValorUnitario, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select += "SUM(Mercadoria.OCM_QUANTIDADE * Mercadoria.OCM_VALOR_UNITARIO) ValorTotal, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" Operador "))
                            joins += " JOIN T_FUNCIONARIO Operador ON OrdemCompra.FUN_CODIGO = Operador.FUN_CODIGO";

                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;

                case "Motivo":
                    if (!select.Contains(" Motivo, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" MotivoCompra "))
                            joins += " LEFT JOIN T_MOTIVO_COMPRA MotivoCompra ON MotivoCompra.MCO_CODIGO = OrdemCompra.MCO_CODIGO ";

                        select += "MotivoCompra.MCO_DESCRICAO Motivo, ";

                        if (!groupBy.Contains("MotivoCompra.MCO_DESCRICAO"))
                            groupBy += "MotivoCompra.MCO_DESCRICAO, ";
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemCompra.ORC_VEICULO";
                            joins += " LEFT OUTER JOIN T_VEICULO VeiculoMercadoria ON VeiculoMercadoria.VEI_CODIGO = Mercadoria.VEI_CODIGO";

                        select += "Coalesce(Veiculo.VEI_PLACA, VeiculoMercadoria.VEI_PLACA) as Veiculo, ";

                        if (!groupBy.Contains("Veiculo.VEI_PLACA"))
                            groupBy += "Coalesce(Veiculo.VEI_PLACA, VeiculoMercadoria.VEI_PLACA), ";
                    }
                    break;

                case "CondicaoPagamento":
                    if (!select.Contains(" CondicaoPagamento, "))
                    {
                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "OrdemCompra.ORC_CONDICAO_PAGAMENTO CondicaoPagamento, ";
                        groupBy += "OrdemCompra.ORC_CONDICAO_PAGAMENTO, ";
                    }
                    break;

                case "Aprovador":
                    if (!count && !select.Contains(".Aprovador, "))
                    {
                        with += "Aprovadores AS (SELECT OrdemCompra.ORC_CODIGO, STRING_AGG(Usuario.FUN_NOME, ', ') AS Aprovador " +
                                                "FROM T_ORDEM_COMPRA OrdemCompra " +
                                                "JOIN T_AUTORIZACAO_ALCADA_ORDEM_COMPRA Aprovacao ON OrdemCompra.ORC_CODIGO = Aprovacao.ORC_CODIGO " +
                                                "JOIN T_FUNCIONARIO Usuario ON Aprovacao.FUN_CODIGO = Usuario.FUN_CODIGO " +
                                               "WHERE AAA_SITUACAO = 1 " +
                                               "GROUP BY OrdemCompra.ORC_CODIGO) ";

                        if (!joins.Contains(" OrdemCompra "))
                            joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        if (!joins.Contains(" Aprovadores "))
                            joins += " LEFT JOIN Aprovadores ON OrdemCompra.ORC_CODIGO = Aprovadores.ORC_CODIGO";

                        select += "Aprovadores.Aprovador, ";

                        groupBy += "Aprovadores.Aprovador, ";
                        groupBy += "OrdemCompra.ORC_NUMERO, ";
                    }
                    break;
      
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioOrdemCompra(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioOrdemCompra filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND OrdemCompra.EMP_CODIGO = '" + filtrosPesquisa.CodigoEmpresa.ToString() + "' ";
            }

            if (filtrosPesquisa.DataGeracaoInicio != DateTime.MinValue || filtrosPesquisa.DataPrevisaoInicio != DateTime.MinValue)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                if (filtrosPesquisa.DataGeracaoInicio != DateTime.MinValue)
                    where += " AND OrdemCompra.ORC_DATA >= '" + filtrosPesquisa.DataGeracaoInicio.ToString(pattern) + "' ";

                if (filtrosPesquisa.DataPrevisaoInicio != DateTime.MinValue)
                    where += " AND OrdemCompra.ORC_DATA_PREVISAO >= '" + filtrosPesquisa.DataPrevisaoInicio.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.DataGeracaoFim != DateTime.MinValue || filtrosPesquisa.DataPrevisaoFim != DateTime.MinValue)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                if (filtrosPesquisa.DataGeracaoFim != DateTime.MinValue)
                    where += " AND OrdemCompra.ORC_DATA <= '" + filtrosPesquisa.DataGeracaoFim.ToString(pattern) + "' ";

                if (filtrosPesquisa.DataPrevisaoFim != DateTime.MinValue)
                    where += " AND OrdemCompra.ORC_DATA_PREVISAO <= '" + filtrosPesquisa.DataPrevisaoFim.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.NumeroInicial > 0 || filtrosPesquisa.NumeroFinal > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                if (filtrosPesquisa.NumeroInicial > 0)
                    where += " AND OrdemCompra.ORC_NUMERO >= " + filtrosPesquisa.NumeroInicial;

                if (filtrosPesquisa.NumeroFinal > 0)
                    where += " AND OrdemCompra.ORC_NUMERO <= " + filtrosPesquisa.NumeroFinal;
            }

            if (filtrosPesquisa.Produto > 0)
                where += " AND Mercadoria.PRO_CODIGO = " + filtrosPesquisa.Produto;

            if (filtrosPesquisa.Operador > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND OrdemCompra.FUN_CODIGO = " + filtrosPesquisa.Operador;
            }

            if (filtrosPesquisa.Fornecedor > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND OrdemCompra.CLI_FORNECEDOR = " + filtrosPesquisa.Fornecedor;
            }

            if (filtrosPesquisa.Transportador > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND OrdemCompra.CLI_TRANSPORTADOR = " + filtrosPesquisa.Transportador;
            }

            if (filtrosPesquisa.Situacao > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND OrdemCompra.ORC_SITUACAO = " + filtrosPesquisa.Situacao.ToString("d");
            }

            if (filtrosPesquisa.Veiculo > 0)
            {
                if (!joins.Contains(" OrdemCompra "))
                    joins += " JOIN T_ORDEM_COMPRA OrdemCompra ON Mercadoria.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += $" AND OrdemCompra.ORC_VEICULO = {filtrosPesquisa.Veiculo} ";
            }
        }

        #endregion

        #region Relatorio Nota de Entrada x Ordem de Compra

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra> ConsultarRelatorioNotaEntradaOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra>();
        }

        public int ContarConsultaRelatorioNotaEntradaOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa = "", string dirAgrupa = "", string propOrdena = "", string dirOrdena = "")
        {
            string sql = ObterSelectConsultaRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioNotaEntradaOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaNotaEntradaOrdemCompra(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaNotaEntradaOrdemCompra(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaNotaEntradaOrdemCompra(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_ORDEM_COMPRA OrdemCompra ";

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

        private void SetarSelectRelatorioConsultaNotaEntradaOrdemCompra(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "NumeroNota":
                    if (!select.Contains(" NumeroNota, "))
                    {
                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " LEFT OUTER JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "DocumentoEntrada.TDE_NUMERO_LONG NumeroNota, ";
                        //groupBy += "DocumentoEntrada.TDE_NUMERO_LONG, ";
                    }
                    break;
                case "NumeroOrdem":
                    if (!select.Contains(" NumeroOrdem, "))
                    {
                        select += "OrdemCompra.ORC_NUMERO NumeroOrdem, ";
                        //groupBy += "OrdemCompra.ORC_NUMERO, ";
                    }
                    break;
                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        if (!joins.Contains(" Fornecedor "))
                            joins += " JOIN T_CLIENTE Fornecedor ON OrdemCompra.CLI_FORNECEDOR = Fornecedor.CLI_CGCCPF";

                        select += "Fornecedor.CLI_NOME Fornecedor, ";
                        //groupBy += "Fornecedor.CLI_NOME, ";
                    }
                    break;
                case "DataEntradaFormatada":
                    if (!select.Contains(" DataEntrada, "))
                    {
                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " LEFT OUTER JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                        select += "DocumentoEntrada.TDE_DATA_ENTRADA DataEntrada, ";
                        //groupBy += "DocumentoEntrada.TDE_DATA_ENTRADA, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!joins.Contains(" ItemOrdemCompra ") && !joins.Contains(" ItemDocumentoEntrada "))
                        {
                            joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                            joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                        }

                        select += @" Produto.PRO_DESCRICAO Produto, ";
                        //groupBy += "OrdemCompra.ORC_CODIGO, ";
                    }
                    break;
                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        if (!joins.Contains(" ItemOrdemCompra "))
                        {
                            joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                            joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                        }

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                        if (!joins.Contains(" ItemDocumentoEntrada "))
                            joins += " LEFT OUTER JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM ItemDocumentoEntrada ON ItemDocumentoEntrada.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO and ItemDocumentoEntrada.TDE_CODIGO = DocumentoEntrada.TDE_CODIGO";

                        select += @" ISNULL(ItemDocumentoEntrada.TDI_QUANTIDADE, 0)  QuantidadeNF, ";
                        //groupBy += "OrdemCompra.ORC_CODIGO, ";
                    }
                    break;
                case "QuantidadeOrdem":
                    if (!select.Contains(" QuantidadeOrdem, "))
                    {
                        if (!joins.Contains(" ItemOrdemCompra ") && !joins.Contains(" ItemDocumentoEntrada "))
                        {
                            joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                            joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                        }

                        select += @" ItemOrdemCompra.OCM_QUANTIDADE  QuantidadeOrdem, ";
                        //groupBy += "OrdemCompra.ORC_CODIGO, ";
                    }
                    break;
                case "ValorNota":
                    if (!select.Contains(" ValorNota, "))
                    {
                        if (!joins.Contains(" ItemOrdemCompra "))
                        {
                            joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                            joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                        }

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                        if (!joins.Contains(" ItemDocumentoEntrada "))
                            joins += " LEFT OUTER JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM ItemDocumentoEntrada ON ItemDocumentoEntrada.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO and ItemDocumentoEntrada.TDE_CODIGO = DocumentoEntrada.TDE_CODIGO";

                        select += " ISNULL(ItemDocumentoEntrada.TDI_VALOR_TOTAL, 0) ValorNota, ";
                    }
                    break;
                case "ValorOrdem":
                    if (!select.Contains(" ValorOrdem, "))
                    {
                        if (!joins.Contains(" ItemOrdemCompra ") && !joins.Contains(" ItemDocumentoEntrada "))
                        {
                            joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                            joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                        }

                        select += @" (ItemOrdemCompra.OCM_QUANTIDADE * ItemOrdemCompra.OCM_VALOR_UNITARIO)  ValorOrdem, ";
                        //groupBy += "OrdemCompra.ORC_CODIGO, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaNotaEntradaOrdemCompra(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataEntrada != DateTime.MinValue)
            {
                if (!joins.Contains(" DocumentoEntrada "))
                    joins += " JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND DocumentoEntrada.TDE_DATA_ENTRADA = '" + filtrosPesquisa.DataEntrada.ToString(pattern) + "' ";
            }

            if (filtrosPesquisa.CodigoNota > 0)
            {
                if (!joins.Contains(" DocumentoEntrada "))
                    joins += " JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND DocumentoEntrada.TDE_CODIGO = " + filtrosPesquisa.CodigoNota;
            }

            if (filtrosPesquisa.Nota > 0)
            {
                if (!joins.Contains(" DocumentoEntrada "))
                    joins += " JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.ORC_CODIGO = OrdemCompra.ORC_CODIGO";

                where += " AND DocumentoEntrada.TDE_NUMERO_LONG = " + filtrosPesquisa.Nota;
            }

            if (filtrosPesquisa.CodigoOrdem > 0)
                where += " AND OrdemCompra.ORC_CODIGO = " + filtrosPesquisa.CodigoOrdem;

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                if (!joins.Contains(" ItemOrdemCompra ") && !joins.Contains(" ItemDocumentoEntrada "))
                {
                    joins += " JOIN T_ORDEM_COMPRA_MERCADORIA ItemOrdemCompra ON ItemOrdemCompra.ORC_CODIGO = OrdemCompra.ORC_CODIGO";
                    joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = ItemOrdemCompra.PRO_CODIGO";
                }

                where += @" AND ItemOrdemCompra.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();
            }

            if (filtrosPesquisa.Fornecedor > 0)
                where += " AND OrdemCompra.CLI_FORNECEDOR = " + filtrosPesquisa.Fornecedor;

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where += " AND OrdemCompra.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.SugestaoCompra> RelatorioSugestaoCompra(int codigoEmpresa, int codigoProduto, int empresa, int codigoGrupoProduto, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "";

            query = @"  SELECT P.PRO_DESCRICAO Produto,                
                GRP_DESCRICAO GrupoProduto,
                EE.EMP_RAZAO Empresa,
                ISNULL(E.PRE_QUANTIDADE, 0) Estoque,
                ISNULL(E.PRE_ESTOQUE_MINIMO, 0) QtdMinima,
                ISNULL(E.PRE_ESTOQUE_MAXIMO, 0) QtdMaximo,
                CASE
	                WHEN (((ISNULL(PRE_ESTOQUE_MINIMO, 0) - ISNULL(PRE_QUANTIDADE, 0)) + 1 ) > ISNULL(E.PRE_ESTOQUE_MAXIMO, 0)) AND ISNULL(E.PRE_ESTOQUE_MAXIMO, 0) > 0 THEN ISNULL(E.PRE_ESTOQUE_MAXIMO, 0)
	                ELSE (ISNULL(PRE_ESTOQUE_MINIMO, 0) - ISNULL(PRE_QUANTIDADE, 0)) + 1 
                END QtdSugestao,
                ISNULL(E.PRE_ULTIMO_CUSTO, 0) UltimoCusto
                FROM T_PRODUTO_ESTOQUE E
                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = E.EMP_CODIGO
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                WHERE ISNULL(PRE_QUANTIDADE, 0) <= ISNULL(PRE_ESTOQUE_MINIMO, 0) ";

            if (codigoEmpresa > 0)
                query += @" AND EE.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                query += @" AND E.PRO_CODIGO= " + codigoProduto.ToString();

            if (empresa > 0)
                query += @" AND EE.EMP_CODIGO = " + empresa.ToString();

            if (codigoGrupoProduto > 0)
                query += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.SugestaoCompra)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Compras.SugestaoCompra>();
        }

        public int ContarRelatorioSugestaoCompra(int codigoEmpresa, int codigoProduto, int empresa, int codigoGrupoProduto)
        {
            string query = "";

            query = @"   SELECT  COUNT(0) as CONTADOR 
                 FROM T_PRODUTO_ESTOQUE E
                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = E.EMP_CODIGO
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                WHERE ISNULL(PRE_QUANTIDADE, 0) <= ISNULL(PRE_ESTOQUE_MINIMO, 0) ";

            if (codigoEmpresa > 0)
                query += @" AND EE.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                query += @" AND E.PRO_CODIGO= " + codigoProduto.ToString();

            if (empresa > 0)
                query += @" AND EE.EMP_CODIGO = " + empresa.ToString();

            if (codigoGrupoProduto > 0)
                query += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion
    }
}
