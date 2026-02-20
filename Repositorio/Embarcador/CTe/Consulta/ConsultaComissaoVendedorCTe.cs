using Dominio.ObjetosDeValor.Embarcador.CTe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaComissaoVendedorCTe : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioComissaoVendedorCTe>
    {
        #region Construtores

        public ConsultaComissaoVendedorCTe() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append(" JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" JOIN T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Cliente.GRP_CODIGO ");
        }

        private void SetarJoinsVendedor(StringBuilder joins)
        {
            SetarJoinsGrupoPessoa(joins);

            if (!joins.Contains(" Vendedor "))
                joins.Append(" JOIN T_GRUPO_PESSOAS_FUNCIONARIO Vendedor on Vendedor.GRP_CODIGO = GrupoPessoa.GRP_CODIGO ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsVendedor(joins);

            if (!joins.Contains(" Funcionario "))
                joins.Append(" JOIN T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = Vendedor.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "Remetente":
                    SetarSelect("NomeRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("CNPJRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "Destinatario":
                    SetarSelect("NomeDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("CNPJDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "Tomador":
                    SetarSelect("NomeTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("CNPJTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "NomeRemetente":
                    if (!select.Contains(" NomeRemetente, "))
                    {
                        select.Append("Remetente.PCT_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("Remetente.PCT_CPF_CNPJ CNPJRemetente, ");
                        groupBy.Append("Remetente.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "NomeDestinatario":
                    if (!select.Contains(" NomeDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME NomeDestinatario, ");
                        groupBy.Append("Destinatario.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_CPF_CNPJ CNPJDestinatario, ");
                        groupBy.Append("Destinatario.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NomeTomador":
                    if (!select.Contains(" NomeTomador, "))
                    {
                        select.Append("Tomador.PCT_NOME NomeTomador, ");
                        groupBy.Append("Tomador.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CNPJTomador":
                    if (!select.Contains(" CNPJTomador, "))
                    {
                        select.Append("Tomador.PCT_CPF_CNPJ CNPJTomador, ");
                        groupBy.Append("Tomador.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO GrupoPessoa, ");
                        groupBy.Append("GrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoa(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("SUM(CTe.CON_VALOR_FRETE) ValorFrete, ");
                    }
                    break;

                case "ValorAReceber":
                    if (!select.Contains(" ValorAReceber, "))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorAReceber, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");
                    }
                    break;

                case "PercentualComissao":
                    if (!select.Contains(" PercentualComissao, "))
                    {
                        select.Append("Vendedor.GPF_PERCENTUAL_COMISSAO PercentualComissao, ");
                        groupBy.Append("Vendedor.GPF_PERCENTUAL_COMISSAO, ");

                        SetarJoinsVendedor(joins);
                    }
                    break;

                case "Vendedor":
                    if (!select.Contains(" Vendedor, "))
                    {
                        select.Append("Funcionario.FUN_NOME Vendedor, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "ValorComissao":
                    if (!select.Contains(" ValorComissao, "))
                    {
                        select.Append("SUM((CTe.CON_VALOR_FRETE * (Vendedor.GPF_PERCENTUAL_COMISSAO / 100))) ValorComissao, ");

                        SetarJoinsVendedor(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND CTE.CON_STATUS = 'A' AND CTE.CON_TIPO_SERVICO = 0 ");

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append($" AND CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append($" AND CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
            {
                where.Append($" AND GrupoPessoa.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoa} ");
                SetarJoinsGrupoPessoa(joins);
            }

            if (filtrosPesquisa.CodigoVendedor > 0)
            {
                where.Append($" AND Funcionario.FUN_CODIGO = {filtrosPesquisa.CodigoVendedor} ");
                SetarJoinsFuncionario(joins);
            }
        }

        #endregion
    }
}
