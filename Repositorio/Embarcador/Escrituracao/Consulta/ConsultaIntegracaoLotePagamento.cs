using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escrituracao
{
    sealed class ConsultaIntegracaoLotePagamento : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento>
    {
        #region Construtores

        public ConsultaIntegracaoLotePagamento() : base(tabela: "T_PAGAMENTO_INTEGRACAO PagamentoIntegracao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" Pagamento "))
                joins.Append(" inner join T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = PagamentoIntegracao.PAG_CODIGO ");
        }

        private void SetarJoinsCancelamentoPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" CancelamentoPagamento "))
                joins.Append(" left join T_CANCELAMENTO_PAGAMENTO CancelamentoPagamento on PagamentoIntegracao.PAG_CODIGO = CancelamentoPagamento.PAG_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoFaturamento "))
                joins.Append(" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on PagamentoIntegracao.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO ");
        }

        private void SetarJoinsCte(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" Cte "))
                joins.Append(" left join T_CTE Cte on Cte.CON_CODIGO = DocumentoFaturamento.CON_CODIGO ");
        }

        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            SetarJoinsCte(joins);

            if (!joins.Contains(" EmpresaSerie "))
                joins.Append(" left join T_EMPRESA_SERIE EmpresaSerie ON Cte.CON_SERIE = EmpresaSerie.ESE_CODIGO ");
        }

        private void SetarJoinsModeloDocFiscal(StringBuilder joins)
        {
            SetarJoinsCte(joins);

            if (!joins.Contains(" ModeloDocFiscal "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocFiscal on ModeloDocFiscal.MOD_CODIGO = Cte.CON_MODELODOC ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCte(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = cte.EMP_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO_PAGAMENTO ");
        }

        private void SetarJoinFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = carga.TOP_CODIGO ");
        }

        #endregion


        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append("Cte.CON_NUM NumeroDocumento, ");
                        groupBy.Append("Cte.CON_NUM, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "SerieDocumento":
                    if (!select.Contains(" SerieDocumento, "))
                    {
                        select.Append("EmpresaSerie.ESE_NUMERO SerieDocumento, ");
                        groupBy.Append("EmpresaSerie.ESE_NUMERO, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "Chave":
                    if (!select.Contains(" Chave, "))
                    {
                        select.Append("Cte.CON_CHAVECTE Chave, ");
                        groupBy.Append("Cte.CON_CHAVECTE, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "DataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("Cte.CON_DATAHORAEMISSAO as DataEmissao, ");
                        groupBy.Append("Cte.CON_DATAHORAEMISSAO, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select.Append("ModeloDocFiscal.MOD_DESCRICAO TipoDocumento, ");
                        groupBy.Append("ModeloDocFiscal.MOD_DESCRICAO, ");

                        SetarJoinsModeloDocFiscal(joins);
                    }
                    break;

                case "Emissor":
                    if (!select.Contains(" Emissor, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Emissor, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("Cte.CON_VALOR_RECEBER ValorFrete, ");
                        groupBy.Append("Cte.CON_VALOR_RECEBER, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "NumeroPagamento":
                    if (!select.Contains(" NumeroPagamento, "))
                    {
                        select.Append("Pagamento.PAG_NUMERO NumeroPagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "SituacaoPagamentoFormatada":
                    if (!select.Contains(" SituacaoPagamento, "))
                    {
                        select.Append("Pagamento.PAG_SITUACAO SituacaoPagamento, ");
                        groupBy.Append("Pagamento.PAG_SITUACAO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "SituacaoIntegracaoFormatada":
                    if (!select.Contains(" SituacaoIntegracao, "))
                    {
                        select.Append("PagamentoIntegracao.INT_SITUACAO_INTEGRACAO SituacaoIntegracao, ");
                        groupBy.Append("PagamentoIntegracao.INT_SITUACAO_INTEGRACAO, ");
                    }
                    break;

                case "RetornoIntegracao":
                    if (!select.Contains(" RetornoIntegracao, "))
                    {
                        select.Append("PagamentoIntegracao.INT_PROBLEMA_INTEGRACAO RetornoIntegracao, ");
                        groupBy.Append("PagamentoIntegracao.INT_PROBLEMA_INTEGRACAO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinTipoOperacao(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        SetarJoinsDocumentoFaturamento(joins);
                        SetarJoinFilial(joins);

                        select.Append(@"(CASE WHEN DocumentoFaturamento.LNM_CODIGO is not null
                                            THEN (select STRING_AGG(Filial.FIL_DESCRICAO, ', ') FROM T_FILIAL Filial
                                            left join T_LANCAMENTO_NFS_MANUAL lancamentoNFsManual on lancamentoNFsManual.FIL_CODIGO = Filial.FIL_CODIGO
                                            WHERE lancamentoNFsManual.LNM_CODIGO = DocumentoFaturamento.LNM_CODIGO) 
                                            ELSE (Filial.FIL_DESCRICAO)
                                            END) Filial, ");

                        if (!groupBy.Contains("Filial.FIL_DESCRICAO,"))
                            groupBy.Append("Filial.FIL_DESCRICAO, ");

                        if (!groupBy.Contains("DocumentoFaturamento.LNM_CODIGO,"))
                            groupBy.Append("DocumentoFaturamento.LNM_CODIGO, ");
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        SetarJoinsDocumentoFaturamento(joins);

                        select.Append(@"(CASE WHEN DocumentoFaturamento.LNM_CODIGO is not null
                                            THEN (select STRING_AGG(Carga.CAR_CODIGO_CARGA_EMBARCADOR, ', ') FROM T_CARGA Carga
                                            left join T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL CargaDocumentoParaEmissaoNFSManual on CargaDocumentoParaEmissaoNFSManual.CAR_CODIGO = Carga.CAR_CODIGO
                                            WHERE CargaDocumentoParaEmissaoNFSManual.LNM_CODIGO = DocumentoFaturamento.LNM_CODIGO)
                                            ELSE (Carga.CAR_CODIGO_CARGA_EMBARCADOR)
                                            END) Carga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_CARGA_EMBARCADOR,"))
                            groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        if (!groupBy.Contains("DocumentoFaturamento.LNM_CODIGO,"))
                            groupBy.Append("DocumentoFaturamento.LNM_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        SetarJoinsDocumentoFaturamento(joins);

                        select.Append(@"(CASE WHEN DocumentoFaturamento.LNM_CODIGO is not null 
                                            THEN (select STRING_AGG(CargaDocumentoParaEmissaoNFSManual.NEM_NUMERO, ', ') FROM T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL CargaDocumentoParaEmissaoNFSManual
                                            WHERE CargaDocumentoParaEmissaoNFSManual.LNM_CODIGO = DocumentoFaturamento.LNM_CODIGO) 
                                            ELSE (select STRING_AGG(XMLNotaFiscal.NF_NUMERO, ', ') FROM T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal
                                            INNER JOIN T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = CTeXMLNotaFiscal.CON_CODIGO 
                                            WHERE CTeXMLNotaFiscal.CON_CODIGO = Cte.CON_CODIGO)
                                            END) NotaFiscal, ");

                        if (!groupBy.Contains("Cte.CON_CODIGO,"))
                            groupBy.Append("Cte.CON_CODIGO, ");

                        if (!groupBy.Contains("DocumentoFaturamento.LNM_CODIGO,"))
                            groupBy.Append("DocumentoFaturamento.LNM_CODIGO, ");
                    }
                    break;

                case "NumeroCancelamento":
                    if (!select.Contains(" NumeroCancelamento, "))
                    {
                        select.Append($@"substring((select ', ' + cast(cancelamento.CPG_NUMERO as varchar)
                                        from T_PAGAMENTO_INTEGRACAO pagamento 
	                                        join T_CANCELAMENTO_PAGAMENTO_PAGAMENTOS cancelamentoPagamento on pagamento.PAG_CODIGO = cancelamentoPagamento.PAG_CODIGO
	                                        join T_CANCELAMENTO_PAGAMENTO cancelamento on cancelamentoPagamento.CPG_CODIGO = cancelamento.CPG_CODIGO
                                        where pagamento.PAG_CODIGO = PagamentoIntegracao.PAG_CODIGO order by pagamento.PAG_CODIGO for xml path('')), 3, 1000) NumeroCancelamento, ");
                        if (!groupBy.Contains("PagamentoIntegracao.PAG_CODIGO"))
                            groupBy.Append("PagamentoIntegracao.PAG_CODIGO, ");

                        SetarJoinsCancelamentoPagamento(joins);
                    }
                    break;

                case "SituacaoCancelamento":
                case "SituacaoCancelamentoFormatada":
                    if (!select.Contains(" SituacaoCancelamento, "))
                    {
                        select.Append(@"(select TOP 1 case 
                                            when CancelamentoPagamento.CPG_SITUACAO = 1
                                                then 'Em cancelamento'
                                            when CancelamentoPagamento.CPG_SITUACAO = 2
                                                then 'Pendencia Cancelamento'
                                            when CancelamentoPagamento.CPG_SITUACAO = 3
                                                then 'Ag. Integracao'
                                            when CancelamentoPagamento.CPG_SITUACAO = 4
                                                then 'Em Integração'
                                            when CancelamentoPagamento.CPG_SITUACAO = 5
                                                then 'Falha na integração'
                                            when CancelamentoPagamento.CPG_SITUACAO = 6
                                                then 'Cancelado'
                                            else ''
                                        end
	                                    from T_CANCELAMENTO_PAGAMENTO CancelamentoPagamento 
	                                    left join T_CANCELAMENTO_PAGAMENTO_PAGAMENTOS Pagamento on Pagamento.CPG_CODIGO = CancelamentoPagamento.CPG_CODIGO
	                                    where Pagamento.PAG_CODIGO = PagamentoIntegracao.PAG_CODIGO) SituacaoCancelamento, ");
                        if (!groupBy.Contains("PagamentoIntegracao.PAG_CODIGO"))
                            groupBy.Append("PagamentoIntegracao.PAG_CODIGO, ");

                        SetarJoinsCancelamentoPagamento(joins);
                    }
                    break;

                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select.Append(@"substring((select ', ' + motivo.MCP_DESCRICAO
                                        from T_PAGAMENTO_INTEGRACAO pagamento 
	                                    join T_CANCELAMENTO_PAGAMENTO_PAGAMENTOS cancelamentoPagamento on pagamento.PAG_CODIGO = cancelamentoPagamento.PAG_CODIGO
	                                    join T_CANCELAMENTO_PAGAMENTO cancelamento on cancelamentoPagamento.CPG_CODIGO = cancelamento.CPG_CODIGO
	                                    join T_MOTIVO_CANCELAMENTO_PAGAMENTO motivo on cancelamento.MCP_CODIGO = motivo.MCP_CODIGO
                                    where pagamento.PAG_CODIGO = PagamentoIntegracao.PAG_CODIGO order by pagamento.PAG_CODIGO for xml path('')), 3, 1000) MotivoCancelamento, ");

                        if (!groupBy.Contains("PagamentoIntegracao.PAG_CODIGO"))
                            groupBy.Append("CancelamentoPagamento.CPG_MOTIVO_REJEICAO_CANCELAMENTO_FECHAMENTO, ");

                        SetarJoinsCancelamentoPagamento(joins);
                    }
                    break;

                case "ProtocoloCTe":
                    if (!select.Contains(" ProtocoloCTe, "))
                    {
                        select.Append("Cte.CON_CODIGO ProtocoloCTe, ");
                        groupBy.Append("Cte.CON_CODIGO, ");

                        SetarJoinsCte(joins);
                    }
                    break;

                case "DataEnvioIntegracao":
                case "DataEnvioIntegracaoFormatada":
                    if (!select.Contains(" DataEnvioIntegracao, "))
                    {
                        select.Append("PagamentoIntegracao.INT_DATA_INTEGRACAO DataEnvioIntegracao, ");
                        groupBy.Append("PagamentoIntegracao.INT_DATA_INTEGRACAO, ");
                    }
                    break;

                case "SituacaoCarga":
                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.NumeroPagamento > 0)
            {
                SetarJoinsPagamento(joins);
                where.Append($" and Pagamento.PAG_NUMERO = {filtrosPesquisa.NumeroPagamento}");
            }

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                SetarJoinsCarga(joins);
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");
            }

            if (filtrosPesquisa.CodigoCTe > 0)
            {
                SetarJoinsCte(joins);
                where.Append($" and Cte.CON_CODIGO = {filtrosPesquisa.CodigoCTe}");
            }

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                where.Append($" and PagamentoIntegracao.INT_SITUACAO_INTEGRACAO = {(int)filtrosPesquisa.SituacaoIntegracao}");

            if (filtrosPesquisa.SituacaoPagamento.HasValue)
            {
                SetarJoinsPagamento(joins);
                where.Append($" and Pagamento.PAG_SITUACAO = {(int)filtrosPesquisa.SituacaoPagamento}");
            }

            if (filtrosPesquisa.CodigosFilial.Count > 0)
            {
                SetarJoinFilial(joins);
                where.Append($" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
            }

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
            {
                SetarJoinsEmpresa(joins);
                where.Append($" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");
            }

            if (filtrosPesquisa.DataInicialEmissaoDocumento != DateTime.MinValue)
            {
                SetarJoinsCte(joins);
                where.Append($" and CAST(Cte.CON_DATAHORAEMISSAO AS DATE) >= '{filtrosPesquisa.DataInicialEmissaoDocumento.ToString(pattern)}'");
            }

            if (filtrosPesquisa.DataFinalEmissaoDocumento != DateTime.MinValue)
            {
                SetarJoinsCte(joins);
                where.Append($" and CAST(Cte.CON_DATAHORAEMISSAO AS DATE) <= '{filtrosPesquisa.DataFinalEmissaoDocumento.ToString(pattern)}'");
            }

            if (filtrosPesquisa.SituacaoCarga.HasValue && filtrosPesquisa.SituacaoCarga.Value != SituacaoCarga.Todas)
            {
                SetarJoinsCarga(joins);
                where.Append($" and Carga.CAR_SITUACAO = {(int)filtrosPesquisa.SituacaoCarga.Value}");
            }

            if (filtrosPesquisa.ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado)
            {
                where.Append($@" and PagamentoIntegracao.PIN_CODIGO = (select max(PIN_CODIGO)
										                                 from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento2 
										                                 left join T_PAGAMENTO_INTEGRACAO pagintegracao on pagintegracao.DFA_CODIGO  = DocumentoFaturamento2.DFA_CODIGO 
										                                 left join T_CTE Cte2 on Cte2.CON_CODIGO = DocumentoFaturamento2.CON_CODIGO
										                                where Cte2.CON_CODIGO = cte.CON_CODIGO)");
            }
        }

        #endregion
    }
}
