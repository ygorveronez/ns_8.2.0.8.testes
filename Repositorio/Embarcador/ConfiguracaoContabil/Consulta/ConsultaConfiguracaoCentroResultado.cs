using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    sealed class ConsultaConfiguracaoCentroResultado : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado>
    {
        #region Construtores

        public ConsultaConfiguracaoCentroResultado() : base(tabela: "T_CONFIGURACAO_CENTRO_RESULTADO as ConfiguracaoCentroResultado") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = ConfiguracaoCentroResultado.EMP_CODIGO ");
        }

        private void SetarJoinsTipoOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOcorrencia "))
                joins.Append(" LEFT JOIN T_OCORRENCIA TipoOcorrencia ON TipoOcorrencia.OCO_CODIGO = ConfiguracaoCentroResultado.OCO_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = ConfiguracaoCentroResultado.TOP_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoProduto "))
                joins.Append(" LEFT JOIN T_GRUPO_PRODUTO GrupoProduto ON GrupoProduto.GPR_CODIGO = ConfiguracaoCentroResultado.GPR_CODIGO ");
        }

        private void SetarJoinsRotaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" RotaFrete "))
                joins.Append(" LEFT JOIN T_ROTA_FRETE RotaFrete ON RotaFrete.ROF_CODIGO = ConfiguracaoCentroResultado.ROF_CODIGO ");
        }

        private void SetarJoinsCentroResultadoContabilizacao(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultadoContabilizacao "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultadoContabilizacao ON CentroResultadoContabilizacao.CRE_CODIGO = ConfiguracaoCentroResultado.CRE_CODIGO_CONTABILIZACAO ");
        }

        private void SetarJoinsCentroResultadoEscrituracao(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultadoEscrituracao "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultadoEscrituracao ON CentroResultadoEscrituracao.CRE_CODIGO = ConfiguracaoCentroResultado.CRE_CODIGO_ESCRITURACAO ");
        }

        private void SetarJoinsCentroResultadoICMS(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultadoICMS "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultadoICMS ON CentroResultadoICMS.CRE_CODIGO = ConfiguracaoCentroResultado.CRE_CODIGO_ICMS ");
        }

        private void SetarJoinsCentroResultadoPIS(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultadoPIS "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultadoPIS ON CentroResultadoPIS.CRE_CODIGO = ConfiguracaoCentroResultado.CRE_CODIGO_PIS ");
        }

        private void SetarJoinsCentroResultadoCOFINS(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultadoCOFINS "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultadoCOFINS ON CentroResultadoCOFINS.CRE_CODIGO = ConfiguracaoCentroResultado.CRE_CODIGO_COFINS ");
        }

        #endregion


        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtroPesquisa)
        {
            switch (propriedade)
            {
                case "RemetenteFormatado":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("ConfiguracaoCentroResultado.CLI_REMETENTE Remetente, ");
                        groupBy.Append("ConfiguracaoCentroResultado.CLI_REMETENTE, ");
                    }
                    break;

                case "DestinatarioFormatado":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("ConfiguracaoCentroResultado.CLI_DESTINATARIO Destinatario, ");
                        groupBy.Append("ConfiguracaoCentroResultado.CLI_DESTINATARIO, ");
                    }
                    break;

                case "TomadorFormatado":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("ConfiguracaoCentroResultado.CLI_TOMADOR Tomador, ");
                        groupBy.Append("ConfiguracaoCentroResultado.CLI_TOMADOR, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO TipoOcorrencia, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO GrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete, "))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO RotaFrete, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;

                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("ConfiguracaoCentroResultado.CCR_ATIVO Situacao, ");
                        groupBy.Append("ConfiguracaoCentroResultado.CCR_ATIVO, ");
                    }
                    break;

                case "CentroResultadoContabilizacao":
                    if (!select.Contains(" CentroResultadoContabilizacao, "))
                    {
                        select.Append("CentroResultadoContabilizacao.CRE_DESCRICAO CentroResultadoContabilizacao, ");
                        groupBy.Append("CentroResultadoContabilizacao.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultadoContabilizacao(joins);
                    }
                    break;

                case "CentroResultadoEscrituracao":
                    if (!select.Contains(" CentroResultadoEscrituracao, "))
                    {
                        select.Append("CentroResultadoEscrituracao.CRE_DESCRICAO CentroResultadoEscrituracao, ");
                        groupBy.Append("CentroResultadoEscrituracao.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultadoEscrituracao(joins);
                    }
                    break;

                case "CentroResultadoICMS":
                    if (!select.Contains(" CentroResultadoICMS, "))
                    {
                        select.Append("CentroResultadoICMS.CRE_DESCRICAO CentroResultadoICMS, ");
                        groupBy.Append("CentroResultadoICMS.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultadoICMS(joins);
                    }
                    break;

                case "CentroResultadoPIS":
                    if (!select.Contains(" CentroResultadoPIS, "))
                    {
                        select.Append("CentroResultadoPIS.CRE_DESCRICAO CentroResultadoPIS, ");
                        groupBy.Append("CentroResultadoPIS.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultadoPIS(joins);
                    }
                    break;

                case "CentroResultadoCOFINS":
                    if (!select.Contains(" CentroResultadoCOFINS, "))
                    {
                        select.Append("CentroResultadoCOFINS.CRE_DESCRICAO CentroResultadoCOFINS, ");
                        groupBy.Append("CentroResultadoCOFINS.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultadoCOFINS(joins);
                    }
                    break;
                case "ItemServico":
                    if (!select.Contains(" ItemServico, "))
                    {
                        select.Append("ConfiguracaoCentroResultado.CRE_ITEM_SERVICO ItemServico, ");
                        groupBy.Append("ConfiguracaoCentroResultado.CRE_ITEM_SERVICO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.Remetente > 0)
                where.Append($" and ConfiguracaoCentroResultado.CLI_REMETENTE = {filtrosPesquisa.Remetente}");

            if (filtrosPesquisa.Destinatario > 0)
                where.Append($" and ConfiguracaoCentroResultado.CLI_DESTINATARIO = {filtrosPesquisa.Destinatario}");

            if (filtrosPesquisa.Tomador > 0)
                where.Append($" and ConfiguracaoCentroResultado.CLI_TOMADOR = {filtrosPesquisa.Tomador}");

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" and ConfiguracaoCentroResultado.EMP_CODIGO = {filtrosPesquisa.Transportador}");

            if (filtrosPesquisa.TipoOperacao > 0)
                where.Append($" and ConfiguracaoCentroResultado.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}");

            if (filtrosPesquisa.TipoOcorrencia > 0)
                where.Append($" and ConfiguracaoCentroResultado.OCO_CODIGO = {filtrosPesquisa.TipoOcorrencia}");

            if (filtrosPesquisa.GrupoProduto > 0)
                where.Append($" and ConfiguracaoCentroResultado.GPR_CODIGO = {filtrosPesquisa.GrupoProduto}");

            if (filtrosPesquisa.RotaFrete > 0)
                where.Append($" and ConfiguracaoCentroResultado.ROF_CODIGO = {filtrosPesquisa.RotaFrete}");

            if (filtrosPesquisa.Situacao.HasValue)
                if (filtrosPesquisa.Situacao.Value)
                    where.Append($" and ConfiguracaoCentroResultado.CCR_ATIVO = 1");
                else
                    where.Append($" and ConfiguracaoCentroResultado.CCR_ATIVO = 0");

            if (filtrosPesquisa.CentroResultadoContabilizacao > 0)
                where.Append($" and ConfiguracaoCentroResultado.CRE_CODIGO_CONTABILIZACAO = {filtrosPesquisa.CentroResultadoContabilizacao}");

            if (filtrosPesquisa.CentroResultadoICMS > 0)
                where.Append($" and ConfiguracaoCentroResultado.CRE_CODIGO_ICMS = {filtrosPesquisa.CentroResultadoICMS}");

            if (filtrosPesquisa.CentroResultadoPIS > 0)
                where.Append($" and ConfiguracaoCentroResultado.CRE_CODIGO_PIS = {filtrosPesquisa.CentroResultadoPIS}");

            if (filtrosPesquisa.CentroResultadoCOFINS > 0)
                where.Append($" and ConfiguracaoCentroResultado.CRE_CODIGO_COFINS = {filtrosPesquisa.CentroResultadoCOFINS}");

        }

        #endregion
    }
}
