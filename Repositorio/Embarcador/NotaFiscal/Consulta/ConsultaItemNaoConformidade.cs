using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.NotaFiscal
{
    sealed class ConsultaItemNaoConformidade : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade>
    {
        #region Construtores

        public ConsultaItemNaoConformidade(bool somenteRegistrosDistintos = false) : base(tabela: "T_ITEM_NAO_CONFORMIDADE as inc", somenteRegistrosDistintos: somenteRegistrosDistintos) { }

        #endregion Construtores

        #region Métodos Privados

        private void SetarJoinsItemNaoConformidadeParticipante(StringBuilder joins)
        {
            if (!joins.Contains(" ItemNaoConformidadeParticipante "))
                joins.Append(" left join T_ITEM_NAO_CONFORMIDADE_PARTICIPANTE itemNaoConformidadeParticipante on inc.INC_CODIGO = itemNaoConformidadeParticipante.INC_CODIGO ");
        }

        private void SetarJoinsItemNaoConformidadeTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" itemNaoConformidadeTipoOperacao "))
                joins.Append(" left join T_ITEM_NAO_CONFORMIDADE_TIPOOPERACAO itemNaoConformidadeTipoOperacao on inc.INC_CODIGO = itemNaoConformidadeTipoOperacao.INC_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsItemNaoConformidadeTipoOperacao(joins);

            if (!joins.Contains(" tipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.TOP_CODIGO = itemNaoConformidadeTipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinsItemNaoConformidadeFilial(StringBuilder joins)
        {
            if (!joins.Contains(" itemNaoConformidadeFilial "))
                joins.Append(" left join T_ITEM_NAO_CONFORMIDADE_FILIAL itemNaoConformidadeFilial on inc.INC_CODIGO = itemNaoConformidadeFilial.INC_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsItemNaoConformidadeFilial(joins);

            if (!joins.Contains(" filial "))
                joins.Append(" left join T_FILIAL filial on filial.FIL_CODIGO = itemNaoConformidadeFilial.FIL_CODIGO ");
        }

        private void SetarJoinsItemNaoConformidadeFornecedor(StringBuilder joins)
        {
            if (!joins.Contains(" itemNaoConformidadeFornecedor "))
                joins.Append(" left join T_ITEM_NAO_CONFORMIDADE_FORNECEDOR itemNaoConformidadeFornecedor on inc.INC_CODIGO = itemNaoConformidadeFornecedor.INC_CODIGO ");
        }

        private void SetarJoinsFornecedor(StringBuilder joins)
        {
            SetarJoinsItemNaoConformidadeFornecedor(joins);

            if (!joins.Contains(" fornecedor "))
                joins.Append(" left join T_CLIENTE fornecedor on fornecedor.CLI_CGCCPF = itemNaoConformidadeFornecedor.CLI_CGCCPF ");
        }

        private void SetarJoinsItemNaoConformidadeCFOP(StringBuilder joins)
        {
            if (!joins.Contains(" itemNaoConformidadeCFOP "))
                joins.Append(" left join T_ITEM_NAO_CONFORMIDADE_CFOP itemNaoConformidadeCFOP on itemNaoConformidadeCFOP.INC_CODIGO = inc.INC_CODIGO ");
        }

        private void SetarJoinsCFOP(StringBuilder joins)
        {
            SetarJoinsItemNaoConformidadeCFOP(joins);

            if (!joins.Contains(" cfop "))
                joins.Append(" left join T_CFOP cfop on cfop.CFO_CODIGO = itemNaoConformidadeCFOP.CFO_CODIGO ");
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa)
        {
            if (!select.Contains("Codigo, "))
            {
                select.Append("inc.INC_CODIGO Codigo, ");

                if (!groupBy.Contains("inc.INC_CODIGO,"))
                    groupBy.Append("inc.INC_CODIGO, ");
            }

            switch (propriedade)
            {

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("inc.INC_DESCRICAO Descricao, ");
                        groupBy.Append("inc.INC_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("inc.INC_CODIGO_INTEGRACAO CodigoIntegracao, ");
                        groupBy.Append("inc.INC_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "Situacao":
                case "SituacaoFormatada":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("inc.INC_STATUS Situacao, ");
                        groupBy.Append("inc.INC_STATUS, ");
                    }
                    break;

                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select.Append("inc.INC_NOTA_FISCAL NotaFiscal, ");
                        groupBy.Append("inc.INC_NOTA_FISCAL, ");
                    }
                    break;

                case "Grupo":
                case "GrupoFormatado":
                    if (!select.Contains(" Grupo, "))
                    {
                        select.Append("inc.INC_GRUPO Grupo, ");
                        groupBy.Append("inc.INC_GRUPO, ");
                    }
                    break;

                case "SubGrupo":
                case "SubGrupoFormatado":
                    if (!select.Contains(" SubGrupo, "))
                    {
                        select.Append("inc.INC_SUBGRUPO SubGrupo, ");
                        groupBy.Append("inc.INC_SUBGRUPO, ");
                    }
                    break;

                case "Area":
                case "AreaFormatada":
                    if (!select.Contains(" Area, "))
                    {
                        select.Append("inc.INC_AREA Area, ");
                        groupBy.Append("inc.INC_AREA, ");
                    }
                    break;

                case "IrrelevanteNaoConformidade":
                case "IrrelevanteNaoConformidadeFormatado":
                    if (!select.Contains(" IrrelevanteNaoConformidade, "))
                    {
                        select.Append("inc.INC_IRRELEVANTE_PARA_NC IrrelevanteNaoConformidade, ");
                        groupBy.Append("inc.INC_IRRELEVANTE_PARA_NC, ");
                    }
                    break;

                case "PermiteContingencia":
                case "PermiteContingenciaFormatado":
                    if (!select.Contains(" PermiteContingencia, "))
                    {
                        select.Append("inc.INC_PERMITE_CONTINGENCIA PermiteContingencia, ");
                        groupBy.Append("inc.INC_PERMITE_CONTINGENCIA, ");
                    }
                    break;

                case "TipoRegra":
                case "TipoRegraFormatado":
                    if (!select.Contains(" TipoRegra, "))
                    {
                        select.Append("inc.INC_TIPO_REGRA TipoRegra, ");
                        groupBy.Append("inc.INC_TIPO_REGRA, ");
                    }
                    break;

                case "Participante":
                case "ParticipanteFormatado":
                    if (!select.Contains(" Participante, "))
                    {
                        SetarJoinsItemNaoConformidadeParticipante(joins);

                        select.Append("itemNaoConformidadeParticipante.INP_PARTICIPANTE Participante, ");
                        groupBy.Append("itemNaoConformidadeParticipante.INP_PARTICIPANTE, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        SetarJoinsTipoOperacao(joins);

                        select.Append("tipooperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("tipooperacao.TOP_DESCRICAO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        SetarJoinsFilial(joins);

                        select.Append("filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("filial.FIL_DESCRICAO, ");
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        SetarJoinsFornecedor(joins);

                        select.Append("fornecedor.CLI_NOME Fornecedor, ");
                        groupBy.Append("fornecedor.CLI_NOME, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP, "))
                    {
                        SetarJoinsCFOP(joins);

                        select.Append("cfop.CFO_CFOP CFOP, ");
                        groupBy.Append("cfop.CFO_CFOP, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
            {
                where.Append($" and inc.INC_DESCRICAO = '{filtrosPesquisa.Descricao}'");
            }

            if (filtrosPesquisa.Situacao.HasValue)
            {
                where.Append($" and inc.INC_STATUS = {filtrosPesquisa.Situacao.Value.GetHashCode()}");
            }

            if (filtrosPesquisa.Grupo.HasValue)
            {
                where.Append($" and inc.INC_GRUPO = {(int)filtrosPesquisa.Grupo.Value}");
            }

            if (filtrosPesquisa.SubGrupo.HasValue)
            {
                where.Append($" and inc.INC_SUBGRUPO = {(int)filtrosPesquisa.SubGrupo.Value}");
            }

            if (filtrosPesquisa.Area.HasValue)
            {
                where.Append($" and inc.INC_AREA = {(int)filtrosPesquisa.Area.Value}");
            }

            if (filtrosPesquisa.IrrelevanteParaNC.HasValue)
            {
                where.Append($" and inc.INC_IRRELEVANTE_PARA_NC = {filtrosPesquisa.IrrelevanteParaNC.Value.GetHashCode()}");
            }

            if (filtrosPesquisa.PermiteContingencia.HasValue)
            {
                where.Append($" and inc.INC_IRRELEVANTE_PARA_NC = {filtrosPesquisa.PermiteContingencia.Value.GetHashCode()}");
            }
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}
