using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Ocorrencias
{
    sealed class ConsultaOcorrencia : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia>
    {
        #region Construtores

        public ConsultaOcorrencia() : base(tabela: "T_CARGA_OCORRENCIA as Ocorrencia") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("left join T_CARGA Carga on Carga.CAR_CODIGO = Ocorrencia.CAR_CODIGO ");
        }

        private void SetarJoinsCargaAgrupada(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" CargaAgrupada "))
                joins.Append("left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO_AGRUPAMENTO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append("left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsCreditor(StringBuilder joins)
        {
            SetarJoinsSolicitacaoCredito(joins);

            if (!joins.Contains(" Creditor "))
                joins.Append("left join T_FUNCIONARIO Creditor on Creditor.FUN_CODIGO = SolicitacaoCredito.FUN_CODIGO_CREDITOR ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsFluxoPatio(StringBuilder joins)
        {
            SetarJoinsTipoOcorrencia(joins);

            if (!joins.Contains(" FluxoPatio "))
                joins.Append($"LEFT join T_FLUXO_GESTAO_PATIO FluxoPatio ON FluxoPatio.CAR_CODIGO = Ocorrencia.CAR_CODIGO and FluxoPatio.FGE_TIPO = {(int)TipoFluxoGestaoPatio.Origem} and FluxoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO <> {(int)SituacaoEtapaFluxoGestaoPatio.Cancelado} ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" GrupoPessoas "))
                joins.Append("left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            SetarJoinsTipoOcorrencia(joins);

            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append("left join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = TipoOcorrencia.MOD_CODIGO ");
        }

        private void SetarJoinsOutroEmitente(StringBuilder joins)
        {
            if (!joins.Contains(" OutroEmitente "))
                joins.Append("left join T_EMPRESA OutroEmitente on OutroEmitente.EMP_CODIGO = Ocorrencia.EMP_OUTRO_EMITENTE ");
        }

        private void SetarJoinsSolicitacaoCredito(StringBuilder joins)
        {
            if (!joins.Contains(" SolicitacaoCredito "))
                joins.Append("left join T_SOLICITACAO_CREDITO SolicitacaoCredito on SolicitacaoCredito.SCR_CODIGO = Ocorrencia.SCR_CODIGO ");
        }

        private void SetarJoinsSolicitante(StringBuilder joins)
        {
            SetarJoinsSolicitacaoCredito(joins);

            if (!joins.Contains(" Solicitante "))
                joins.Append("left join T_FUNCIONARIO Solicitante on Solicitante.FUN_CODIGO = SolicitacaoCredito.FUN_CODIGO_SOLICITANTE ");
        }

        private void SetarJoinsTipoOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOcorrencia "))
                joins.Append("left join T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append("left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Ocorrencia.FUN_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaEntrega "))
                joins.Append("left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinsGrupoOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoOcorrencia "))
                joins.Append("left join T_GRUPO_TIPO_OCORRENCIA GrupoOcorrencia on GrupoOcorrencia.GTO_CODIGO = TipoOcorrencia.GTO_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Carencia":
                    if (!select.Contains(" Carencia, "))
                        select.Append("5 as Carencia, ");
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("( ");
                        select.Append("     select top(1) ");

                        select.Append("     case when configuracaoOcorrencia.COO_EXIBIR_DESTINATARIO_OCORRENCIA = 1 then ");
                        select.Append("         ((case when isnull(Destinatario.CLI_CODIGO_INTEGRACAO, '') <> '' then Destinatario.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + Destinatario.CLI_NOME) ");
                        select.Append("     else ");
                        select.Append("         ((case when isnull(Cliente.CLI_CODIGO_INTEGRACAO, '') <> '' then Cliente.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) + Cliente.CLI_NOME) ");
                        select.Append("     end ");

                        select.Append("       from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("       join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                        select.Append("       join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("       join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_TOMADOR_PAGADOR_CTE ");
                        select.Append("       join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");
                        select.Append("       left outer join T_CTE_PARTICIPANTE ParticipanteCteDestinatario on ParticipanteCteDestinatario.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("       left outer join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = ParticipanteCteDestinatario.CLI_CODIGO, T_CONFIGURACAO_OCORRENCIA configuracaoOcorrencia ");
                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append(") as Cliente, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CPFCNPJClienteDescricao":
                    if (!select.Contains(" CPFCNPJCliente, "))
                    {
                        select.Append("( ");
                        select.Append("     select top(1) ");

                        select.Append("     case when configuracaoOcorrencia.COO_EXIBIR_DESTINATARIO_OCORRENCIA = 1 then ");
                        select.Append("         Destinatario.CLI_CGCCPF ");
                        select.Append("     else ");
                        select.Append("         Cliente.CLI_CGCCPF ");
                        select.Append("     end ");

                        select.Append("       from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("       join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                        select.Append("       join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("       join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_TOMADOR_PAGADOR_CTE ");
                        select.Append("       join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");
                        select.Append("       left outer join T_CTE_PARTICIPANTE ParticipanteCteDestinatario on ParticipanteCteDestinatario.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("       left outer join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = ParticipanteCteDestinatario.CLI_CODIGO, T_CONFIGURACAO_OCORRENCIA configuracaoOcorrencia ");
                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append(") as CPFCNPJCliente, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CNPJEmpresa":
                case "CNPJTransportadora":
                    if (!select.Contains(" CNPJEmpresa,"))
                    {
                        select.Append("isnull(Transportador.EMP_CNPJ, OutroEmitente.EMP_CNPJ) cnpjempresa, ");
                        groupBy.Append("Transportador.EMP_CNPJ, OutroEmitente.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                        SetarJoinsOutroEmitente(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Ocorrencia.COC_CODIGO as Codigo, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataAlteracao":
                    if (!select.Contains(" DataAlteracao,"))
                    {
                        select.Append("Ocorrencia.COC_ALTERACAO as DataAlteracao, ");
                        groupBy.Append("Ocorrencia.COC_ALTERACAO, ");
                    }
                    break;

                case "CargaAgrupada":
                    if (!select.Contains(" CargaAgrupada,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + AG.CAR_CODIGO_CARGA_AGRUPADO ");
                        select.Append("      from T_CARGA_CODIGOS_AGRUPADOS AG ");
                        select.Append("      where AG.CAR_CODIGO = Ocorrencia.CAR_CODIGO ");
                        select.Append("      GROUP BY AG.CAR_CODIGO_CARGA_AGRUPADO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CargaAgrupada, ");

                        if (!groupBy.Contains("Ocorrencia.CAR_CODIGO,"))
                            groupBy.Append("Ocorrencia.CAR_CODIGO, ");
                    }
                    break;

                case "DataAprovacao":
                    if (!select.Contains(" DataAprovacao,"))
                    {
                        select.Append("case when Ocorrencia.COC_SITUACAO_OCORRENCIA = 4 THEN  Ocorrencia.COC_DATA_BASE_APROVACAO_AUTOMATICA ELSE  Ocorrencia.COC_DATA_APROVACAO END as DataAprovacao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_DATA_APROVACAO,"))
                            groupBy.Append("Ocorrencia.COC_DATA_APROVACAO, ");
                        if (!groupBy.Contains("Ocorrencia.COC_DATA_BASE_APROVACAO_AUTOMATICA,"))
                            groupBy.Append("Ocorrencia.COC_DATA_BASE_APROVACAO_AUTOMATICA, ");
                        if (!groupBy.Contains("Ocorrencia.COC_SITUACAO_OCORRENCIA,"))
                            groupBy.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA, ");
                    }
                    break;

                case "DataAprovacaoFormatada":
                    SetarSelect("DataAprovacao", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("DataRetornoSolicitacaoCredito", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "DataCarga":
                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga,"))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO as DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataChegada":
                case "DataChegadaFormatada":
                    if (!select.Contains(" DataChegada,"))
                    {
                        select.Append("FluxoPatio.FGP_CHEGADA_VEICULO as DataChegada, ");

                        if (!groupBy.Contains("FluxoPatio.FGP_CHEGADA_VEICULO,"))
                            groupBy.Append("FluxoPatio.FGP_CHEGADA_VEICULO, ");

                        SetarJoinsFluxoPatio(joins);
                    }
                    break;

                case "DataChegadaReentrega":
                case "DataChegadaReentregaFormatada":
                    if (!select.Contains(" DataChegadaReentrega,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) OcorrenciaParamentros.COC_DATA ");
                        select.Append("      from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("      join T_PARAMETROS_OCORRENCIA Parametros ON Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"    where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Data} ");
                        select.Append("       and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     order by OcorrenciaParamentros.POC_CODIGO ");
                        select.Append(") as DataChegadaReentrega, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataRetornoReentrega":
                case "DataRetornoReentregaFormatada":
                    if (!select.Contains(" DataRetornoReentrega,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) OcorrenciaParamentros.COC_DATA ");
                        select.Append("      from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("      join T_PARAMETROS_OCORRENCIA Parametros ON Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"    where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Data} ");
                        select.Append("       and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     order by OcorrenciaParamentros.POC_CODIGO desc");
                        select.Append(") as DataRetornoReentrega, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataSaida":
                case "DataSaidaFormatada":
                    if (!select.Contains(" DataSaida,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) OcorrenciaParamentros.COC_DATA_FIM ");
                        select.Append("      from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("      join T_PARAMETROS_OCORRENCIA Parametros on Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"    where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Periodo} ");
                        select.Append("       and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     order by OcorrenciaParamentros.POC_CODIGO ");
                        select.Append(") as DataSaida, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataEmbarque":
                case "DataEmbarqueFormatada":
                    if (!select.Contains(" DataEmbarque,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) OcorrenciaParamentros.COC_DATA_INICIO ");
                        select.Append("      from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("      join T_PARAMETROS_OCORRENCIA Parametros on Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"    where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Periodo} ");
                        select.Append("       and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     order by OcorrenciaParamentros.POC_CODIGO ");
                        select.Append(") as DataEmbarque, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataRetornoSolicitacaoCredito":
                    if (!select.Contains(" DataRetornoSolicitacaoCredito,"))
                    {
                        select.Append("SolicitacaoCredito.SCR_DATA_RETORNO as DataRetornoSolicitacaoCredito, ");
                        groupBy.Append("SolicitacaoCredito.SCR_DATA_RETORNO, ");

                        SetarJoinsSolicitacaoCredito(joins);
                    }
                    break;

                case "DataSolicitacao":
                    if (!select.Contains(" DataSolicitacao, "))
                    {
                        select.Append("Ocorrencia.COC_DATA_OCORRENCIA as DataSolicitacao, ");
                        groupBy.Append("Ocorrencia.COC_DATA_OCORRENCIA, ");
                    }
                    break;

                case "DescricaoOcorrencia":
                    if (!select.Contains(" DescricaoOcorrencia, "))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO as DescricaoOcorrencia, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);
                    }
                    break;

                case "CodigoIntegracaoDestinatarios":
                    if (!select.Contains(" CodigoIntegracaoDestinatarios, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(ClienteCte.CLI_CODIGO_INTEGRACAO, ClientePreCte.CLI_CODIGO_INTEGRACAO) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CodigoIntegracaoDestinatarios, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Destinatarios":
                    if (!select.Contains(" Destinatarios, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(ClienteCte.CLI_NOME, ClientePreCte.CLI_NOME) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Destinatarios, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Categoria":
                    if (!select.Contains(" Categoria, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(CategoriaClienteCte.CTP_DESCRICAO, CategoriaClientePreCte.CTP_DESCRICAO) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");
                        select.Append("      left join T_CATEGORIA_PESSOA CategoriaClienteCte on CategoriaClienteCte.CTP_CODIGO = ClienteCte.CTP_CODIGO ");


                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");
                        select.Append("      left join T_CATEGORIA_PESSOA CategoriaClientePreCte on CategoriaClientePreCte.CTP_CODIGO = ClientePreCte.CTP_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Categoria, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO as GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_EXPEDIDORES as Expedidor, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_EXPEDIDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_RECEBEDORES as Recebedor, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_RECEBEDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento,"))
                    {
                        select.Append("Ocorrencia.COC_OBSERVACAO_CANCELAMENTO as MotivoCancelamento, ");
                        groupBy.Append("Ocorrencia.COC_OBSERVACAO_CANCELAMENTO, ");
                    }
                    break;

                case "JanelaDescarga":
                    if (!select.Contains(" JanelaDescarga,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) ClienteDescarga.CLD_HORA_INICIO_DESCARGA + ' até ' + ClienteDescarga.CLD_HORA_LIMETE_DESCARGA ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("      join T_CLIENTE_DESCARGA ClienteDescarga on Pedido.CLI_CODIGO = ClienteDescarga.CLI_CGCCPF and Pedido.CLI_CODIGO_REMETENTE = ClienteDescarga.CLI_CGCCPF_ORIGEM ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append(") as JanelaDescarga, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "JustificativaRejeicao":
                    if (!select.Contains(" JustificativaRejeicao,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + Justificativa.MRO_DESCRICAO ");
                        select.Append("      from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append("      join T_MOTIVO_REJEICAO_OCORRENCIA Justificativa on Justificativa.MRO_CODIGO = OcorrenciaAutorizacao.MRO_CODIGO ");
                        select.Append($"    where OcorrenciaAutorizacao.COA_SITUACAO = {(int)SituacaoOcorrenciaAutorizacao.Rejeitada} ");
                        select.Append("       and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     group by Justificativa.MRO_DESCRICAO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as JustificativaRejeicao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado,"))
                    {
                        select.Append("ISNULL( ");
                        select.Append("  SUBSTRING(( ");
                        select.Append("    SELECT ', ' + CentroResultado.CRE_DESCRICAO ");
                        select.Append("    FROM T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append("    JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = OcorrenciaAutorizacao.CRE_CODIGO ");
                        select.Append("    WHERE OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("    GROUP BY CentroResultado.CRE_DESCRICAO ");
                        select.Append("    FOR XML PATH('') ");
                        select.Append("  ), 3, 1000), ");
                        select.Append("  SUBSTRING(( ");
                        select.Append("    SELECT ', ' + CentroResultado.CRE_DESCRICAO ");
                        select.Append("    FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("    JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("    JOIN T_CENTRO_RESULTADO CentroResultado ON Pedido.CRE_CODIGO = CentroResultado.CRE_CODIGO ");
                        select.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("    GROUP BY CentroResultado.CRE_DESCRICAO ");
                        select.Append("    FOR XML PATH('') ");
                        select.Append("  ), 3, 1000) ");
                        select.Append(") AS CentroResultado, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "MotivoRejeicao":
                case "MotivoRejeicaoFormatado":
                    if (!select.Contains(" MotivoRejeicao,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + OcorrenciaAutorizacao.COA_MOTIVO ");
                        select.Append("      from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append($"    where OcorrenciaAutorizacao.COA_SITUACAO = {(int)SituacaoOcorrenciaAutorizacao.Rejeitada} ");
                        select.Append("       and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     group by OcorrenciaAutorizacao.COA_MOTIVO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as MotivoRejeicao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "MotivoAprovacao":
                    if (!select.Contains(" MotivoAprovacao,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select top 1 Justificativa.MRO_DESCRICAO ");
                        select.Append("      from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append("      join T_MOTIVO_REJEICAO_OCORRENCIA Justificativa on Justificativa.MRO_CODIGO = OcorrenciaAutorizacao.MRO_CODIGO ");
                        select.Append($"    where OcorrenciaAutorizacao.COA_SITUACAO = {(int)SituacaoOcorrenciaAutorizacao.Aprovada} ");
                        select.Append("       and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     group by Justificativa.MRO_DESCRICAO ");
                        select.Append("), 0, 1000) as MotivoAprovacao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ObservacaoAprovacao":
                    if (!select.Contains(" ObservacaoAprovacao,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select top 1 OcorrenciaAutorizacao.COA_MOTIVO");
                        select.Append("      from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append("      join T_MOTIVO_REJEICAO_OCORRENCIA Justificativa on Justificativa.MRO_CODIGO = OcorrenciaAutorizacao.MRO_CODIGO ");
                        select.Append($"    where OcorrenciaAutorizacao.COA_SITUACAO = {(int)SituacaoOcorrenciaAutorizacao.Aprovada} ");
                        select.Append("       and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     group by OcorrenciaAutorizacao.COA_MOTIVO");
                        select.Append("), 0, 1000) as ObservacaoAprovacao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + Motorista.FUN_NOME + case Motorista.FUN_FONE when null then '' else ' (' + Motorista.FUN_FONE  + ')' end ");
                        select.Append("      from T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      join T_FUNCIONARIO Motorista on CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     where CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Motorista, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NomeCreditor":
                    if (!select.Contains(" NomeCreditor,"))
                    {
                        select.Append("Creditor.FUN_NOME as NomeCreditor, ");
                        groupBy.Append("Creditor.FUN_NOME, ");

                        SetarJoinsCreditor(joins);
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais,"))
                    {
                        select.Append(" case when ISNULL(Ocorrencia.COC_UTILIZAR_SELECAO_POR_NOTAS_FISCAIS_CTE, 0) = 1 then ");
                        select.Append("    SUBSTRING(( ");
                        select.Append("        select distinct ', ' + convert(nvarchar(20), NotaFiscal.NF_NUMERO) ");
                        select.Append("          from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("          join T_CARGA_OCORRENCIA_DOCUMENTO_XML_NOTAS_FISCAIS OcorrenciaDocumentoNotasFiscais on OcorrenciaDocumentoNotasFiscais.COD_CODIGO = OcorrenciaDocumento.COD_CODIGO ");
                        select.Append("          join T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = OcorrenciaDocumentoNotasFiscais.NFX_CODIGO ");
                        select.Append("          where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("           for xml path('') ), 3, 1000) ");
                        select.Append(" else ");
                        select.Append("    SUBSTRING(( ");
                        select.Append("        select distinct ', ' + ISNULL(DocumentoCte.NFC_NUMERO, DocumentoPreCte.PNF_NUMERO) ");
                        select.Append("          from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("          join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                        select.Append("          left join T_CTE_DOCS DocumentoCte on DocumentoCte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("          left join T_PRE_CTE_DOCS DocumentoPreCte on DocumentoPreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("          where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("           for xml path('')), 3, 1000) ");
                        select.Append(" end as NotasFiscais, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Ocorrencia.COC_UTILIZAR_SELECAO_POR_NOTAS_FISCAIS_CTE,"))
                            groupBy.Append("Ocorrencia.COC_UTILIZAR_SELECAO_POR_NOTAS_FISCAIS_CTE, ");
                    }
                    break;

                case "SerieNotasFiscais":
                    if (!select.Contains(" SerieNotasFiscais,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + ISNULL(DocumentoCte.NFC_SERIE, '') ");
                        select.Append("      FROM T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      JOIN T_CARGA_CTE CargaCte ON CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                        select.Append("      LEFT JOIN T_CTE_DOCS DocumentoCte ON DocumentoCte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      LEFT JOIN T_PRE_CTE_DOCS DocumentoPreCte ON DocumentoPreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      WHERE OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("      FOR XML PATH('') ");
                        select.Append("), 3, 1000) AS SerieNotasFiscais, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;
                case "Chamado":
                    if (!select.Contains(" Chamado, "))
                    {
                        select.Append(@"SUBSTRING(
                                            (SELECT DISTINCT ', ' + CAST(chamado.CHA_NUMERO AS NVARCHAR(20))
                                            FROM T_CHAMADOS chamado
				                            JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.CHA_CODIGO = chamado.CHA_CODIGO
				                            WHERE chamadoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO
                                                FOR XML PATH('')), 3, 2000) Chamado, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NumeroGlog":
                    if (!select.Contains(" NumeroGlog,"))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) OcorrenciaParamentros.COC_TEXTO ");
                        select.Append("      from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("      join T_PARAMETROS_OCORRENCIA Parametros on Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"    where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Texto} ");
                        select.Append("       and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     order by OcorrenciaParamentros.POC_CODIGO ");
                        select.Append(") as NumeroGlog, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia,"))
                    {
                        select.Append("Ocorrencia.COC_NUMERO_CONTRATO as NumeroOcorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");
                    }
                    break;

                case "NumeroOcorrenciaCliente":
                    if (!select.Contains(" NumeroOcorrenciaCliente,"))
                    {
                        select.Append("Ocorrencia.COC_NUMERO_OCORRENCIA_CLIENTE as NumeroOcorrenciaCliente, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_OCORRENCIA_CLIENTE, ");
                    }
                    break;

                case "NumerosCTeOriginal":
                    if (!select.Contains(" NumerosCTeOriginal,"))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + convert(nvarchar(20), Cte.CON_NUM) ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO = CargaCTe.CCT_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as NumerosCTeOriginal, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NumerosCTeOcorrencia":
                    if (!select.Contains(" NumerosCTeOcorrencia,"))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + convert(nvarchar(20), Cte.CON_NUM) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO CargaOcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCTe ON CargaOcorrenciaDocumento.CCT_CODIGO = CargaCTe.CCT_CODIGO ");
                        select.Append("      join T_CTE Cte ON Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaOcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO IS NOT NULL ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as NumerosCTeOcorrencia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NumerosCTes":
                    if (!select.Contains(" NumerosCTes,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + cast(Cte.CON_NUM as nvarchar(20)) ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as NumerosCTes, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("Ocorrencia.COC_OBSERVACAO as Observacao, ");
                        groupBy.Append("Ocorrencia.COC_OBSERVACAO, ");
                    }
                    break;

                case "ObservacaoCTeComp":
                    if (!select.Contains(" ObservacaoCTeComp,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct '; ' + Cte.CON_OBSGERAIS ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as ObservacaoCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ObservacaoCTeOriginal":
                    if (!select.Contains(" ObservacaoCTeOriginal,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct '; ' + Cte.CON_OBSGERAIS ");
                        select.Append("      from T_CARGA_CTE CargaCTe ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Cte.CON_OBSGERAIS is not null ");
                        select.Append("       and CargaCTe.CCC_CODIGO is null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as ObservacaoCTeOriginal, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Operador":
                    SetarSelect("NomeCreditor", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("Operadores", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "Operadores":
                    if (!select.Contains(" Operadores,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + Funcionario.FUN_NOME ");
                        select.Append("      from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                        select.Append("      join T_FUNCIONARIO Funcionario ON OcorrenciaAutorizacao.FUN_CODIGO = Funcionario.FUN_CODIGO ");
                        select.Append($"    where OcorrenciaAutorizacao.COA_SITUACAO <> {(int)SituacaoOcorrenciaAutorizacao.Pendente} ");
                        select.Append("       and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("     group by Funcionario.FUN_NOME ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Operadores, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa,"))
                    {
                        select.Append("( ");
                        select.Append("    ( ");
                        select.Append("        select Veiculo.VEI_PLACA ");
                        select.Append("          from T_VEICULO Veiculo ");
                        select.Append("         where Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append("    ) +  ");
                        select.Append("    ISNULL(( ");
                        select.Append("        select ', ' + Veiculo.VEI_PLACA ");
                        select.Append("          from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado ");
                        select.Append("          join T_VEICULO Veiculo on VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("         where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") as Placa, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "QuantidadeDeHoras":
                    if (!select.Contains(" QuantidadeDeHoras,"))
                    {
                        select.Append("( ");
                        select.Append("   select sum(cast( ");
                        select.Append("              case ");
                        select.Append("                  when OcorrenciaParamentros.COC_TOTAL_HORAS > 0 then OcorrenciaParamentros.COC_TOTAL_HORAS ");
                        select.Append("                  else cast(datediff(minute, OcorrenciaParamentros.COC_DATA_INICIO, OcorrenciaParamentros.COC_DATA_FIM) as numeric(18,2)) / 60 ");
                        select.Append("              end as decimal(18,2)) ");
                        select.Append("          ) ");
                        select.Append("     from T_CARGA_OCORRENCIA_PARAMETROS OcorrenciaParamentros ");
                        select.Append("     join T_PARAMETROS_OCORRENCIA Parametros on Parametros.POC_CODIGO = OcorrenciaParamentros.POC_CODIGO ");
                        select.Append($"   where Parametros.POC_TIPO_PARAMETRO = {(int)TipoParametroOcorrencia.Periodo} ");
                        select.Append("      and OcorrenciaParamentros.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append(") as QuantidadeDeHoras, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CodigoIntegracaoRemetentes":
                    if (!select.Contains(" CodigoIntegracaoRemetentes, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(ClienteCte.CLI_CODIGO_INTEGRACAO, ClientePreCte.CLI_CODIGO_INTEGRACAO) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_REMETENTE_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_REMETENTE_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CodigoIntegracaoRemetentes, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Remetentes":
                    if (!select.Contains(" Remetentes,"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(ClienteCte.CLI_NOME, ClientePreCte.CLI_NOME) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_REMETENTE_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_REMETENTE_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as Remetentes, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Responsavel":
                    if (!select.Contains(" Responsavel, "))
                    {
                        select.Append("(");
                        select.Append("    SELECT ");
                        select.Append("        CASE ");
                        select.Append("            WHEN ResponsavelBase = 0 THEN 'Remetente' ");
                        select.Append("            WHEN ResponsavelBase = 1 THEN 'Expedidor' ");
                        select.Append("            WHEN ResponsavelBase = 2 THEN 'Recebedor' ");
                        select.Append("            WHEN ResponsavelBase = 3 THEN 'Destinatário' ");
                        select.Append("            WHEN ResponsavelBase = 4 THEN 'Outros' ");
                        select.Append("            WHEN ResponsavelBase = 5 THEN 'Intermediário' ");
                        select.Append("            ELSE '' ");
                        select.Append("        END AS Responsavel ");
                        select.Append("    FROM ( ");
                        select.Append("        SELECT ");
                        select.Append("            COALESCE( ");
                        select.Append("                OcorrenciaResponsavel.COC_RESPONSAVEL, ");
                        select.Append("                (SELECT TOP 1 Cte.CON_TOMADOR ");
                        select.Append("                 FROM T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("                 JOIN T_CARGA_CTE CargaCTe ON CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("                 JOIN T_CTE Cte ON Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("                 WHERE CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("                ) ");
                        select.Append("            ) AS ResponsavelBase ");
                        select.Append("        FROM T_CARGA_OCORRENCIA OcorrenciaResponsavel ");
                        select.Append("        WHERE OcorrenciaResponsavel.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("    ) AS TipoResponsavel ");
                        select.Append(") AS Responsavel, ");

                        if (!groupBy.Contains("Ocorrencia.COC_RESPONSAVEL,"))
                            groupBy.Append("Ocorrencia.COC_RESPONSAVEL, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Setor":
                    if (!select.Contains(" Setor, "))
                    {
                        select.Append("(");
                        select.Append("    case  ");
                        select.Append($"       when Ocorrencia.COC_RESPONSAVEL = {(int)TipoTomador.Remetente} then '' ");
                        select.Append($"       when Ocorrencia.COC_RESPONSAVEL = {(int)TipoTomador.Destinatario} then '' ");
                        select.Append("        else ( select TOP 1 SetorResponsavel.SET_DESCRICAO ");
                        select.Append("                   from T_CARGA_OCORRENCIA_AUTORIZACAO CargaAutorizacao ");
                        select.Append("                   join T_FUNCIONARIO ResponsavelAutorizacao on ResponsavelAutorizacao.FUN_CODIGO = CargaAutorizacao.FUN_CODIGO ");
                        select.Append("                   join T_SETOR SetorResponsavel ON SetorResponsavel.SET_CODIGO = ResponsavelAutorizacao.SET_CODIGO ");
                        select.Append("                  where CargaAutorizacao.COA_ETAPA_AUTORIZACAO = ( ");
                        select.Append("                           case  ");
                        select.Append($"                              when Ocorrencia.COC_SITUACAO_OCORRENCIA = {(int)SituacaoOcorrencia.AgAprovacao} then {(int)EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia} ");
                        select.Append($"                              else {(int)EtapaAutorizacaoOcorrencia.EmissaoOcorrencia} ");
                        select.Append("                           end ");
                        select.Append("                        ) ");

                        select.Append("                  and CargaAutorizacao.COA_SITUACAO = ( ");
                        select.Append("                           case  ");
                        select.Append($"                              when Ocorrencia.COC_SITUACAO_OCORRENCIA = {(int)SituacaoOcorrencia.Finalizada} then {(int)SituacaoOcorrenciaAutorizacao.Aprovada} ");
                        select.Append($"                              else {(int)SituacaoOcorrenciaAutorizacao.Pendente} ");
                        select.Append("                           end ");
                        select.Append("                        ) ");

                        select.Append("                    and CargaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("                     order by CargaAutorizacao.COA_DATA DESC ");
                        select.Append("             ) ");
                        select.Append("    end");
                        select.Append(") as Setor, ");

                        if (!groupBy.Contains("Ocorrencia.COC_RESPONSAVEL,"))
                            groupBy.Append("Ocorrencia.COC_RESPONSAVEL, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Ocorrencia.COC_SITUACAO_OCORRENCIA,"))
                            groupBy.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA, ");
                    }
                    break;

                case "Situacao":
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA as Situacao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_SITUACAO_OCORRENCIA,"))
                            groupBy.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA, ");
                    }
                    break;

                case "SituacaoCancelamento":
                case "DescricaoSituacaoCancelamento":
                    if (!select.Contains(" SituacaoCancelamento,"))
                    {
                        select.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA_NO_CANCELAMENTO as SituacaoCancelamento, ");
                        groupBy.Append("Ocorrencia.COC_SITUACAO_OCORRENCIA_NO_CANCELAMENTO, ");
                    }
                    break;

                case "Solicitante":
                    if (!select.Contains(" Solicitante, "))
                    {
                        select.Append("coalesce(Solicitante.FUN_NOME, Usuario.FUN_NOME) as Solicitante, ");
                        groupBy.Append("Solicitante.FUN_NOME, Usuario.FUN_NOME, ");

                        SetarJoinsSolicitante(joins);
                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) ( ");
                        select.Append("               case ");
                        select.Append("                   when Veiculo.MVC_CODIGO is not null then ModeloVeicularVeiculo.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ");
                        select.Append("                   else ModeloVeicularCarga.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ");
                        select.Append("               end ");
                        select.Append("           ) ");
                        select.Append("      from T_VEICULO Veiculo ");
                        select.Append("      left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
                        select.Append("      left join T_MODELO_VEICULAR_CARGA ModeloVeicularVeiculo on ModeloVeicularVeiculo.MVC_CODIGO = Veiculo.MVC_CODIGO ");
                        select.Append("     where Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                        select.Append(") as TipoVeiculo, ");

                        groupBy.Append("Carga.MVC_CODIGO, Carga.CAR_VEICULO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CodigoIntegracaoTomador":
                    if (!select.Contains(" CodigoIntegracaoTomador, "))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) TomadorCTe.PCT_CODIGO_INTEGRACAO ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("      join T_CTE_PARTICIPANTE TomadorCTe on TomadorCTe.PCT_CODIGO = Cte.CON_TOMADOR_PAGADOR_CTE ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as CodigoIntegracaoTomador, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("( ");
                        select.Append("    select top(1) TomadorCTe.PCT_NOME ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("      join T_CTE_PARTICIPANTE TomadorCTe on TomadorCTe.PCT_CODIGO = Cte.CON_TOMADOR_PAGADOR_CTE ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as Tomador, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Transportadora":
                    if (!select.Contains(" Transportadora, "))
                    {
                        select.Append("isnull(Transportador.EMP_RAZAO, OutroEmitente.EMP_RAZAO) Transportadora, ");
                        groupBy.Append("Transportador.EMP_RAZAO, OutroEmitente.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                        SetarJoinsOutroEmitente(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor,"))
                    {
                        select.Append("Ocorrencia.COC_VALOR_OCORRENCIA as Valor, ");
                        groupBy.Append("Ocorrencia.COC_VALOR_OCORRENCIA, ");
                    }
                    break;

                case "CodigoIntegracaoFilial":
                    if (!select.Contains(" CodigoIntegracaoFilial,"))
                    {
                        select.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR as CodigoIntegracaoFilial, ");
                        groupBy.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilialFormatado":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append("Filial.FIL_CNPJ as CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ChaveCTeComp":
                    if (!select.Contains(" ChaveCTeComp,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Cte.CON_CHAVECTE ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as ChaveCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataEmissaoCTeComp":
                    if (!select.Contains(" DataEmissaoCTeComp,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + convert(varchar(10), Cte.CON_DATAHORAEMISSAO, 103) ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as DataEmissaoCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ValorReceberCTeComp":
                    if (!select.Contains(" ValorReceberCTeComp,"))
                    {
                        select.Append("(    select sum(ISNULL(Cte.CON_VALOR_RECEBER, 0))");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ValorReceberCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ValorIcmsCTeComp":
                    if (!select.Contains(" ValorIcmsCTeComp,"))
                    {
                        select.Append("(    select sum(ISNULL(Cte.CON_VAL_ICMS, 0))");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ValorIcmsCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ValorCBSCTeComp":
                    if (!select.Contains(" ValorCBSCTeComp,"))
                    {
                        select.Append("(    select sum(ISNULL(Cte.CON_VALOR_CBS, 0))");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ValorCBSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CSTIBSCBSCTeComp":
                    if (!select.Contains(" CSTIBSCBSCTeComp,"))
                    {
                        select.Append("(    select top(1) Cte.CON_CST_IBSCBS");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as CSTIBSCBSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ClassTribIBSCBSCTeComp":
                    if (!select.Contains(" ClassTribIBSCBSCTeComp,"))
                    {
                        select.Append("(    select top(1) Cte.CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ClassTribIBSCBSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ValorIBSMunicipalCTeComp":
                    if (!select.Contains(" ValorIBSMunicipalCTeComp,"))
                    {
                        select.Append("(    select sum(ISNULL(Cte.CON_VALOR_IBS_MUNICIPAL, 0))");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ValorCBSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "ValorIBSUFCTeComp":
                    if (!select.Contains(" ValorIBSUFCTeComp,"))
                    {
                        select.Append("(    select sum(ISNULL(Cte.CON_VALOR_IBS_ESTADUAL, 0))");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append(") as ValorCBSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "StatusCTeComp":
                    if (!select.Contains(" StatusCTeComp, "))
                    {
                        select.Append("substring(( ");
                        select.Append(@"    select distinct ', ' + CASE Cte.CON_STATUS 
		                                                                WHEN 'A' THEN 'Autorizado' 
		                                                                WHEN 'P' THEN 'Pendente' 
		                                                                WHEN 'E' THEN 'Enviado' 
		                                                                WHEN 'R' THEN 'Rejeitado' 
		                                                                WHEN 'C' THEN 'Cancelado' 
		                                                                WHEN 'I' THEN 'Inutilizado' 
		                                                                WHEN 'D' THEN 'Denegado' 
		                                                                WHEN 'S' THEN 'Em Digitação' 
		                                                                WHEN 'K' THEN 'Em Cancelamento' 
		                                                                WHEN 'L' THEN 'Em Inutilização' 
                                                                        WHEN 'Z' THEN 'Anulado' 
		                                                                ELSE ''
                                                                    END ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as StatusCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "RetornoSefazCTeComp":
                    if (!select.Contains(" RetornoSefazCTeComp, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Cte.CON_RETORNOCTE ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as RetornoSefazCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CSTICMSCTeComp":
                    if (!select.Contains(" CSTICMSCTeComp,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Cte.CON_CST ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCC_CODIGO = CargaCTe.CCC_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CSTICMSCTeComp, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "TipoOperacaoCarga":
                    if (!select.Contains(" TipoOperacaoCarga, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacaoCarga, ");

                        if (!groupBy.Contains("TipoOperacao.TOP_DESCRICAO,"))
                            groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "TipoCreditoDebito":
                    if (!select.Contains(" TipoCreditoDebito, "))
                    {
                        select.Append(" CASE ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO ");
                        select.Append("     WHEN 1 ");
                        select.Append("         THEN 'DÉBITO' ");
                        select.Append("     WHEN 0 ");
                        select.Append("         THEN 'CRÉDITO' ");
                        select.Append("     ELSE ");
                        select.Append("         '' END AS TipoCreditoDebito, ");

                        if (!groupBy.Contains("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO,"))
                            groupBy.Append("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;

                case "CargaPeriodo":
                    if (!select.Contains(" CargaPeriodo, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + cast(CargaOcorrenciaCarga.CAR_CODIGO AS nvarchar(20)) ");
                        select.Append("      FROM T_CARGA_OCORRENCIA_CARGAS CargaOcorrenciaCarga ");
                        select.Append("      WHERE CargaOcorrenciaCarga.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("      FOR XML path('')), 3, 1000) AS CargaPeriodo, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "MesPeriodo":
                    if (!select.Contains(" MesPeriodo, "))
                    {
                        select.Append("CAST(Month(Ocorrencia.COC_PERIODO_INICIO) AS nvarchar(2)) AS MesPeriodo, ");

                        if (!groupBy.Contains("Ocorrencia.COC_PERIODO_INICIO,"))
                            groupBy.Append("Ocorrencia.COC_PERIODO_INICIO, ");
                    }
                    break;

                case "AnoPeriodo":
                    if (!select.Contains(" AnoPeriodo, "))
                    {
                        select.Append("CAST(Year(Ocorrencia.COC_PERIODO_INICIO) AS nvarchar(4)) AS AnoPeriodo, ");

                        if (!groupBy.Contains("Ocorrencia.COC_PERIODO_INICIO,"))
                            groupBy.Append("Ocorrencia.COC_PERIODO_INICIO, ");
                    }
                    break;

                case "TipoDocumentoFormatado":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select.Append("    (select top 1 ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO = CargaCTe.CCT_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("      join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = Cte.CON_MODELODOC ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null) as TipoDocumento, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NumeroAcerto":
                    if (!select.Contains(" NumeroAcerto, "))
                    {
                        select.Append("    (select top 1 acertoViagem.ACV_NUMERO ");
                        select.Append("      from T_ACERTO_DE_VIAGEM acertoViagem ");
                        select.Append("      join T_ACERTO_OCORRENCIA acertoOcorrencia on acertoOcorrencia.ACV_CODIGO = acertoViagem.ACV_CODIGO ");
                        select.Append("     where acertoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and acertoViagem.ACV_SITUACAO <> 3) as NumeroAcerto, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "NomeFantasiaDestinatarios":
                    if (!select.Contains(" NomeFantasiaDestinatarios, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(ClienteCte.CLI_NOMEFANTASIA, ClientePreCte.CLI_NOMEFANTASIA) ");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as NomeFantasiaDestinatarios, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "Pedidos":
                case "PedidosFormatado":
                    if (!select.Contains(" Pedidos, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + 
										                cast(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as varchar(30)) 
										                FROM T_CARGA_ENTREGA_PEDIDO cep
										                inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = cep.CPE_CODIGO 
										                inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
										                WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO 
						                for xml path('')),3,1000) Pedidos, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "Destinos":
                    if (!select.Contains(" Destinos, "))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + 
									                CASE isnull(ClienteEntrega.CLI_NOMEFANTASIA, '') WHEN '' THEN ClienteEntrega.CLI_NOME ELSE ClienteEntrega.CLI_NOMEFANTASIA END
									                + ' (' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA + ')' AS[text()]
									                FROM T_CARGA_ENTREGA CargaEntrega
									                JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
									                JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
									                WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
									                ORDER BY CargaEntrega.CEN_ORDEM
						                FOR XML PATH, TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) Destinos,");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CNPJDestinatariosFormatado":
                    if (!select.Contains(" CNPJDestinatarios, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + ISNULL(CAST(LTRIM(STR(ClienteCte.CLI_CGCCPF,50)) AS NVARCHAR(50)), CAST(LTRIM(STR(ClientePreCte.CLI_CGCCPF,50)) AS NVARCHAR(50)))");
                        select.Append("      from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                        select.Append("      join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");

                        select.Append("      left join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipanteCte on ParticipanteCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClienteCte on ClienteCte.CLI_CGCCPF = ParticipanteCte.CLI_CODIGO ");

                        select.Append("      left join T_PRE_CTE PreCte on PreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                        select.Append("      left join T_CTE_PARTICIPANTE ParticipantePreCte on ParticipantePreCte.PCT_CODIGO = PreCte.PCO_DESTINATARIO_CTE ");
                        select.Append("      left join T_CLIENTE ClientePreCte on ClientePreCte.CLI_CGCCPF = ParticipantePreCte.CLI_CODIGO ");

                        select.Append("      where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CNPJDestinatarios, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataCarregamentoFormatada":
                    if (!select.Contains(" DataCarregamento, "))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;


                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ParametroOcorrencia"))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("     SELECT ', ' + ");
                        select.Append("         CASE POC.POC_TIPO_PARAMETRO ");
                        select.Append("             WHEN 1 THEN (CONVERT(VARCHAR(10),COP.COC_DATA_INICIO,103) + ' ATÉ ' + CONVERT(VARCHAR(10),COP.COC_DATA_FIM,103)) ");
                        select.Append("             WHEN 2 THEN CAST(COP.COP_BOOLEANO AS VARCHAR(1)) ");
                        select.Append("             WHEN 3 THEN CAST(COP.COC_TEXTO AS VARCHAR(1000)) ");
                        select.Append("             WHEN 4 THEN CAST(COP.COC_TEXTO AS VARCHAR(1000)) ");
                        select.Append("             WHEN 5 THEN FORMAT(COP.COC_DATA, 'dd/MM/yyyy HH:mm:ss') ");
                        select.Append("             ELSE '' ");
                        select.Append("         END ");
                        select.Append("     FROM T_CARGA_OCORRENCIA_PARAMETROS COP ");
                        select.Append("         INNER JOIN T_PARAMETROS_OCORRENCIA POC ON COP.POC_CODIGO = POC.POC_CODIGO ");
                        select.Append($"     WHERE COP.COC_CODIGO = Ocorrencia.COC_CODIGO AND COP.POC_CODIGO = {codigoDinamico} ");
                        select.Append("     FOR XML PATH('') ");
                        select.Append($"), 3, 1000) {propriedade}, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "DataInicioEstadia":
                    if (!select.Contains(" DataInicioEstadia, "))
                    {
                        select.Append("CONVERT(nvarchar(10), Ocorrencia.COC_DATA_INICIAL_ESTADIA, 103) + ' ' + CONVERT(nvarchar(5), Ocorrencia.COC_DATA_INICIAL_ESTADIA, 8) DataInicioEstadia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_DATA_INICIAL_ESTADIA,"))
                            groupBy.Append("Ocorrencia.COC_DATA_INICIAL_ESTADIA, ");
                    }
                    break;

                case "DataFimEstadia":
                    if (!select.Contains(" DataFimEstadia, "))
                    {
                        select.Append("CONVERT(nvarchar(10), Ocorrencia.COC_DATA_FINAL_ESTADIA, 103) + ' ' + CONVERT(nvarchar(5), Ocorrencia.COC_DATA_FINAL_ESTADIA, 8) DataFimEstadia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_DATA_FINAL_ESTADIA,"))
                            groupBy.Append("Ocorrencia.COC_DATA_FINAL_ESTADIA, ");
                    }
                    break;

                case "HorasTotaisEstadia":
                    if (!select.Contains(" HorasTotaisEstadia, "))
                    {
                        select.Append("CAST(DATEDIFF(MINUTE, Ocorrencia.COC_DATA_INICIAL_ESTADIA, Ocorrencia.COC_DATA_FINAL_ESTADIA) / 60 AS VARCHAR(8)) + ':' + FORMAT(DATEDIFF(MINUTE, Ocorrencia.COC_DATA_INICIAL_ESTADIA, Ocorrencia.COC_DATA_FINAL_ESTADIA) % 60, 'D2') HorasTotaisEstadia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_DATA_FINAL_ESTADIA,"))
                            groupBy.Append("Ocorrencia.COC_DATA_FINAL_ESTADIA, ");

                        if (!groupBy.Contains("Ocorrencia.COC_DATA_INICIAL_ESTADIA,"))
                            groupBy.Append("Ocorrencia.COC_DATA_INICIAL_ESTADIA, ");
                    }
                    break;

                case "HorasExcedentesEstadia":
                    if (!select.Contains(" HorasExcedentesEstadia, "))
                    {
                        select.Append("CAST(FLOOR(Ocorrencia.COC_HORAS_ESTADIA) AS VARCHAR(8)) + ':' + FORMAT(CONVERT(int, FLOOR((Ocorrencia.COC_HORAS_ESTADIA - FLOOR(Ocorrencia.COC_HORAS_ESTADIA)) * 60)), 'D2') HorasExcedentesEstadia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_HORAS_ESTADIA,"))
                            groupBy.Append("Ocorrencia.COC_HORAS_ESTADIA, ");
                    }
                    break;

                case "HorasFreetime":
                    if (!select.Contains(" HorasFreetime, "))
                    {
                        select.Append("CAST(FLOOR(Ocorrencia.COC_HORAS_FREETIME) AS VARCHAR(8)) + ':' + FORMAT(CONVERT(int, FLOOR((Ocorrencia.COC_HORAS_FREETIME - FLOOR(Ocorrencia.COC_HORAS_FREETIME)) * 60)), 'D2') HorasFreetime, ");

                        if (!groupBy.Contains("Ocorrencia.COC_HORAS_FREETIME,"))
                            groupBy.Append("Ocorrencia.COC_HORAS_FREETIME, ");
                    }
                    break;

                case "EtapaEstadia":
                    if (!select.Contains(" EtapaEstadia, "))
                    {
                        select.Append("CASE Ocorrencia.COC_TIPO_CARGA_ENTREGA WHEN 1 THEN 'Entrega' WHEN 2 THEN 'Coleta' WHEN 3 THEN 'Fronteira' ELSE 'Nenhum' END EtapaEstadia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_TIPO_CARGA_ENTREGA,"))
                            groupBy.Append("Ocorrencia.COC_TIPO_CARGA_ENTREGA, ");
                    }
                    break;

                case "ValorOriginal":
                    if (!select.Contains(" ValorOriginal, "))
                    {
                        select.Append("ISNULL(Ocorrencia.COC_VALOR_ORIGINAL_OCORRENCIA, 0) ValorOriginal, ");

                        if (!groupBy.Contains("Ocorrencia.COC_VALOR_ORIGINAL_OCORRENCIA,"))
                            groupBy.Append("Ocorrencia.COC_VALOR_ORIGINAL_OCORRENCIA, ");
                    }
                    break;


                case "Pagamento":
                case "PagamentoFormatado":
                    if (!select.Contains(" Pagamento, "))
                    {
                        select.Append("Ocorrencia.COC_PAGAMENTO Pagamento, ");

                        if (!groupBy.Contains("Ocorrencia.COC_PAGAMENTO,"))
                            groupBy.Append("Ocorrencia.COC_PAGAMENTO, ");
                    }
                    break;

                case "ChavesCTeOriginal":
                    if (!select.Contains(" ChavesCTeOriginal,"))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + convert(nvarchar(50), Cte.CON_CHAVECTE) ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO = CargaCTe.CCT_CODIGO ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       and CargaCTe.CON_CODIGO is not null ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as ChavesCTeOriginal, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "JustificativaOcorrencia":
                    if (!select.Contains(" JustificativaOcorrencia, "))
                    {
                        select.Append(@"SUBSTRING(
                                            (SELECT DISTINCT ', ' + MotivoChamado.MCH_DESCRICAO
                                            FROM T_MOTIVO_CHAMADA MotivoChamado
                                            JOIN T_CHAMADOS chamado on chamado.MCH_CODIGO = MotivoChamado.MCH_CODIGO
				                            JOIN T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.CHA_CODIGO = chamado.CHA_CODIGO
				                            WHERE chamadoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO
                                                FOR XML PATH('')), 3, 2000) JustificativaOcorrencia, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");
                    }
                    break;

                case "CodigoAprovacao":
                    if (!select.Contains(" CodigoAprovacao, "))
                    {
                        select.Append(@"Ocorrencia.COC_CODIGO_APROVACAO CodigoAprovacao, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO_APROVACAO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO_APROVACAO, ");
                    }
                    break;
                case "CPFMotorista":
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select ', ' + Motorista.FUN_CPF ");
                        select.Append("      from T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      join T_FUNCIONARIO Motorista on CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     where CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as CPFMotorista, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO,"))
                            groupBy.Append("Ocorrencia.COC_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_ORIGENS as Origem, ");

                        if (!groupBy.Contains("CargaDadosSumarizados.CDS_ORIGENS,"))
                            groupBy.Append("CargaDadosSumarizados.CDS_ORIGENS, ");
                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;
                case "ProtocoloOcorrencia":
                    if (!select.Contains(" ProtocoloOcorrencia,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + cast(Cte.CON_CODIGO as nvarchar(20)) ");
                        select.Append("      from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                        select.Append("      join T_CTE Cte on Cte.CON_CODIGO = CargaCteComplementoInfo.CON_CODIGO ");
                        select.Append("     where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) as ProtocoloOcorrencia, ");
                    }
                    break;
                case "GrupoOcorrencia":
                    if (!select.Contains(" GrupoOcorrencia, "))
                    {
                        select.Append("GrupoOcorrencia.GTO_DESCRICAO as GrupoOcorrencia, ");
                        groupBy.Append("GrupoOcorrencia.GTO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);
                        SetarJoinsGrupoOcorrencia(joins);
                    }
                    break;
            }

        }
        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append($" and Ocorrencia.COC_INATIVA = 0");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CargaAgrupada))
            {
                where.Append($" AND CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CargaAgrupada}'");
                SetarJoinsCargaAgrupada(joins);
            }

            if (filtrosPesquisa.DataSolicitacaoInicial.HasValue)
                where.Append($" and CAST(Ocorrencia.COC_DATA_OCORRENCIA AS DATE) >= '{filtrosPesquisa.DataSolicitacaoInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataSolicitacaoFinal.HasValue)
                where.Append($" and CAST(Ocorrencia.COC_DATA_OCORRENCIA AS DATE) <= '{filtrosPesquisa.DataSolicitacaoFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.NumeroOcorrenciaInicial > 0)
                where.Append($" and Ocorrencia.COC_NUMERO_CONTRATO >= {filtrosPesquisa.NumeroOcorrenciaInicial}");

            if (filtrosPesquisa.NumeroOcorrenciaFinal > 0)
                where.Append($" and Ocorrencia.COC_NUMERO_CONTRATO <= {filtrosPesquisa.NumeroOcorrenciaFinal}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrenciaCliente))
                where.Append($" and Ocorrencia.COC_NUMERO_OCORRENCIA_CLIENTE like '%{filtrosPesquisa.NumeroOcorrenciaCliente}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                //where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = :CARGA_CAR_CODIGO_CARGA_EMBARCADOR");
                parametros.Add(new Consulta.ParametroSQL("CARGA_CAR_CODIGO_CARGA_EMBARCADOR", filtrosPesquisa.CodigoCargaEmbarcador));
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.Recebedores)})))");
                SetarJoinsCarga(joins);
            }

            else if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append($" and Carga.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas}");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosOcorrencia.Count > 0)
                where.Append($" and Ocorrencia.OCO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOcorrencia)})");

            if (filtrosPesquisa.CodigoRecebedor > 0)
            {
                where.Append($" and SolicitacaoCredito.FUN_CODIGO_CREDITOR = {filtrosPesquisa.CodigoRecebedor}");
                SetarJoinsSolicitacaoCredito(joins);
            }

            if (filtrosPesquisa.CodigoSolicitante.Count > 0)
            {
                where.Append($" and (SolicitacaoCredito.FUN_CODIGO_SOLICITANTE in ({string.Join(", ", filtrosPesquisa.CodigoSolicitante)}) or Ocorrencia.FUN_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoSolicitante)})) ");
                SetarJoinsSolicitacaoCredito(joins);
            }

            if (filtrosPesquisa.CodigoGrupoOcorrencia.Count > 0)
            {
                where.Append($" and GrupoOcorrencia.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoGrupoOcorrencia)})");
                SetarJoinsTipoOcorrencia(joins);
                SetarJoinsGrupoOcorrencia(joins);
            }

            if (filtrosPesquisa.CodigoResponsavelChamado > 0)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CHAMADOS chamado ");
                where.Append("           join T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.CHA_CODIGO = chamado.CHA_CODIGO ");
                where.Append("          where chamadoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"           and chamado.FUN_RESPONSAVEL = {filtrosPesquisa.CodigoResponsavelChamado} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.CodigoTransportadorChamado > 0)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CHAMADOS chamado ");
                where.Append("           join T_CHAMADO_OCORRENCIA chamadoOcorrencia on chamadoOcorrencia.CHA_CODIGO = chamado.CHA_CODIGO ");
                where.Append("          where chamadoOcorrencia.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"           and chamado.EMP_CODIGO = {filtrosPesquisa.CodigoTransportadorChamado} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.CodigosTransportadorCarga?.Count > 0)
            {
                where.Append($" and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportadorCarga)}) ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.SituacoesOcorrencia?.Count > 0)
            {
                string situacoes = string.Join(", ", (from situacao in filtrosPesquisa.SituacoesOcorrencia where situacao != SituacaoOcorrencia.Anulada select (int)situacao));

                if (!string.IsNullOrWhiteSpace(situacoes))
                    where.Append($" and Ocorrencia.COC_SITUACAO_OCORRENCIA in ({situacoes})");
            }

            if (filtrosPesquisa.SituacoesCancelamento?.Count > 0)
            {
                string situacoes = string.Join(", ", (from situacao in filtrosPesquisa.SituacoesCancelamento select (int)situacao));

                where.Append($" and Ocorrencia.COC_SITUACAO_OCORRENCIA_NO_CANCELAMENTO in ({situacoes})");
            }

            if (filtrosPesquisa.ValorInicial > 0m)
                where.Append($" and Ocorrencia.COC_VALOR_OCORRENCIA >= {filtrosPesquisa.ValorInicial.ToString("n2").Replace(".", "").Replace(",", ".")}");

            if (filtrosPesquisa.ValorFinal > 0m)
                where.Append($" and Ocorrencia.COC_VALOR_OCORRENCIA <= {filtrosPesquisa.ValorFinal.ToString("n2").Replace(".", "").Replace(",", ".")}");

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_MOTORISTA CargaMotorista ");
                where.Append("          where CargaMotorista.CAR_CODIGO = Ocorrencia.CAR_CODIGO ");
                where.Append($"           and CargaMotorista.CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append(" and ( ");
                where.Append($"        Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or ");
                where.Append("         exists ( ");
                where.Append("             select top(1) 1 ");
                where.Append("               from T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados ");
                where.Append("              where VeiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"               and VeiculosVinculados.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");
                where.Append("         ) ");
                where.Append("     )");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
            {
                where.Append(" and exists (");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_PEDIDO CargaPedido ");
                where.Append("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                where.Append("          where CargaPedido.CAR_CODIGO = Ocorrencia.CAR_CODIGO ");
                where.Append($"           and Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjPessoa.ToString("F0")} ");
                where.Append("     ) ");
            }

            if (filtrosPesquisa.DataCancelamentoInicial.HasValue || filtrosPesquisa.DataCancelamentoFinal.HasValue)
            {
                where.Append(" and exists ( ");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_OCORRENCIA_CANCELAMENTO OcorrenciaCancelamento ");
                where.Append("          where OcorrenciaCancelamento.COC_CODIGO = Ocorrencia.COC_CODIGO ");

                if (filtrosPesquisa.DataCancelamentoInicial.HasValue)
                    where.Append($"       and CAST(OcorrenciaCancelamento.CAO_DATA_CANCELAMENTO AS DATE) >= '{filtrosPesquisa.DataCancelamentoInicial.Value.ToString(pattern)}'");

                if (filtrosPesquisa.DataCancelamentoFinal.HasValue)
                    where.Append($"       and CAST(OcorrenciaCancelamento.CAO_DATA_CANCELAMENTO AS DATE) <= '{filtrosPesquisa.DataCancelamentoFinal.Value.ToString(pattern)}'");

                if (filtrosPesquisa.SituacoesOcorrencia.All(o => o == SituacaoOcorrencia.Anulada))
                    where.Append($"       and OcorrenciaCancelamento.CAO_TIPO = {(int)TipoCancelamentoOcorrencia.Anulacao}");
                else if (filtrosPesquisa.SituacoesOcorrencia.All(o => o == SituacaoOcorrencia.Cancelada))
                    where.Append($"       and OcorrenciaCancelamento.CAO_TIPO = {(int)TipoCancelamentoOcorrencia.Cancelamento}");

                where.Append("     ) ");
            }

            if (filtrosPesquisa.DataOcorrenciaInicial.HasValue || filtrosPesquisa.DataOcorrenciaFinal.HasValue)
            {
                where.Append(" and ( ");
                where.Append("         ( ");
                where.Append("             select top(1) CAST(OcorrenciaAutorizacao.COA_DATA AS DATE) ");
                where.Append("               from T_CARGA_OCORRENCIA_AUTORIZACAO OcorrenciaAutorizacao ");
                where.Append($"             where OcorrenciaAutorizacao.COA_SITUACAO in ({(int)SituacaoOcorrenciaAutorizacao.Aprovada}, {(int)SituacaoOcorrenciaAutorizacao.Rejeitada}) ");
                where.Append("                and OcorrenciaAutorizacao.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append("               order by OcorrenciaAutorizacao.COA_DATA desc ");
                where.Append("         ) ");

                if (filtrosPesquisa.DataOcorrenciaInicial.HasValue && filtrosPesquisa.DataOcorrenciaFinal.HasValue)
                    where.Append($" between '{filtrosPesquisa.DataOcorrenciaInicial.Value.ToString(pattern)}' and '{filtrosPesquisa.DataOcorrenciaFinal.Value.ToString(pattern)}'");
                else if (filtrosPesquisa.DataOcorrenciaInicial.HasValue)
                    where.Append($" >= '{filtrosPesquisa.DataOcorrenciaInicial.Value.ToString(pattern)}'");
                else
                    where.Append($" <= '{filtrosPesquisa.DataOcorrenciaFinal.Value.ToString(pattern)}'");

                where.Append("     ) ");
            }

            if (filtrosPesquisa.NumeroCTeGerado > 0)
            {
                where.Append(" and exists ( ");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                where.Append("           join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO ");
                where.Append("           join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                where.Append("          where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"           and Cte.CON_NUM = {filtrosPesquisa.NumeroCTeGerado} ");
                where.Append("     )");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.NumeroCTeOriginal > 0)
            {
                where.Append(" and exists ( ");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_CTE CargaCte ");
                where.Append("           join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                where.Append("          where CargaCte.CAR_CODIGO = Ocorrencia.CAR_CODIGO ");
                where.Append($"           and Cte.CON_NUM = {filtrosPesquisa.NumeroCTeOriginal} ");
                where.Append("     )");
            }

            if (filtrosPesquisa.NumeroCTeOriginal > 0)
            {
                where.Append(" and exists ( ");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                where.Append("           join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO ");
                where.Append("           join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                where.Append("          where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"           and Cte.CON_NUM = {filtrosPesquisa.NumeroCTeOriginal} ");
                where.Append("     )");
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                where.Append(" and (case when ISNULL(Ocorrencia.COC_UTILIZAR_SELECAO_POR_NOTAS_FISCAIS_CTE, 0) = 1 then ");
                where.Append("          (select top(1) 1 ");
                where.Append("             from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                where.Append("             join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                where.Append("             join T_CARGA_OCORRENCIA_DOCUMENTO_XML_NOTAS_FISCAIS OcorrenciaDocumentoNotasFiscais on OcorrenciaDocumentoNotasFiscais.COD_CODIGO = OcorrenciaDocumento.COD_CODIGO ");
                where.Append("             join T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = OcorrenciaDocumentoNotasFiscais.NFX_CODIGO ");
                where.Append("            where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"             and NotaFiscal.NF_NUMERO = '{filtrosPesquisa.NumeroNotaFiscal}') ");
                where.Append("      else ");
                where.Append("          (select top(1) 1 ");
                where.Append("             from T_CARGA_OCORRENCIA_DOCUMENTO OcorrenciaDocumento ");
                where.Append("             join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = OcorrenciaDocumento.CCT_CODIGO ");
                where.Append("             left join T_CTE_DOCS DocumentoCte on DocumentoCte.CON_CODIGO = CargaCte.CON_CODIGO ");
                where.Append("             left join T_PRE_CTE_DOCS DocumentoPreCte on DocumentoPreCte.PCO_CODIGO = CargaCte.PCO_CODIGO ");
                where.Append("            where OcorrenciaDocumento.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"             and ISNULL(DocumentoCte.NFC_NUMERO, DocumentoPreCte.PNF_NUMERO) = '{filtrosPesquisa.NumeroNotaFiscal}') ");
                where.Append("      end) = 1 ");
            }

            if (filtrosPesquisa.TiposOperacaoCarga.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposOperacaoCarga)})");

            if (filtrosPesquisa.TipoDocumentoCreditoDebito != TipoDocumentoCreditoDebito.Todos)
            {
                SetarJoinsModeloDocumentoFiscal(joins);

                if (filtrosPesquisa.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito)
                    where.Append("  and ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO = 1");
                else
                    where.Append("  and ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO != 1");
            }

            if (filtrosPesquisa.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Todos)
            {
                where.Append(" and exists ( ");
                where.Append("         select ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO ");
                where.Append("           from T_CARGA_CTE_COMPLEMENTO_INFO CargaCteComplementoInfo ");
                where.Append("           join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = CargaCteComplementoInfo.CCT_CODIGO_COMPLEMENTADO ");
                where.Append("           join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
                where.Append("           join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = Cte.CON_MODELODOC ");
                where.Append("          where CargaCteComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO ");
                where.Append($"           and ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO = {(int)filtrosPesquisa.TipoDocumentoEmissao} )");
            }

            if (filtrosPesquisa.OcorrenciaEstadia.HasValue)
                where.Append($" AND Ocorrencia.COC_OCORRENCIA_DE_ESTADIA = {(filtrosPesquisa.OcorrenciaEstadia.Value ? '1' : '0')}");

            if (filtrosPesquisa.EtapaEstadia?.Count > 0)
                where.Append($" AND Ocorrencia.COC_TIPO_CARGA_ENTREGA IN ({string.Join(",", filtrosPesquisa.EtapaEstadia.Select(o => o.ToString("D")))})");
        }

        #endregion
    }
}
