using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA", EntityName = "Carga", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.Carga", NameType = typeof(Carga))]
    public class Carga : CargaBase, IEquatable<Carga>, Interfaces.Embarcador.Entidade.IEntidade
    {

        public Carga()
        {
            DataAtualizacaoCarga = DateTime.Now;
            DataCriacaoCarga = DateTime.Now;
        }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_TROCA_NOTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_AGRUPAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_VINCULADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaVinculada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CARGA_ESPELHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaEspelho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCarga", Column = "CAR_SITUACAO", TypeType = typeof(SituacaoCarga), NotNull = true)]
        public virtual SituacaoCarga SituacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracaoFreteCarga", Column = "CAR_SITUACAO_ALTERACAO_FRETE_CARGA", TypeType = typeof(SituacaoAlteracaoFreteCarga), NotNull = false)]
        public virtual SituacaoAlteracaoFreteCarga SituacaoAlteracaoFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLiberacaoEscrituracaoPagamentoCarga", Column = "CAR_SITUACAO_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA", TypeType = typeof(SituacaoLiberacaoEscrituracaoPagamentoCarga), NotNull = false)]
        public virtual SituacaoLiberacaoEscrituracaoPagamentoCarga SituacaoLiberacaoEscrituracaoPagamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAutorizacaoIntegracaoCTe", Column = "CAR_SITUACAO_AUTORIZACAO_INTEGRACAO_CTE", TypeType = typeof(SituacaoAutorizacaoIntegracaoCTe), NotNull = false)]
        public virtual SituacaoAutorizacaoIntegracaoCTe SituacaoAutorizacaoIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCargaEmbarcador", Column = "CAR_CODIGO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequenciaCarga", Column = "CAR_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequenciaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAlfanumericoEmpresa", Column = "CAR_CODIGO_ALFANUMERICO_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAlfanumericoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAlfanumericoCarga", Column = "CAR_CODIGO_ALFANUMERICO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAlfanumericoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NUMERO_DOCA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NUMERO_ORDEM", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NUMERO_DOCA_ENCOSTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDocaEncosta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPendenciaConfiguracaoContabil", Column = "CAR_POSSUI_PENDENCIA_CONFIGURACAO_CONTABIL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiPendenciaConfiguracaoContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaFaturamentoLiberado", Column = "CAR_ETAPA_FATURAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaFaturamentoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RejeitadaPeloTransportador", Column = "CAR_REJEITADA_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RejeitadaPeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaSVM", Column = "CAR_CARGA_SVM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaTakeOrPay", Column = "CAR_CARGA_TAKE_OR_PAY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaTakeOrPay { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDemurrage", Column = "CAR_CARGA_DEMURRAGE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDemurrage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDetention", Column = "CAR_CARGA_DETETION", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDetention { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaSVMTerceiro", Column = "CAR_CARGA_SVM_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaSVMTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDestinadaCTeComplementar", Column = "CAR_CARGA_DESTINADA_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDestinadaCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaTrocaDeNota", Column = "CAR_TROCA_DE_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaTrocaDeNota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoSolicitacaoFrete", Column = "MSF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete MotivoSolicitacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoSolicitacaoFrete", Column = "CAR_OBSERVACAO_SOLICITACAO_FRETE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoSolicitacaoFrete { get; set; }

        /// <summary>05/
        /// É o valor liquido da carga, ou seja, o valor real que será pago pela carga, por exemplo, quando ocorrer um transbordo, parte do valor será destinado a outra carga, logo será descontado do valor liquido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteLiquido", Column = "CAR_VALOR_FRETE_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoSeguro", Column = "CAR_DESCONTO_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DescontoSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDescontoSeguro", Column = "CAR_PERCENTUAL_DESCONTO_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDescontoSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualBonificacaoTransportador", Column = "CAR_PERCENTUAL_BONIFICACAO_TRANSPORTADOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBonificacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BonificacaoTransportador", Column = "BNT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.BonificacaoTransportador BonificacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoFilial", Column = "CAR_DESCONTO_FILIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DescontoFilial { get; set; }

        /// <summary>
        /// Valor de frete que será utilizado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CAR_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteResidual", Column = "CAR_VALOR_FRETE_RESIDUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseFrete", Column = "CAR_VALOR_BASE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaiorValorBaseFreteDosPedidos", Column = "CAR_MAIOR_VALOR_BASE_FRETE_DOS_PEDIDOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MaiorValorBaseFreteDosPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteGerarCargaDistribuidor", Column = "CAR_PENDENTE_GERAR_CARGA_DISTRIBUIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteGerarCargaDistribuidor { get; set; }

        /// <summary>
        /// Valor do frete somando os componentes de frete (Exemplo: Pedágio, Descarga, entre outros)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAPagar", Column = "CAR_VALOR_FRETE_PAGAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAPagar { get; set; }

        /// <summary>
        /// Valor do frete segundo a tabela de frete somando os componentes de frete (Exemplo: Pedágio, ICMS, Descarga, entre outros)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteContratoFrete", Column = "CAR_VALOR_FRETE_CONTRATO_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteContratoFreteExcedente", Column = "CAR_VALOR_FRETE_CONTRATO_FRETE_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteContratoFreteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValorContrato", Column = "CAR_DATA_VALOR_CONTRATO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValorContrato { get; set; }

        /// <summary>
        /// Valor herdado do contrato de frete para fins de cálculo em relatório
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ContratoFreteFranquiaValorPorKm", Column = "CAR_CONTRATO_FRETE_FRANQUIA_VALOR_POR_KM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ContratoFreteFranquiaValorPorKm { get; set; }

        /// <summary>
        /// Valor herdado do contrato de frete para fins de cálculo em relatório
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ContratoFreteFranquiaValorKmExcedente", Column = "CAR_CONTRATO_FRETE_FRANQUIA_VALOR_KM_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ContratoFreteFranquiaValorKmExcedente { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaExcedenteContrato", Column = "CAR_DISTANCIA_EXCEDENTE_CONTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaExcedenteContrato { get; set; }

        /// <summary>
        /// Valor do frete segundo a tabela de frete somando os componentes de frete (Exemplo: Pedágio, ICMS, Descarga, entre outros)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTabelaFrete", Column = "CAR_VALOR_FRETE_TABELA_DE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTabelaFreteFilialEmissora", Column = "CAR_VALOR_FRETE_TABELA_DE_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabelaFreteFilialEmissora { get; set; }

        /// <summary>
        /// Valor informado pelo Operador manualmente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteOperador", Column = "CAR_VALOR_FRETE_OPERADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteOperador { get; set; }

        /// <summary>
        /// Valor informado pelo Embarcador (via integração, por exemplo: Natura)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteEmbarcador", Column = "CAR_VALOR_FRETE_EMBARCADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Valor do leilão para a carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteLeilao", Column = "CAR_VALOR_FRETE_LEILAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFreteEscolhido", Column = "CAR_TIPO_FRETE_ESCOLHIDO", TypeType = typeof(TipoFreteEscolhido), NotNull = false)]
        public virtual TipoFreteEscolhido TipoFreteEscolhido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CAR_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "CAR_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "CAR_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        /// <summary>
        /// Usado apenas para nível informativo da Carga em um contexto geral a nível de controle deve usar o tipo de contrato da carga pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratacaoCarga", Column = "CAR_CONTRATACAO_CARGA", TypeType = typeof(TipoContratacaoCarga), NotNull = false)]
        public virtual TipoContratacaoCarga TipoContratacaoCarga { get; set; }

        /// <summary>
        /// Usado apenas para customizar a timeline da carga para projeto cabotagem, não pode interferir com o TipoContratacaoCarga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoCarga", Column = "CAR_TIPO_SERVICO_CARGA", TypeType = typeof(TipoServicoCarga), NotNull = false)]
        public virtual TipoServicoCarga? TipoServicoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCondicaoPagamento", Column = "CAR_TIPO_CONDICAO_PAGAMENTO", TypeType = typeof(Enumeradores.TipoCondicaoPagamento), NotNull = false)]
        public virtual Enumeradores.TipoCondicaoPagamento? TipoCondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "CAR_VALOR_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        #region Filial Emissora

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFilialEmissora", Column = "CAR_VALOR_ICMS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFilialEmissora", Column = "CAR_VALOR_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAPagarFilialEmissora", Column = "CAR_VALOR_FRETE_PAGAR_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAPagarFilialEmissora { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgConfirmacaoUtilizacaoCredito", Column = "CAR_AG_CONFIRMACAO_UTILIZACAO_CREDITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgConfirmacaoUtilizacaoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPendencia", Column = "CAR_POSSUI_PENDENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenciaEmissaoAutomatica", Column = "CAR_SEM_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenciaEmissaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgNFSManual", Column = "CAR_AG_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgImportacaoCTe", Column = "CAR_AG_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaSemTodosPreCTes", Column = "CAR_LIBERADA_SEM_TODOS_PRE_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaSemTodosPreCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaEtapaFaturamentoBloqueada", Column = "CAR_LIBERADA_ETAPA_FATURAMENTO_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaEtapaFaturamentoBloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaSemRetiradaContainer", Column = "CAR_LIBERADA_SEM_RETIRADA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaSemRetiradaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTesEmDigitacao", Column = "CAR_CTES_EM_DIGITACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTesEmDigitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgImportacaoMDFe", Column = "CAR_AG_IMPORTACAO_MDFe", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgImportacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgValorRedespacho", Column = "CAR_AG_VALOR_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgValorRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NAO_GERAR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NAO_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_TIPO_PAGAMENTO_VALE_PEDAGIO", TypeType = typeof(Dominio.Enumeradores.TipoPagamentoValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamentoValePedagio TipoPagamentoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CARGA_AGRUPADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaAgrupada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CARGA_DE_VINCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDeVinculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AG_SELECAO_ROTA_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgSelecaoRotaOperador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_ANALISTA_RESPONSAVEL_MONITORAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario AnalistaResponsavelMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_OPERADOR_CONTRATOU_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorContratouCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_RESPONSAVEL_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario ResponsavelEntrega { get; set; }

        /// <summary>
        /// É o grupo de pessoas principal da Carga, responsável pelo Frete (Os pedidos podem ter mais, mas é necessário ter um grupo principal para poder direcionar as cargas para os operadores)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoaPrincipal { get; set; }


        /// <summary>
        /// É o grupo de pessoas para qual o veículo estava vinculado no momento da carga (segmento)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_SEGMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas SegmentoGrupoPessoas { get; set; }
        /// <summary>
        /// É o modelo veicular para qual o veículo informado na carga estava configurado (segmento)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO_SEGMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga SegmentoModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.ContainerTipo TipoContainer { get; set; }

        /// <summary>
        /// Quando uma carga é uma carga de terceiro seta a flag.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "FreteDeTerceiro", Column = "CAR_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FreteDeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PROVEDOR_OS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ProvedorOS { get; set; }

        /// <summary>
        /// indica qual é o local/cliente que a próxima coleta deve ser feita.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_COLETA_LIBERADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeColetaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "CAR_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirCTeComplementar", Column = "CAR_EMITIR_CTE_COMPLMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaColeta", Column = "CAR_CARGA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemRoteirizacaoDefinida", Column = "CAR_ORDEM_COLETA_PRE_DEFINIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdemRoteirizacaoDefinida { get; set; }

        /// <summary>
        /// Indica quando se o veículo que vai fazer o transporte foi integrado ao Embarcador.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoIntegradoEmbarcador", Column = "CAR_VEICULO_INTEGRADO_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool VeiculoIntegradoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFreteCliente", Column = "CAR_CALCULAR_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoCarga", Column = "CAR_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacaoCarga", Column = "CAR_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFechamentoCarga", Column = "CAR_DATA_FECHAMENTO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoCarga", Column = "CAR_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_INICIO_GERACAO_CTES", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioGeracaoCTes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FixarUtilizacaoContratoTransportador", Column = "CAR_FIXAR_UTILIZACAO_CONTRATO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FixarUtilizacaoContratoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        /// <summary>
        /// Usada para informar a filial de origem da carga, quando por exemplo uma carga sofre um redespacho em outra filial a filial será trocada porém mantem a referencia da filial de origem.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Filiais.Filial FilialOrigem { get; set; }

        /// <summary>
        /// Caso o destinatario da carga é uma filial sera armazenado neste campo, usado para listar as cargas onde o operador tenha restricao por filiais considerando a filial da carga e tambem filial destino da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial FilialDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_CARGA_AGRUPADA_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial FilialCargaAgrupadaValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaixaTemperatura", Column = "FTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura FaixaTemperatura { get; set; }

        /// <summary>
        /// utilizado para armazenar a integracao da temperatura em caso de não identificar na integração da carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGRACAO_TEMPERATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IntegracaoTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProcedimentoEmbarque", Column = "PRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque ProcedimentoEmbarque { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoTerminoCarga", Column = "CAR_DATA_TERMINO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoTerminoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialPrevisaoCarregamento", Column = "CAR_DATA_INICIAL_PREVISAO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialPrevisaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalPrevisaoCarregamento", Column = "CAR_DATA_FINAL_PREVISAO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalPrevisaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoTerminoViagem", Column = "CAR_DATA_PREVISAO_TERMINO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoTerminoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoStopTracking", Column = "CAR_DATA_PREVISAO_STOP_TRACKING", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoStopTracking { get; set; }

        /// <summary>
        /// Quando uma carga é uma carga de substituição, esse campo indentifica a carga cancelada (Entidade CargaCancelamento) que gerou a nova carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }


        /// <summary>
        /// Quando uma carga é uma carga de transbordo seta a flag.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaTransbordo", Column = "CAR_CARGA_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaTransbordo { get; set; }

        /// <summary>
        /// Quando existe uma pendencia no calculo do frete, está propriedade serve para informar o motivo da pendencia
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoPendenciaFrete", Column = "CAR_MOTIVO_PENDENCIA_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete MotivoPendenciaFrete { get; set; }

        /// <summary>
        /// Quando existe uma pendencia, está propriedade serve para informar o motivo da pendencia
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoPendencia", Column = "CAR_MOTIVO_PENDENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoPendencia { get; set; }

        /// <summary>
        /// Indica quando a carga está fechada ou em processo de montagem (alocação de pedidos). Somente depois de fechada é possível configura-lá e emitir seus documentos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaFechada", Column = "CAR_CARGA_FECHADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargaFechada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaGeradaViaDocumentoTransporte", Column = "CAR_CARGA_GERADA_VIA_DOCUMENTO_TRANSPORTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargaGeradaViaDocumentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcultarNoPatio", Column = "CAR_OCULTAR_NO_PATIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool OcultarNoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearAlteracaoJanelaDescarregamento", Column = "CAR_BLOQUEAR_ALTERACAO_JANELA_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAlteracaoJanelaDescarregamento { get; set; }

        /// <summary>
        /// Indica se a carga já foi integrada ao embarcador.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaIntegradaEmbarcador", Column = "CAR_CARGA_INTEGRADA_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CargaIntegradaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO_NFE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.WebService.Integradora IntegradoraNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AVERBANDO_CTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AverbandoCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_EMITINDO_NFE_REMESSA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmitindoNFeRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGRANDO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrandoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGRANDO_GNRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrandoGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitindoCTes", Column = "CAR_EMITINDO_CTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmitindoCTes { get; set; }

        /// <summary>
        /// Flag indica que todo o controle da emissão do CRT na primeira etapa de geracao dos documentos, apos a carga ir para a etapa de ag.Nfe depois de emitir o CRT a flag é setada para false (para entao emitir o cte normal) usada para cargas do tipo Mercosul.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitindoCRT", Column = "CAR_EMITINDO_CRT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitindoCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "problemaCTE", Column = "CAR_PROBLEMA_CTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool problemaCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "problemaNFS", Column = "CAR_PROBLEMA_NFS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool problemaNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "problemaMDFe", Column = "CAR_PROBLEMA_MDFE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool problemaMDFe { get; set; }

        /// <summary>
        /// Flag indica que todo o controle da emissão dos CT-es está ocorrendo na emissão do Ct-e de subcontratação da filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaParaEmissaoCTeSubContratacaoFilialEmissora", Column = "CAR_LIBERARA_PARA_EMISSAO_CTE_SUBCONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaParaEmissaoCTeSubContratacaoFilialEmissora { get; set; }

        /// <summary>
        /// inidica que está emitindo os Ct-es de Subcontratacao da filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmEmissaoCTeSubContratacaoFilialEmissora", Column = "CAR_EM_EMISSAO_CTE_SUBCONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmEmissaoCTeSubContratacaoFilialEmissora { get; set; }

        /// <summary>
        /// inidica que está emitindo os Ct-es de Subcontratacao da filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AgGeracaoCTesAnteriorFilialEmissora", Column = "AG_GERACAO_CTE_ANTERIOR_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgGeracaoCTesAnteriorFilialEmissora { get; set; }

        /// <summary>
        /// Inidica que a carga recebeu CT-es Terceiros gerados a partir de CT-es Filial Emissora de Trecho Anterior
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaGeradaComCTeAnteriorFilialEmissora", Column = "CAR_GERADA_COM_CTE_ANTERIOR_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaGeradaComCTeAnteriorFilialEmissora { get; set; }

        /// <summary>
        /// Indica se o MDF-e deve ser emitido pela filial emissora ou se pelo transportador, quando ouver emissão de filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteMDFeFilialEmissora", Column = "CAR_EMITIR_MDFE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteMDFeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracaoCIOT", Column = "CAR_PROBLEMA_INTEGRACAO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaIntegracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGRANDO_CIOT", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrandoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "problemaAverbacaoCTe", Column = "CAR_PROBLEMA_AVERBACAO_CTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool problemaAverbacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "problemaEmissaoNFeRemessa", Column = "CAR_PROBLEMA_EMISSAO_NFE_REMESSA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool problemaEmissaoNFeRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracaoPagamentoMotorista", Column = "CAR_PROBLEMA_INTEGRACAO_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaIntegracaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgIntegracaoPagamentoMotorista", Column = "CAR_AGUARDANDO_INTEGRACAO_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgIntegracaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracaoValePedagio", Column = "CAR_PROBLEMA_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ProblemaIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracaoGNRE", Column = "CAR_PROBLEMA_INTEGRACAO_GNRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaIntegracaoGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoComProblemaValePedagio", Column = "CAR_LIBERADO_COM_PROBLEMA_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberadoComProblemaValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoComProblemaGNRE", Column = "CAR_LIBERADO_COM_PROBLEMA_GNRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoComProblemaGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarComProblemaAverbacao", Column = "CAR_LIBERAR_COM_PROBLEMA_AVERBACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarComProblemaAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoComProblemaPagamentoMotorista", Column = "CAR_LIBERAR_COM_PROBLEMA_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoComProblemaPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoComProblemaCIOT", Column = "CAR_LIBERAR_COM_PROBLEMA_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoComProblemaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AVERBOU_TODOS_CTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AverbouTodosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AutorizouTodosCTes", Column = "CAR_AUTORIZOU_TODOS_CTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AutorizouTodosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimentoUltimaNFe", Column = "CAR_DATA_RECEBIMENTO_ULTIMA_NFE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimentoUltimaNFe { get; set; }

        /// <summary>
        /// Indica quando a carga está recebendo as notas fiscais por email, registrando a data da ultima nota recebida.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioUltimaNFe", Column = "CAR_DATA_ENVIO_ULTIMA_NFE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioUltimaNFe { get; set; }

        /// <summary>
        /// Indica quando a carga foi emitida, ou seja, a data que o veículo fiscalmente saiu do cliente. OBS se uma carga for duplicada essa data fica com a data da primeira carga caso já tenha sido emitida.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacaoEmissao", Column = "CAR_DATA_FINALIZACAO_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoEmissao { get; set; }

        /// <summary>
        /// Quando verdadeiro não é possível informar o veiculo o motorista e o modelo veícular.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigeVeiculoParaEmissao", Column = "CAR_NAO_EXIGE_VEICULO_PARA_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoExigeVeiculoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardandoEmissaoDocumentoAnterior", Column = "CAR_AG_EMISSAO_DOCUMENTO_ANTERIOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AguardandoEmissaoDocumentoAnterior { get; set; }

        /// <summary>
        /// Exige que a nota fiscal seja enviada para calcular o Frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeNotaFiscalParaCalcularFrete", Column = "CAR_EXIGE_NOTA_FISCAL_PARA_CALCULAR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExigeNotaFiscalParaCalcularFrete { get; set; }

        [Obsolete] //FOI REMOVIDA PARA UTILIZACAO DE UM INTEIRO CalcularFreteLote QUE CONTROLA 3 THREDS PARA CALCULO DE FRETE
        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFretePorLote", Column = "CAR_CALCULAR_FRETE_POR_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CalcularFretePorLote { get; set; }

        //0 E NULL cargas que vao para a thread padrao ControleCargaCalculoFrete
        //1 para cargas que vao para a thread controleCargaCalculoFreteIntegracao
        //2 para cargas que serao reprocessadas em outra thread ControleCargaCalculoFreteReprocessamento (desativada por padrao)
        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFreteLote", Column = "CAR_CALCULAR_FRETE_LOTE", TypeType = typeof(Enumeradores.LoteCalculoFrete), NotNull = false)]
        public virtual Enumeradores.LoteCalculoFrete? CalcularFreteLote { get; set; }

        /// <summary>
        /// Está gerando os registros de integrações de documentos. Nesta situação não deve permitir fazer nenhum tipo de alteração na carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoIntegracoes", Column = "CAR_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerandoIntegracoes { get; set; }

        /// <summary>
        /// Prioridade de envio de integrações.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeEnvioIntegracao", Column = "CAR_PRIORIDADE_ENVIO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeEnvioIntegracao { get; set; }

        /// <summary>
        /// Dados sumarizados da carga (remetentes, destinatários, origens, destinos, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDadosSumarizados", Column = "CDS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados DadosSumarizados { get; set; }

        /// <summary>
        /// Indicar se controla ou não o tempo que o CT-e pode ficar em emissão, por exemplo, cargas com muitos CT-es tendem a levar um grande tempo para emissão.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaTempoParaEmissao", Column = "CAR_CONTROLA_TEMPO_PARA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaTempoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO_CLIENTE_DESLOCAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaClienteDeslocamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CLIENTE_DESLOCAMENTO_QUILOMETROS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DeslocamentoQuilometros { get; set; }

        /// <summary>
        /// Indica se houve desistência da carga (à partir do cancelamento do pedido).
        /// Deve ser calculado para este caso somente o valor do frete mínimo de acordo com o % informado no tipo de operação.
        /// Não devem ser calculados valores de componentes da tabela de frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DESISTENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Desistencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DESISTENCIA_PERCENTUAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDesistencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_QUANTIDADE_HORAS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_QUANTIDADE_HORAS_EXCEDENTES", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeHorasExcedentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaAverbacaoMDFe", Column = "CAR_PROBLEMA_AVERBACAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaAverbacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarComProblemaAverbacaoMDFe", Column = "CAR_LIBERAR_COM_PROBLEMA_AVERBACAO_MDFE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarComProblemaAverbacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM_TROCA_NOTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade OrigemTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "CAR_DISTANCIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRateioProdutos", Column = "CAR_TIPO_RATEIO_PRODUTOS", TypeType = typeof(Enumeradores.TipoRateioProdutos), NotNull = false)]
        public virtual Enumeradores.TipoRateioProdutos TipoRateioProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroImpressora", Column = "CAR_NUMERO_IMPRESSORA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroImpressora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CARGA_EMITIDA_PARCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaEmitidaParcialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NAO_AVANCAR_AUTOMATICAMENTE_ETAPA_DOCUMENTO_POR_PENDENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAvancarAutomaticamenteEtapaDocumentoPorPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_NAO_AVANCAR_AUTOMATICAMENTE_ETAPA_FRETE_POR_PENDENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAvancarAutomaticamenteEtapaFretePorPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SeparacaoMercadoriaConfirmada", Column = "CAR_SEPARACAO_MERCADORIA_CONFIRMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaConfirmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioSeparacaoMercadoria", Column = "CAR_DATA_INICIO_SEPARACAO_MERCADORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioSeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacaoSeparacaoMercadoria", Column = "CAR_DATA_ATUALIZACAO_SEPARACAO_MERCADORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacaoSeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSeparacaoMercadoria", Column = "CAR_PERCENTUAL_SEPARACAO_MERCADORIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PercentualSeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Containeres", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + cote.CTR_NUMERO
                                                                                        FROM T_CARGA_PEDIDO cargaPedido
                                                                                        inner join T_PEDIDO ped ON ped.PED_CODIGO = cargaPedido.PED_CODIGO
                                                                                        inner join T_CONTAINER cote ON cote.CTR_CODIGO = ped.CTR_CODIGO
                                                                                        WHERE cargaPedido.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Containeres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModaisCarga", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + (CASE 
									                                                                                    WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 THEN '1 - Porto a Porta' 
									                                                                                    WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 2 THEN '2 - Porta a Porto' 
									                                                                                    WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 THEN '3 - Porta a Porta' 
									                                                                                    WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 THEN '4 - Porto a Porto'
									                                                                                    ELSE ''
									                                                                                    END)
                                                                                    FROM T_CARGA_PEDIDO cargaPedido
                                                                                    WHERE cargaPedido.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ModaisCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TiposTomador", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + (CASE 
											                                                                            WHEN cargaPedido.PED_TIPO_TOMADOR = 0 THEN 'Remetente' 
											                                                                            WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 THEN 'Destinatário' 
											                                                                            WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 THEN 'Outros' 
											                                                                            WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 2 THEN 'Recebedor'
											                                                                            WHEN cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 THEN 'Expedidor'
											                                                                            ELSE ''
											                                                                            END)
		                                                                            FROM T_CARGA_PEDIDO cargaPedido
		                                                                            WHERE cargaPedido.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string TiposTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacasVeiculos", Formula = @"((select vei.VEI_PLACA from T_VEICULO vei where vei.VEI_CODIGO = CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), ''))", TypeType = typeof(string), Lazy = true)]
        public virtual string PlacasVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFrotasVeiculos", Formula = @"((select vei.VEI_NUMERO_FROTA from T_VEICULO vei where vei.VEI_CODIGO = CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_NUMERO_FROTA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = CAR_CODIGO AND veiculo1.VEI_NUMERO_FROTA <> '' FOR XML PATH('')), ''))", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroFrotasVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotoristas", Formula = @"SUBSTRING((SELECT ', ' + motorista1.FUN_NOME + (CASE WHEN motorista1.FUN_FONE is null or motorista1.FUN_FONE = '' THEN '' ELSE ' (' + motorista1.FUN_FONE  + ')' END) FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NomeMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPrimeiroMotorista", Formula = @"ISNULL((SELECT TOP 1 motorista1.FUN_CODIGO FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO), 0)", TypeType = typeof(string), Lazy = true)]
        public virtual string CodigoPrimeiroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFPrimeiroMotorista", Formula = @"ISNULL((SELECT TOP 1 motorista1.FUN_CPF FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string CPFPrimeiroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomePrimeiroMotorista", Formula = @"ISNULL((SELECT TOP 1 motorista1.FUN_NOME FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string NomePrimeiroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RGMotoristas", Formula = @"SUBSTRING((SELECT ', ' + motorista1.FUN_RG FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string RGMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CAR_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosCTes", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS NVARCHAR(20))
                                                                                    FROM T_CARGA_CTE cargaCTe
                                                                                 inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                    WHERE cargaCTe.CAR_CODIGO = CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(SELECT SUM(cte.CON_VALOR_RECEBER)
                                                               FROM T_CARGA_CTE cargaCTe
                                                            inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                               WHERE cargaCTe.CAR_CODIGO = CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL and cargaCTe.CCC_CODIGO is null)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal? ValorAReceberCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(SELECT SUM(cte.CON_VALOR_FRETE)
                                                               FROM T_CARGA_CTE cargaCTe
                                                            inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                               WHERE cargaCTe.CAR_CODIGO = CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL and cargaCTe.CCC_CODIGO is null)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal? ValorFreteCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(SELECT SUM(cte.CON_VAL_ICMS)
                                                               FROM T_CARGA_CTE cargaCTe
                                                            inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                               WHERE cargaCTe.CAR_CODIGO = CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL and cargaCTe.CCC_CODIGO is null)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal? ValorICMSCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosCTesOriginal", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS NVARCHAR(20))
                                                                                    FROM T_CARGA_CTE cargaCTe
                                                                                 inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                    WHERE cargaCTe.CAR_CODIGO = CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL AND cargaCTe.CCC_CODIGO IS NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosCTesOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaEmissaoERP", Column = "CAR_LIBERADA_EMISSAO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaEmissaoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloVeicularPlaca", Formula = @"(SELECT TOP 1 (CASE WHEN vei.MVC_CODIGO IS NOT NULL THEN mve.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ELSE mov.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR END)  
                                                                                              FROM T_CARGA car
                                                                                              LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA mov ON mov.MVC_CODIGO = car.MVC_CODIGO
                                                                                              JOIN T_VEICULO vei ON car.CAR_VEICULO = vei.VEI_CODIGO
                                                                                              LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA mve ON mve.MVC_CODIGO = vei.MVC_CODIGO
                                                                                              WHERE car.CAR_CODIGO = CAR_CODIGO)", TypeType = typeof(string), Lazy = true)]
        public virtual string ModeloVeicularPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public override ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CPE_CODIGO")]
        public virtual ICollection<CargaPedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Monitoramento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Monitoramento", Column = "MON_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.Monitoramento> Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaOrigemPedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO_ORIGEM")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CPE_CODIGO")]
        public virtual ICollection<CargaPedido> CargaOrigemPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargasAgrupamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO_AGRUPAMENTO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Carga> CargasAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Entregas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntrega", Column = "CEN_CODIGO")]
        public virtual ICollection<ControleEntrega.CargaEntrega> Entregas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosCarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_VEICULOS_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaCTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaOrigemCTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO_ORIGEM")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaOrigemCTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaMDFes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFe", Column = "CMD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> CargaMDFes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Motoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "CAR_MOTORISTA")]
        public virtual IList<Dominio.Entidades.Usuario> Motoristas { get; set; }

        public override ICollection<Usuario> ListaMotorista
        {
            get { return Motoristas; }
            set { Motoristas = value?.ToList(); }
        }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ajudantes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_AJUDANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "CAR_AJUDANTE")]
        public virtual IList<Dominio.Entidades.Usuario> Ajudantes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COMPONENTES_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaComponentesFrete", Column = "CCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaIntegracao", Column = "CIN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaIntegracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CARGA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCargaIntegracao", Column = "CAI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> CargaIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "IntegracoesAvon", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_AVON")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaIntegracaoAvon", Column = "CIA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon> IntegracoesAvon { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "IntegracoesNatura", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_NATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaIntegracaoNatura", Column = "CNA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> IntegracoesNatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Lacres", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_LACRE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaLacre", Column = "CLA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaLacre> Lacres { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transbordos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSBORDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Transbordo", Column = "TRB_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.Transbordo> Transbordos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Percursos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PERCURSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPercurso", Column = "CPD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> Percursos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LocaisPrestacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_LOCAIS_PRESTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaLocaisPrestacao", Column = "CLP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> LocaisPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoLocalEntrega", Column = "CAR_OBSERVACAO_LOCAL_ENTREGA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoLocalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CodigosAgrupados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CODIGOS_AGRUPADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CAR_CODIGO_CARGA_AGRUPADO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> CodigosAgrupados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaOcorrencia", Column = "COC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Ocorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoCarga", Column = "ACC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> AcertosViagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaCIOTs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CIOT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCIOT", Column = "CCO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> CargaCIOTs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_POSSUI_SEPARACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_POSSUI_SEPARACAO_VOLUME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiSeparacaoVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_SEPARACAO_CONFERIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoConferida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CALCULANDO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalculandoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_ORIGEM_FRETE_JANELA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrigemFretePelaJanelaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CALCULAR_FRETE_SEM_ESTORNAR_COMPLEMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteSemEstornarComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaDeAgrupamento", Column = "CAR_PLACA_AGRUPAMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PlacaDeAgrupamento { get; set; }

        /// <summary>
        /// Após emitir todos os CT-es/MDF-es a carga deverá passar pelo processo de finalização, que gerará movimentos, títulos e etc.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_FINALIZANDO_PROCESSO_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FinalizandoProcessoEmissao { get; set; }

        /// <summary>
        /// Após a emissão dos documentos, a carga (se configurada) deverá passar pela geração de coleta/entrega.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GERANDO_CONTROLE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoControleColetaEntrega { get; set; }

        /// <summary>
        /// Indica se gerou a movimentação de autorização da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_MOVIMENTACAO_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouMovimentacaoAutorizacao { get; set; }

        /// <summary>
        /// Indica se gerou a movimentação de autorização da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_MOVIMENTACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouMovimentacaoCancelamento { get; set; }

        /// <summary>
        /// Inidica de gerou todos os títulos da carga na finalização da emissão.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_TITULO_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouTituloAutorizacao { get; set; }

        /// <summary>
        /// Inidica de gerou todos os títulos da carga na finalização da emissão.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_TITULO_GNRE_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouTituloGNREAutorizacao { get; set; }

        /// <summary>
        /// Inidica de gerou a fatura par ao take or pay.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_FATURAMENTO_TAKE_OR_PAY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouFaturamentoTakeOrPay { get; set; }

        /// <summary>
        /// Indica se foram gerados todos os controles de faturamento da carga na finalização da emissão.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_CONTROLE_FATURAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouControleFaturamento { get; set; }

        /// <summary>
        /// Indica se foram gerados todos os canhotos de CT-es da carga na finalização da emissão.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GEROU_CANHOTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouCanhoto { get; set; }

        /// <summary>
        /// Seta quando o usuário seleciona uma tabela na etapa de frete (tabela por rotas, utilizado na Tirol) para calcular na thread de cálculo de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteRota", Column = "TFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteRota TabelaFreteRota { get; set; }

        /// <summary>
        /// Caso a carga tenha diferença do valor do frete informado pelo embarcador pelo calculado pela tabela, será bloqueada (de acordo com as configurações), tendo que ser liberada manualmente na etapa de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_BLOQUEADA_DIFERENCA_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloqueadaDiferencaValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_DIFERENCA_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiferencaValorFrete { get; set; }

        /// <summary>
        /// Caso a carga tenha diferença do valor do frete informado pelo embarcador pelo calculado pela tabela, indica se irá gerar uma ocorrência da diferença do valor
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GERAR_OCORRENCIA_DIFERENCA_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaDiferencaValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_DIFERENCA_VALOR_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaDiferencaValorFrete { get; set; }

        /// <summary>
        /// Informa se não deve ajustar os dados de pagamento dos pedidos ao calcular o frete, pois os dados foram informados manualmente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DADOS_PAGAMENTO_INFORMADOS_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DadosPagamentoInformadosManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CARGA_RETORNADA_ETAPA_NFE_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaRetornadaEtapaNFeManualmente { get; set; }

        /// <summary>
        /// Informa se a apólice de seguro foi informada manualmente, para evitar que se remova a apólice que o operador informou em alguns processos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_APOLICE_SEGURO_INFORMADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApoliceSeguroInformadaManualmente { get; set; }

        /// <summary>
        /// Campo utilizado para controle da numeração da carga (evitar duplicidade em banco com chave única permitindo o cancelamento com duplicação de carga)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CONTROLE_NUMERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ControleNumeracao { get; set; }

        /// <summary>
        /// Campo utilizado para controlar a situação da roteirizacao
        /// </summary>
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoRoteirizacaoRota", Column = "CAR_SITUACAO_ROTEIRIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao SituacaoRoteirizacaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoRoteirizacaoCarga", Column = "CAR_SITUACAO_CARGA_ROTEIRIZADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao SituacaoRoteirizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativasRoteirizacaoCarga", Column = "CAR_NUMERO_TENTATIVAS_ROTEIRIZACAO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasRoteirizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_ULTIMA_TENTATIVA_ROTEIRIZACAO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaTentativaRoteirizacaoCarga { get; set; }

        /// <summary>
        /// Campo utilizado para não gerar roteirização quando a mesma é informada pelo método InformarRotaCarga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CARGA_ROTA_FRETE_INFORMADA_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaRotaFreteInformadaViaIntegracao { get; set; }

        /// <summary>
        /// Indica se a carga está realizando o processamento de avanço da etapa de notas fiscais (antigo processamento realizado diretamente no controller).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_PROCESSANDO_DOCUMENTOS_FISCAIS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ProcessandoDocumentosFiscais { get; set; }

        /// <summary>
        /// Indica se a carga já foi integrada com o transportador/MultiTMS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGROU_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrouTransportador { get; set; }

        /// <summary>
        /// Indica se a carga de terceiro já foi integrada com o transportador/MultiTMS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTEGROU_TERCEIRO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrouTerceiroTransportador { get; set; }

        /// <summary>
        /// Campo utilizado para controle do protocolo de integração da carga (retornado nas integrações para os clientes)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_PROTOCOLO", TypeType = typeof(int), NotNull = false)]
        public virtual int Protocolo { get; set; }

        /// <summary>
        /// Campo utilizado para controlar as cargas que estão sendo fechadas por thread
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_FECHANDO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FechandoCarga { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "PracasPedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PRACA_PEDAGIO")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PracaPedagio", Column = "PRP_CODIGO")]
        //public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> PracasPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_INICIO_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_INICIO_VIAGEM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeInicioViagem", Column = "CAR_LATITUDE_INICIO_VIAGEM", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeInicioViagem", Column = "CAR_LONGITUDE_INICIO_VIAGEM", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioDeViagemNoRaio", Column = "CAR_INICIO_VIAGEM_NO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDeViagemNoRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDeComplemento", Column = "CAR_CARGA_DE_COMPLEMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDeComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDePreCarga", Column = "CAR_CARGA_DE_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDePreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDePreCargaEmFechamento", Column = "CAR_CARGA_DE_PRE_CARGA_EM_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDePreCargaEmFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDePreCargaFechada", Column = "CAR_CARGA_DE_PRE_CARGA_FECHADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDePreCargaFechada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_PRE_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargaVincularPreCarga", Column = "CAR_NUMERO_CARGA_VINCULAR_PRE_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCargaVincularPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDI", Column = "CAR_NUMERO_DI", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_FIM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_FIM_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_FIM_VIAGEM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimViagemReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_CONFIGURACAO_TABELA_FRETE_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfiguracaoTabelaFretePorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioCarregamentoInformadoNoPedido", Column = "CAR_HORARIO_CARREGAMENTO_INFORMADO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioCarregamentoInformadoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContingenciaFSDA", Column = "CAR_CONTINGENCIA_FSDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ContingenciaFSDA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoViagem", Column = "CAR_CODIGO_INTEGRACAO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCancelamentoCarga", Column = "CAR_BLOQUEAR_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCusteioSVM", Column = "CAR_VALOR_CUSTEIO_SVM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCusteioSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPossuiOutrosNumerosEmbarcador", Column = "CAR_POSSUI_OUTROS_NUMEROS_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPossuiOutrosNumerosEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarregamentoIntegradoERP", Column = "CAR_CARREGAMENTO_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CarregamentoIntegradoERP { get; set; }

        /// <summary>
        /// Redespacho que originou a carga
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Redespacho", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Redespacho Redespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardarIntegracaoEtapaTransportador", Column = "CAR_AGUARDAR_INTEGRACAO_ETAPA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AguardarIntegracaoEtapaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_FRETE_NEGOCIADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteNegociado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_VALOR_TOTAL_MOEDA_PAGAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoedaPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AGRUPAR_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracaoGrMotoristaVeiculo", Column = "CAR_PROBLEMA_INTEGRACAO_MOTORISTA_TELERISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaIntegracaoGrMotoristaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoComProblemaIntegracaoGrMotoristaVeiculo", Column = "CAR_LIBERADO_COM_INTEGRACAO_MOTORISTA_TELERISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoComProblemaIntegracaoGrMotoristaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LicencaInvalida", Column = "CAR_LICENCA_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LicencaInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaComLicencaInvalida", Column = "CAR_LIBERADA_COM_LICENCA_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaComLicencaInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemProblemaIntegracaoGrMotoristaVeiculo", Column = "CAR_MENSAGEM_PROBLEMA_INTEGRACAO_MOTORISTA_TELERISCO", Type = "StringClob", NotNull = false)]
        public virtual string MensagemProblemaIntegracaoGrMotoristaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloIntegracaoGR", Column = "CAR_PROTOCOLO_INTEGRACAO_GR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloIntegracaoGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoChegadaOrigem", Column = "CAR_DATA_PREVISAO_CHEGADA_ORIGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegadaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPager", Column = "CAR_NUMERO_PAGER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroPager { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeEmpurrador", Column = "CAR_NOME_EMPURRADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeEmpurrador { get; set; }

        /// <summary>
        /// Data que a carga está realizando o processamento de avanço da etapa de notas fiscais (antigo processamento realizado diretamente no controller).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_FINALIZACAO_PROCESSAMENTO_DOCUMENTOS_FISCAIS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoProcessamentoDocumentosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgendaExtra", Column = "CAR_AGENDA_EXTRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgendaExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioConfirmacaoDocumentosFiscais", Column = "CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioConfirmacaoDocumentosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSolicitacaoConfirmacaoDocumentosFiscais", Column = "CAR_DATA_SOLICITACAO_CONFIRMACAO_DOCUMENTOS_FISCAIS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoConfirmacaoDocumentosFiscais { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioCalculoFrete", Column = "CAR_DATA_INICIO_CALCULO_FRETE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioEmissaoDocumentos", Column = "CAR_DATA_INICIO_EMISSAO_DOCUMENTOS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioEmissaoDocumentos { get; set; }

        //flag para identificar que o agrupamento foi gerado após a emissao dos documentos
        [NHibernate.Mapping.Attributes.Property(0, Name = "AgrupadaPosEmissaoDocumento", Column = "CAR_AGRUPADA_POS_EMISSAO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgrupadaPosEmissaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "CAR_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTransportador { get; set; }

        /// <summary>
        /// Usado no método BuscarCargasAgrupadasAguardandoIntegracao do WS. Só é válido quando é uma carga agrupada. Só fica false quando o cliente confirma que já visualizou
        /// essa carga agrupada (através do método ConfirmarIntegracaoCargasAgrupadas), já que antes dele visualizar ele não tem como saber qual
        /// é o novo id da carga (ele tem apenas os ids originais das cargas antes de serem agrupadas). Olhar referências e métodos do WS para entender melhor.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AG_INTEGRACAO_AGRUPAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgIntegracaoAgrupamentoCarga { get; set; }

        //Encerramento carga
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEncerramentoCarga", Column = "CAR_DATA_ENCERRAMENTO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSugestaoEntrega", Column = "CAR_DATA_SUGESTAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSugestaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracaoSugestaoEntrega", Column = "CAR_DATA_ALTERACAO_SUGESTAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracaoSugestaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoParaFaturamento", Column = "CAR_OBSERVACAO_PARA_FATURAMENTO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoParaFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPossuiPreCalculoFrete", Column = "CAR_POSSUI_PRE_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPossuiPreCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSalvamentoDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_PRIMEIRO_SALVAMENTO_DADOS_TRANSPORTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrimeiroSalvamentoDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_CONFIRMACAO_VALOR_FRETE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacaoDocumentosFiscais", Column = "CAR_DATA_CONFIRMACAO_DOCUMENTOS_FISCAIS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoDocumentosFiscais { get; set; }

        /// <summary>
        /// Após emitir todos os CT-es/MDF-es esta flag irá deixar a carga pendente para envio das documentações em lote
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AGUARDANDO_ENVIO_DOCUMENTACAO_LOTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AguardandoEnvioDocumentacaoLote { get; set; }

        /// <summary>
        /// Indica se é uma carga internacional (mercosul), isto define o fluxo (etapas) da carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_MERCOSUL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Mercosul { get; set; }

        /// <summary>
        /// Indica se é uma carga internacional (igual ao mercosul, mas diferente), isto define o fluxo (etapas) da carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_INTERNACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Internacional { get; set; }

        /// <summary>
        /// Data criada originalmente para a Aurora na tarefa #34632. Representa a data de abate dos suínos. Pode ser usada para outras coisas no futuro.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSeparacaoMercadoria", Column = "CAR_DATA_SEPARACAO_MERCADORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSeparacaoMercadoria { get; set; }

        /// <summary>
        /// Operador que realizou a inserção da carga. Criado originalmente para não ser alterado, mantendo o registro do usuário que iseriu a carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_OPERADOR_INSERCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorInsercao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_AGUARDAR_INTEGRACAO_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarIntegracaoDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_PROBLEMA_INTEGRACAO_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaIntegracaoDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_LIBERADA_COM_PROBLEMA_INTEGRACAO_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaComProblemaIntegracaoDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComparecido", Column = "CAR_NAO_COMPARECIDO", TypeType = typeof(TipoNaoComparecimento), NotNull = false)]
        public virtual TipoNaoComparecimento NaoComparecido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLimiteConfirmacaoMotorista", Column = "CAR_DATA_LIMITE_CONFIRMACAO_MOTORISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimiteConfirmacaoMotorista { get; set; }

        /// <summary>
        /// Flag indica que a Carga foi feita pelo método GerarCarregamento no WS da Carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaGeradaPeloMetodoGerarCarregamento", Column = "CAR_CARGA_GERADA_PELO_METODO_GERAR_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaGeradaPeloMetodoGerarCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaFixadaControleCargas", Column = "CAR_CARGA_FIXADA_CONTROLE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaFixadaControleCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCheckout", Column = "CAR_DATA_CHECKOUT", TypeType = typeof(DateTime), Lazy = true, NotNull = false)]
        public virtual DateTime? DataCheckout { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCheckin", Column = "CAR_TIPO_CHECKIN", TypeType = typeof(TipoCheckin), Lazy = true, NotNull = false)]
        public virtual TipoCheckin? TipoCheckin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReagendamento", Column = "CAR_DATA_REAGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaBloqueadaParaEdicaoIntegracao", Column = "CAR_CARGA_BLOQUEADA_EDICAO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaBloqueadaParaEdicaoIntegracao { get; set; }

        /// <summary>
        /// Indica quando o Tipo de Operação tem horário pra avisar o responsável da Filial que a carga ainda não teve os dados de transporte e já foi realizado o envio do email, este DATETIME vai guardar a hora que fez o envio do email.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioEmailTipoOperacaoAvisoResponsavelFilial", Column = "CAR_DATA_ENVIO_EMAIL_TIPO_OPERACAO_AVISO_RESPONSAVEL_FILIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioEmailTipoOperacaoAvisoResponsavelFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_RETORNO_CD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoCD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_PROTOCOLO_CARGA_INTEGRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int CargaProtocoloIntegrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaRecebidaDeIntegracao", Column = "CAR_CARGA_RECEBIDA_DE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaRecebidaDeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EncerramentoManualViagem", Column = "EMV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem EncerramentoManualViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoEncerramentoManualViagem", Column = "CAR_OBSERVACAO_ENCERRAMENTO_MANUAL_VIAGEM", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string ObservacaoEncerramentoManualViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPortoPortoPendenciaDocumento", Column = "CAR_CARGA_PORTO_PORTO_PENDENCIA_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPortoPortoPendenciaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCargaPortoPortoPendenciaDocumento", Column = "CAR_MOTIVO_CARGA_PORTO_PORTO_PENDENCIA_DOCUMENTO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string MotivoCargaPortoPortoPendenciaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosCTesComMercante", Column = "CAR_TODOS_CTES_COM_MERCANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TodosCTesComMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosCTesComManifesto", Column = "CAR_TODOS_CTES_COM_MANIFESTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TodosCTesComManifesto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosCTesFaturados", Column = "CAR_TODOS_CTES_FATURADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TodosCTesFaturados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeAquaviarioGeradoMasNaoVinculado", Column = "CAR_MDFE_AQUAVIARIO_GERADO_MAS_NAO_VINCULADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? MDFeAquaviarioGeradoMasNaoVinculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeAquaviarioVinculado", Column = "CAR_MDFE_AQUAVIARIO_VINCULADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MDFeAquaviarioVinculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosCTesFaturadosIntegrados", Column = "CAR_TODOS_CTES_FATURADOS_INTEGRADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TodosCTesFaturadosIntegrados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExternalID1", Column = "CAR_EXTERNALDT_1", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ExternalID1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExternalID2", Column = "CAR_EXTERNALDT_2", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ExternalID2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDIdentificacaoTrizzy", Column = "CAR_ID_IDENTIFICACAO_TRIZZY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IDIdentificacaoTrizzy { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDocumentoTransporte", Column = "TDT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte TipoDocumentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CteGlobalizado", Column = "CAR_CTE_GLOBALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CteGlobalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaCargaEmbarcador", Column = "CAR_CATEGORIA_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CategoriaCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SetPointVeiculo", Column = "CAR_SET_POINT_VEICULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SetPointVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFacturaFake", Column = "CAR_POSSUI_FACTURA_FAKE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFacturaFake { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardandoIntegracaoFrete", Column = "CAR_AGUARDANDO_INTEGRACAO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoIntegracaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenciaIntegracaoFrete", Column = "CAR_PENDENCIA_INTEGRACAO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenciaIntegracaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaComPendenciaIntegracaoFrete", Column = "CAR_LIBERADA_COM_PENDENCIA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaComPendenciaIntegracaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleIntegracaoEmbarcador", Column = "CAR_CONTROLE_INTEGRACAO_EMBARCADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ControleIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StagesGeradas", Column = "CAR_STAGES_GERADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool StagesGeradas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEixosCheckin", Column = "CAR_NUMERO_EIXO_CHECKIN", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEixosCheckin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessamentoEspecial", Column = "CAR_PROCESSAMENTO_ESPECIAL", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ProcessamentoEspecial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCarregamento", Column = "TCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoCarregamento TipoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSalvouDadosCarga", Column = "CAR_DATA_SALVOU_DADOS_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSalvouDadosCarga { get; set; }

        //Processo Arcelor calculo previsoes
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamentoCarga", Column = "CAR_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamentoCarga { get; set; }

        /// <summary>
        ///Usado apenas no método InformarDadosTransporteCarga do WS de Carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarregamentoCarga", Column = "CAR_DATA_DESCARREGAMENTO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRealFaturamento", Column = "CAR_DATA_REAL_FATURAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRealFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLoger", Column = "CAR_DATA_LOGER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLoger { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusLoger", Column = "CAR_STATUS_LOGER", TypeType = typeof(string), Length = 450, NotNull = false)]
        public virtual string StatusLoger { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaCargaRelacionada", Column = "CAR_JUSTIFICATIVA_CARGA_RELACIONADA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string JustificativaCargaRelacionada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEnvioEmailDocumentacaoCarga", Column = "CAR_SITUACAO_ENVIO_EMAIL_DOCUMENTACAO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmailDocumentacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmailDocumentacaoCarga? SituacaoEnvioEmailDocumentacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TAGPedagio", Column = "CAR_TAG_PEDAGIO", TypeType = typeof(TAGPedagio), NotNull = false)]
        public virtual TAGPedagio? TAGPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiComponenteFreteComImpostoIncluso", Column = "CAR_POSSUI_COMPONENTE_FRETE_COM_IMPOSTO_INCLUSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiComponenteFreteComImpostoIncluso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasInclusaoICMS", Column = "RFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS RegraInclusaoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTrecho", Column = "TTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ComprovanteCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COMPROVANTE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ComprovanteCarga", Column = "COC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga> ComprovanteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardandoSalvarDadosTransporteCarga", Column = "CAR_AGUARDANDO_SALVAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoSalvarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JaEnviouAlertaMotoristaViagem", Column = "CAR_JA_ENVIOU_ALERTA_MOTORISTA_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JaEnviouAlertaMotoristaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAgendamento", Column = "CAR_NUMERO_AGENDAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPreViagemInicio", Column = "CAR_DATA_PRE_VIAGEM_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPreViagemInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPreViagemFim", Column = "CAR_DATA_PRE_VIAGEM_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPreViagemFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenciaDocumentoTransportador", Column = "CAR_PENDENCIA_DOCUMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenciaDocumentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaIntegracao", Column = "CAR_FORMA_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao? FormaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnotacoesCard", Column = "CAR_ANOTACOES_CARD", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string AnotacoesCard { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarValorLimiteApolice", Column = "CAR_NAO_VALIDAR_VALOR_LIMITE_APOLICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarValorLimiteApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenciaValorLimiteApolice", Column = "CAR_PENDENCIA_VALOR_LIMITE_APOLICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenciaValorLimiteApolice { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_USUARIO_CONFIRMOU_ENVIO_DOCUMENTOS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioConfirmouEnvioDocumentos { get; set; }

        /// <summary>
        /// Usado para liberar a emissão da carga para o Transportador (tem o mesmo efeito da flag "Esse transportador emite os documentos fora do ME?", mas apenas em uma carga)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ContingenciaEmissao", Column = "CAR_CONTINGENCIA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ContingenciaEmissao { get; set; }

        /// <summary>
        ///É recorrente se for criada via planilha. Se for criada via planilha e for adicionado um pedido de forma manual, não é mais recorrente. #73516
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaRecorrente", Column = "CAR_ROTA_RECORRENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RotaRecorrente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacaoCtes", Column = "CAR_DATA_CONFIRMACAO_CTES", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoCtes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoJustificativaAprovacaoCarga", Column = "CAR_OBSERVACAO_JUSTIFICATIVA_APROVACAO_CARGA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoJustificativaAprovacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaAutorizacaoCarga", Column = "JAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga JustificativaAutorizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaOS", Column = "CAR_CATEGORIA_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CategoriaOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CategoriaOS? CategoriaOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NecessariaAverbacao", Column = "CAR_NECESSARIA_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessariaAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoProvedor", Column = "CAR_DOCUMENTO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DocumentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DocumentoProvedor? DocumentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalProvedor", Column = "CAR_VALOR_TOTAL_PROVEDOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarPagamento", Column = "CAR_LIBERAR_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOS", Column = "CAR_TIPO_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOS? TipoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOSConvertido", Column = "CAR_TIPO_OS_CONVERTIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido? TipoOSConvertido { get; set; }

        [Obsolete("Campo criado errado, migrado para TipoDirecionamentoCustoExtra")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DirecionamentoOS", Column = "CAR_DIRECIONAMENTO_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DirecionamentoOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DirecionamentoOS? DirecionamentoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoXML", Column = "CAR_TIPO_SERVICO_XML", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoXML), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoXML? TipoServicoXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicLiberacaoOk", Column = "CAR_INDIC_LIBERACAO_OK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicLiberacaoOk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RolagemCarga", Column = "CAR_ROLAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RolagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_MENSAGEM_RETORNO_ETAPA_DOCUMENTO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string MensagemRetornoEtapaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_AVANCO_ETAPA_DOCUMENTO_LOTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAvancoEtapaDocumentoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvancouCargaEtapaDocumentoLote", Column = "CAR_AVANCOU_CARGA_ETAPA_DOCUMENTO_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancouCargaEtapaDocumentoLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_DOC_OS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDocOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DirecionamentoCustoExtra", Column = "CAR_DIRECIONAMENTO_CUSTO_EXTRA", TypeType = typeof(TipoDirecionamentoCustoExtra), NotNull = false)]
        public virtual TipoDirecionamentoCustoExtra? DirecionamentoCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConfirmouConferenciaManualDeFrete", Column = "CAR_CONFIRMOU_CONFERENCIA_MANUAL_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmouConferenciaManualDeFrete { get; set; }

        /// <summary>
        /// Flag indica que a Carga teve seu controle de entregas gerado posteriormente pelo controle de qualidade
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregasGeradasPeloControleQualidade", Column = "CAR_CONTROLE_ENTREGA_GERADO_CONTROLE_QUALIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregasGeradasPeloControleQualidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativaGeracaoEntregasControleQualidade", Column = "CAR_NUMERO_TENTATIVAS_GERAR_ENTREGAS_CONTROLE_QUALIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativaGeracaoEntregasControleQualidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCarregamentoRoteirizacao", Column = "CAR_OBSERVACAO_CARREGAMENTO_ROTEIRIZACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoCarregamentoRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizandoOperacaoContainer", Column = "CAR_REALIZANDO_OPERACAO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizandoOperacaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiOperacaoContainer", Column = "CAR_POSSUI_OPERACAO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiOperacaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouAdiantamentoTerceiroContainer", Column = "CAR_GEROU_ADIANTAMENTO_TERCEIRO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouAdiantamentoTerceiroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarValorFreteOperador", Column = "CAR_OBRIGATORIO_INFORMAR_VALOR_FRETE_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarValorFreteOperador { get; set; }

        #endregion Propriedades

        #region Integração Leilão OTM

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagemIntegracaoLeilao", Column = "CAR_NUMERO_VIAGEM_INTEGRACAO_LEILAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroViagemIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DivisoriaIntegracaoLeilao", Column = "CAR_DIVISORIA_INTEGRACAO_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DivisoriaIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPerigosaIntegracaoLeilao", Column = "CAR_CARGA_PERIGOSA_INTEGRACAO_LEILAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPerigosaIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoPlanejadoIntegracaoLeilao", Column = "CAR_CUSTO_PLANEJADO_INTEGRACAO_LEILAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CustoPlanejadoIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoAtualIntegracaoLeilao", Column = "CAR_CUSTO_ATUAL_INTEGRACAO_LEILAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CustoAtualIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RazaoIntegracaoLeilao", Column = "CAR_RAZAO_INTEGRACAO_LEILAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RazaoIntegracaoLeilao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoIntegracaoLeilao", Column = "CAR_OBSERVACAO_INTEGRACAO_LEILAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoIntegracaoLeilao { get; set; }

        #endregion Integração Leilão OTM

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemSituacao", Column = "CAR_ORIGEM_SITUACAO", TypeType = typeof(OrigemSituacaoEntrega), NotNull = false)]
        public virtual OrigemSituacaoEntrega? OrigemSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemSituacaoFimViagem", Column = "CAR_ORIGEM_SITUACAO_FIM_VIAGEM", TypeType = typeof(OrigemSituacaoEntrega), NotNull = false)]
        public virtual OrigemSituacaoEntrega? OrigemSituacaoFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOT", Column = "CAR_NUMERO_OT", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string NumeroOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_POSICAO_FILA_PROCESSAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoFilaProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAE", Column = "CAR_SITUACAO_AE", TypeType = typeof(SituacaoAE), NotNull = false)]
        public virtual SituacaoAE? SituacaoAE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoSituacaoAE", Column = "CAR_DESCRICAO_SITUACAO_AE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoSituacaoAE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoSituacaoAE", Column = "CAR_MOTIVO_SITUACAO_AE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MotivoSituacaoAE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_DATA_MUDOU_SITUACAO_PARA_EM_TRANSPORTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMudouSituacaoParaEmTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_BALSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Balsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ViagemIniciadaViaFinalizacaoMonitoramento", Column = "CAR_VIAGEM_INICIADA_VIA_FINALIZACAO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ViagemIniciadaViaFinalizacaoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ViagemFinalizadaViaFinalizacaoMonitoramento", Column = "CAR_VIAGEM_FINALIZADA_VIA_FINALIZACAO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ViagemFinalizadaViaFinalizacaoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadoSemCargaOrganizacao", Column = "CAR_LIBERADO_SEM_CARGA_ORGANIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadoSemCargaOrganizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCTesAnterioresComoCTeFilialEmissora", Column = "CAR_UTILIZAR_CTES_ANTERIORES_COMO_CTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCTesAnterioresComoCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaComQuebraDeContainer", Column = "CAR_CARGA_COM_QUEBRA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaComQuebraDeContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessandoImportacaoPacote", Column = "CAR_PROCESSANDO_IMPORTACAO_PACOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProcessandoImportacaoPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusCustoExtra", Column = "CAR_STATUS_CUSTO_EXTRA", TypeType = typeof(StatusCustoExtra), NotNull = false)]
        public virtual StatusCustoExtra? StatusCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaComCargaSemPlanejamento", Column = "CAR_LIBERADA_COM_CARGA_SEM_PLANEJAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaComCargaSemPlanejamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_USUARIO_AUTORIZOU_ALTERACAO_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAutorizouAlteracaoFrete { get; set; }

        [Obsolete("Criado na entidade errada")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPesagemCarga", Column = "CAR_SITUACAO_PESAGEM_CARGA", TypeType = typeof(SituacaoPesagemCarga), NotNull = false)]
        public virtual SituacaoPesagemCarga? SituacaoPesagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoInsuficienteContratoFreteCliente", Column = "CAR_SALDO_INSUFICIENTE_CONTRATO_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SaldoInsuficienteContratoFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorDeRota", Column = "CAR_IDENTIFICADOR_DE_ROTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdentificadorDeRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaCritica", Column = "CAR_CARGA_CRITICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CargaCritica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_SUBCONTRATADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaSubcontrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteGerarCargaEspelho", Column = "CAR_PENDENTE_GERAR_CARGA_ESPELHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteGerarCargaEspelho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Parqueada", Column = "CAR_PARQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Parqueada { get; set; }                

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTransferencia", Column = "CAR_NUMERO_TRANSFERENCIA", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string NumeroTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Alocacao", Column = "CAR_ALOCACAO", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string Alocacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "CAR_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "CAR_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "CAR_VALOR_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadualFilialEmissora", Column = "CAR_VALOR_IBS_ESTADUAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadualFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipalFilialEmissora", Column = "CAR_VALOR_IBS_MUNICIPAL_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipalFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBSFilialEmissora", Column = "CAR_VALOR_CBS_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBSFilialEmissora { get; set; }

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual int? DiferencaInicioViagem
        {
            get
            {
                return DiffTimeMinutes(this.DataInicioViagemPrevista, this.DataInicioViagem);
            }
        }

        public virtual int? DiferencaFimViagem
        {
            get { return DiffTimeMinutes(this.DataFimViagemPrevista, this.DataFimViagem); }
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.Carga)this.MemberwiseClone();
        }

        public virtual decimal ValorTotalAReceberComICMSeISSFilialEmissora
        {
            get
            {
                if (this.Pedidos != null)
                    return this.Pedidos.Sum(obj => obj.ValorTotalAReceberComICMSeISSFilialEmissora);
                else
                    return 0m;
            }
        }

        public virtual decimal ValorTotalAReceberComICMSeISS
        {
            get
            {
                if (this.Pedidos != null)
                    return this.Pedidos.Sum(obj => obj.ValorTotalAReceberComICMSeISS);
                else
                    return 0m;
            }
        }

        public virtual decimal ValorFreteContratoFreteTotal
        {
            get { return this.ValorFreteContratoFrete + this.ValorFreteContratoFreteExcedente; }
        }

        public virtual string RetornarCodigoCargaParaVisualizacao
        {
            get { return this.CodigoCargaEmbarcador; }
        }

        public virtual string RetornarPlacasComModelo
        {
            get
            {
                List<string> placas = new List<string>();

                if (Veiculo != null)
                    placas.Add(Veiculo.Placa);

                if (VeiculosVinculados?.Count > 0)
                    placas.AddRange(from veiculo in VeiculosVinculados select $"{veiculo.Placa}{(veiculo.ModeloVeicularCarga != null ? " (" + veiculo.ModeloVeicularCarga.Descricao + " )" : "")}{(veiculo.ModeloCarroceria != null ? " - " + veiculo.ModeloCarroceria.Descricao : "")}");

                return string.Join(", ", placas);
            }
        }

        public virtual Dominio.Entidades.Empresa ObterEmpresaEmissora
        {
            get
            {
                if (this.EmpresaFilialEmissora != null)
                    return this.EmpresaFilialEmissora;
                else
                    return this.Empresa;
            }
        }

        public virtual string Descricao
        {
            get { return CodigoCargaEmbarcador; }
        }

        public virtual string DescricaoSituacaoCarga
        {
            get { return this.SituacaoCarga.ObterDescricao(); }
        }

        public virtual string DescricaoSituacaoMercanteCarga
        {
            get
            {
                if (this.SituacaoCarga == SituacaoCarga.Cancelada || this.SituacaoCarga == SituacaoCarga.Anulada)
                    return this.SituacaoCarga.ObterDescricao();
                else if (this.SituacaoCarga == SituacaoCarga.AgNFe && this.DataEnvioUltimaNFe.HasValue)
                    return "Aguardando Emissão";
                else if (this.SituacaoCarga == SituacaoCarga.AgNFe && !this.DataEnvioUltimaNFe.HasValue)
                    return "Pendente Emissão CT-e";
                else if (this.SituacaoCarga == SituacaoCarga.AgIntegracao)
                    return "Pendente Integração CT-e";
                else if (this.SituacaoCarga == SituacaoCarga.AgIntegracao || this.SituacaoCarga == SituacaoCarga.PendeciaDocumentos || this.problemaCTE)
                    return "Com Erro";
                else if (((bool?)this.MDFeAquaviarioVinculado ?? false) == false && this.SituacaoCarga != SituacaoCarga.Cancelada && this.SituacaoCarga != SituacaoCarga.Anulada &&
                            this.Pedidos.Any(ped => ped.TipoCobrancaMultimodal == TipoCobrancaMultimodal.CTEAquaviario))
                    return "Pendente MDF-e";
                else if (((bool?)this.TodosCTesComMercante ?? false) == false && this.SituacaoCarga != SituacaoCarga.Cancelada && this.SituacaoCarga != SituacaoCarga.Anulada &&
                            this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalProprio))
                    return "Pendente Mercante";
                else if (((bool?)this.TodosCTesFaturados ?? false) == false && this.SituacaoCarga != SituacaoCarga.Cancelada && this.SituacaoCarga != SituacaoCarga.Anulada &&
                            this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                                ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder))
                    return "Pendente Faturamento";
                else if (((bool?)this.TodosCTesFaturadosIntegrados ?? false) == false && this.SituacaoCarga != SituacaoCarga.Cancelada && this.SituacaoCarga != SituacaoCarga.Anulada &&
                            this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                                ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder))
                    return "Pendente Integração Fatura";
                else if (this.SituacaoCarga == SituacaoCarga.Encerrada &&
                            (this.TodosCTesComMercante || !this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)) &&
                            (this.TodosCTesComManifesto || !this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)) &&
                            (this.TodosCTesFaturados || !this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                                    ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder)) &&
                            (this.MDFeAquaviarioVinculado || !this.Pedidos.Any(ped => ped.TipoCobrancaMultimodal == TipoCobrancaMultimodal.CTEAquaviario)))
                    return "Finalizada";
                else if (this.SituacaoCarga != SituacaoCarga.Cancelada && this.SituacaoCarga != SituacaoCarga.Anulada &&
                            this.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta) &&
                            this.CargaCTes.Any(c => c.CTe.NumeroControleSVM == ""))
                    return "Pendente SVM";
                else
                    return "Não definido";

            }
        }


        public virtual bool ExigeConfirmacaoAntesEmissao
        {
            get
            {
                bool exige = this.Filial?.ExigeConfirmacaoFreteAntesEmissao ?? false;
                if (!exige)
                    exige = this.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? true;

                return exige;
            }
        }

        public virtual double TempoPrevistoEmHoras
        {
            get
            {
                if (this.DataInicioViagemPrevista == null || this.DataFimViagemPrevista == null) return 0;

                return (this.DataFimViagemPrevista - this.DataInicioViagemPrevista).Value.TotalHours;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Privados

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public virtual bool Equals(Carga other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos

        #region Métodos Sobrescritos

        public override bool IsCarga()
        {
            return true;
        }

        protected override string ObterDescricaoEntidade()
        {
            return CargaDePreCarga ? "pré carga" : "carga";
        }

        protected override string ObterNumero()
        {
            return CodigoCargaEmbarcador;
        }

        #endregion Métodos Sobrescritos
    }

}
