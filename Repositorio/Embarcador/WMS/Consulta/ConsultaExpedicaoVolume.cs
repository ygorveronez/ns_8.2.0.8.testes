using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaExpedicaoVolume : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume>
    {
        #region Construtores

        public ConsultaExpedicaoVolume() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedidoDocumento(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CargaPedidoDocumento "))
                joins.Append(" left join T_CARGA_PEDIDO_DOCUMENTO_CTE CargaPedidoDocumento on CargaPedidoDocumento.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsCargaPedidoDocumento(joins);

            if (!joins.Contains(" CTE "))
                joins.Append(" left join T_CTE CTE on CTE.CON_CODIGO = CargaPedidoDocumento.CON_CODIGO ");
        }

        private void SetarJoinsCTeParticipanteRemetente(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CTeParticipanteRemetente "))
                joins.Append(" left join T_CTE_PARTICIPANTE CTeParticipanteRemetente on CTeParticipanteRemetente.PCT_CODIGO = CTE.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsCTeParticipanteDestinatario(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CTeParticipanteDestinatario "))
                joins.Append(" left join T_CTE_PARTICIPANTE CTeParticipanteDestinatario on CTeParticipanteDestinatario.PCT_CODIGO = CTE.CON_DESTINATARIO_CTE ");
        }

        private void SetarJoinsCTeXMLNotasFiscais(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CTeXMLNotasFiscais "))
                joins.Append(" left join T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscais on CTeXMLNotasFiscais.CON_CODIGO = CTE.CON_CODIGO ");
        }

        private void SetarJoinsXMLNotaFiscal(StringBuilder joins)
        {
            SetarJoinsCTeXMLNotasFiscais(joins);

            if (!joins.Contains(" XMLNotaFiscal "))
                joins.Append(" left join T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscais.NFX_CODIGO and  XMLNotaFiscal.NF_ATIVA = 1 ");
        }

        private void SetarJoinsCargaControleExpedicao(StringBuilder joins)
        {
            if (!joins.Contains(" CargaControleExpedicao "))
                joins.Append(" left join T_CARGA_CONTROLE_EXPEDICAO CargaControleExpedicao on Carga.CAR_CODIGO = CargaControleExpedicao.CAR_CODIGO ");
        }

        private void SetarJoinsConferenciaSeparacao(StringBuilder joins)
        {
            SetarJoinsCargaControleExpedicao(joins);
            SetarJoinsXMLNotaFiscal(joins);
            SetarJoinsCTeParticipanteRemetente(joins);

            if (!joins.Contains(" ConferenciaSeparacao "))
                joins.Append(@" left join T_CONFERENCIA_SEPARACAO ConferenciaSeparacao on ConferenciaSeparacao.CCX_CODIGO = CargaControleExpedicao.CCX_CODIGO and ConferenciaSeparacao.COS_TIPO_RECEBIMENTO = 2 AND 
				                ConferenciaSeparacao.COS_CODIGO_BARRAS LIKE 
                                CASE
	                                WHEN XMLNotaFiscal.NF_NUMERO_SOLICITACAO IS NULL OR XMLNotaFiscal.NF_NUMERO_SOLICITACAO = '' THEN REPLICATE('0', 14 - LEN(CTeParticipanteRemetente.PCT_CPF_CNPJ)) + RTrim(CTeParticipanteRemetente.PCT_CPF_CNPJ) + REPLICATE('0', 9 - LEN(XMLNotaFiscal.NF_NUMERO)) + RTrim(XMLNotaFiscal.NF_NUMERO) + '%'
	                                ELSE XMLNotaFiscal.NF_NUMERO_SOLICITACAO + '%'
                                END");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsCargaControleExpedicao(joins);

            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = CargaControleExpedicao.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtroPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroNota":
                    if (!select.Contains(" NumeroNota, "))
                    {
                        select.Append("CAST(XMLNotaFiscal.NF_NUMERO AS VARCHAR(30)) as NumeroNota, ");
                        groupBy.Append("XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "SerieNota":
                    if (!select.Contains(" SerieNota, "))
                    {
                        select.Append("XMLNotaFiscal.NF_SERIE as SerieNota, ");
                        groupBy.Append("XMLNotaFiscal.NF_SERIE, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "CodigoBarras":
                    if (!select.Contains(" CodigoBarras, "))
                    {
                        select.Append("ISNULL(ConferenciaSeparacao.COS_CODIGO_BARRAS, REPLICATE('0', 14 - LEN(CTeParticipanteRemetente.PCT_CPF_CNPJ)) + RTrim(CTeParticipanteRemetente.PCT_CPF_CNPJ) + REPLICATE('0', 9 - LEN(XMLNotaFiscal.NF_NUMERO)) + RTrim(XMLNotaFiscal.NF_NUMERO)) CodigoBarras, ");
                        groupBy.Append("ConferenciaSeparacao.COS_CODIGO_BARRAS, CTeParticipanteRemetente.PCT_CPF_CNPJ, XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsConferenciaSeparacao(joins);
                        SetarJoinsCTeParticipanteRemetente(joins);
                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "NumeroSolicitacao":
                    if (!select.Contains(" NumeroSolicitacao, "))
                    {
                        select.Append("XMLNotaFiscal.NF_NUMERO_SOLICITACAO as NumeroSolicitacao, ");
                        groupBy.Append("XMLNotaFiscal.NF_NUMERO_SOLICITACAO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "Volumes":
                    if (!select.Contains(" Volumes, "))
                    {
                        select.Append("CAST(XMLNotaFiscal.NF_VOLUMES AS DECIMAL(18,4)) as Volumes, ");
                        groupBy.Append("XMLNotaFiscal.NF_VOLUMES, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "Embarcados":
                    if (!select.Contains(" Embarcados, "))
                    {
                        select.Append("ISNULL(ConferenciaSeparacao.COS_QUANTIDADE, 0) Embarcados, ");
                        groupBy.Append("ConferenciaSeparacao.COS_QUANTIDADE, ");

                        SetarJoinsConferenciaSeparacao(joins);
                    }
                    break;
                case "Faltantes":
                    if (!select.Contains(" Faltantes, "))
                    {
                        select.Append("ISNULL(ConferenciaSeparacao.COS_QUANTIDADE_FALTANTE, CAST(XMLNotaFiscal.NF_VOLUMES AS DECIMAL(18,4))) Faltantes, ");
                        groupBy.Append("ConferenciaSeparacao.COS_QUANTIDADE_FALTANTE, XMLNotaFiscal.NF_VOLUMES, ");

                        SetarJoinsConferenciaSeparacao(joins);
                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("CTeParticipanteRemetente.PCT_NOME + ' (' + CTeParticipanteRemetente.PCT_CPF_CNPJ + ')' as Remetente, ");
                        groupBy.Append("CTeParticipanteRemetente.PCT_NOME, CTeParticipanteRemetente.PCT_CPF_CNPJ, ");

                        SetarJoinsCTeParticipanteRemetente(joins);
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("CTeParticipanteDestinatario.PCT_NOME + ' (' + CTeParticipanteDestinatario.PCT_CPF_CNPJ + ')' as Destinatario, ");
                        groupBy.Append("CTeParticipanteDestinatario.PCT_NOME, CTeParticipanteDestinatario.PCT_CPF_CNPJ, ");

                        SetarJoinsCTeParticipanteDestinatario(joins);
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;
                case "Conferente":
                    if (!select.Contains(" Conferente, "))
                    {
                        select.Append("Funcionario.FUN_NOME as Conferente, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;
                case "DataExpedicaoFormatada":
                    if (!select.Contains(" DataExpedicao, "))
                    {
                        select.Append("CargaControleExpedicao.CCX_DATA_CONFIRMACAO as DataExpedicao, ");
                        groupBy.Append("CargaControleExpedicao.CCX_DATA_CONFIRMACAO, ");

                        SetarJoinsCargaControleExpedicao(joins);
                    }
                    break;
                case "DataEmbarqueFormatada":
                    if (!select.Contains(" DataEmbarque, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO as DataEmbarque, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append(" AND Carga.CAR_CODIGO = " + filtrosPesquisa.CodigoCarga.ToString());

            if (filtrosPesquisa.CodigoConferente > 0)
                where.Append(" AND Funcionario.FUN_CODIGO = " + filtrosPesquisa.CodigoConferente.ToString());

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                where.Append(" AND XMLNotaFiscal.NF_NUMERO_SOLICITACAO = '" + filtrosPesquisa.NumeroPedido + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNota))
                where.Append(" AND XMLNotaFiscal.NF_NUMERO = '" + filtrosPesquisa.NumeroNota + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoBarras))
                where.Append(" AND ConferenciaSeparacao.COS_CODIGO_BARRAS = '" + filtrosPesquisa.CodigoBarras + "'");

            if (filtrosPesquisa.DataExpedicaoInicial != DateTime.MinValue)
                where.Append(" and CAST(CargaControleExpedicao.CCX_DATA_CONFIRMACAO) >= '" + filtrosPesquisa.DataExpedicaoInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataExpedicaoFinal != DateTime.MinValue)
                where.Append(" and CAST(CargaControleExpedicao.CCX_DATA_CONFIRMACAO) <= '" + filtrosPesquisa.DataExpedicaoFinal.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataEmbarqueInicial != DateTime.MinValue)
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO) >= '" + filtrosPesquisa.DataEmbarqueInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataEmbarqueFinal != DateTime.MinValue)
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO) <= '" + filtrosPesquisa.DataEmbarqueFinal.ToString(pattern) + "' ");
        }

        #endregion
    }
}
