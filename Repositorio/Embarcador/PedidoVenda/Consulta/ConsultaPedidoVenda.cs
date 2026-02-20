using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.PedidoVenda.Consulta
{
    sealed class ConsultaPedidoVenda : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda>
    {
        #region Construtores

        public ConsultaPedidoVenda() : base(tabela: "T_PEDIDO_VENDA as PedidoVenda") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = PedidoVenda.CLI_CGCCPF ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = PedidoVenda.FUN_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = PedidoVenda.VEI_CODIGO ");
        }

        private void SetarJoinsPedidoVendaItem(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoVendaItem "))
                joins.Append(" left join T_PEDIDO_VENDA_ITENS PedidoVendaItem on PedidoVendaItem.PEV_CODIGO = PedidoVenda.PEV_CODIGO ");
        }

        private void SetarJoinsFuncionarioServico(StringBuilder joins)
        {
            SetarJoinsPedidoVendaItem(joins);

            if (!joins.Contains(" FuncionarioServico "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioServico on FuncionarioServico.FUN_CODIGO = PedidoVendaItem.FUN_CODIGO ");
        }

        private void SetarJoinsClienteServico(StringBuilder joins)
        {
            SetarJoinsPedidoVendaItem(joins);

            if (!joins.Contains(" ClienteServico "))
                joins.Append(" left join T_CLIENTE ClienteServico on ClienteServico.CLI_CGCCPF = PedidoVendaItem.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PedidoVenda.PEV_CODIGO as Codigo, ");
                        groupBy.Append("PedidoVenda.PEV_CODIGO, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("PedidoVenda.PEV_NUMERO as Numero, ");
                        groupBy.Append("PedidoVenda.PEV_NUMERO, ");
                    }
                    break;

                case "Referencia":
                    if (!select.Contains(" Referencia, "))
                    {
                        select.Append("PedidoVenda.PEV_REFERENCIA as Referencia, ");
                        groupBy.Append("PedidoVenda.PEV_REFERENCIA, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("PedidoVenda.PEV_OBSERVACAO as Observacao, ");
                        groupBy.Append("PedidoVenda.PEV_OBSERVACAO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("PedidoVenda.PEV_DATA_EMISSAO as DataEmissao, ");
                        groupBy.Append("PedidoVenda.PEV_DATA_EMISSAO, ");
                    }
                    break;

                case "DataEntregaFormatada":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("PedidoVenda.PEV_DATA_ENTREGA as DataEntrega, ");
                        groupBy.Append("PedidoVenda.PEV_DATA_ENTREGA, ");
                    }
                    break;

                case "DescricaoTipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("PedidoVenda.PEV_TIPO as Tipo, ");
                        groupBy.Append("PedidoVenda.PEV_TIPO, ");
                    }
                    break;

                case "DescricaoStatus":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("PedidoVenda.PEV_STATUS as Status, ");
                        groupBy.Append("PedidoVenda.PEV_STATUS, ");
                    }
                    break;

                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select.Append("PedidoVenda.PEV_VALOR_TOTAL as ValorTotal, ");
                        groupBy.Append("PedidoVenda.PEV_VALOR_TOTAL, ");
                    }
                    break;

                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        select.Append("PedidoVenda.PEV_VALOR_PRODUTOS as ValorProdutos, ");
                        groupBy.Append("PedidoVenda.PEV_VALOR_PRODUTOS, ");
                    }
                    break;

                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        select.Append("PedidoVenda.PEV_VALOR_SERVICOS as ValorServicos, ");
                        groupBy.Append("PedidoVenda.PEV_VALOR_SERVICOS, ");
                    }
                    break;

                case "KMTotal":
                    if (!select.Contains(" KMTotal, "))
                    {
                        select.Append("PedidoVendaItem.PVI_KM_TOTAL as KMTotal, ");
                        groupBy.Append("PedidoVendaItem.PVI_KM_TOTAL, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;
                
                case "HotaTotal":
                    if (!select.Contains(" HotaTotal, "))
                    {
                        select.Append("CONVERT(VARCHAR(5),CAST(PedidoVendaItem.PVI_HORA_TOTAL AS DATETIME),108) AS HotaTotal, ");
                        groupBy.Append("PedidoVendaItem.PVI_HORA_TOTAL, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "CodigoItem":
                    if (!select.Contains(" CodigoItem, "))
                    {
                        select.Append("PedidoVendaItem.PVI_CODIGO_ITEM as CodigoItem, ");
                        groupBy.Append("PedidoVendaItem.PVI_CODIGO_ITEM, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "DescricaoItem":
                    if (!select.Contains(" DescricaoItem, "))
                    {
                        select.Append("PedidoVendaItem.PVI_DESCRICAO_ITEM as DescricaoItem, ");
                        groupBy.Append("PedidoVendaItem.PVI_DESCRICAO_ITEM, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "QuantidadeItem":
                    if (!select.Contains(" QuantidadeItem, "))
                    {
                        select.Append("PedidoVendaItem.PVI_QUANTIDADE as QuantidadeItem, ");
                        groupBy.Append("PedidoVendaItem.PVI_QUANTIDADE, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "ValorUnitarioItem":
                    if (!select.Contains(" ValorUnitarioItem, "))
                    {
                        select.Append("PedidoVendaItem.PVI_VALOR_UNITARIO as ValorUnitarioItem, ");
                        groupBy.Append("PedidoVendaItem.PVI_VALOR_UNITARIO, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "ValorTotalItem":
                    if (!select.Contains(" ValorTotalItem, "))
                    {
                        select.Append("PedidoVendaItem.PVI_VALOR_TOTAL as ValorTotalItem, ");
                        groupBy.Append("PedidoVendaItem.PVI_VALOR_TOTAL, ");

                        SetarJoinsPedidoVendaItem(joins);
                    }
                    break;

                case "FornecedorServico":
                    if (!select.Contains(" FornecedorServico, "))
                    {
                        select.Append("ClienteServico.CLI_NOME as FornecedorServico, ");
                        groupBy.Append("ClienteServico.CLI_NOME, ");

                        SetarJoinsClienteServico(joins);
                    }
                    break;

                case "FuncionarioServico":
                    if (!select.Contains(" FuncionarioServico, "))
                    {
                        select.Append("FuncionarioServico.FUN_NOME as FuncionarioServico, ");
                        groupBy.Append("FuncionarioServico.FUN_NOME, ");

                        SetarJoinsFuncionarioServico(joins);
                    }
                    break;

                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        select.Append("Cliente.CLI_NOME as Pessoa, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "Vendedor":
                    if (!select.Contains(" Vendedor, "))
                    {
                        select.Append("Funcionario.FUN_NOME as Vendedor, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(nota.NFI_NUMERO AS NVARCHAR(10))
		                                            FROM T_NOTA_FISCAL_PEDIDO notaPedido
                                                    INNER JOIN T_NOTA_FISCAL nota ON nota.NFI_CODIGO = notaPedido.NFI_CODIGO
		                                            WHERE notaPedido.PEV_CODIGO = PedidoVenda.PEV_CODIGO FOR XML PATH('')), 3, 1000) AS NotasFiscais, ");

                        if (!groupBy.Contains("PedidoVenda.PEV_CODIGO, "))
                            groupBy.Append("PedidoVenda.PEV_CODIGO, ");
                    }
                    break;

                case "NumeroInterno":
                    if (!select.Contains(" NumeroInterno, "))
                    {
                        select.Append("PedidoVenda.PEV_NUMERO_INTERNO as NumeroInterno, ");
                        groupBy.Append("PedidoVenda.PEV_NUMERO_INTERNO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append($" AND PedidoVenda.PEV_NUMERO >= {filtrosPesquisa.NumeroInicial}");

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append($" AND PedidoVenda.PEV_NUMERO <= {filtrosPesquisa.NumeroFinal}");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and PedidoVenda.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigoVendedor > 0)
                where.Append($" and PedidoVenda.FUN_CODIGO = {filtrosPesquisa.CodigoVendedor}");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and PedidoVenda.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");

            if (filtrosPesquisa.CnpjPessoa > 0)
                where.Append($" and PedidoVenda.CLI_CGCCPF = {filtrosPesquisa.CnpjPessoa}");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(PedidoVenda.PEV_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(PedidoVenda.PEV_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataEntregaInicial != DateTime.MinValue)
                where.Append($" and CAST(PedidoVenda.PEV_DATA_ENTREGA AS DATE) >= '{filtrosPesquisa.DataEntregaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEntregaFinal != DateTime.MinValue)
                where.Append($" and CAST(PedidoVenda.PEV_DATA_ENTREGA AS DATE) <= '{filtrosPesquisa.DataEntregaFinal.ToString(pattern)}'");

            if (filtrosPesquisa.Status > 0)
                where.Append($" and PedidoVenda.PEV_STATUS = {filtrosPesquisa.Status.ToString("D")}");

            if (filtrosPesquisa.Tipo > 0)
                where.Append($" and PedidoVenda.PEV_TIPO = {filtrosPesquisa.Tipo.ToString("D")}");

            if (filtrosPesquisa.CodigoProduto > 0)
                where.Append($" and PedidoVenda.PEV_CODIGO in (SELECT PEV_CODIGO FROM T_PEDIDO_VENDA_ITENS WHERE PRO_CODIGO = {filtrosPesquisa.CodigoProduto})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoServico > 0)
                where.Append($" and PedidoVenda.PEV_CODIGO in (SELECT PEV_CODIGO FROM T_PEDIDO_VENDA_ITENS WHERE SER_CODIGO = {filtrosPesquisa.CodigoServico})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CnpjFornecedorServico > 0)
                where.Append($" and PedidoVenda.PEV_CODIGO in (SELECT PEV_CODIGO FROM T_PEDIDO_VENDA_ITENS WHERE CLI_CGCCPF = {filtrosPesquisa.CnpjFornecedorServico})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoFuncionarioServico > 0)
                where.Append($" and PedidoVenda.PEV_CODIGO in (SELECT PEV_CODIGO FROM T_PEDIDO_VENDA_ITENS WHERE FUN_CODIGO = {filtrosPesquisa.CodigoFuncionarioServico})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.NumeroInternoInicial > 0)
                where.Append($" AND PedidoVenda.PEV_NUMERO_INTERNO >= {filtrosPesquisa.NumeroInternoInicial}");

            if (filtrosPesquisa.NumeroInternoFinal > 0)
                where.Append($" AND PedidoVenda.PEV_NUMERO_INTERNO <= {filtrosPesquisa.NumeroInternoFinal}");
        }

        #endregion

        #region Métodos Públicos

        public string ObterSqlPesquisaItens(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            sql.Append("select PedidoVenda.PEV_CODIGO CodigoPedido, ");
            sql.Append("       Itens.PVI_CODIGO_ITEM CodigoItem, ");
            sql.Append("       Itens.PVI_DESCRICAO_ITEM DescricaoItem, ");
            sql.Append("       Itens.PVI_QUANTIDADE QuantidadeItem, ");
            sql.Append("       Itens.PVI_VALOR_UNITARIO ValorUnitarioItem, ");
            sql.Append("       Itens.PVI_VALOR_TOTAL ValorTotalItem ");
            sql.Append("  from T_PEDIDO_VENDA_ITENS Itens ");
            sql.Append("  join T_PEDIDO_VENDA PedidoVenda on PedidoVenda.PEV_CODIGO = Itens.PEV_CODIGO ");
            sql.Append(joins.ToString());

            if (where.Length > 0)
                sql.Append($" where {where.ToString().Trim().Substring(3)} ");

            return sql.ToString();
        }

        #endregion
    }
}
