using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.RH.Consulta
{
    sealed class ConsultaFolhaLancamento : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento>
    {
        #region Construtores

        public ConsultaFolhaLancamento() : base(tabela: "T_FOLHA_LANCAMENTO as FolhaLancamento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append(" join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = FolhaLancamento.FUN_CODIGO ");
        }

        private void SetarJoinsFolhaInformacao(StringBuilder joins)
        {
            if (!joins.Contains(" FolhaInformacao "))
                joins.Append(" join T_FOLHA_INFORMACAO FolhaInformacao on FolhaInformacao.FOI_CODIGO = FolhaLancamento.FOI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("FolhaLancamento.FOL_CODIGO Codigo, ");

                        if (!groupBy.Contains("FolhaLancamento.FOL_CODIGO"))
                            groupBy.Append("FolhaLancamento.FOL_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("FolhaLancamento.FOL_DESCRICAO Descricao, ");
                        groupBy.Append("FolhaLancamento.FOL_DESCRICAO, ");
                    }
                    break;

                case "DataInicialFormatada":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select.Append("FolhaLancamento.FOL_DATA_INICIAL DataInicial, ");
                        groupBy.Append("FolhaLancamento.FOL_DATA_INICIAL, ");
                    }
                    break;

                case "DataFinalFormatada":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select.Append("FolhaLancamento.FOL_DATA_FINAL DataFinal, ");
                        groupBy.Append("FolhaLancamento.FOL_DATA_FINAL, ");
                    }
                    break;

                case "NumeroEvento":
                    if (!select.Contains(" NumeroEvento, "))
                    {
                        select.Append("FolhaLancamento.FOL_NUMERO_EVENTO NumeroEvento, ");
                        groupBy.Append("FolhaLancamento.FOL_NUMERO_EVENTO, ");
                    }
                    break;

                case "NomeFuncionario":
                    if (!select.Contains(" NomeFuncionario, "))
                    {
                        select.Append("Funcionario.FUN_NOME NomeFuncionario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "CpfFuncionarioFormatado":
                    if (!select.Contains(" CpfFuncionario, "))
                    {
                        select.Append("Funcionario.FUN_CPF CpfFuncionario, ");
                        groupBy.Append("Funcionario.FUN_CPF, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "CodigoIntegracaoFuncionario":
                    if (!select.Contains(" CodigoIntegracaoFuncionario, "))
                    {
                        select.Append("Funcionario.FUN_CODIGO_INTEGRACAO CodigoIntegracaoFuncionario, ");
                        groupBy.Append("Funcionario.FUN_CODIGO_INTEGRACAO, ");
                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "InformacaoFolha":
                    if (!select.Contains(" InformacaoFolha,"))
                    {
                        select.Append("FolhaInformacao.FOI_DESCRICAO InformacaoFolha, ");
                        groupBy.Append("FolhaInformacao.FOI_DESCRICAO, ");
                        SetarJoinsFolhaInformacao(joins);
                    }
                    break;

                case "ValorBase":
                    if (!select.Contains(" ValorBase,"))
                    {
                        select.Append("AVG(FolhaLancamento.FOL_BASE) ValorBase, ");
                        //groupBy.Append("FolhaLancamento.FOL_BASE, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor,"))
                    {
                        select.Append("SUM(FolhaLancamento.FOL_VALOR) Valor, ");
                        //groupBy.Append("FolhaLancamento.FOL_VALOR, ");
                    }
                    break;

                case "DataCompetenciaFormatada":
                    if (!select.Contains(" DataCompetencia,"))
                    {
                        select.Append("FolhaLancamento.FOL_DATA_COMPETENCIA DataCompetencia, ");
                        groupBy.Append("FolhaLancamento.FOL_DATA_COMPETENCIA, ");
                    }
                    break;

                case "SituacaoFuncionario":
                case "SituacaoFuncionarioDescricao":
                    if (!select.Contains(" SituacaoFuncionario, "))
                    {
                        select.Append("Funcionario.FUN_STATUS SituacaoFuncionario, ");
                        groupBy.Append("Funcionario.FUN_STATUS, ");
                        SetarJoinsFuncionario(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoFuncionario > 0)
            {
                where.Append($" and Funcionario.FUN_CODIGO = {filtrosPesquisa.CodigoFuncionario} ");
                SetarJoinsFuncionario(joins);
            }

            if (filtrosPesquisa.CodigoInformacaoFolha > 0)
            {
                where.Append($" and FolhaInformacao.FOI_CODIGO = {filtrosPesquisa.CodigoInformacaoFolha} ");
                SetarJoinsFolhaInformacao(joins);
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(FolhaLancamento.FOL_DATA_INICIAL AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}' ");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(FolhaLancamento.FOL_DATA_FINAL AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}' ");

            if (filtrosPesquisa.DataCompetenciaInicial != DateTime.MinValue)
                where.Append($" AND FolhaLancamento.FOL_DATA_COMPETENCIA >= '{filtrosPesquisa.DataCompetenciaInicial.ToString(pattern)}' ");

            if (filtrosPesquisa.DataCompetenciaFinal != DateTime.MinValue)
                where.Append($" AND FolhaLancamento.FOL_DATA_COMPETENCIA <= '{filtrosPesquisa.DataCompetenciaFinal.ToString(pattern)}' ");

            if (!string.IsNullOrEmpty(filtrosPesquisa.SituacaoFuncionario))
            {
                where.Append($" and Funcionario.FUN_STATUS = '{filtrosPesquisa.SituacaoFuncionario}' ");
                SetarJoinsFuncionario(joins);
            }
        }

        #endregion
    }
}
