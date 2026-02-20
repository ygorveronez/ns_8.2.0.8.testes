using Dominio.ObjetosDeValor.WebService.Frota;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO", DynamicUpdate = true, EntityName = "TipoOperacao", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacao", NameType = typeof(TipoOperacao))]
    public class TipoOperacao : EntidadeBase, IComparable<TipoOperacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoMobile", Column = "COM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoMobile ConfiguracaoMobile { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoCalculoFrete", Column = "CTC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoCalculoFrete ConfiguracaoCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoEmissaoDocumento", Column = "CTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoEmissaoDocumento ConfiguracaoEmissaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoAgendamentoColetaEntrega", Column = "CCE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoAgendamentoColetaEntrega ConfiguracaoAgendamentoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoCanhoto", Column = "CNH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoCanhoto ConfiguracaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoFreeTime", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoFreeTime ConfiguracaoFreeTime { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoIntegracao", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoIntegracao ConfiguracaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoCarga", Column = "CCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoCarga ConfiguracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoTerceiro", Column = "CTT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoTerceiro ConfiguracaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoImpressao", Column = "CIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoImpressao ConfiguracaoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoTransportador", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoTransportador ConfiguracaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoIntegracaoDiageo", Column = "CID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoIntegracaoDiageo ConfiguracaoTipoOperacaoIntegracaoDiageo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoIntegracaoTransSat", Column = "CIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoIntegracaoTransSat ConfiguracaoTipoOperacaoIntegracaoTransSat { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoPagamentos", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoPagamentos ConfiguracaoPagamentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoIntercab", Column = "CTI_CODIGO_INTERCAB", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoIntercab ConfiguracaoIntercab { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoEMP", Column = "CTI_CODIGO_EMP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoEMP ConfiguracaoEMP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoTrizy", Column = "CTT_CODIGO_TRIZY", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoTrizy ConfiguracaoTrizy { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoPedido", Column = "CTD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoPedido ConfiguracaoPedido { get; set; }
        /// <summary>
        /// Da aba Configuração Emissão, que é um componente com o cadastro do Grupo de Pessoas e Pessoas
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoEmissao", Column = "CTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoEmissao ConfiguracaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoDocumentoEmissao", Column = "CDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoDocumentoEmissao ConfiguracaoDocumentoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoControleEntrega", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoControleEntrega ConfiguracaoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoChamado", Column = "CCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoChamado ConfiguracaoTipoOperacaoChamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoLicenca", Column = "COL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoLicenca ConfiguracaoTipoOperacaoLicenca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoJanelaCarregamento", Column = "COJ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoJanelaCarregamento ConfiguracaoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoMontagemCarga", Column = "CMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoMontagemCarga ConfiguracaoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoGestaoDevolucao", Column = "CGD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoGestaoDevolucao ConfiguracaoGestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoCotacaoPedido", Column = "CCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoCotacaoPedido ConfiguracaoCotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo", Column = "CTV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo ConfiguracaoTipoPropriedadeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoContainer", Column = "CCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoContainer ConfiguracaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TOP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TOP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TOP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "TOP_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRedespachoAutomaticamente", Column = "TOP_GERAR_REDESPACHO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRedespachoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos", Column = "TOP_GERAR_REDESPACHO_AUTOMATICAMENTE_POR_PEDIDO_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRedespachoParaOutrasEtapasCarregamento", Column = "TOP_GERAR_REDESPACHO_AUTOMATICAMENTE_OUTRAS_ETAPAS_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRedespachoParaOutrasEtapasCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Reentrega", Column = "TOP_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reentrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_REDESPACHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TOP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "TOP_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_ADICIONAR_NA_JANELA_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarNaJanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_ADICAO_NA_JANELA_DESCARREGAMENTO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAdicaoNaJanelaDescarregamentoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_TRANSPORTADOR_ENVIAR_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorEnviarNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_EDI_FATURA_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEDIFaturaPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeNotaFiscalParaCalcularFrete", Column = "TOP_EXIGE_NOTA_FISCAL_PARA_CALCULAR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigeNotaFiscalParaCalcularFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarExpedidorComoTransportador", Column = "TOP_UTILIZAR_EXPEDIDOR_COMO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarExpedidorComoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroPedido", Column = "TOP_EXIGIR_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigeVeiculoParaEmissao", Column = "TOP_NAO_EXIGE_VEICULO_PARA_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoExigeVeiculoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirDocumentosRetroativamente", Column = "TOP_EMITIR_DOCUMENTOS_RETROATIVAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmitirDocumentosRetroativamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteOperacao", Column = "TOP_PRODUTO_PREDOMINANTE_OPERACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoPredominanteOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoDeRedespacho", Column = "TOP_OPERACAO_REDESPACHO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool OperacaoDeRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirCRT", Column = "TOP_IMPRIMIR_CRT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeRecebedor", Column = "TOP_EXIGE_RECEBEDOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigeRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeQueVeiculoIgualModeloVeicularDaCarga", Column = "TOP_EXIGE_VEICULO_IGUAL_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigeQueVeiculoIgualModeloVeicularDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FretePorContadoCliente", Column = "TOP_FRETE_POR_CONTA_CLIENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FretePorContadoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsaJanelaCarregamentoPorEscala", Column = "TOP_USA_JANELA_CARREGAMENTO_POR_ESCALA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UsaJanelaCarregamentoPorEscala { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_TRANSPORTADOR_AVANCAR_ETAPA_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteTransportadorAvancarEtapaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarTipoOperacaoPreCargaAoGerarCarga", Column = "TOP_UTILIZAR_TIPO_OPERACAO_PRE_CARGA_AO_GERAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarTipoOperacaoPreCargaAoGerarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirQualquerModeloVeicular", Column = "TOP_PERMITIR_QUALQUER_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirQualquerModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransbordarNotasDeOutrasCargas", Column = "TOP_PERMITIR_TRANSBORDAR_NOTAS_DE_OUTRAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransbordarNotasDeOutrasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteCTeFilialEmissora", Column = "TOP_EMITE_CTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarDocumentoDaSubcontratacao", Column = "TOP_AVERBAR_DOCUMENTOS_SUBCONTRATADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarDocumentoDaSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalculaFretePorTabelaFreteFilialEmissora", Column = "TOP_CALCULA_FRETE_POR_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalculaFretePorTabelaFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarGestaoPatio", Column = "TOP_HABILITAR_GESTAO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarGestaoPatioDestino", Column = "TOP_HABILITAR_GESTAO_PATIO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarGestaoPatioDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OPERACAO_RECOLHIMENTO_TROCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoRecolhimentoTroca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigePlacaTracao", Column = "TOP_EXIGE_PLACA_TRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigePlacaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarTransportadorImportacaoDocumento", Column = "TOP_NAO_VALIDAR_TRANSPORTADOR_IMPORTACAO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarTransportadorImportacaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoDocumentosForaDoSistema", Column = "TOP_EMISSAO_DOCUMENTOS_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoDocumentosForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CompraValePedagioDocsEmitidosFora", Column = "TOP_COMPRA_VALE_PEDAGIO_DOCS_EMITIDOS_FORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CompraValePedagioDocsEmitidosFora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCargaPadraoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarConfiguracaoEmissao", Column = "TOP_USAR_CONFIGURACAO_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarConfiguracaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeDocumentos", Column = "TOP_TIPO_EMISSAO_CTE_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoEmissaoCTeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeParticipantes", Column = "TOP_TIPO_EMISSA_CTE_PARTICIPANTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes TipoEmissaoCTeParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeEmitidoNoEmbarcador", Column = "TOP_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteUtilizarEmContratoFrete", Column = "TOP_PERMITE_UTILIZAR_EM_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteUtilizarEmContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcadorPadraoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarDocumentosParaNFsManual", Column = "TOP_DISPONIBILIZAR_DOCUMENTOS_PARA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaNFsManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMPRESA_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoNotaFiscal", Column = "AIN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal ArquivoImportacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteImportarDocumentosManualmente", Column = "TOP_PERMITE_IMPORTAR_DOCUMENTOS_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteImportarDocumentosManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarNotaFiscalPeloDestinatario", Column = "TOP_VALIDAR_NOTA_FISCAL_PELO_DESTINATARIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarNotaFiscalPeloDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarTituloGNREAutomatico", Column = "TOP_NAO_GERAR_TITULO_GNRE_AUTOMATICO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGerarTituloGNREAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparMovimentoFinanceiroPorPedido", Column = "TOP_AGRUPAR_MOVIMENTO_FINANCEIRO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparMovimentoFinanceiroPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterUnicaCargaNoAgrupamento", Column = "TOP_MANTER_UNICA_CARGA_AGRUPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterUnicaCargaNoAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteAgruparCargas", Column = "TOP_NAO_PERMITE_AGRUPAR_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteAgruparCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaViaMontagemDoTipoPreCarga", Column = "TOP_GERAR_CARGA_VIA_MONTAGEM_DO_TIPO_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaViaMontagemDoTipoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarRecebedorComoPontoPartidaCarga", Column = "TOP_USAR_RECEBEDOR_COMO_PONTO_PARTIDA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarRecebedorComoPontoPartidaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_VALIDAR_NOTA_FISCAL_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotaFiscalExistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGE_CONFIRMACAO_FRETE_ANTES_EMITIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeConformacaoFreteAntesEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_EXIGE_CONFIRMACAO_DAS_NOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigeConformacaoDasNotasEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_LIBERAR_CARGA_SEM_NFE_AUTOMATICAMENTE_APOS_LIBERAR_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_DOCUMENTO_PADRAO_PARA_CADA_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDocumentoPadraoParaCadaPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_LIBERAR_CARGA_AUTOMATICAMENTE_PARA_FATURAMENTO_APOS_PRAZO_ESGOTADO_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGE_PERCURSO_ENTRE_CNPJ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigePercursoEntreCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DESCRICAO_COMPONENTE_FRETE_EMBARCADOR", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DescricaoComponenteFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EMISSAO_MDFE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarDocumentosParaLoteEscrituracao", Column = "TOP_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ESCRITURAR_SOMENTE_DOCUMENTOS_EMITIDOS_PARA_NFES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscriturarSomenteDocumentosEmitidosParaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarDocumentosParaPagamento", Column = "TOP_DISPONIBILIZAR_DOCUMENTOS_PARA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_QUITAR_DOCUMENTO_AUTOMATICAMENTE_AO_GERAR_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuitarDocumentoAutomaticamenteAoGerarLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VINCULAR_MOTORISTA_FILA_CARREGAMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool VincularMotoristaFilaCarregamentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirGerarRedespacho", Column = "TOP_PERMITIR_GERAR_REDESPACHO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirGerarRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirGerarRecorrenciaRedespacho", Column = "TOP_PERMITIR_GERAR_RECORRENCIA_REDESPACHO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirGerarRecorrenciaRedespacho { get; set; }

        /// <summary>
        /// Direto no banco, funciona apenas para integração via WS, se verdadeira seta o tipo de operação para a carga integrada
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_COMO_PADRAO_QUANDO_ORIGEM_E_DESTINO_FILIAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoQuandoOrigemDestinoFiliais { get; set; }


        /// <summary>
        /// Direto no banco, funciona apenas para integração via WS, se verdadeira seta o tipo de operação para a carga integrada
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_COMO_PADRAO_QUANDO_NENHUMA_OPERACAO_FOR_INFORMADA_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoQuandoNenhumaOperacaoForInformadaNaIntegracao { get; set; }

        /// <summary>
        /// Direto no banco, funciona apenas para integração via WS, se verdadeira seta o tipo de operação para a carga integrada quando na integração vier a informação de observação de entrega
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_COMO_PADRAO_QUANDO_OBSERVACAO_LOCAL_ENTREGA_FOR_INFORMADA_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoQuandoObservacaoLocalEntregaForInformadaNaIntegracao { get; set; }

        /// <summary>
        /// Direto no banco, funciona apenas para integração via WS, se verdadeira seta o tipo de operação para a carga integrada quando na integração vier a informação de observação de entrega
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_COMO_PADRAO_QUANDO_CARGA_FOR_TOTALMENTE_SUBCONTRATADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoQuandoCargaForTotalmenteSubcontratada { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_PADRAO_PARA_FRETES_DENTRO_DO_PAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoParaFretesDentroDoPais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USAR_PADRAO_PARA_FRETES_FORA_DO_PAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoParaFretesForaDoPais { get; set; }


        /// <summary>
        /// Direto no banco, Vale para as duas opções de padrão, porém fica de exclusividade da filial
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_SETAR_PADRAO_NA_INTEGRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoasSetarPadraoNaIntegracao { get; set; }

        /// <summary>
        /// Direto no banco, Vale para as duas opções de padrão, porém fica de exclusividade da filial
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_PARA_SETAR_PADRAO_NA_INTEGRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial FilialParaSetarPadraoNaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALE_PEDAGIO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValePedagioObrigatorio { get; set; }

        /// <summary>
        /// Utilizado para identificar se no remetente do CT-e será informado um indicador globalizado, ou seja, será enviado os dados do transportador com a razão social DIVERSOS para emissão
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INDICADOR_GLOBALIZADO_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorGlobalizadoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoDeImportacaoExportacao", Column = "TOP_OPERACAO_DE_IMPORTACA_EXPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoDeImportacaoExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeraCargaAutomaticamente", Column = "TOP_GERA_CARGA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressao", Column = "TOP_TIPO_IMPRESSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoImpressao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_DESISTENCIA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDesistenciaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_COBRAR_DESISTENCIA_CARGA_APOS_HORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrarDesistenciaCargaAposHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_HORA_COBRAR_DESISTENCIA_CARGA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraCobrarDesistenciaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_COBRAR_DESISTENCIA_CARGA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal PercentualCobrarDesistenciaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_DESISTENCIA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDesistenciaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_COBRAR_DESISTENCIA_CARREGAMENTO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal PercentualCobrarDesistenciaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_FATOR_CUBAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_FATOR_CUBAGEM", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal? FatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_USO_FATOR_CUBAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoUsoFatorCubagem? TipoUsoFatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_PALETIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPaletizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBRIGATORIO_PASSAGEM_EXPEDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioPassagemExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PESO_POR_PALLET", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal? PesoPorPallet { get; set; }

        /// <summary>
        /// Utilizar outro modelo de documento quanto for emissão municipal (NFS-e ou NFS Manual)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_OUTRO_MODELO_DOCUMENTO_EMISSAO_MUNICIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarOutroModeloDocumentoEmissaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_EMITIR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PROVISIONAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisionarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_INTEGRACAO_GERENCIADORA_RISCO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoGerenciadoraRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_DADOS_PEDIDO_PARA_NOTAS_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDadosPedidoParaNotasExterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_CONFIGURACAO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarConfiguracaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_CONFIGURACAO_TERCEIRO_COMO_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarConfiguracaoTerceiroComoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_ADIANTAMENTO_FRETE_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAdiantamentoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_ABASTECIMENTO_FRETE_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAbastecimentoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_COBRANCA_PADRAO_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCobrancaPadraoTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DIAS_VENCIMENTO_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DIAS_VENCIMENTO_SALDO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoSaldoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_MDFE_TRANSBORDO_SEM_CONSIDERAR_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMDFeTransbordoSemConsiderarOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_COLETA_EM_PRODUTOR_RURAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaEmProdutorRural { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_UTILIZA_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirValorFreteAvancarCarga", Column = "TOP_EXIGIR_VALOR_FRETE_AVANCAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirValorFreteAvancarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteGerarPedidoSemDestinatario", Column = "TOP_PERMITE_GERAR_PEDIDO_SEM_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteGerarPedidoSemDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigeRotaRoteirizada", Column = "TOP_NAO_EXIGE_ROTA_ROTEIRIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigeRotaRoteirizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteirizarPorLocalidade", Column = "TOP_ROTEIRIZAR_POR_LOCALIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarPorLocalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComprarValePedagio", Column = "TOP_NAO_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagio { get; set; }

        /// <summary>
        /// Modelo de documento fiscal para emissões municipais
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_EMISSAO_MUNICIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalEmissaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_IMPORTAR_REDESPACHO_INTERMEDIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMITENTE_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente EmitenteImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ExpedidorImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RecebedorImportacaoRedespachoIntermediario { get; set; }

        /// <summary>
        /// Descrição do item utilizado para obter o peso do CT-e para subcontratação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DESCRICAO_ITEM_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoItemPesoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DESCRICAO_CARAC_TRANSP_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaTransporteCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REGEX_VALIDACAO_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexValidacaoNumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_ANTT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_INTERCAB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoIntercab { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_TRAFEGUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoTrafegus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_CONSULTAR_POR_PACOTES_LOGGI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteConsultarPorPacotesLoggi { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIBE_VALOR_UNITARIO_DO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoExibeValorUnitarioDoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REMETENTE_CTE_SERA_DESTINATARIO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemetenteDoCTeSeraODestinatarioDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_MULTIPLOS_DESTINATARIOS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplosDestinatariosPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_MULTIPLOS_REMETENTES_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplosRemetentesPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_DESLOCAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDeslocamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OPERACAO_TROCA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_TROCA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OPERACAO_EXIGE_INFORMAR_CARGA_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoExigeInformarCargaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_CTE_COMPLEMENTAR_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCTeComplementarNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXCLUSIVA_SUBCONTRATACAO_OU_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExclusivaDeSubcontratacaoOuRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ADICIONAR_OU_REMOVER_PEDIDOS_REPLICA_AOS_TRECHOS_ANTERIORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_SEMPRE_EMITIR_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreEmitirSubcontratacao { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_FRETE_ZERADO", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool BloquearFreteZerado { get; set; }

        /// <summary>
        /// Deve ser utilizada como informação na etapa de frete da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBSERVACAO_EMISSAO_CARGA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_ENVIO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe? TipoEnvioEmail { get; set; }

        /// <summary>
        /// Valor máximo de emissões pendentes de pagamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALOR_MAXIMO_EMISSAO_PENDENTE_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMaximoEmissaoPendentePagamento { get; set; }

        /// <summary>
        /// Configuração para utilizar o recebedor apenas como participante, utilizando para calculo de frete e impostos o Destinat[ario
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_RECEBEDOR_APENAS_COMO_PARTICIPANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRecebedorApenasComoParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarDiariaMotoristaProprio", Column = "TOP_GERAR_DIARIA_MOTORISTA_PROPRIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDiariaMotoristaProprio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_GOLDEN_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirUtilizarPlacaContrato", Column = "TOP_PERMITIR_UTILIZAR_PLACA_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirUtilizarPlacaContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CONFIGURACAO_TABELA_FRETE_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfiguracaoTabelaFretePorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo? TipoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirConfirmacaoDadosTransportadorAvancarCarga", Column = "TOP_EXIGIR_CONFIRMACAO_DADOS_TRANSPORTADOR_AVANCAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConfirmacaoDadosTransportadorAvancarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPedidoNoRecebientoNFe", Column = "TOP_GERAR_PEDIDO_RECEBIMENTO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoNoRecebientoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarAnexoSolicitacaoFrete", Column = "TOP_OBRIGATORIO_INFORMAR_ANEXO_SOLICITACAO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarAnexoSolicitacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBRIGAR_ROTA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarRotaNaMontagemDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_ALTERAR_VALOR_FRETE_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarValorFreteNaCarga { get; set; }

        /// <summary>
        /// Atualiza as informações da CARGA_PEDIDO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ATUALIZAR_PRODUTOS_POR_XML_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarProdutosPorXmlNotaFiscal { get; set; }

        /// <summary>
        /// Se é para atualizar/retornar saldo de um pedido para montagem carga...(Default = AtualizarProdutosPorXmlNotaFiscal)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ATUALIZAR_SALDO_PEDIDO_PRODUTOS_POR_XML_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarSaldoPedidoProdutosPorXmlNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LayoutsEDI", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_LAYOUT_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacaoLayoutEDI", Column = "TLY_CODIGO")]
        public virtual IList<Embarcador.Pedidos.TipoOperacaoLayoutEDI> LayoutsEDI { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ApolicesSeguro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_APOLICE_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacaoApoliceSeguro", Column = "TOA_CODIGO")]
        public virtual IList<TipoOperacaoApoliceSeguro> ApolicesSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoOperacaoConfiguracoesComponentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_CONFIGURACAO_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacaoConfiguracaoComponentes", Column = "TOC_CODIGO")]
        public virtual IList<Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes> TipoOperacaoConfiguracoesComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacaoIntegracao", Column = "TOI_CODIGO")]
        public virtual IList<TipoOperacaoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "TOP_TIPO_ULTIMO_PONTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? TipoUltimoPontoRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EixosSuspenso", Column = "TOP_EIXOS_SUSPENSO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso? EixosSuspenso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_CTE_EMITIDO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrenciaCTeEmitidoEmbarcador { get; set; }

        /// <summary>
        /// Utilizado para identificar se no destinatário do CT-e será informado um indicador globalizado, ou seja, será enviado os dados do transportador com a razão social DIVERSOS para emissão
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INDICADOR_GLOBALIZADO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorGlobalizadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_SEMPRE_USAR_INDICADOR_GLOBALIZADO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreUsarIndicadorGlobalizadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INDICADOR_GLOBALIZADO_DESTINATARIO_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorGlobalizadoDestinatarioNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NOTIFICAR_CASO_NUMERO_PEDIDO_FOR_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCasoNumeroPedidoForExistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PESO_MINIMO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal PesoMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PESO_MAXIMO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal PesoMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaMultimodal", Column = "TOP_TIPO_COBRANCA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalPropostaMultimodal", Column = "TOP_MODAL_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultimodal", Column = "TOP_TIPO_SERVICO_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaMultimodal", Column = "TOP_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDosDestinatario", Column = "TOP_BLOQUEAR_EMISSAO_DESTINATARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDeEntidadeSemCadastro", Column = "TOP_BLOQUEAR_EMISSAO_ENTIDADES_SEM_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDeEntidadeSemCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObrigacaoUsoTerminal", Column = "TOP_TIPO_OBRIGACAO_USO_TERMINAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObrigacaoUsoTerminal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObrigacaoUsoTerminal? TipoObrigacaoUsoTerminal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_DATA_NFE_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataNFeEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirDataRetiradaCtrnJanelaCarregamentoTransportador", Column = "TOP_EXIGIR_DATA_RETIRADA_CTRN_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirDataRetiradaCtrnJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirMaxGrossJanelaCarregamentoTransportador", Column = "TOP_EXIGIR_MAX_GROSS_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirMaxGrossJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroContainerJanelaCarregamentoTransportador", Column = "TOP_EXIGIR_NUMERO_CONTAINER_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirNumeroContainerJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirTaraContainerJanelaCarregamentoTransportador", Column = "TOP_EXIGIR_TARA_CONTAINER_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigirTaraContainerJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_COR_JANELA_CARREGAMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CorJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_INCLUIR_ICMS_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirICMSFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_COMISSAO_PARCIAL_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarComissaoParcialMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_COMISSAO_PARCIAL_MOTORISTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualComissaoParcialMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_QUANTIDADE_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarQuantidadeMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteEmitirCargaDiferentesOrigensParcialmente", Column = "TOP_PERMITE_EMITIR_CARGA_DIFERENTES_ORIGENS_PARCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmitirCargaDiferentesOrigensParcialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_VALOR_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarValorNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_VALOR_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_NCM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_CFOP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarCFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_INFORMAR_M3", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarM3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_CONTROLE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarControleColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_CONTROLE_COLETA_ENTREGA_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarControleColetaEntregaAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_AGENDAMENTO_GERA_APENAS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgendamentoGeraApenasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_VEICULO_DIFERENTE_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVeiculoDiferenteMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEDIDOS_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_CONTROLE_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarControleColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_ADICIONAR_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_IMPRESSAO_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteImpressaoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_RETIFICAR_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteRetificarMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_CALCULADORA_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCalculadoraMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITE_REJEITAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteRejeitarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_EMITIR_CARGA_COM_VALOR_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirCargaComValorZerado { get; set; }

        /// <summary>
        /// A carga não pode ser emitida caso o valor do frete líquido esteja zerado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_VALOR_FRETE_LIQUIDO_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirValorFreteLiquidoZerado { get; set; }

        /// <summary>
        /// nao permite a atualizacao da data entrega reprogramada quando entrega ja esta no raio do cliente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_ATUALIZAR_DATA_ENTREGA_REPROGRAMADA_APOS_ENTRADA_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarDataReprogramadaAposEntradaRaio { get; set; }

        /// <summary>
        /// Se ativa, a baixa no controle de entrega ocorre na entrada no raio, e não na saída (que é o padrão)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REALIZAR_BAIXA_ENTRADA_NO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarBaixaEntradaNoRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CAMPOS_SECUNDARIOS_OBRIGATORIOS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CamposSecundariosObrigatoriosPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesBloquearEmissaoDosDestinatario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_CLIENTE_BLOQUEAR_EMISSAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO")]
        public virtual ICollection<Cliente> ClientesBloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_AVANCAR_CARGA_AUTOMATICA_APOS_MONTAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarCargaAutomaticaAposMontagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_AVANCAR_ETAPA_FRETE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarEtapaFreteAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_TOMADOR_DO_PEDIDO_DIFERENTE_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTomadorDoPedidoDiferenteDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_IMPORTAR_TERMINAL_ORIGEM_COMO_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarTerminalOrigemComoExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZA_CENTRO_DE_CUSTO_OU_PEP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoUtilizaCentroDeCustoPEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZA_CONTA_RAZAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoUtilizaContaRazao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_IMPORTAR_TERMINAL_DESTINO_COMO_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarTerminalDestinoComoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_SOLICITAR_NOTAS_FISCAIS_SEM_ENCERRAR_MDFE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DESLOCAMENTO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_MONTAGEM_CARGA_SEM_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearMontagemCargaSemNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_INFORMAR_RECEBEDOR_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarRecebedorAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBSERVACAO_CTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EMAIL_AGENDAMENTO_COLETA", TypeType = typeof(string), Length = 1500, NotNull = false)]
        public virtual string EmailAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EMAIL_DISPONIBILIDADE_CARGA", TypeType = typeof(string), Length = 1500, NotNull = false)]
        public virtual string EmailDisponibilidadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXPRESSAO_BOOKING", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ExpressaoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXPRESSAO_CONTAINER", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ExpressaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_VEICULO_COM_RASTREADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirVeiculoComRastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXPRESSAO_REGULAR_NUMERO_BOOKING_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpressaoRegularNumeroBookingObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXPRESSAO_REGULAR_NUMERO_CONTAINER_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpressaoRegularNumeroContainerObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBRIGATORIO_VINCULAR_CONTAINER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioVincularContainerCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBRIGATORIO_REALIZAR_CONFERENCIA_CONTAINER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioRealizarConferenciaContainerCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_SE_CARGA_POSSUI_VINCULO_COM_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSeCargaPossuiVinculoComPreCarga { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CARGA_BLOQUEADA_PARA_EDICAO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaBloqueadaParaEdicaoIntegracao { get; set; }

        /// <summary>
        /// Parâmetro descontinuado. Utilizar o parâmetro LayoutImpressaoOrdemColeta (TipoOperacao.ConfiguracaoJanelaCarregamento.LayoutImpressaoOrdemColeta) no objeto ConfiguracaoTipoOperacaoJanelaCarregamento.cs
        /// </summary>
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_LAYOUT_COLETA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLayoutColetaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CARGA_TIPO_CONSOLIDACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaTipoConsolidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_OPERACAO_MERCOSUL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoMercosul { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REMESSA_SAP", TypeType = typeof(int), NotNull = false)]
        public virtual int RemessaSAP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OBSERVACAO_CTE_TERCEIRO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENCERRAR_MDFE_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarMDFeManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_CIOT_PARA_TODAS_AS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasAsCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CARGA_PROPRIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPropria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_LIBERAR_AUTOMATICAMENTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarAutomaticamentePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CRIAR_NOVO_PEDIDO_AO_VINCULAR_NOTA_FISCAL_COM_DIFERENTES_PARTICIPANTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_RETORNO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirSelecionarNotasCompativeis", Column = "TOP_PERMITIR_SELECIONAR_NOTA_COMPATIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarNotasCompativeis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransportadorInformeNotasCompativeis", Column = "TOP_PERMITIR_TRANSPORTADOR_INFORME_NOTA_COMPATIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorInformeNotasCompativeis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_NOME_DESTINATARIO_NOTA_FISCAL_PARA_EMITIR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EMISSAO_AUTOMATICA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoAutomaticaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoDiarioBordo", Column = "TOP_TIPO_IMPRESSAO_DIARIO_BORDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoDiarioBordo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoDiarioBordo TipoImpressaoDiarioBordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_ADICIONAR_REMOVER_PEDIDOS_ETAPA1", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarRemoverPedidosEtapa1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TOMADOR_CTE_SUBCONTRATACAO_DEVE_SER_DO_CTE_ORIGINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TomadorCTeSubcontratacaoDeveSerDoCTeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_FIXAR_VALOR_FRETE_NEGOCIADO_RATEIO_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FixarValorFreteNegociadoRateioPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_VALOR_FRETE_ORIGINAL_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorFreteOriginalSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_FATURAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGerarFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_EXPEDIDOR_RECEBEDOR_IGUAL_REMETENTE_DESTINATARIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirExpedidorRecebedorIgualRemetenteDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ALTERAR_REMETENTE_DO_PEDIDO_CONFORME_NOTA_FISCAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AlterarRemetentePedidoConformeNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_RETORNAR_CANHOTO_QUANDO_TODAS_NOTAS_DO_CTE_ESTIVEREM_CONFIRMADAS_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_RETORNAR_CARRREGAMENTO_PENDENTE_APOS_ETEPA_CTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RetornarCarregamentoPendenteAposEtapaCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_INTEGRAR_OPENTECH", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoIntegrarOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_PEDIDOS_NA_INTEGRACAO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarPedidosNaIntegracaoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALOR_MINIMO_MERCADORIA_OPENTECH", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoMercadoriaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NOTIFICAR_REMETENTE_POR_EMAIL_SOLICITAR_NOTASH", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NotificarRemetentePorEmailAoSolicitarNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_MOTORISTA_TELERISCO_AO_CONFIRMAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarMotoristaTeleriscoAoConfirmarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_MOTORISTA_BUONNY_AO_CONFIRMAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarMotoristaBuonnyAoConfirmarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_CANHOTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGerarCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_MAIOR_DISTANCIA_PEDIDO_NA_MONTAGEM_DE_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarMaiorDistanciaPedidoNaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_MOTORISTA_VEICULO_BRASILRISK_AO_CONFIRMAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_DADOS_TRANSPORTE_BRASILRISK_AO_ATUALIZAR_VEICULO_MOTORISTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeProdutoEmbarcadorPedido", Column = "TOP_EXIGE_PRODUTO_EMBARCADOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeProdutoEmbarcadorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmisssaoComMesmoLocalDeOrigemEDestino", Column = "TOP_BLOQUEAR_EMISSAO_COM_MESMO_LOCAL_DE_ORIGEM_E_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmisssaoComMesmoLocalDeOrigemEDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos", Column = "TOP_DISPONIBILIZAR_PEDIDOS_SEPARACAO_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos", Column = "TOP_DISPONIBILIZAR_PEDIDOS_COM_RECEBEDOR_SEPARACAO_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransbordoRodoviario", Column = "TOP_TRANSBORDO_RODOVIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransbordoRodoviario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogisticaReversa", Column = "TOP_LOGISTICA_REVERSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LogisticaReversa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirNFeRemessa", Column = "TOP_EMITIR_NF_REMESSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirNFeRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER_SEM_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceberSemICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTipoOperacao", Column = "GTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao GrupoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_TIPO_CARGA_PEDIDO_CALCULO_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarTipoCargaPedidoCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteDividirPedidoEmCargasDiferentes", Column = "TOP_PERMITE_DIVIDIR_PEDIDO_EM_CARGAS_DIFERENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDividirPedidoEmCargasDiferentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoProcessarTrocaAlvoViaMonitoramento", Column = "TOP_NAO_PROCESSAR_TROCA_ALVO_VIA_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoProcessarTrocaAlvoViaMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_CARGA_SEM_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCargaSemAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGE_CHAVE_VENDA_ANTES_CONFIRMAR_NOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeChaveVendaAntesConfirmarNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_NOVO_NUMERO_CARGA_NO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNovoNumeroCargaNoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_OCORRENCIA_COMPLEMENTO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_SUBCONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_VALIDAR_NOTAS_FISCAIS_COM_DIFERENTES_PORTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotasFiscaisComDiferentesPortos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_VINCULAR_CTE_COMPLEMENTAR_EM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularCTeComplementarEmCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_LICENCA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLicencaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_LICENCA_VEICULO_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLicencaVeiculoPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_AVANCAR_ETAPA_COM_LICENCA_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAvancarEtapaComLicencaInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_CTES_PARA_TRANSBORDO_COM_DESTINO_DIFERENTE_DO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_SOLICITAR_NOTAS_FISCAIS_AO_SALVAR_DADOS_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarNotasFiscaisAoSalvarDadosTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_SELECIONAR_RETIRADA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SelecionarRetiradaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_ALTERACAO_HORARIO_CARREGAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAlteracaoHorarioCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_VALOR_FRETE_INFORMADO_PELO_EMBARCADOR_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirValorFreteInformadoPeloEmbarcadorZerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_IMPORTAR_CTE_COM_CHAVE_NFE_DIFERENTE_NO_PRE_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarCTeComChaveNFeDiferenteNoPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_IMPRIMIR_RELATORIO_ROMANEIO_ETAPA_IMPRESSAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirRelatorioRomaneioEtapaImpressaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_EXIBIR_DETALHES_DO_FRETE_PORTAL_DO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirDetalhesDoFretePortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIDO_SELECIONAR_TIPO_DE_OPERACAO_NA_MONTAGEM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DEVOLUCAO_PRODUTOS_POR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoProdutosPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_INFORMAR_PESO_CUBADO_NA_MONTAGEM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarPesoCubadoNaMontagemDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigeRoteirizacaoMontagemCarga", Column = "TOP_NAO_EXIGE_ROTEIRIZACAO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigeRoteirizacaoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_ADICIONAR_PEDIDO_CARGA_FECHADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarPedidoCargaFechada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_DISPONIBILIZAR_CARGA_PARA_INTEGRACAO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDisponibilizarCargaParaIntegracaoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITE_AVANCAR_CARGA_SEM_DATA_PREVISAO_DE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_PEDIDO_RECEBER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoAoReceberCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_VALOR_FRETE_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorFreteNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INSERIR_DADOS_CONTABEIS_XCAMPO_XTEXTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InserirDadosContabeisXCampoXTextCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_ENVIAR_IMAGEM_PARA_MULTIPLOS_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnviarImagemParaMultiplosCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_INFORMAR_DATA_ENTREGA_PARA_MULTIPLOS_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarDataEntregaParaMultiplosCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_PLANO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPlanoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENVIAR_EMAIL_PLANO_VIAGEM_SOLICITAR_NOTAS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailPlanoViagemSolicitarNotasCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENVIAR_EMAIL_PLANO_VIAGEM_FINALIZAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailPlanoViagemFinalizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_NECESSARIO_CONFIRMAR_IMPRESSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoNecessarioConfirmarImpressaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteReordenarEntregasCarga", Column = "TOP_PERMITE_REORDENAR_ENTREGAS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteReordenarEntregasCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DOCUMENTO_XCAMPO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DocumentoXCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DOCUMENTO_XTEXTO", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string DocumentoXTexto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_XCAMPO_SOMENTE_NO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarXCampoSomenteNoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarIscaNaCarga", Column = "TOP_PERMITE_INFORMAR_ISCA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarIscaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeInformarIscaNaCarga", Column = "TOP_EXIGE_INFORMAR_ISCA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeInformarIscaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaFinalizada", Column = "TOP_GERAR_CARGA_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_ALTERAR_DATA_INICIO_CARREGAMENTO_NO_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarDataInicioCarregamentoNoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ALTERAR_DATA_CARREGAMENTO_AO_ATUALIZAR_AGENDAMENTO_ENTREGA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_LOTE_ENTREGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLoteEntregaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_REALIZAR_IMPRESSAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteRealizarImpressaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEDIDO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEDIDO_UTILIZAR_RECEBEDOR_PEDIDO_PARA_SVM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRecebedorPedidoParaSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_TRANSPORTADOR_CONFIRMAR_REJEITAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorConfirmarRejeitarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NOTIFICAR_TRANSPORTADOR_AO_AGENDAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorAoAgendarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_GERAR_COMISSAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_ALTERAR_HORARIO_CARREGAMENTO_CARGAS_FATURADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarHorarioCarregamentoCargasFaturadas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ProdutosPadroes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_PRODUTOS_PADRAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacaoProdutosPadrao", Column = "TPP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao> ProdutosPadroes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_TIPO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO")]
        public virtual IList<Dominio.Entidades.TipoDeOcorrenciaDeCTe> TiposOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PRAZO_SOLICITACAO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSolicitacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_EMITIR_CARGA_COM_MESMO_NUMERO_OUTRO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_AGENDAR_ENTREGA_SOMENTE_APOS_INICIO_VIAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAgendarEntregaSomenteAposInicioViagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CONSIDERAR_APENAS_DIAS_UTEIS_NA_PREVISAO_DE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarApenasDiasUteisNaPrevisaoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENVIAR_LINK_ACOMPANHAMENTO_PARA_CLIENTE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkAcompanhamentoParaClienteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_OCULTAR_CARGAS_COM_ESSE_TIPO_OPERACAO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarCargasComEsseTipoOperacaoNoPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_TRANSPORTADOR_INFORMAR_OBSERVACAO_IMPRESSA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorInformarObservacaoImpressaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_LIBERACAO_CARGAS_COM_PEDIDOS_DATAS_AGENDADAS_DIVERGENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_AGENDAR_DESCARGA_APOS_DATA_ENTREGA_SUGERIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAgendarDescargaAposDataEntregaSugerida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_EXIGIR_QUE_ENTREGAS_SEJAM_AGENDADAS_COM_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigirQueEntregasSejamAgendadasComCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSIBILITAR_INICIO_VIAGEM_VIA_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossibilitarInicioViagemViaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_PERMITIR_AVANCAR_CARGA_COM_VOLUMES_ZERADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAvancoCargaVolumesZerados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_DATA_INICIO_VIAGEM_ANTERIOR_DATA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDataInicioViagemAnteriorDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO_DESEMBARQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo CheckListDesembarque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo CheckListEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo CheckListColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_AGUARDAR_IMPORTACAO_CTE_PARA_AVANCAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAguardarImportacaoDoCTeParaAvancar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEMITIR_ALTERAR_VOLUMES_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarVolumesNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEMITIR_ATUALIZAR_ENTREGAS_CARGAS_FINALIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarEntregasCargasFinalizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ATUALIZAR_ROTA_REALIZADA_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarRotaRealizadaDoMonitoramento { get; set; }

        /// <summary>
        /// Propriedade utilizada para o DPA, ao importar os pedidos que gera uma pré-carga, 
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_MANTER_ORDEM_ROTEIRIZAR_AGENDA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterOrdemAoRoteirizarAgendaEntrega { get; set; }

        //PELA DEMANDA DE CONSULTAS PRECISA SER NESSA TABELA.
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_CARGA_ROTEIRIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirCargaRoteirizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_HABILITAR_TIPO_PAGAMENTO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTipoPagamentoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_PAGAMENTO_VALE_PEDAGIO", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamentoValePedagio TipoPagamentoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_QUANDO_PROCESSAR_ROTA_REALIZADA_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento QuandoProcessarMonitoramento { get; set; }

        [Obsolete("Movido para ConfiguracaoTipoOperacaoControleEntrega")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_FINALIZAR_ENTREGAS_POR_TRACKING_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarEntregasPorTrackingMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITE_ENVIAR_NOTAS_COMPLEMENTARES_APOS_EMISSAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_ADICIONAR_PEDIDO_REENTREGA_APOS_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarPedidoReentregaAposInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_ENTREGA_POR_NOTA_FISCAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEntregaPorNotaFiscalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VINCULAR_APENAS_UMA_ENTREGA_POR_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularApenasUmaNotaPorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarRecebedorDaNotaFiscal", Column = "TOP_NAO_UTILIZAR_RECEBEDOR_DA_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarRecebedorDaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoInsumos", Column = "TOP_INSUMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoInsumos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EXIGIR_TERMO_ACEITE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTermoAceite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TERMO_ACEITE", Type = "StringClob", NotNull = false)]
        public virtual string TermoAceite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_PAGAMENTO_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete? TipoPagamentoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeProcImportacaoPedido", Column = "TOP_EXIGE_PROC_IMPORTACAO_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeProcImportacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularPautaFiscal", Column = "TOP_CALCULAR_PAUTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPautaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirAvancarCargaSemRegraICMS", Column = "TOP_NAO_PERMITIR_AVANCAR_CARGA_SEM_REGRA_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAvancarCargaSemRegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeSenhaConfirmacaoEntrega", Column = "TOP_EXIGIR_SENHA_CONFIGURACAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeSenhaConfirmacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativaSenhaConfirmacaoEntrega", Column = "TOP_NUMERO_TENTATIVAS_SENHA_CONFIRMACAO_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativaSenhaConfirmacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarComprovanteEntregaAposFinalizacaoEntrega", Column = "TOP_ENVIAR_COMPROVANTE_ENTREGA_APOS_FINALIZACAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarComprovanteEntregaAposFinalizacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciarViagemPeloStatusViagem", Column = "TOP_INICIAR_VIAGEM_PELO_STATUS_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarViagemPeloStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioViagemPorCargaGerada", Column = "TOP_INICIAR_VIAGEM_POR_CARGA_GERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioViagemPorCargaGerada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessarDocumentoEmLote", Column = "TOP_PROCESSAR_DOCUMENTOS_EM_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessarDocumentoEmLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamentoTicks", Column = "TOP_TEMPO_CARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoCarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarregamentoTicks", Column = "TOP_TEMPO_DESCARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoDescarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarCobrancaEstadiaAutomaticaPeloTracking", Column = "TOP_HABILITAR_COBRANCA_ESTADIA_AUTOMATICA_PELO_TRACKING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarCobrancaEstadiaAutomaticaPeloTracking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMinimoCobrancaEstadia", Column = "TOP_TEMPO_MINIMO_COBRANCA_ESTADIA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinimoCobrancaEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_MODELO_COBRANCA_ESTADIA_TRACKING", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloCobrancaEstadiaTracking), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloCobrancaEstadiaTracking? ModeloCobrancaEstadiaTracking { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrenciaPedidoEntregueForaPrazo", Column = "TOP_GERAR_OCORRENCIA_PEDIDO_ENTREGUE_FORA_PRAZO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaPedidoEntregueForaPrazo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_PEDIDO_FORA_PRAZO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaPedidoEntregueForaPrazo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_CONDICAO_PAGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoCondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarControlePaletes", Column = "TOP_NAO_GERAR_CONTROLE_PALETES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarControlePaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCTesPorWebService", Column = "TOP_ENVIAR_CTES_POR_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCTesPorWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarSeguroAverbacaoPorWebService", Column = "TOP_ENVIAR_SEGURO_E_AVERBACAO_POR_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarSeguroAverbacaoPorWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceberCTesAverbacaoPorWebService", Column = "TOP_RECEBER_CTES_E_AVERBACAO_POR_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReceberCTesAverbacaoPorWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoDestinadaCTeComplementar", Column = "TOP_OPERACAO_DESTINADA_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoDestinadaCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarRecebedorMontagemCarga", Column = "TOP_PERMITIR_INFORMAR_RECEBEDOR_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarRecebedorMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoTransferenciaContainer", Column = "TOP_OPERACAO_TRANSFERENCIA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OperacaoTransferenciaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarMDFeEmitidoPeloEmbarcador", Column = "TOP_OBRIGATORIO_INFORMAR_MDFE_EMITIDO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarMDFeEmitidoPeloEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirFinalizarEntregaRejeitada", Column = "TOP_NAO_PERMITIR_FINALIZAR_ENTREGA_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirFinalizarEntregaRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciarMonitoramentoAutomaticamenteDataCarregamento", Column = "TOP_INICIAR_MONITORAMENTO_DATA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarMonitoramentoAutomaticamenteDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAlterarDataChegadaVeiculo", Column = "TOP_PERMITIR_ALTERAR_DATA_DA_CHEGADA_DO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarInformarRICnaColetaDeConteiner", Column = "TOP_OBRIGAR_RIC_COLETA_CONTEINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarRICnaColetaDeConteiner { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarDadosNotaCte", Column = "TOP_INFORMAR_NOTA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDadosNotaCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirJustificativaParaEncerramentoManualViagem", Column = "TOP_EXIGIR_JUSTIFICATIVA_PARA_ENCERRAMENTO_MANUAL_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirJustificativaParaEncerramentoManualViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteImprimirOrdemColetaNaGuarita", Column = "TOP_PERMITE_IMPRIMIR_ORDEM_COLETA_NA_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteImprimirOrdemColetaNaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarPesoLiquidoLinkNotas", Column = "TOP_ENVIAR_PESO_LIQUIDO_LINK_NOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPesoLiquidoLinkNotas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoRetornoCarga", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga TipoRetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_CONSOLIDACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao TipoConsolidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_MODAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal ModalCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_PRECHECKIN", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoPrecheckin { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_PRECHECKIN_TRANSFERENCIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoPrecheckinTransferencia { get; set; }

        [Obsolete("Campo não é mais necessário")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_DA_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDaEntrega? TipoDaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCriarAprovacaoCargaConfirmarDocumento", Column = "TOP_NAO_CRIAR_APROVACAO_CARGA_CONFIRMAR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCriarAprovacaoCargaConfirmarDocumento{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarTomadorPedido", Column = "TOP_CONSIDERAR_TOMADOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarTomadorPedido { get; set; }

        #region Configuracao Fatura
        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarConfiguracaoFaturaPorTipoOperacao", Column = "TOP_USAR_CONF_FATURA_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarConfiguracaoFaturaPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoFatura", Column = "TOF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura ConfiguracaoTipoOperacaoFatura { get; set; }

        #endregion

        //FAVOR ADICIONAREM AS NOVAS CONFIGURAÇÕES EM TABELAS RELACIONADAS, JÁ TEM VÁRIAS CRIADAS, SEGUIR EXEMPLO CASO AINDA NÃO TENHA
        //QUANDO CRIAR TABELA NOVA, FAZER O INSERT DO PRIMEIRO REGISTRO

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual int CompareTo(TipoOperacao other)
        {
            if (other == null)
                return -1;

            return other.Codigo.CompareTo(Codigo);
        }
        public virtual TimeSpan TempoCarregamento
        {
            get { return TimeSpan.FromTicks(TempoCarregamentoTicks); }
            set { TempoCarregamentoTicks = value.Ticks; }
        }

        public virtual TimeSpan TempoDescarregamento
        {
            get { return TimeSpan.FromTicks(TempoDescarregamentoTicks); }
            set { TempoDescarregamentoTicks = value.Ticks; }
        }

        #endregion

        #region Repom

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_INTEGRACAO_REPOM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoRepom { get; set; }

        #endregion

        #region AngelLira

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRACAO_PROCEDIMENTO_EMBARQUE", TypeType = typeof(int), NotNull = false)]
        public virtual int IntegracaoProcedimentoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_MODELO_CONTRATACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoModeloContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TEMPO_ENTREGA_ANGELLIRA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEntregaAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_ENVIAR_DATA_INICIO_TERMINO_VIAGEM_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarDataInicioETerminoViagemAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_PRE_SM_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarPreSMAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REINTEGRAR_SM_CARGA_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReintegrarSMCargaAngelLira { get; set; }

        #endregion

        #region BrasilRisk

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_BRASILRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PEDIDO_LOGISTICO_BRASILRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoLogisticoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CENTRO_CUSTO_BRASILRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CentroCustoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_TRANSPORTADORA_BRASILRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJTransportadoraBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_CLIENTE_BRASILRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJClienteBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PRODUTO_BRASILRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENVIAR_NUMERO_PEDIDO_EMBARCADOR_CODIGO_CONTROLE_BRASILRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_ENVIAR_ORIGEM_COMO_ULTIMO_PONTO_BRASILRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarOrigemComoUltimoPontoRota { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposComprovante", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_OPERACAO_TIPO_COMPROVANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoComprovante", Column = "CTC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> TiposComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_UTILIZAR_CONFIGURACOES_DE_COMPROVANTES_DO_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa { get; set; }

        /// <summary>
        /// Caso o valor do frete enviado pelo embarcador seja diferente do valor calculado pela tabela de frete, é necessário autorização para emissão
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Só bloqueia se a diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete seja maior que um percentual
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERCENTUAL_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Caso exija a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_EMITIR_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirComplementoDiferencaFreteEmbarcador { get; set; }

        /// <summary>
        /// Tipo de ocorrência para a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoDiferencaFreteEmbarcador { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_GERAR_OCORRENCIA_SEM_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaSemTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_TIPO_OCORRENCIA_SEM_TABELA_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaSemTabelaFrete { get; set; }

        #endregion

        #region MundialRisk

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoMundialRisk", Column = "TOP_POSSUI_INTEGRACAO_MUNDIAL_RISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CENTRO_CUSTO_MUNDIALRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CentroCustoMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_TRANSPORTADORA_MUNDIALRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJTransportadoraMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_CLIENTE_MUNDIALRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJClienteMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PRODUTO_MUNDIALRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoMundialRisk { get; set; }

        #endregion

        #region Logiun

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoLogiun", Column = "TOP_POSSUI_INTEGRACAO_LOGIUN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CENTRO_CUSTO_LOGIUN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CentroCustoLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_TRANSPORTADORA_LOGIUN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJTransportadoraLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CNPJ_CLIENTE_LOGIUN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJClienteLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PRODUTO_LOGIUN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoLogiun { get; set; }

        #endregion

        #region Golden Service

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_INTEGRACAO_GOLDEN_SERVICE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoGoldenService { get; set; }

        #endregion

        #region Integração MultiEmbarcador

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_HABILITAR_INTEGRACAO_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TOKEN_INTEGRACAO_MULTIEMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TokenIntegracaoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_URL_INTEGRACAO_MULTIEMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_CIOT_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IntegrarCIOTMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_CARGAS_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IntegrarCargasMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DATA_INICIAL_CARGAS_MULTIEMBARCADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialCargasMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_IMPORTAR_CARGAS_COMPLEMENTARES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoImportarCargasComplementaresMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_CARGA_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarCargaMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_INTEGRAR_CANCELAMENTO_MULTIEMBARCADOR_COM_DADOS_INVALIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VINCULAR_DOCUMENTOS_AUTOMATICAMENTE_CARGA_EXISTENTE_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_UTILIZAR_GERACAO_DE_NFSE_AVANCADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarGeracaoDeNFSeAvancada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REGEX_NUMERO_PEDIDO_OBSERVACAO_CTE_MULTIEMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador { get; set; }

        #endregion

        #region Raster

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_RASTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_FILIAL_RASTER", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoFilialRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_PERFIL_SEGURANCA_RASTER", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoPerfilSegurancaRaster { get; set; }

        #endregion

        #region Pagbem

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DATA_ENTREGA_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DataEntregaPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PESO_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PesoPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TICKET_BALANCA_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? TicketBalancaPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_AVARIA_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AvariaPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CANHOTO_NFE_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CanhotoNFePagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_COMPROVANTE_PEDAGIO_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ComprovantePedagioPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DACTE_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DACTEPagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CONTRATO_TRANSPORTE_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ContratoTransportePagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DATA_DESEMBARQUE_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DataDesembarquePagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_RELATORIO_INSPECAO_DESEMBARQUE_PAGBEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RelatorioInspecaoDesembarquePagbem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_FRETE_TIPO_PESO_PAG_BEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FreteTipoPesoPagBem { get; set; }

        #endregion

        #region NOX

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_NOX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALOR_MINIMO_MERCADORIA_NOX", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ValorMinimoMercadoriaNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_PRE_SM_NOX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarPreSMNOX { get; set; }

        #endregion

        #region OpenTech

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_HABILITAR_OUTRA_CONFIGURACAO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarOutraConfiguracaoOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_USUARIO_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_SENHA_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_DOMINIO_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DominioOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_CLIENTE_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoClienteOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_PAS_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPASOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_URL_OPENTECH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_PRODUTO_PADRAO_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoPadraoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CODIGO_TRANSPORTADOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTransportadorOpenTech { get; set; }

        #endregion

        #region A52

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_POSSUI_INTEGRACAO_A52", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoA52 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_A52", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoA52 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_CARGA_A52", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoCargaA52 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TEMPO_ENTREGA_A52", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEntregaA52 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_OPERACAO_A52", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoOperacaoA52 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_PEDIDO_A52", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarPedidoA52 { get; set; }

        #endregion

        #region Infolog

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPlanoInfolog", Column = "TOP_TIPO_PLANO_INFOLOG", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoPlanoInfolog { get; set; }

        #endregion

        #region Adagio

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_VALIDAR_MOTORISTA_VEICULO_ADAGIO_AO_CONFIRMAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarMotoristaVeiculoAdagioAoConfirmarTransportador { get; set; }

        /// <summary>
        /// Não deve gerar contrato de frete ou CIOT.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_GERAR_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGerarContratoFreteTerceiro { get; set; }

        #endregion

        #region Buonny

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_MONITORAR_RETORNO_CARGA_BUONNY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MonitorarRetornoCargaBuonny { get; set; }

        #endregion

        #region Trizy

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_ENVIAR_COMPROVANTES_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarComprovantesDaCarga { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_HABILITAR_APP_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAppTrizy { get; set; }

        #endregion

        #region AX

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_NAO_REALIZAR_INTEGRACAO_COM_AX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoRealizarIntegracaoComAX { get; set; }

        #endregion

        #region MIC/DTA

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_REALIZAR_INTEGRACAO_COM_MIC_DTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoComMicDta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRAR_MIC_DTA_COM_SISCOMEX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarMICDTAComSiscomex { get; set; }

        #endregion

        #region Sem Parar

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_PERMITIR_CONSULTA_VALORES_PEDAGIO_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConsultaDeValoresPedagioSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_CONSULTA_VALORES_PEDAGIO_ADICIONAR_COMPONENTE_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ConsultaDeValoresPedagioAdicionarComponenteFrete { get; set; }

        #endregion

        #region Mercado Livre

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_INTEGRACAO_MERCADO_LIVRE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre? TipoIntegracaoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRACAO_MERCADO_LIVRE_CONSULTA_ROTAFACILITY_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_INTEGRACAO_MERCADO_LIVRE_AVANCAR_ETAPA_NFE_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TIPO_ACRESCIMO_DECRESCIMO_DATA_PREVISAO_SAIDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida TipoTempoAcrescimoDecrescimoDataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks", Column = "TOP_ACRESCENTAR_DATA_PREVISAO_SAIDA_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks { get; set; }

        public virtual TimeSpan TempoAcrescimoDecrescimoDataPrevisaoSaida
        {
            get { return TimeSpan.FromTicks(TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks); }
            set { TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks = value.Ticks; }
        }

        #endregion
    }
}