using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaPacotes : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes>
    {
        #region Construtores

        public ConsultaPacotes() : base(tabela: "T_PACOTE as Pacote") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaPedidoPacote(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedPacote "))
                joins.Append(" left join T_CARGA_PEDIDO_PACOTE as CargaPedPacote on CargaPedPacote.PCT_CODIGO = Pacote.PCT_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedidoPacote(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = CargaPedPacote.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO as Pedido on  Pedido.PED_CODIGO = CargaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA as Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO as TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsContratante(StringBuilder joins)
        {
            if (!joins.Contains(" Contratante "))
                joins.Append(" left join T_CLIENTE as Contratante on Contratante.CLI_CGCCPF = Pacote.CLI_CGCCPF_CONTRATANTE ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append(" left join T_CLIENTE as Origem on Origem.CLI_CGCCPF = Pacote.CLI_CGCCPF_ORIGEM ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            if (!joins.Contains(" Destino "))
                joins.Append(" left join T_CLIENTE as Destino on Destino.CLI_CGCCPF = Pacote.CLI_CGCCPF_DESTINO ");
        }

        private void SetarJoinsCteAnterior(StringBuilder joins)
        {
            if (!joins.Contains(" CteAnterior "))
                joins.Append(" left join T_CTE_TERCEIRO as CteAnterior on Pacote.PCT_LOG_KEY = CteAnterior.CPS_IDENTIFICACAO_PACOTE ");
        }

        private void SetarJoinsCteAnteriorNFe(StringBuilder joins)
        {
            SetarJoinsCteAnterior(joins);

            if (!joins.Contains(" CteAnteriorNFe "))
                joins.Append(" left join T_CTE_TERCEIRO_NFE as CteAnteriorNFe on CteAnteriorNFe.CPS_CODIGO = CteAnterior.CPS_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Pacote.PCT_CODIGO as Codigo, ");
                        groupBy.Append("Pacote.PCT_CODIGO, ");
                    }
                    break;

                case "Pedido":
                    if (!select.Contains(" Pedido,"))
                    {
                        select.Append("Pedido.PED_NUMERO as Pedido, ");
                        groupBy.Append("Pedido.PED_NUMERO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "DataRecebimentoFormatada":
                    if (!select.Contains(" DataRecebimento,"))
                    {
                        select.Append("Pacote.PCT_DATA_RECEBIMENTO as DataRecebimento,  ");
                        groupBy.Append("Pacote.PCT_DATA_RECEBIMENTO, ");
                    }
                    break;

                case "LogKey":
                    if (!select.Contains(" LogKey,"))
                    {
                        select.Append("Pacote.PCT_LOG_KEY as LogKey,  ");
                        groupBy.Append("Pacote.PCT_LOG_KEY, ");

                    }
                    break;

                case "Contratante":
                    if (!select.Contains(" Contratante,"))
                    {
                        select.Append("Contratante.CLI_NOME as Contratante,  ");
                        groupBy.Append("Contratante.CLI_NOME, ");

                        SetarJoinsContratante(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select.Append("Origem.CLI_NOME as Origem,  ");
                        groupBy.Append("Origem.CLI_NOME, ");
                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select.Append("Destino.CLI_NOME as Destino,  ");
                        groupBy.Append("Destino.CLI_NOME, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "Cubagem":
                    if (!select.Contains(" Cubagem,"))
                    {
                        select.Append("Pacote.PCT_CUBAGEM as Cubagem,  ");
                        groupBy.Append("Pacote.PCT_CUBAGEM, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso,"))
                    {
                        select.Append("Pacote.PCT_PESO as Peso,  ");
                        groupBy.Append("Pacote.PCT_PESO, ");
                    }
                    break;

                case "CteAnterior":
                    if (!select.Contains(" CteAnterior, "))
                    {
                        select.Append("CteAnterior.CPS_NUMERO as CteAnterior, ");
                        groupBy.Append("CteAnterior.CPS_NUMERO, ");

                        SetarJoinsCteAnterior(joins);
                    }
                    break;
                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CteAnterior.CPS_CHAVE_ACESSO as ChaveCTe, ");
                        groupBy.Append("CteAnterior.CPS_CHAVE_ACESSO, ");

                        SetarJoinsCteAnterior(joins);
                    }
                    break;
                case "CNPJOrigem":
                    if (!select.Contains(" CNPJOrigem, "))
                    {
                        select.Append("Origem.CLI_CGCCPF as CNPJOrigem,  ");
                        groupBy.Append("Origem.CLI_CGCCPF, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;
                case "CNPJDestino":
                    if (!select.Contains(" CNPJDestino, "))
                    {
                        select.Append("Destino.CLI_CGCCPF as CNPJDestino,  ");
                        groupBy.Append("Destino.CLI_CGCCPF, ");

                        SetarJoinsDestino(joins);
                    }
                    break;
                case "CNPJContratante":
                    if (!select.Contains(" CNPJContratante, "))
                    {
                        select.Append("Contratante.CLI_CGCCPF as CNPJContratante, ");
                        groupBy.Append("Contratante.CLI_CGCCPF, ");

                        SetarJoinsContratante(joins);
                    }
                    break;

                case "ValorCTeAnterior":
                    if (!select.Contains(" ValorCTeAnterior, "))
                    {
                        select.Append("CteAnterior.CPS_VALOR_TOTAL_MERC as ValorCTeAnterior, ");
                        groupBy.Append("CteAnterior.CPS_VALOR_TOTAL_MERC, ");

                        SetarJoinsCteAnterior(joins);
                    }
                    break;

                case "NumeroNFeVinculada":
                    if (!select.Contains(" NumeroNFeVinculada, "))
                    {
                        select.Append("CteAnteriorNFe.CNE_NUMERO as NumeroNFeVinculada, ");
                        groupBy.Append("CteAnteriorNFe.CNE_NUMERO, ");

                        SetarJoinsCteAnteriorNFe(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.Cubagem > 0m)
            {
                where.Append($" and Pacote.PCT_CUBAGEM = {filtrosPesquisa.Cubagem.ToString().Replace(".", "").Replace(",", ".")}");
            }

            if (filtrosPesquisa.Peso > 0m)
            {
                where.Append($" and Pacote.PCT_PESO= {filtrosPesquisa.Peso.ToString().Replace(".", "").Replace(",", ".")}");
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.LogKey))
            {
                where.Append($" and Pacote.PCT_LOG_KEY = '{filtrosPesquisa.LogKey}'");
            }

            if (filtrosPesquisa.CodigoPedido > 0)
            {
                where.Append($" and Pedido.PED_CODIGO = '{filtrosPesquisa.CodigoPedido}'");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataRecebimentoInicial != DateTime.MinValue)
                where.Append($" and Pacote.PCT_DATA_RECEBIMENTO >= '{filtrosPesquisa.DataRecebimentoInicial.ToString("yyyy/MM/dd HH:mm")}'");

            if (filtrosPesquisa.DataRecebimentoFinal != DateTime.MinValue)
                where.Append($" and Pacote.PCT_DATA_RECEBIMENTO <= '{filtrosPesquisa.DataRecebimentoFinal.ToString("yyyy/MM/dd HH:mm")}'");

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                where.Append($" and Carga.CAR_CODIGO = '{filtrosPesquisa.CodigoCarga}'");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($" and TipoOperacao.TOP_CODIGO = '{filtrosPesquisa.CodigoTipoOperacao}'");

                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.CodigoContratante > 0d)
            {
                where.Append($" and Contratante.CLI_CGCCPF = '{filtrosPesquisa.CodigoContratante}'");

                SetarJoinsContratante(joins);
            }

            if (filtrosPesquisa.CodigoOrigem > 0d)
            {
                where.Append($" and Origem.CLI_CGCCPF = '{filtrosPesquisa.CodigoOrigem}'");

                SetarJoinsOrigem(joins);
            }

            if (filtrosPesquisa.CodigoDestino > 0d)
            {
                where.Append($" and Destino.CLI_CGCCPF = '{filtrosPesquisa.CodigoDestino}'");

                SetarJoinsDestino(joins);
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.Append($" and CteAnterior.CPS_NUMERO = '{filtrosPesquisa.NumeroCTe}'");

                SetarJoinsCteAnterior(joins);
            }

            if (filtrosPesquisa.ChaveCTe.Length > 0)
            {
                where.Append($" and CteAnterior.CPS_CHAVE_ACESSO = '{filtrosPesquisa.ChaveCTe}'");

                SetarJoinsCteAnterior(joins);
            }

        }
    }
}
