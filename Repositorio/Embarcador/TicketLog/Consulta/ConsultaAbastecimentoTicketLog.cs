using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.TicketLog.Consulta
{
    sealed class ConsultaAbastecimentoTicketLog : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog>
    {
        #region Construtores

        public ConsultaAbastecimentoTicketLog() : base(tabela: "T_ABASTECIMENTO_TICKETLOG as AbastecimentoTicketLog") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsAbastecimento(StringBuilder joins)
        {
            if (!joins.Contains(" Abastecimento "))
                joins.Append(" left join T_ABASTECIMENTO Abastecimento on AbastecimentoTicketLog.ABA_CODIGO = Abastecimento.ABA_CODIGO_ABASTECIMENTO_TICKETLOG ");
        }
        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsAbastecimento(joins);

            if (!joins.Contains(" TabelaVeiculo "))
                joins.Append(" left join T_VEICULO TabelaVeiculo on Abastecimento.VEI_CODIGO = TabelaVeiculo.VEI_CODIGO ");
        }
        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsAbastecimento(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append(" left join T_CLIENTE Cliente on Abastecimento.CLI_CGCCPF = Cliente.CLI_CGCCPF ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtroPesquisa)
        {

            if (!select.Contains(" Codigo,"))
            {
                select.Append("AbastecimentoTicketLog.ABA_CODIGO as Codigo, ");
                groupBy.Append("AbastecimentoTicketLog.ABA_CODIGO, ");
            }
            switch (propriedade)
            {
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("Abastecimento.ABA_SITUACAO as Situacao, ");
                        groupBy.Append("Abastecimento.ABA_SITUACAO, ");

                        SetarJoinsAbastecimento(joins);
                    }
                    break;

                case "CodigoTransacao":
                    if (!select.Contains(" CodigoTransacao,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_CODIGO_TRANSACAO as CodigoTransacao, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_CODIGO_TRANSACAO, ");
                    }
                    break;

                case "ValorTransacao":
                    if (!select.Contains(" ValorTransacao,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_VALOR_TRANSACAO_DECIMAL as ValorTransacao, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_VALOR_TRANSACAO_DECIMAL, ");
                    }
                    break;

                case "DataTransacaoFormatada":
                    if (!select.Contains(" DataTransacao,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_DATA_TRANSACAO as DataTransacao, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_DATA_TRANSACAO, ");
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor,"))
                    {
                        select.Append(" Cliente.CLI_NOME as Fornecedor, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "CNPJFornecedorFormatado":
                    if (!select.Contains(" CNPJFornecedor,"))
                    {
                        select.Append("Cliente.CLI_CGCCPF as CNPJFornecedor, ");
                        groupBy.Append("Cliente.CLI_CGCCPF, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "Litros":
                    if (!select.Contains(" Litros,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_LITROS_DECIMAL as Litros, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_LITROS_DECIMAL, ");
                    }
                    break;

                case "TipoCombustivel":
                    if (!select.Contains(" TipoCombustivel,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_TIPO_COMBUSTIVEL as TipoCombustivel, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_TIPO_COMBUSTIVEL, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select.Append("TabelaVeiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("TabelaVeiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ValorLitro":
                    if (!select.Contains(" ValorLitro,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_VALOR_LITRO_DECIMAL as ValorLitro, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_VALOR_LITRO_DECIMAL, ");
                    }
                    break;

                case "Quilometragem":
                    if (!select.Contains(" Quilometragem,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_QUILOMETRAGEM_INT as Quilometragem, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_QUILOMETRAGEM_INT, ");
                    }
                    break;

                case "DataIntegracaoFormatada":
                    if (!select.Contains(" DataIntegracao,"))
                    {
                        select.Append("AbastecimentoTicketLog.ABA_DATA_INTEGRACAO as DataIntegracao, ");
                        groupBy.Append("AbastecimentoTicketLog.ABA_DATA_INTEGRACAO, ");
                    }
                    break;

                case "CodigoAbastecimento":
                    if (!select.Contains(" CodigoAbastecimento,"))
                    {
                        select.Append("Abastecimento.ABA_CODIGO_ABASTECIMENTO_TICKETLOG as CodigoAbastecimento, ");
                        groupBy.Append("Abastecimento.ABA_CODIGO_ABASTECIMENTO_TICKETLOG, ");

                        SetarJoinsAbastecimento(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" AND AbastecimentoTicketLog.ABA_DATA_TRANSACAO >= '{filtrosPesquisa.DataInicial.ToString("dd-MM-yyyy")}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" AND AbastecimentoTicketLog.ABA_DATA_TRANSACAO <= '{filtrosPesquisa.DataFinal.ToString("dd-MM-yyyy")}'");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" AND TabelaVeiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.CNPJFornecedor > 0d)
            {
                where.Append($" AND Cliente.CLI_CGCCPF = {filtrosPesquisa.CNPJFornecedor}");
                SetarJoinsCliente(joins);
            }

            if (filtrosPesquisa.CodigoTransacao > 0)
            {
                where.Append($" AND AbastecimentoTicketLog.ABA_CODIGO_TRANSACAO = {filtrosPesquisa.CodigoTransacao}");
            }
        }

    }

}
