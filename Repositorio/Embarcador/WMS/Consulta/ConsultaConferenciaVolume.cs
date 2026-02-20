using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaConferenciaVolume : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioConferenciaVolume>
    {
        #region Construtores

        public ConsultaConferenciaVolume() : base(tabela: "T_RECEBIMENTO_MERCADORIA as RecebimentoMercadoria") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsRecebimento(StringBuilder joins)
        {
            if (!joins.Contains(" Recebimento "))
                joins.Append(" left join T_RECEBIMENTO Recebimento on Recebimento.RME_CODIGO = RecebimentoMercadoria.RME_CODIGO ");
        }
        private void SetarJoinsClienteRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RecebimentoMercadoria.CPF_CNPJ_REMETENTE ");
        }
        private void SetarJoinsClienteDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteDestinatario "))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = RecebimentoMercadoria.CPF_CNPJ_DESTINATARIO ");
        }
        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsRecebimento(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = Recebimento.CAR_CODIGO ");
        }
        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }
        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsRecebimento(joins);

            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = Recebimento.FUN_CODIGO ");
        }
        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsRecebimento(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Recebimento.VEI_CODIGO ");
        }
        private void SetarJoinsManifesto(StringBuilder joins)
        {
            SetarJoinsRecebimento(joins);

            if (!joins.Contains(" Manifesto "))
                joins.Append(" left join T_MDFE Manifesto on Manifesto.MDF_CODIGO = Recebimento.MDF_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioConferenciaVolume filtroPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroNota":
                    if (!select.Contains(" NumeroNota, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_NUMERO_LOTE as NumeroNota, ");
                        groupBy.Append("RecebimentoMercadoria.REM_NUMERO_LOTE, ");
                    }
                    break;

                case "SerieNota":
                    if (!select.Contains(" SerieNota, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_SERIE as SerieNota, ");
                        groupBy.Append("RecebimentoMercadoria.REM_SERIE, ");
                    }
                    break;

                case "CodigoBarras":
                    if (!select.Contains(" CodigoBarras, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_CODIGO_BARRAS as CodigoBarras, ");
                        groupBy.Append("RecebimentoMercadoria.REM_CODIGO_BARRAS, ");
                    }
                    break;

                case "NumeroSolicitacao":
                    if (!select.Contains(" NumeroSolicitacao, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_DESCRICAO as NumeroSolicitacao, ");
                        groupBy.Append("RecebimentoMercadoria.REM_DESCRICAO, ");
                    }
                    break;

                case "Volumes":
                    if (!select.Contains(" Volumes, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_QUANTIDADE_LOTE as Volumes, ");
                        groupBy.Append("RecebimentoMercadoria.REM_QUANTIDADE_LOTE, ");
                    }
                    break;

                case "Embarcados":
                    if (!select.Contains(" Embarcados, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_QUANTIDADE_CONFERIDA as Embarcados, ");
                        groupBy.Append("RecebimentoMercadoria.REM_QUANTIDADE_CONFERIDA, ");
                    }
                    break;

                case "Faltantes":
                    if (!select.Contains(" Faltantes, "))
                    {
                        select.Append("RecebimentoMercadoria.REM_QUANTIDADE_FALTANTE as Faltantes, ");
                        groupBy.Append("RecebimentoMercadoria.REM_QUANTIDADE_FALTANTE, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_NOME + ' (' + LTRIM(STR(ClienteRemetente.CLI_CGCCPF, 25, 0))  + ')' as Remetente, ");
                        groupBy.Append("ClienteRemetente.CLI_NOME, ClienteRemetente.CLI_CGCCPF, ");

                        SetarJoinsClienteRemetente(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_NOME + ' (' + LTRIM(STR(ClienteDestinatario.CLI_CGCCPF, 25, 0))  + ')' as Destinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_NOME, ClienteDestinatario.CLI_CGCCPF, ");

                        SetarJoinsClienteDestinatario(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "MDFe":
                    if (!select.Contains(" MDFe, "))
                    {
                        select.Append("Manifesto.MDF_NUMERO as MDFe, ");
                        groupBy.Append("Manifesto.MDF_NUMERO, ");

                        SetarJoinsManifesto(joins);
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

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DataConferenciaFormatada":
                    if (!select.Contains(" DataConferencia, "))
                    {
                        select.Append("Recebimento.RME_DATA as DataConferencia, ");
                        groupBy.Append("Recebimento.RME_DATA, ");

                        SetarJoinsRecebimento(joins);
                    }
                    break;

                case "DataEmbarqueFormatada":
                    if (!select.Contains(" DataEmbarque, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO as DataEmbarque, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioConferenciaVolume filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append(" AND Carga.CAR_CODIGO = " + filtrosPesquisa.CodigoCarga.ToString());

            if (filtrosPesquisa.CodigoMDFe > 0)
                where.Append(" AND Recebimento.MDF_CODIGO = " + filtrosPesquisa.CodigoMDFe.ToString());

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append(" AND Recebimento.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString());

            if (filtrosPesquisa.CodigoConferente > 0)
                where.Append(" AND Funcionario.FUN_CODIGO = " + filtrosPesquisa.CodigoConferente.ToString());

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append(" AND Recebimento.RME_SITUACAO = " + filtrosPesquisa.Situacao.Value.ToString("d"));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                where.Append(" AND RecebimentoMercadoria.REM_DESCRICAO = '" + filtrosPesquisa.NumeroPedido + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNota))
                where.Append(" AND RecebimentoMercadoria.REM_NUMERO_LOTE = '" + filtrosPesquisa.NumeroNota + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoBarras))
                where.Append(" AND RecebimentoMercadoria.REM_CODIGO_BARRAS = '" + filtrosPesquisa.CodigoBarras + "'");

            if (filtrosPesquisa.DataConferenciaInicial != DateTime.MinValue)
                where.Append(" and CAST(Recebimento.RME_DATA) >= '" + filtrosPesquisa.DataConferenciaInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataConferenciaFinal != DateTime.MinValue)
                where.Append(" and CAST(Recebimento.RME_DATA) <= '" + filtrosPesquisa.DataConferenciaFinal.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataEmbarqueInicial != DateTime.MinValue)
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO) >= '" + filtrosPesquisa.DataEmbarqueInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataEmbarqueFinal != DateTime.MinValue)
                where.Append(" and CAST(Carga.CAR_DATA_CARREGAMENTO) <= '" + filtrosPesquisa.DataEmbarqueFinal.ToString(pattern) + "' ");
        }

        #endregion
    }
}
