using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaMovimentoFinanceiro : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro>
    {
        #region Construtores

        public ConsultaMovimentoFinanceiro() : base(tabela: "T_MOVIMENTO_FINANCEIRO as MovimentoFinanceiro") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append(" LEFT OUTER JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = MovimentoFinanceiro.CLI_CGCCPF ");
        }

        private void SetarJoinsPlanoConta(StringBuilder joins)
        {
            if (!joins.Contains(" PlanoDebito "))
                joins.Append(" LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoDebito ON PlanoDebito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_DEBITO ");

            if (!joins.Contains(" PlanoCredito "))
                joins.Append(" LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoCredito ON PlanoCredito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_CREDITO ");
        }

        private void SetarJoinsTipoMovimento(StringBuilder joins)
        {
            if (!joins.Contains(" TipoMovimento "))
                joins.Append(" LEFT JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = MovimentoFinanceiro.TIM_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" LEFT JOIN T_GRUPO_PESSOAS GrupoPessoa ON GrupoPessoa.GRP_CODIGO = MovimentoFinanceiro.GRP_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" LEFT JOIN T_FUNCIONARIO Usuario ON Usuario.FUN_CODIGO = MovimentoFinanceiro.FUN_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = MovimentoFinanceiro.CRE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_CODIGO as Codigo, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_CODIGO, ");
                    }
                    break;
                case "TipoMovimento":
                    if (!select.Contains(" TipoMovimento, "))
                    {
                        select.Append("TipoMovimento.TIM_DESCRICAO as TipoMovimento, ");
                        groupBy.Append("TipoMovimento.TIM_DESCRICAO, ");

                        SetarJoinsTipoMovimento(joins);
                    }
                    break;
                case "PlanoDebito":
                    if (!select.Contains(" PlanoDebito, "))
                    {
                        select.Append("PlanoDebito.PLA_DESCRICAO as PlanoDebito, ");
                        groupBy.Append("PlanoDebito.PLA_DESCRICAO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "PlanoCredito":
                    if (!select.Contains(" PlanoCredito, "))
                    {
                        select.Append("PlanoCredito.PLA_DESCRICAO as PlanoCredito, ");
                        groupBy.Append("PlanoCredito.PLA_DESCRICAO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "ValorMovimento":
                    if (!select.Contains(" ValorMovimento, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_VALOR as ValorMovimento, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_VALOR, ");

                    }
                    break;
                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        select.Append("Pessoa.CLI_NOME as Pessoa, ");
                        groupBy.Append("Pessoa.CLI_NOME, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;
                case "Data":
                    if (!select.Contains(" Data, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_DATA as Data, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_DATA, ");
                    }
                    break;
                case "DataBaseFinanceiro":
                    if (!select.Contains(" DataBaseFinanceiro, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_DATA_BASE as DataBaseFinanceiro, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_DATA_BASE, ");
                    }
                    break;
                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_DOCUMENTO as NumeroDocumento, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_DOCUMENTO, ");
                    }
                    break;
                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO as GrupoPessoa, ");
                        groupBy.Append("GrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoa(joins);
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("MovimentoFinanceiro.MOV_OBSERVACAO as Observacao, ");
                        groupBy.Append("MovimentoFinanceiro.MOV_OBSERVACAO, ");
                    }
                    break;
                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Usuario.FUN_NOME as Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO as CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;
                case "ContaPlanoDebito":
                    if (!select.Contains(" ContaPlanoDebito, "))
                    {
                        select.Append("PlanoDebito.PLA_PLANO as ContaPlanoDebito, ");
                        groupBy.Append("PlanoDebito.PLA_PLANO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "CodigoPlanoDebito":
                    if (!select.Contains(" CodigoPlanoDebito, "))
                    {
                        select.Append("PlanoDebito.PLA_CODIGO as CodigoPlanoDebito, ");
                        groupBy.Append("PlanoDebito.PLA_CODIGO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "ContaPlanoCredito":
                    if (!select.Contains(" ContaPlanoCredito, "))
                    {
                        select.Append("PlanoCredito.PLA_PLANO as ContaPlanoCredito, ");
                        groupBy.Append("PlanoCredito.PLA_PLANO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "CodigoPlanoCredito":
                    if (!select.Contains(" CodigoPlanoCredito, "))
                    {
                        select.Append("PlanoCredito.PLA_CODIGO as CodigoPlanoCredito, ");
                        groupBy.Append("PlanoCredito.PLA_CODIGO, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;
                case "ContaFornecedorEBS":
                    if (!select.Contains(" ContaFornecedorEBS, "))
                    {
                        select.Append("Pessoa.CLI_CONTA_FORNECEDOR_EBS as ContaFornecedorEBS, ");
                        groupBy.Append("Pessoa.CLI_CONTA_FORNECEDOR_EBS, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;
                default:
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            if (filtrosPesquisa.CentroResultado > 0)
            {
                where.Append($" AND CentroResultado.CRE_CODIGO = {filtrosPesquisa.CentroResultado}");
                SetarJoinsCentroResultado(joins);
            }

            if (filtrosPesquisa.GrupoPessoa > 0)
            {
                where.Append($" AND GrupoPessoa.GRP_CODIGO = {filtrosPesquisa.GrupoPessoa}");
                SetarJoinsGrupoPessoa(joins);
            }

            if (filtrosPesquisa.DataMovimentoInicial != DateTime.MinValue)
                where.Append($" AND CAST(MovimentoFinanceiro.MOV_DATA AS DATE) >= '{filtrosPesquisa.DataMovimentoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataMovimentoFinal != DateTime.MinValue)
                where.Append($" AND CAST(MovimentoFinanceiro.MOV_DATA AS DATE) <= '{filtrosPesquisa.DataMovimentoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataBaseFinanceiro != DateTime.MinValue)
                where.Append($" AND CAST(MovimentoFinanceiro.MOV_DATA_BASE AS DATE) <= '{filtrosPesquisa.DataBaseFinanceiro.ToString(pattern)}'");

            if (filtrosPesquisa.Codigo > 0)
                where.Append($" AND MovimentoFinanceiro.MOV_CODIGO = {filtrosPesquisa.Codigo.ToString()}");

            if (filtrosPesquisa.PlanoDebito > 0)
                where.Append($" AND MovimentoFinanceiro.PLA_CODIGO_DEBITO = {filtrosPesquisa.PlanoDebito.ToString()}");

            if (filtrosPesquisa.PlanoCredito > 0)
                where.Append($" AND MovimentoFinanceiro.PLA_CODIGO_CREDITO = {filtrosPesquisa.PlanoCredito.ToString()}");

            if (filtrosPesquisa.ValorMovimento > 0)
                where.Append($" AND MovimentoFinanceiro.MOV_VALOR = {filtrosPesquisa.ValorMovimento.ToString("g", cultura)}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                where.Append($" AND MovimentoFinanceiro.MOV_DOCUMENTO = '{filtrosPesquisa.NumeroDocumento}'");

            if (filtrosPesquisa.TipoMovimento > 0)
            {
                where.Append($" AND TipoMovimento.TIM_CODIGO = {filtrosPesquisa.TipoMovimento.ToString()}");
                SetarJoinsTipoMovimento(joins);
            }

            if (filtrosPesquisa.Pessoa > 0)
            {
                where.Append($" AND Pessoa.CLI_CGCCPF = {filtrosPesquisa.Pessoa.ToString()}");
                SetarJoinsPessoa(joins);
            }

            if (!filtrosPesquisa.VisualizarTitulosPagamentoSalario)
                where.Append($" AND (MovimentoFinanceiro.MOV_FORMA_TITULO <> {FormaTitulo.PagamentoSalario.ToString("d")} OR MovimentoFinanceiro.MOV_FORMA_TITULO IS NULL)");
        }

        #endregion
    }
}
