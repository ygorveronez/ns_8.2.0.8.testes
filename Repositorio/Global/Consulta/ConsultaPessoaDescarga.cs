using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaPessoaDescarga : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga>
    {
        #region Construtores

        public ConsultaPessoaDescarga() : base(tabela: "T_CLIENTE_DESCARGA as ClienteDescarga") { }

        #endregion

        #region Métodos Privados
        private void SetarJoinsPessoaOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PessoaOrigem "))
                joins.Append(" LEFT JOIN T_CLIENTE PessoaOrigem ON PessoaOrigem.CLI_CGCCPF = ClienteDescarga.CLI_CGCCPF_ORIGEM ");
        }

        private void SetarJoinsPessoaDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PessoaDestino "))
                joins.Append(" LEFT JOIN T_CLIENTE PessoaDestino ON PessoaDestino.CLI_CGCCPF = ClienteDescarga.CLI_CGCCPF ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtroPesquisa)
        {
            if (!groupBy.Contains("ClienteDescarga.CLD_CODIGO"))
                groupBy.Append("ClienteDescarga.CLD_CODIGO, ");

            switch (propriedade)
            {
                case "PessoaOrigem":
                    if (!select.Contains(" PessoaOrigem, "))
                    {
                        select.Append("PessoaOrigem.CLI_NOME as PessoaOrigem, ");
                        groupBy.Append("PessoaOrigem.CLI_NOME, ");

                        select.Append("PessoaOrigem.CLI_FISJUR as TipoPessoaOrigem, ");
                        groupBy.Append("PessoaOrigem.CLI_FISJUR, ");
                    }

                    SetarJoinsPessoaOrigem(joins);

                    break;
                case "CnpjOrigemFormatado":
                    if (!select.Contains(" CnpjOrigem, "))
                    {
                        select.Append("PessoaOrigem.CLI_CGCCPF as CnpjOrigem, ");
                        groupBy.Append("PessoaOrigem.CLI_CGCCPF, ");
                    }

                    SetarJoinsPessoaOrigem(joins);

                    break;

                case "PessoaDestino":
                    if (!select.Contains(" PessoaDestino, "))
                    {
                        select.Append("PessoaDestino.CLI_NOME as PessoaDestino, ");
                        groupBy.Append("PessoaDestino.CLI_NOME, ");

                        select.Append("PessoaDestino.CLI_FISJUR as TipoPessoaDestino, ");
                        groupBy.Append("PessoaDestino.CLI_FISJUR, ");
                    }

                    SetarJoinsPessoaDestino(joins);

                    break;
                case "CnpjDestinoFormatado":
                    if (!select.Contains(" CnpjDestino, "))
                    {
                        select.Append("PessoaDestino.CLI_CGCCPF as CnpjDestino, ");
                        groupBy.Append("PessoaDestino.CLI_CGCCPF, ");
                    }

                    SetarJoinsPessoaDestino(joins);

                    break;

                case "HoraInicio":
                    if (!select.Contains(" HoraInicio, "))
                    {
                        select.Append("ClienteDescarga.CLD_HORA_INICIO_DESCARGA as HoraInicio, ");
                        groupBy.Append("ClienteDescarga.CLD_HORA_INICIO_DESCARGA, ");
                    }
                    break;

                case "HoraFim":
                    if (!select.Contains(" HoraFim, "))
                    {
                        select.Append("ClienteDescarga.CLD_HORA_LIMETE_DESCARGA as HoraFim, ");
                        groupBy.Append("ClienteDescarga.CLD_HORA_LIMETE_DESCARGA, ");
                    }
                    break;

                case "ValorPorPallet":
                    if (!select.Contains(" ValorPorPallet, "))
                    {
                        select.Append("ClienteDescarga.CLD_VALOR_FIXO as ValorPorPallet, ");
                        groupBy.Append("ClienteDescarga.CLD_VALOR_FIXO, ");
                    }

                    break;

                case "ValorPorVolume":
                    if (!select.Contains(" ValorPorVolume, "))
                    {
                        select.Append("ClienteDescarga.CLD_VALOR_POR_VOLUME as ValorPorVolume, ");
                        groupBy.Append("ClienteDescarga.CLD_VALOR_POR_VOLUME, ");
                    }
                    break;

                case "DeixaReboqueParaDescargaFormatada":
                    if (!select.Contains(" DeixaReboqueParaDescarga, "))
                    {
                        select.Append("ClienteDescarga.CLD_DEIXAR_REBOQUE_PARA_DESCARGA as DeixaReboqueParaDescarga, ");
                        groupBy.Append("ClienteDescarga.CLD_DEIXAR_REBOQUE_PARA_DESCARGA, ");
                    }

                    break;

                default:
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.PessoaOrigem > 0d)
                where.Append($" AND ClienteDescarga.CLI_CGCCPF_ORIGEM = " + filtrosPesquisa.PessoaOrigem.ToString());

            if (filtrosPesquisa.PessoaDestino > 0d)
                where.Append($" AND ClienteDescarga.CLI_CGCCPF = " + filtrosPesquisa.PessoaDestino.ToString());

            if (filtrosPesquisa.PessoaDeixaReboqueParaDescarga)
                where.Append($" AND CLD_DEIXAR_REBOQUE_PARA_DESCARGA = 1");

        }

        #endregion
    }
}
