using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE", EntityName = "PreConhecimentoDeTransporteEletronico", DynamicUpdate = true, Name = "Dominio.Entidades.PreConhecimentoDeTransporteEletronico", NameType = typeof(PreConhecimentoDeTransporteEletronico))]
    public class PreConhecimentoDeTransporteEletronico : EntidadeBase, IEquatable<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalTransporte", Column = "MOA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModalTransporte ModalTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TRANSPORTADOR_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente TransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "PCO_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "PCO_DATAHORAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoPagamento", Column = "PCO_DATA_LIBERACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoLiberado", Column = "PCO_PAGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressao", Column = "PCO_IMPRESSAO", TypeType = typeof(Dominio.Enumeradores.TipoImpressao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaracteristicaTransporte", Column = "PCO_CARAC_TRANSP", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaracteristicaServico", Column = "PCO_CARAC_SERV", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CaracteristicaServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTE", Column = "PCO_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "PCO_VERSAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTEReferenciado", Column = "PCO_CHAVE_CTE_REFERENCIADO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCTEReferenciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "PCO_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retira", Column = "PCO_RETIRA", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Retira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalhesRetira", Column = "PCO_DETALHES_RETIRA", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string DetalhesRetira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "PCO_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "PCO_REMETENTE_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "PCO_END_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "PCO_EXPEDIDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "PCO_END_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "PCO_DESTINATARIO_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "PCO_END_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "PCO_RECEBEDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "PCO_END_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "PCO_TOMADOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe OutrosTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "PCO_END_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "PCO_CLIENTE_RETIRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteRetira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "PCO_CLIENTE_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoServico", Column = "PCO_VALOR_PREST_SERVICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceber", Column = "PCO_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PCO_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "PCO_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PCO_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "PCO_ALIQ_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PCO_VAL_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoICMS", Column = "PCO_PER_RED_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPresumido", Column = "PCO_VAL_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDevido", Column = "PCO_VAL_ICMS_DEVIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSDevido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SimplesNacional", Column = "PCO_SIMPLES_NAC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao SimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "PCO_VALOR_TOTAL_MERC", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominante", Column = "PCO_PRODUTO_PRED", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoDaCarga", Column = "PCO_OBS_CARGA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ObservacaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Container", Column = "PCO_CONTAINER", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaContainer", Column = "PCO_DATA_PREV_CONTAINER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LacreContainer", Column = "PCO_LACRE_CONTAINER", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string LacreContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTESubComp", Column = "PCO_CHAVE_CTE_SUB_COMP", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveCTESubComp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "PCO_RNTRC", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaEntrega", Column = "PCO_DATAPREVISTAENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lotacao", Column = "PCO_LOTACAO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Lotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOT", Column = "PCO_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieColeta", Column = "PCO_SERIECOLETA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroColeta", Column = "PCO_NUMCOLETA", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string NumeroColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataColeta", Column = "PCO_DATACOLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesGerais", Column = "PCO_OBSGERAIS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacoesGerais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesDigitacao", Column = "PCO_OBSDIGITACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacoesDigitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "PCO_LOCEMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "PCO_LOCINICIOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeInicioPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "PCO_LOCTERMINOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelado", Column = "PCO_CANCELADO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "PCO_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRecibo", Column = "PCO_NUMERO_RECIBO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "PCO_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "PCO_ALIQ_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BasePIS", Column = "PCO_BASE_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BasePIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "PCO_CST_PIS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "PCO_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCOFINS", Column = "PCO_BASE_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "PCO_ALIQ_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "PCO_CST_COFINS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSNoFrete", Column = "PCO_INCLUIR_ICMS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirICMSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualICMSIncluirNoFrete", Column = "PCO_PERCENTUAL_INCLUIR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualICMSIncluirNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCancelamento", Column = "PCO_OBS_CANCELAMENTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string ObservacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "PCO_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasCaracteristicasDaCarga", Column = "PCO_OUTRAS_CARAC_CARGA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string OutrasCaracteristicasDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ESCRITURACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ICMS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_PIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoPIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_COFINS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "PCO_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoAdicionalFisco", Column = "PCO_INFORMACAO_ADICIONAL_FISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string InformacaoAdicionalFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPedidoCliente", Column = "PCO_CODIGO_PEDIDO_CLIENTE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string CodigoPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCanalEntrega", Column = "PCO_CODIGO_CANAL_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalCarga", Column = "PCO_PESO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoresFormulaCalculoFreteCarga", Column = "PCO_VALORES_FORMULA_CALCULO_FRETE_CARGA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ValoresFormulaCalculoFreteCarga { get; set; }

        #region NFSe

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSNoFrete", Column = "PCO_INCLUIR_ISS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirISSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISSRetido", Column = "PCO_ISS_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualISSRetido", Column = "PCO_PERCENTUAL_ISS_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal PercentualISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISSRetido", Column = "PCO_VALOR_ISS_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "PCO_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "PCO_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "PCO_VALOR_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        #endregion

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "PCO_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "PCO_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "PCO_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "PCO_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "PCO_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "PCO_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "PCO_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "PCO_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "PCO_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "PCO_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "PCO_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "PCO_VALOR_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #endregion Imposto CBS/IBS

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRE_CTE_DOCS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentosPreCTE", Column = "PNF_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentosPreCTE> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosTransporteAnterior", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRE_CTE_SUBCONTRATADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoDeTransporteAnteriorPreCTE", Column = "PCS_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> DocumentosTransporteAnterior { get; set; }

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return "PRE CT-e";
            }
        }

        public virtual string DescricaoTipoServico
        {
            get
            {
                switch (TipoServico)
                {
                    case Enumeradores.TipoServico.Normal:
                        return "Normal";
                    case Enumeradores.TipoServico.Redespacho:
                        return "Redespacho";
                    case Enumeradores.TipoServico.RedIntermediario:
                        return "Red. Intermediário";
                    case Enumeradores.TipoServico.SubContratacao:
                        return "Subcontratação";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoCTE
        {
            get
            {
                switch (TipoCTE)
                {
                    case Enumeradores.TipoCTE.Anulacao:
                        return "Anulação";
                    case Enumeradores.TipoCTE.Complemento:
                        return "Complementar";
                    case Enumeradores.TipoCTE.Normal:
                        return "Normal";
                    case Enumeradores.TipoCTE.Substituto:
                        return "Substituição";
                    default:
                        return "";
                }
            }
        }

        public virtual ParticipanteCTe Tomador
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Enumeradores.TipoTomador.Destinatario:
                        return this.Destinatario;
                    case Enumeradores.TipoTomador.Expedidor:
                        return this.Expedidor;
                    case Enumeradores.TipoTomador.Outros:
                        return this.OutrosTomador;
                    case Enumeradores.TipoTomador.Recebedor:
                        return this.Recebedor;
                    case Enumeradores.TipoTomador.Remetente:
                        return this.Remetente;
                    default:
                        return null;
                }
            }
        }

        public virtual ParticipanteCTe ObterParticipante(Dominio.Enumeradores.TipoTomador tipoParticipante)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    return this.Destinatario;
                case Enumeradores.TipoTomador.Expedidor:
                    return this.Expedidor;
                case Enumeradores.TipoTomador.Outros:
                    return this.OutrosTomador;
                case Enumeradores.TipoTomador.Recebedor:
                    return this.Recebedor;
                case Enumeradores.TipoTomador.Remetente:
                    return this.Remetente;
                default:
                    return null;
            }
        }

        public virtual void SetarParticipante(Dominio.Entidades.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.ObjetosDeValor.CTe.Cliente participante, Dominio.Entidades.Localidade localidade, Dominio.ObjetosDeValor.Endereco endereco = null)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, endereco, participante, localidade);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, endereco, participante, localidade);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, endereco, participante, localidade);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, endereco, participante, localidade);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, endereco, participante, localidade);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, pais);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, pais);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.CTe.Cliente cliente, Dominio.Entidades.Cliente clienteExportacao, Enumeradores.TipoTomador tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, clienteExportacao, pais);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, clienteExportacao, pais);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, clienteExportacao, pais);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, clienteExportacao, pais);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, clienteExportacao, pais);
                    break;
                default:
                    break;
            }
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, Cliente cliente, Dominio.ObjetosDeValor.Endereco endereco, Dominio.ObjetosDeValor.CTe.Cliente participanteCTe, Dominio.Entidades.Localidade localidade)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = cliente.Atividade;
                participante.Cidade = null;

                participante.CPF_CNPJ = cliente.CPF_CNPJ_SemFormato;
                participante.Cliente = cliente;
                participante.Email = cliente.Email;
                participante.EmailContador = cliente.EmailContador;
                participante.EmailContadorStatus = cliente.EmailContadorStatus == "A" ? true : false;
                participante.EmailContato = cliente.EmailContato;
                participante.EmailContatoStatus = cliente.EmailContatoStatus == "A" ? true : false;
                participante.EmailStatus = cliente.EmailStatus == "A" ? true : false;
                participante.Exterior = false;
                participante.IE_RG = cliente.IE_RG;
                participante.InscricaoMunicipal = cliente.InscricaoMunicipal;
                participante.InscricaoSuframa = cliente.InscricaoSuframa;
                participante.Nome = cliente.Nome;
                participante.NomeFantasia = cliente.NomeFantasia;
                participante.Pais = null;
                participante.Telefone2 = cliente.Telefone2;
                participante.Tipo = cliente.Tipo == "J" ? Enumeradores.TipoPessoa.Juridica : Enumeradores.TipoPessoa.Fisica;

                if (endereco == null)
                {
                    if (participanteCTe == null)
                    {
                        participante.Bairro = cliente.Bairro;
                        participante.CEP = cliente.CEP;
                        participante.Complemento = cliente.Complemento;
                        participante.Endereco = cliente.Endereco.Left(80);
                        participante.Localidade = cliente.Localidade;
                        participante.Numero = cliente.Numero;
                        participante.Telefone1 = cliente.Telefone1;
                        participante.SalvarEndereco = true;
                    }
                    else
                    {
                        participante.Bairro = participanteCTe.Bairro;
                        participante.CEP = participanteCTe.CEP;
                        participante.Complemento = participanteCTe.Complemento;
                        participante.Endereco = participanteCTe.Endereco.Left(80);
                        participante.Localidade = localidade != null ? localidade : cliente.Localidade;
                        participante.Numero = participanteCTe.Numero;
                        participante.Telefone1 = participanteCTe.Telefone1;
                        participante.SalvarEndereco = false;
                    }
                }
                else
                {
                    participante.Bairro = endereco.Bairro;
                    participante.CEP = endereco.CEP;
                    participante.Complemento = endereco.Complemento;
                    participante.Endereco = endereco.Logradouro.Left(80);
                    participante.Localidade = endereco.Cidade;
                    participante.Numero = endereco.Numero;
                    participante.Telefone1 = endereco.Telefone;
                    participante.SalvarEndereco = false;
                }
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, ObjetosDeValor.Cliente cliente, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, ObjetosDeValor.CTe.Cliente cliente, Dominio.Entidades.Cliente clienteExportacao, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Cliente = clienteExportacao;
                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        /// <summary>
        /// Indicador CIF ou FOB, utilizado para o EDI
        /// </summary>
        public virtual string CondicaoPagamento
        {
            get
            {
                return this.TipoPagamento == Enumeradores.TipoPagamento.Pago ? "C" : "F";
            }
        }

        /// <summary>
        /// Indicador de Substituição tributária, utilizado para o EDI
        /// </summary>
        public virtual string SubstituicaoTributaria
        {
            get
            {
                return this.CST == "60" ? "1" : "2";
            }
        }

        public virtual void SetarRegraOutraAliquota(int codigoOutraAliquota)
        {
            OutrasAliquotas = codigoOutraAliquota > 0 ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas() { Codigo = codigoOutraAliquota } : null;
        }

        public virtual bool Equals(PreConhecimentoDeTransporteEletronico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
