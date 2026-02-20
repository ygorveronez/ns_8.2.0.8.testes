using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro.Consulta
{
    sealed class ConsultaContratoFinanceiro : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro>
    {
        #region Construtores

        public ConsultaContratoFinanceiro() : base(tabela: "T_CONTRATO_FINANCIAMENTO as ContratoFinanciamento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFonecedor(StringBuilder joins)
        {
            if (!joins.Contains(" Fornecedor "))
                joins.Append("JOIN T_CLIENTE Fornecedor on Fornecedor.CLI_CGCCPF = ContratoFinanciamento.CLI_CGCCPF ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = ContratoFinanciamento.EMP_CODIGO ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_CODIGO Codigo, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("ContratoFinanciamento.CFI_DATA_EMISSAO, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_NUMERO Numero, ");
                        groupBy.Append("ContratoFinanciamento.CFI_NUMERO, ");
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_NUMERO_DOCUMENTO NumeroDocumento, ");
                        groupBy.Append("ContratoFinanciamento.CFI_NUMERO_DOCUMENTO, ");
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        select.Append("Fornecedor.CLI_NOME Fornecedor, ");
                        groupBy.Append("Fornecedor.CLI_NOME, ");

                        SetarJoinsFonecedor(joins);
                    }
                    break;

                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA
                                        FROM T_CONTRATO_FINANCIAMENTO_VEICULO ContratoVeiculo
                                        JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ContratoVeiculo.VEI_CODIGO
                                        WHERE ContratoVeiculo.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO FOR XML PATH('')), 3, 1000) Veiculo, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoEntrada":
                    if (!select.Contains(" NumeroDocumentoEntrada, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(Doc.TDE_NUMERO_LONG AS NVARCHAR(20))
                                        FROM T_CONTRATO_FINANCIAMENTO_DOCUMENTO_ENTRADA DocumentoEntrada
                                        JOIN T_TMS_DOCUMENTO_ENTRADA Doc on Doc.TDE_CODIGO = DocumentoEntrada.TDE_CODIGO
                                        WHERE DocumentoEntrada.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO FOR XML PATH('')), 3, 1000) NumeroDocumentoEntrada, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");

                    }
                    break;

                case "QuantidadeParcela":
                    if (!select.Contains(" QuantidadeParcela, "))
                    {
                        select.Append("(SELECT COUNT(1) FROM T_CONTRATO_FINANCIAMENTO_PARCELA Parcela WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) QuantidadeParcela, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");

                    }
                    break;

                case "ValorPagoCapital":
                    if (!select.Contains(" ValorPagoCapital, "))
                    {
                        select.Append(@"(SELECT SUM(Titulo.TIT_VALOR_PAGO) FROM T_CONTRATO_FINANCIAMENTO_PARCELA Parcela
                                         JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = Parcela.TIT_CODIGO
                                         WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) ValorPagoCapital, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");

                    }
                    break;

                case "ValorPagoJuros":
                    if (!select.Contains(" ValorPagoJuros, "))
                    {
                        select.Append(@"(SELECT SUM(Titulo.TIT_VALOR_ACRESCIMO) FROM T_CONTRATO_FINANCIAMENTO_PARCELA Parcela 
                                        JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = Parcela.TIT_CODIGO
                                        WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) ValorPagoJuros, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");

                    }
                    break;

                case "ValorCapital":
                    if (!select.Contains(" ValorCapital, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_VALOR_TOTAL ValorCapital, ");
                        groupBy.Append("ContratoFinanciamento.CFI_VALOR_TOTAL, ");
                    }
                    break;

                case "ValorJuros":
                    if (!select.Contains(" ValorJuros, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_VALOR_ACRESCIMO ValorJuros, ");
                        groupBy.Append("ContratoFinanciamento.CFI_VALOR_ACRESCIMO, ");
                    }
                    break;

                case "SituacaoFormatada":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("ContratoFinanciamento.CFI_SITUACAO Situacao, ");
                        groupBy.Append("ContratoFinanciamento.CFI_SITUACAO, ");
                    }
                    break;
                case "ValorPagoParcela":
                    if(!select.Contains(" ValorPagoParcela, "))
                    {
                        select.Append(@"(SELECT SUM(Parcela.CFP_VALOR) 
                                        from T_CONTRATO_FINANCIAMENTO Contrato
                                        JOIN T_CONTRATO_FINANCIAMENTO_PARCELA Parcela on Parcela.CFI_CODIGO = Contrato.CFI_CODIGO
                                        left outer join T_TITULO Titulo on Titulo.TIT_CODIGO = Parcela.TIT_CODIGO
                                        WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO and Titulo.TIT_STATUS = 3) ValorPagoParcela, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO "))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");
                    }
                    break;
                case "ValorAcrescimo":
                    if (!select.Contains(" ValorAcrescimo, "))
                    {
                        select.Append(@"(SELECT SUM(Parcela.CFP_VALOR_ACRESCIMO) 
                                        from T_CONTRATO_FINANCIAMENTO Contrato
                                        JOIN T_CONTRATO_FINANCIAMENTO_PARCELA Parcela on Parcela.CFI_CODIGO = Contrato.CFI_CODIGO
                                        left outer join T_TITULO Titulo on Titulo.TIT_CODIGO = Parcela.TIT_CODIGO
                                        WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO and Titulo.TIT_STATUS = 3) ValorAcrescimo, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");
                    }
                    break;
                case "ValorAcrescimoTitulo":
                    if (!select.Contains(" ValorAcrescimoTitulo, "))
                    {
                        select.Append(@"(SELECT SUM(Titulo.TIT_ACRESCIMO ) 
                                        from T_CONTRATO_FINANCIAMENTO Contrato
                                        JOIN T_CONTRATO_FINANCIAMENTO_PARCELA Parcela on Parcela.CFI_CODIGO = Contrato.CFI_CODIGO
                                        left outer join T_TITULO Titulo on Titulo.TIT_CODIGO = Parcela.TIT_CODIGO
                                        WHERE Parcela.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO and Titulo.TIT_STATUS = 3) ValorAcrescimoTitulo, ");

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy.Append("ContratoFinanciamento.CFI_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($"AND CAST (ContratoFinanciamento.CFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($"AND CAST (ContratoFinanciamento.CFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "' ");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($"AND ContratoFinanciamento.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                where.Append($"AND ContratoFinanciamento.CFI_NUMERO_DOCUMENTO = '{filtrosPesquisa.NumeroDocumento}' ");

            if (filtrosPesquisa.Numero > 0)
                where.Append($"AND ContratoFinanciamento.CFI_NUMERO = {filtrosPesquisa.Numero} ");

            if (filtrosPesquisa.CpfCnpjFornecedor > 0)
                where.Append($"AND ContratoFinanciamento.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjFornecedor);

            if (filtrosPesquisa.Situacoes?.Count > 0)
                where.Append($"AND ContratoFinanciamento.CFI_SITUACAO IN ({ string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ToString("D"))) }) ");

            if (filtrosPesquisa.CodigosVeiculos.Count > 0)
                where.Append($@" and EXISTS(select ContratoFinanciamento.CFI_CODIGO from T_CONTRATO_FINANCIAMENTO_VEICULO ContratoFinanciamentoVeiculo			
				                where ContratoFinanciamentoVeiculo.VEI_CODIGO IN (" + string.Join(", ", filtrosPesquisa.CodigosVeiculos) + "))");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumentoEntrada))
                where.Append($@" and EXISTS (select ContratoFinanciamentoDocumentoEntrada.CFI_CODIGO from T_CONTRATO_FINANCIAMENTO_DOCUMENTO_ENTRADA ContratoFinanciamentoDocumentoEntrada 
                               JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.TDE_CODIGO = ContratoFinanciamentoDocumentoEntrada.TDE_CODIGO 
                                WHERE ContratoFinanciamento.CFI_CODIGO = DocumentoEntrada.CFI_CODIGO AND DocumentoEntrada.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroDocumentoEntrada + ")");
        }

        #endregion
    }
}
