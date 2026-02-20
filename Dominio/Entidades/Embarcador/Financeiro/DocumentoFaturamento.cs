using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_FATURAMENTO", EntityName = "DocumentoFaturamento", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento", NameType = typeof(DocumentoFaturamento))]
    public class DocumentoFaturamento : EntidadeBase
    {
        public DocumentoFaturamento() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATAHORAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_LIBERACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_ANULACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "DFA_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor SistemaEmissor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_VINCULO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVinculoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.EmpresaSerie EmpresaSerie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_CST", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupado", Column = "CCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado CargaCTeAgrupado { get; set; }

        /// <summary>
        /// Armazena qual a carga que gerou o documento,para compartivo de provisão (apenas embarcador)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_PAGAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaPagamento { get; set; }

        /// <summary>
        /// Armazena qual a carga que gerou o documento,para compartivo de provisão (apenas embarcador)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        /// <summary>
        /// Armazena qual a ocorrencia que gerou o documento,para compartivo de provisão (apenas embarcador)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO_PAGAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrenciaPagamento { get; set; }

        /// <summary>
        /// Armazena qual a ocorrencia que gerou o documento,para compartivo de provisão (apenas embarcador)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        /// <summary>
        /// Indica se o documento deve cair para liquidação em fatura ou em pagamento ao transportador.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_TIPO_LIQUIDACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao TipoLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO_LIBERACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento PagamentoLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoPagamento", Column = "CPG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento CancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MovimentoFinanceiroGerado", Column = "DFA_MOVIMENTO_FINANCEIRO_GERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MovimentoFinanceiroGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEnvioAutomatico", Column = "DFA_BLOQUEAR_ENVIO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEnvioAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoDocumentoBloqueado", Column = "DFA_PAGAMENTO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoDocumentoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarPagamentoAutomaticamente", Column = "DFA_LIBERAR_PAGAMENTO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPagamentoAutomaticamente { get; set; }

        /// <summary>
        /// Quando possui filial emissora, porém a provisão do documento é por nota fiscal.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ProvisaoPorNotaFiscal", Column = "DFA_PROVISAO_POR_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisaoPorNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_LIQUIDO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_ISS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_ALIQUOTA_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_ALIQUOTA_ISS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_PERCENTUAL_RETENCAO_ISS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_DOCUMENTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_PAGO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_EM_FATURA", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorEmFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_A_FATURAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorAFaturar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_NUMERO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_NUMERO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_NUMERO_FECHAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_NUMERO_DOCUMENTO_ORIGINARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroDocumentoOriginario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_EMISSAO_DOCUMENTO_ORIGINARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoDocumentoOriginario { get; set; }

        /// <summary>
        /// Data em que o último canhoto foi recebido pela empresa.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_ENVIO_ULTIMO_CANHOTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioUltimoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_CANHOTOS_RECEBIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanhotosRecebidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_CANHOTOS_DIGITALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanhotosDigitalizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracaoEmbarcador", Column = "DFA_PENDENTE_INTEGRACAO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracaoEmbarcador { get; set; }

        /// <summary>
        /// Data em que o último canhoto do CT-e foi alterado de "Pendente" para qualquer outra situação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_CHEGADA_ULTIMO_CANHOTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaUltimoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardandoAutorizacao", Column = "DFA_AGUARDANDO_AUTORIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_MOEDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_VALOR_AVARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoPermissaoExclusiva", Column = "DFA_FATURAMENTO_PERMISSAO_EXCLUSIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermissaoExclusiva { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "DFA_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual ICollection<string> NumeroPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "DFA_NUMERO_PEDIDO_OCORRENCIA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual ICollection<string> NumeroPedidoOcorrenciaCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DFA_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTE", Column = "DFA_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControle", Column = "DFA_NUMERO_CONTROLE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFiscal", Column = "DFA_NUMERO_FISCAL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloqueioGeracaoAutomaticaPagamento", Column = "DFA_BLOQUEIO_GERACAO_AUTOMATICA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloqueioGeracaoAutomaticaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProvisaoGerada", Column = "DFA_PROVISAO_GERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisaoGerada { get; set; }

        //DADOS DA MIRO
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFolha", Column = "DFA_NUMERO_FOLHA", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroFolha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMiro", Column = "DFA_NUMERO_MIRO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroMiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEstorno", Column = "DFA_NUMERO_ESTORNO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroEstorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueio", Column = "DFA_BLOQUEIO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string Bloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMiro", Column = "DFA_DATA_MIRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencimento", Column = "DFA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermosPagamento", Column = "TPG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TermosPagamento TermosPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoBloqueio", Column = "DFA_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe:
                        return ModeloDocumentoFiscal?.Abreviacao ?? "CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga:
                        return "Carga";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoNumeroDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe:
                        return CTe.Numero.ToString() + "-" + CTe.Serie.Numero.ToString() + " (" + (ModeloDocumentoFiscal?.Abreviacao ?? "CT-e") + ")";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga:
                        return Carga.CodigoCargaEmbarcador + " (Carga)";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento Clonar()
        {
            return (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento)this.MemberwiseClone();
        }

        #endregion
    }
}
