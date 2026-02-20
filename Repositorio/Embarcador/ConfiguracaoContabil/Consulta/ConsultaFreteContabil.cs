using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    sealed class ConsultaFreteContabil : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaFreteContabil>
    {
        #region Construtores

        public ConsultaFreteContabil() : base(tabela: "T_DOCUMENTO_CONTABIL as DocumentoContabil") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append("left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = DocumentoContabil.CRE_CODIGO ");
        }

        private void SetarJoinsCidadeDestino(StringBuilder joins)
        {
            if (!joins.Contains(" CidadeDestino "))
                joins.Append("left join T_LOCALIDADES CidadeDestino on CidadeDestino.LOC_CODIGO = DocumentoContabil.LOC_DESTINO ");
        }

        private void SetarJoinsCidadeOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" CidadeOrigem "))
                joins.Append("left join T_LOCALIDADES CidadeOrigem on CidadeOrigem.LOC_CODIGO = DocumentoContabil.LOC_ORIGEM ");
        }

        private void SetarJoinsCompanhia(StringBuilder joins)
        {
            if (!joins.Contains(" Companhia "))
                joins.Append("left join T_CLIENTE Companhia on Companhia.CLI_CGCCPF = DocumentoContabil.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsEmitente(StringBuilder joins)
        {
            if (!joins.Contains(" Emitente "))
                joins.Append("left join T_CLIENTE Emitente on Emitente.CLI_CGCCPF = DocumentoContabil.CLI_TOMADOR ");
        }

        private void SetarJoinsPlanoConta(StringBuilder joins)
        {
            if (!joins.Contains(" PlanoConta "))
                joins.Append("left join T_PLANO_DE_CONTA PlanoConta on PlanoConta.PLA_CODIGO = DocumentoContabil.PLA_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append("left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = DocumentoContabil.EMP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaFreteContabil filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CentroCusto":
                    if (!select.Contains(" CentroCusto"))
                    {
                        select.Append("CentroResultado.CRE_PLANO_CONTABILIDADE as CentroCusto, ");
                        groupBy.Append("CentroResultado.CRE_PLANO_CONTABILIDADE, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "CpfCnpjCompanhiaFormatado":
                    if (!select.Contains(" CpfCnpjCompanhia"))
                    {
                        select.Append("Companhia.CLI_CGCCPF as CpfCnpjCompanhia, Companhia.CLI_FISJUR as TipoPessoaCompanhia, ");
                        groupBy.Append("Companhia.CLI_CGCCPF, Companhia.CLI_FISJUR, ");

                        SetarJoinsCompanhia(joins);
                    }
                    break;

                case "CpfCnpjEmitenteFormatado":
                    if (!select.Contains(" CpfCnpjEmitente"))
                    {
                        select.Append("Emitente.CLI_CGCCPF as CpfCnpjEmitente, Emitente.CLI_FISJUR as TipoPessoaEmitente, ");
                        groupBy.Append("Emitente.CLI_CGCCPF, Emitente.CLI_FISJUR, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "CnpjTransportador":
                    if (!select.Contains(" CnpjTransportador"))
                    {
                        select.Append("Transportador.EMP_CNPJ as CnpjTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "ContaContabil":
                    if (!select.Contains(" ContaContabil"))
                    {
                        select.Append("PlanoConta.PLA_PLANO_CONTABILIDADE as ContaContabil, ");
                        groupBy.Append("PlanoConta.PLA_PLANO_CONTABILIDADE, ");

                        SetarJoinsPlanoConta(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao"))
                    {
                        select.Append("DocumentoContabil.DCB_DATA_EMISSAO as DataEmissao, ");
                        groupBy.Append("DocumentoContabil.DCB_DATA_EMISSAO, ");
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino"))
                    {
                        select.Append("CidadeDestino.LOC_DESCRICAO as Destino, ");
                        groupBy.Append("CidadeDestino.LOC_DESCRICAO, ");

                        SetarJoinsCidadeDestino(joins);
                    }
                    break;

                case "NomeCompanhia":
                    if (!select.Contains(" NomeCompanhia"))
                    {
                        select.Append("Companhia.CLI_NOME as NomeCompanhia, ");
                        groupBy.Append("Companhia.CLI_NOME, ");

                        SetarJoinsCompanhia(joins);
                    }
                    break;

                case "NomeEmitente":
                    if (!select.Contains(" NomeEmitente"))
                    {
                        select.Append("Emitente.CLI_NOME as NomeEmitente, ");
                        groupBy.Append("Emitente.CLI_NOME, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "NomeTransportador":
                    if (!select.Contains(" NomeTransportador"))
                    {
                        select.Append("Transportador.EMP_RAZAO as NomeTransportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem"))
                    {
                        select.Append("CidadeOrigem.LOC_DESCRICAO as Origem, ");
                        groupBy.Append("CidadeOrigem.LOC_DESCRICAO, ");

                        SetarJoinsCidadeOrigem(joins);
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie"))
                    {
                        select.Append("DocumentoContabil.DCB_SERIE_DOCUMENTO as Serie, ");
                        groupBy.Append("DocumentoContabil.DCB_SERIE_DOCUMENTO, ");
                    }
                    break;

                case "TipoContabilizacao":
                    if (!select.Contains(" TipoContabilizacao"))
                    {
                        select.Append("(case when DocumentoContabil.CCT_TIPO_CONTABILIZACAO = 1 then 'Crédito' else 'Débito' end) as TipoContabilizacao, ");
                        groupBy.Append("DocumentoContabil.CCT_TIPO_CONTABILIZACAO, ");
                    }
                    break;

                case "ValorLancamento":
                    if (!select.Contains(" ValorLancamento"))
                    {
                        select.Append("DocumentoContabil.DCB_VALOR_CONTABILIZACAO as ValorLancamento, ");
                        groupBy.Append("DocumentoContabil.DCB_VALOR_CONTABILIZACAO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaFreteContabil filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and DocumentoContabil.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.CpfCnpjCompanhia > 0)
                where.Append($" and DocumentoContabil.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjCompanhia}");

            if (filtrosPesquisa.CpfCnpjEmitente > 0)
                where.Append($" and DocumentoContabil.CLI_TOMADOR = {filtrosPesquisa.CpfCnpjEmitente}");

            if (filtrosPesquisa.TipoContabilizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Todos)
                where.Append($" and DocumentoContabil.CCT_TIPO_CONTABILIZACAO = {(int)filtrosPesquisa.TipoContabilizacao}");

            if (filtrosPesquisa.DataEmissaoInicio.HasValue)
                where.Append($" and DocumentoContabil.DCB_DATA_EMISSAO >= '{filtrosPesquisa.DataEmissaoInicio.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
                where.Append($" and DocumentoContabil.DCB_DATA_EMISSAO <= '{filtrosPesquisa.DataEmissaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");
        }

        #endregion
    }
}
