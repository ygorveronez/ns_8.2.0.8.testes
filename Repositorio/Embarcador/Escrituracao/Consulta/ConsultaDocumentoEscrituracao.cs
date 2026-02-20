using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Embarcador.Escrituracao
{
    sealed class ConsultaDocumentoEscrituracao : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia>
    {
        #region Construtores

        public ConsultaDocumentoEscrituracao() : base(tabela: "T_DOCUMENTO_PROVISAO DocumentoProvisao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = DocumentoProvisao.CAR_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoFaturamento "))
                joins.Append(" LEFT JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = DocumentoProvisao.DFA_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" CTe "))
                joins.Append(" LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = DocumentoProvisao.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            if (!joins.Contains(" Destino "))
                joins.Append(" LEFT JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = DocumentoProvisao.LOC_DESTINO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" INNER JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = DocumentoProvisao.FIL_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append(" LEFT JOIN T_MODDOCFISCAL ModeloDocumentoFiscal ON ModeloDocumentoFiscal.MOD_CODIGO = DocumentoProvisao.CON_MODELODOC ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" Ocorrencia "))
                joins.Append(" LEFT JOIN T_CARGA_OCORRENCIA Ocorrencia ON Ocorrencia.COC_CODIGO = DocumentoProvisao.COC_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append(" LEFT JOIN T_LOCALIDADES Origem ON Origem.LOC_CODIGO = DocumentoProvisao.LOC_ORIGEM ");
        }

        private void SetarJoinsPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" Pagamento "))
                joins.Append(" LEFT JOIN T_PAGAMENTO Pagamento ON Pagamento.PAG_CODIGO = DocumentoProvisao.PAG_CODIGO ");
        }

        private void SetarJoinsProvisao(StringBuilder joins)
        {
            if (!joins.Contains(" Provisao "))
                joins.Append(" LEFT JOIN T_PROVISAO Provisao ON Provisao.PRV_CODIGO = DocumentoProvisao.PRV_CODIGO ");
        }

        private void SetarJoinsProvisaoCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" ProvisaoCancelamento "))
                joins.Append(" LEFT JOIN T_PROVISAO_CANCELAMENTO ProvisaoCancelamento ON ProvisaoCancelamento.CPV_CODIGO = DocumentoProvisao.CPV_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" LEFT JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = DocumentoProvisao.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsRotaFrete(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" RotaFrete "))
                joins.Append(" LEFT JOIN T_ROTA_FRETE RotaFrete ON RotaFrete.ROF_CODIGO = Carga.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoCarga "))
                joins.Append(" LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" INNER JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = DocumentoProvisao.TOP_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append(" LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = DocumentoProvisao.CLI_TOMADOR ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" INNER JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = DocumentoProvisao.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorConfiguracao "))
                joins.Append(" LEFT JOIN T_CONFIG TransportadorConfiguracao ON TransportadorConfiguracao.COF_CODIGO = Transportador.COF_CODIGO");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsCanhoto(StringBuilder joins)
        {
            if (!joins.Contains(" Canhoto "))
                joins.Append(" LEFT JOIN T_CANHOTO_NOTA_FISCAL Canhoto ON Canhoto.NFX_CODIGO = DocumentoProvisao.NFX_CODIGO ");
        }

        private void SetarJoinsXMLNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" XMLNotaFiscal "))
                joins.Append(" LEFT JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = DocumentoProvisao.NFX_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CNPJTomador":
                case "NomeTomador":
                case "TipoTomador":
                case "Tomador":
                    if (!select.Contains(" CNPJTomador, "))
                    {
                        select.Append("Tomador.CLI_CGCCPF CNPJTomador, ");
                        groupBy.Append("Tomador.CLI_CGCCPF, ");
                    }

                    if (!select.Contains(" TipoTomador, "))
                    {
                        select.Append("Tomador.CLI_FISJUR TipoTomador, ");
                        groupBy.Append("Tomador.CLI_FISJUR, ");
                    }

                    if (!select.Contains(" NomeTomador, "))
                    {
                        select.Append("Tomador.CLI_NOME NomeTomador, ");
                        groupBy.Append("Tomador.CLI_NOME, ");
                    }

                    SetarJoinsTomador(joins);
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJTransportador":
                case "NomeTransportador":
                case "Transportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");
                    }

                    if (!select.Contains(" NomeTransportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO NomeTransportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");
                    }

                    SetarJoinsTransportador(joins);
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CONCAT(Origem.LOC_DESCRICAO, ' - ' , Origem.UF_SIGLA) Origem, ");
                        groupBy.Append("Origem.LOC_DESCRICAO, Origem.UF_SIGLA, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("CONCAT(Destino.LOC_DESCRICAO, ' - ' , Destino.UF_SIGLA) Destino, ");
                        groupBy.Append("Destino.LOC_DESCRICAO, Destino.UF_SIGLA, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" _Ocorrencia, "))
                    {
                        select.Append("Ocorrencia.COC_NUMERO_CONTRATO _Ocorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "DataEmissaoOcorrenciaFormatada":
                    if (!select.Contains(" DataEmissaoOcorrencia, "))
                    {
                        select.Append("Ocorrencia.COC_DATA_OCORRENCIA DataEmissaoOcorrencia, ");
                        groupBy.Append("Ocorrencia.COC_DATA_OCORRENCIA, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataEmissaoCargaFormatada":
                    if (!select.Contains(" DataEmissaoCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO DataEmissaoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Pagamento":
                    if (!select.Contains(" Pagamento, "))
                    {
                        select.Append("Pagamento.PAG_NUMERO Pagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "Provisao":
                    if (!select.Contains(" Provisao, "))
                    {
                        select.Append("Provisao.PRV_NUMERO Provisao, ");
                        groupBy.Append("Provisao.PRV_NUMERO, ");

                        SetarJoinsProvisao(joins);
                    }
                    break;

                case "CancelamentoProvisao":
                    if (!select.Contains(" CancelamentoProvisao, "))
                    {
                        select.Append("ProvisaoCancelamento.CPV_NUMERO CancelamentoProvisao, ");
                        groupBy.Append("ProvisaoCancelamento.CPV_NUMERO, ");

                        SetarJoinsProvisaoCancelamento(joins);
                    }
                    break;

                case "NumeroNFS":
                    if (!select.Contains(" _NumeroNFS, "))
                    {
                        select.Append("DocumentoProvisao.DPV_NUMERO_DOCUMENTO _NumeroNFS, ");
                        groupBy.Append("DocumentoProvisao.DPV_NUMERO_DOCUMENTO, ");
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("DocumentoProvisao.DPV_PESO_BRUTO PesoBruto, ");
                        groupBy.Append("DocumentoProvisao.DPV_PESO_BRUTO, ");
                    }
                    break;

                case "Rota":
                    if (!select.Contains(" Rota, "))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO Rota, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;

                case "DataEmissaoNFsManualFormatada":
                    if (!select.Contains(" DataEmissaoNFsManual, "))
                    {
                        select.Append("DocumentoProvisao.DPV_DATA_EMISSAO DataEmissaoNFsManual, ");
                        groupBy.Append("DocumentoProvisao.DPV_DATA_EMISSAO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("DocumentoProvisao.DPV_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("DocumentoProvisao.DPV_DATA_EMISSAO, ");

                    }
                    break;

                case "ValorCOFINS":
                case "ValorPIS":
                    if (!select.Contains(" ValorFrete, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_PROVISAO_COMPETENCIA) ValorFrete, ");

                    if (!select.Contains(" _ValorISS, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_ISS_COMPETENCIA) _ValorISS, ");

                    if (!select.Contains(" ValorISSRetido, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_RETENCAO_ISS_COMPETENCIA) ValorISSRetido, ");

                    if (!select.Contains(" _ICMSInclusoBC, "))
                    {
                        select.Append("DocumentoProvisao.DPV_ICMS_INCLUSO_BC_COMPETENCIA _ICMSInclusoBC, ");
                        groupBy.Append("DocumentoProvisao.DPV_ICMS_INCLUSO_BC_COMPETENCIA, ");
                    }

                    if (!select.Contains(" _ISSInclusoBC, "))
                    {
                        select.Append("DocumentoProvisao.DPV_ISS_INCLUSO_BC_COMPETENCIA _ISSInclusoBC, ");
                        groupBy.Append("DocumentoProvisao.DPV_ISS_INCLUSO_BC_COMPETENCIA, ");
                    }

                    if (!select.Contains(" _TipoDocumentoEmissao, "))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO _TipoDocumentoEmissao, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }

                    if (!select.Contains(" _AliquotaCOFINS, "))
                    {
                        select.Append("SUM(TransportadorConfiguracao.COF_ALIQUOTA_COFINS) _AliquotaCOFINS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                    }

                    if (!select.Contains(" _AliquotaPIS, "))
                    {
                        select.Append("SUM(TransportadorConfiguracao.COF_ALIQUOTA_PIS) _AliquotaPIS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                    }

                    break;

                case "ValorFrete":
                case "ValorFreteSemICMS":
                case "ICMS":
                case "CST":
                    if (!select.Contains(" ValorFrete, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_PROVISAO_COMPETENCIA) ValorFrete, ");

                    if (!select.Contains(" _ICMS, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_ICMS_COMPETENCIA) _ICMS, ");

                    if (!select.Contains(" CST, "))
                    {
                        select.Append("DocumentoProvisao.DPV_CST_COMPETENCIA CST, ");
                        groupBy.Append("DocumentoProvisao.DPV_CST_COMPETENCIA, ");
                    }

                    break;

                case "Aliquota":
                    if (!select.Contains(" Aliquota, "))
                        select.Append("SUM(DocumentoProvisao.DPV_PERCENTUAL_ALICOTA_COMPETENCIA) Aliquota, ");

                    break;

                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS, "))
                        select.Append("SUM(DocumentoProvisao.DPV_PERCENTUAL_ALICOTA_ISS_COMPETENCIA) AliquotaISS, ");

                    break;

                case "ValorISS":
                case "ValorISSRetido":
                    if (!select.Contains(" _ValorISS, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_ISS_COMPETENCIA) _ValorISS, ");

                    if (!select.Contains(" ValorISSRetido, "))
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_RETENCAO_ISS_COMPETENCIA) ValorISSRetido, ");

                    break;

                case "NumeroCte":
                case "_NumeroCte":
                    if (!select.Contains(" _NumeroCte, "))
                    {
                        select.Append("CTe.CON_NUM _NumeroCte, ");
                        groupBy.Append("CTe.CON_NUM, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ModeloDocumento":
                    if (!select.Contains(" ModeloDocumento, "))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_DESCRICAO ModeloDocumento, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_DESCRICAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;

                case "CNPJDestinatario":
                case "NomeDestinatario":
                case "TipoDestinatario":
                case "Destinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF CNPJDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, ");
                    }

                    if (!select.Contains(" TipoDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_FISJUR, ");
                    }

                    if (!select.Contains(" NomeDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME NomeDestinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");
                    }

                    SetarJoinsDestinatario(joins);
                    break;

                case "CNPJRemetente":
                case "NomeRemetente":
                case "TipoRemetente":
                case "Remetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF CNPJRemetente, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, ");
                    }

                    if (!select.Contains(" TipoRemetente, "))
                    {
                        select.Append("Remetente.CLI_FISJUR TipoRemetente, ");
                        groupBy.Append("Remetente.CLI_FISJUR, ");
                    }

                    if (!select.Contains(" NomeRemetente, "))
                    {
                        select.Append("Remetente.CLI_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");
                    }

                    SetarJoinsRemetente(joins);
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "Placa":
                case "PlacaFormatada":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "DataPagamentoFormatada":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        select.Append("Pagamento.PAG_DATA_CRIACAO DataPagamento, ");
                        groupBy.Append("Pagamento.PAG_DATA_CRIACAO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "DataAprovacaoPagamentoFormatada":
                    if (!select.Contains(" DataAprovacaoPagamento, "))
                    {
                        select.Append("(select max(AAL_DATA) from T_AUTORIZACAO_ALCADA_PAGAMENTO where PAG_CODIGO = Pagamento.PAG_CODIGO and AAL_SITUACAO = 1) DataAprovacaoPagamento, ");
                        groupBy.Append("Pagamento.PAG_CODIGO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "DataDigitalizacaoCanhotoFormatada":
                    if (!select.Contains(" DataDigitalizacaoCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_DATA_DIGITALIZACAO DataDigitalizacaoCanhoto, ");
                        groupBy.Append("Canhoto.CNF_DATA_DIGITALIZACAO, ");

                        SetarJoinsCanhoto(joins);
                    }
                    break;

                case "NumeroValePedagio":
                    if (!select.Contains("NumeroValePedagio,"))
                    {
                        select.Append(@" (substring((
                                            select distinct ', ' + CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO
                                              from  T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio
                                             where CargaIntegracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO
                                               for xml path('')
                                        ), 3, 1000)
                                    ) NumeroValePedagio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroNotaFiscalServico":
                    if (!select.Contains(" NumeroNotaFiscalServico, "))
                    {
                        select.Append("XMLNotaFiscal.NF_NUMERO NumeroNotaFiscalServico, ");
                        groupBy.Append("XMLNotaFiscal.NF_NUMERO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;

                case "CPFCNPJRemetenteFormatado":
                case "CPFCNPJRemetente":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF CPFCNPJRemetente, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "DataEmissaoNotaFiscalServicoManualFormatada":
                    if (!select.Contains(" DataEmissaoNotaFiscalServicoManual, "))
                    {
                        select.Append("XMLNotaFiscal.NF_DATA_EMISSAO DataEmissaoNotaFiscalServicoManual, ");
                        groupBy.Append("XMLNotaFiscal.NF_DATA_EMISSAO, ");

                        SetarJoinsXMLNotaFiscal(joins);
                    }
                    break;
                
                case "NumeroOcorrencia":
                    if (!select.Contains(" NumeroOcorrencia, "))
                    {
                        select.Append("Ocorrencia.COC_NUMERO_CONTRATO NumeroOcorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "IDAgrupador":
                    if (!select.Contains(" IDAgrupador, "))
                    {
                        select.Append(@"SUBSTRING((
                            SELECT DISTINCT ', ' +
                            CAST(_cargaPreAgrupamentoAgrupador.PAA_CODIGO_AGRUPAMENTO AS VARCHAR(20))
                            FROM T_CARGA_PEDIDO _cargaPedido
                            INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
                            INNER JOIN T_CARGA_PRE_AGRUPAMENTO _cargaPreAgrupamento ON _cargaPreAgrupamento.CAR_CODIGO = _carga.CAR_CODIGO
                            INNER JOIN T_CARGA_PRE_AGRUPAMENTO_AGRUPADOR _cargaPreAgrupamentoAgrupador 
                                ON _cargaPreAgrupamentoAgrupador.PAA_CODIGO = _cargaPreAgrupamento.PAA_CODIGO
                            WHERE _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                            FOR XML PATH('')
                            ), 3, 1000) AS IDAgrupador, ");

                        if (!groupBy.Contains("carga.CAR_CODIGO"))
                            groupBy.Append("carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.VisualizarTambemDocumentosAguardandoProvisao)
                where.Append(" AND (DocumentoProvisao.DPV_SITUACAO = 2 OR DocumentoProvisao.DPV_SITUACAO = 3 OR DocumentoProvisao.DPV_SITUACAO = 1)");
            else
                where.Append(" AND (DocumentoProvisao.DPV_SITUACAO = 2 OR DocumentoProvisao.DPV_SITUACAO = 3)");

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                where.Append($" AND Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial}");

                SetarJoinsFilial(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" AND Transportador.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

                SetarJoinsTransportador(joins);
            }

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append($" AND CAST(DocumentoProvisao.DPV_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataEmissaoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append($" AND CAST(DocumentoProvisao.DPV_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataEmissaoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.CnpjCpfTomador > 0)
            {
                where.Append($" AND Tomador.CLI_CGCCPF = {filtrosPesquisa.CnpjCpfTomador}");

                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.DataCargaInicial != DateTime.MinValue || filtrosPesquisa.DataCargaFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataCargaInicial != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCargaInicial.ToString(pattern)}'");

                if (filtrosPesquisa.DataCargaFinal != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataCargaFinal.AddDays(1).ToString(pattern)}'");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($" AND TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.DataEmissaoCTeInicial != DateTime.MinValue || filtrosPesquisa.DataEmissaoCTeFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataEmissaoCTeInicial != DateTime.MinValue)
                    where.Append($" AND CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoCTeInicial.ToString(pattern)}'");

                if (filtrosPesquisa.DataEmissaoCTeFinal != DateTime.MinValue)
                    where.Append($" AND CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoCTeFinal.AddDays(1).ToString(pattern)}'");

                SetarJoinsCTe(joins);
            }

            if (filtrosPesquisa.DataEmissaoNotaInicial != DateTime.MinValue || filtrosPesquisa.DataEmissaoNotaFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataEmissaoNotaInicial != DateTime.MinValue)
                    where.Append($" AND CAST(XMLNotaFiscal.NF_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataEmissaoNotaInicial.ToString(pattern)}'");

                if (filtrosPesquisa.DataEmissaoNotaFinal != DateTime.MinValue)
                    where.Append($" AND CAST(XMLNotaFiscal.NF_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataEmissaoNotaFinal.ToString(pattern)}'");

                SetarJoinsXMLNotaFiscal(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroValePedagio))
            {
                where.Append($@" AND EXISTS (select CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO 
                                            from T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio 
                                            where CargaIntegracaoValePedagio.CAR_CODIGO = Carga.CAR_CODIGO and CargaIntegracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO = '{filtrosPesquisa.NumeroValePedagio}')");

                SetarJoinsCarga(joins);
            }
        }

        #endregion
    }
}
