using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE", EntityName = "ConhecimentoDeTransporteEletronico", DynamicUpdate = true, Name = "Dominio.Entidades.ConhecimentoDeTransporteEletronico", NameType = typeof(ConhecimentoDeTransporteEletronico))]
    public class ConhecimentoDeTransporteEletronico : EntidadeBase, IEquatable<Dominio.Entidades.ConhecimentoDeTransporteEletronico>
    {
        public ConhecimentoDeTransporteEletronico() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalTransporte", Column = "MOA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModalTransporte ModalTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CON_NUM", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_TIPO_CONTROLE", TypeType = typeof(long), NotNull = false)]
        public virtual long TipoControle { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "CON_SERIE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CON_CHAVECTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CON_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "CON_MODELODOC", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CON_DATAHORAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "CON_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DATA_ANULACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressao", Column = "CON_IMPRESSAO", TypeType = typeof(Dominio.Enumeradores.TipoImpressao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        /// <summary>
        /// 1 - Normal
        /// 4 - EPEC pela SVC
        /// 5 - Contingência FSDA
        /// 7 - SVC-RS
        /// 8 - SVC-SP
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissao", Column = "CON_TPEMIS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaracteristicaTransporteCTe", Column = "CON_CARAC_TRANSP", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaTransporteCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaracteristicaServicoCTe", Column = "CON_CARAC_SERV", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CaracteristicaServicoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "CON_TIPO_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTE", Column = "CON_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "CON_VERSAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTEReferenciado", Column = "CON_CHAVE_CTE_REFERENCIADO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCTEReferenciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "CON_TIPO_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServico), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retira", Column = "CON_RETIRA", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Retira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalhesRetira", Column = "CON_DETALHES_RETIRA", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string DetalhesRetira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CON_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente_old { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_REMETENTE_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "CON_END_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoRemetente { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Expedidor_old { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_EXPEDIDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "CON_END_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoExpedidor { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario_old { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_DESTINATARIO_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "CON_END_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoDestinatario { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Recebedor_old { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_RECEBEDOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "CON_END_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoRecebedor { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_OUTROS_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente OutrosTomador_old { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_TOMADOR_CTE", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe OutrosTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoParticipanteCTe", Column = "CON_END_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual EnderecoParticipanteCTe EnderecoTomador { get; set; }

        /// <summary>
        /// Este deve ser o tomador real do CT-e.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_TOMADOR_PAGADOR_CTE", NotNull = false, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParticipanteCTe TomadorPagador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContingencia", Column = "CON_DATACONTINGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContingencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaContingencia", Column = "CON_JUST_CONTINGENCIA", TypeType = typeof(string), Length = 256, NotNull = false)]
        public virtual string JustificativaContingencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_CLIENTE_RETIRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteRetira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_CLIENTE_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoServico", Column = "CON_VALOR_PREST_SERVICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceber", Column = "CON_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CON_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "CON_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "CON_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "CON_ALIQ_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CON_VAL_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBaseCalculoICMS", Column = "CON_PER_RED_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPresumido", Column = "CON_VAL_PRESUMIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDevido", Column = "CON_VAL_ICMS_DEVIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSDevido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibeICMSNaDACTE", Column = "CON_EXIBE_ICMS_DACTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibeICMSNaDACTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnuladoGerencialmente", Column = "CON_ANULADO_GERENCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AnuladoGerencialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SimplesNacional", Column = "CON_SIMPLES_NAC", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao SimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSSimples", Column = "CON_ALIQ_ICMS_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSimples", Column = "CON_VAL_ICMS_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "CON_VALOR_TOTAL_MERC", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCommodities", Column = "CON_VALOR_COMMODITIES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCommodities { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteCTe", Column = "CON_PRODUTO_PRED", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ProdutoPredominanteCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteCEAN", Column = "CON_PRODUTO_PRED_CEAN", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ProdutoPredominanteCEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominanteNCM", Column = "CON_PRODUTO_PRED_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ProdutoPredominanteNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoDaCarga", Column = "CON_OBS_CARGA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ObservacaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Container", Column = "CON_CONTAINER", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaContainer", Column = "CON_DATA_PREV_CONTAINER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LacreContainer", Column = "CON_LACRE_CONTAINER", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string LacreContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotorista", Column = "CON_NOME_MOTORISTA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NomeMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFMotorista", Column = "CON_CPF_MOTORISTA", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPFMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTESubComp", Column = "CON_CHAVE_CTE_SUB_COMP", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveCTESubComp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "CON_RNTRC", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevistaEntrega", Column = "CON_DATAPREVISTAENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lotacao", Column = "CON_LOTACAO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Lotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOT", Column = "CON_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieColeta", Column = "CON_SERIECOLETA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroColeta", Column = "CON_NUMCOLETA", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string NumeroColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataColeta", Column = "CON_DATACOLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesGerais", Column = "CON_OBSGERAIS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacoesGerais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesAvancadas", Column = "CON_OBS_AVANCADAS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacoesAvancadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesDigitacao", Column = "CON_OBSDIGITACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacoesDigitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "CON_LOCEMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "CON_LOCINICIOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeInicioPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "CON_LOCTERMINOPRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelado", Column = "CON_CANCELADO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CON_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRecibo", Column = "CON_NUMERO_RECIBO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloCancelamentoInutilizacao", Column = "CON_PROTOCOLOCANINU", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloCancelamentoInutilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoSefaz", Column = "CON_RETORNOCTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MensagemRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetornoSefaz", Column = "CON_RETORNOCTEDATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCTeIntegrador", Column = "CON_CODCTE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoCTeIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusIntegrador", Column = "CON_STATUSINTEGRADOR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodStatusProtocolo", Column = "CON_CODSTATUS_PROTOCOLO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodStatusProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ErroSefaz", Column = "ERR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ErroSefaz MensagemStatus { get; set; }


        /// <summary>
        /// P - PENDENTE
        /// E - ENVIADO
        /// R - REJEICAO
        /// A - AUTORIZADO
        /// C - CANCELADO
        /// I - INUTILIZADO
        /// D - DENEGADO
        /// S - EM DIGITACAO
        /// K - EM CANCELAMENTO
        /// L - EM INUTILIZACAO
        /// Z - ANULADO (AUTORIZADO, MAS GERENCIALMENTE FICA CANCELADO)
        /// X - AGUARDANDO ASSINATURA (CERTIFICADO A3)
        /// V - AGUARDANDO ASSINATURA CANCELAMENTO (CERTIFICADO A3)
        /// B - AGUARDANDO ASSINATURA INUTILIZACAO (CERTIFICADO A3)
        /// M - AGUARDANDO EMISSAO E-MAIL
        /// F - EMITIDO EM CONTINGÊNCIA FSDA
        /// Q - EMITIDO EM CONTINGÊNCIA EPEC
        /// Y - AGUARDANDO FINALIZAR CARGA INTEGRACAO
        /// N - AGUARDANDO NFSE AUTORIZAR (RPS PROCESSADO)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CON_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "CON_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "CON_ALIQ_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BasePIS", Column = "CON_BASE_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BasePIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "CON_CST_PIS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "CON_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCOFINS", Column = "CON_BASE_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "CON_ALIQ_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "CON_CST_COFINS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSNoFrete", Column = "CON_INCLUIR_ICMS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirICMSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualICMSIncluirNoFrete", Column = "CON_PERCENTUAL_INCLUIR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualICMSIncluirNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCancelamento", Column = "CON_OBS_CANCELAMENTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string ObservacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "CON_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogIntegracao", Column = "CON_LOG_INTEGRACAO", Type = "StringClob", NotNull = false)]
        public virtual string LogIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasCaracteristicasDaCargaCTe", Column = "CON_OUTRAS_CARAC_CARGA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string OutrasCaracteristicasDaCargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportacaoPreCTe", Column = "CON_IMPORTACAO_PRE_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportacaoPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusImportacaoPreCTe", Column = "CON_STATUS_IMPORTACAO_PRE_CTE", TypeType = typeof(Enumeradores.StatusImportacaoPreCTe), NotNull = false)]
        public virtual Enumeradores.StatusImportacaoPreCTe StatusImportacaoPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoAdicionalFisco", Column = "CON_INFORMACAO_ADICIONAL_FISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string InformacaoAdicionalFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSInterna", Column = "CON_ALIQUOTA_ICMS_INTERNA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualICMSPartilha", Column = "CON_PERCENTUAL_ICMS_PARTILHA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualICMSPartilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSUFOrigem", Column = "CON_VALOR_ICMS_UF_ORIGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSUFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSUFDestino", Column = "CON_VALOR_ICMS_UF_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSUFDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSFCPFim", Column = "CON_VALOR_ICMS_FCP_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSFCPFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeSemCarga", Column = "CON_SEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTeSemCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseCalculoIR", Column = "CON_BASE_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "CON_ALIQ_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "CON_VALOR_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseCalculoINSS", Column = "CON_BASE_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseCalculoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaINSS", Column = "CON_ALIQ_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "CON_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseCalculoCSLL", Column = "CON_BASE_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseCalculoCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCSLL", Column = "CON_ALIQ_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCSLL", Column = "CON_VALOR_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCSLL { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        /// <summary>
        /// 0 - Normal
        /// 1 - Lote
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvio", Column = "CON_TIPO_ENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorGlobalizado", Column = "CON_IND_GLOBALIZADO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IndicadorGlobalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorIETomador", Column = "CON_IND_IE_TOMADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? IndicadorIETomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCarbaAverbacao", Column = "CON_VALOR_CARGA_AVERB", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorCarbaAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveContingencia", Column = "CON_CHAVE_CONTINGENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveContingencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativaReenvio", Column = "CON_TENTATIVA_REENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativaReenvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_TIPO_CTE_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TipoCTeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoEmbarcador", Column = "CON_CODIGO_PRODUTO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_FATOR_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PESO_FATURADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoFaturado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_VOLUMES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PALLETS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_MOEDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubstituicaoTomador", Column = "CON_SUBS_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SubstituicaoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoOperacaoContainer", Column = "CON_DOCUMENTO_OPERACAO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentoOperacaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoCampoIdentificador", Column = "CON_NOVO_CAMPO_IDENTIFICADOR", TypeType = typeof(string), NotNull = false)]
        public virtual string NovoCampoIdentificador { get; set; }

        #region NFSe

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPrefeituraNFSe", Column = "CON_NFSE_NUMERO_PREFEUTURA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroPrefeituraNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDeducoes", Column = "CON_VALOR_DEDUCOES", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDeducoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSNoFrete", Column = "CON_INCLUIR_ISS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirISSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISSRetido", Column = "CON_ISS_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualISSRetido", Column = "CON_PERCENTUAL_ISS_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal PercentualISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISSRetido", Column = "CON_VALOR_ISS_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasRetencoes", Column = "CON_VALOR_OUTRAS_RETENCOES", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorOutrasRetencoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoIncondicionado", Column = "CON_VALOR_DESC_INCONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorDescontoIncondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoCondicionado", Column = "CON_VALOR_DESC_CONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorDescontoCondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "CON_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "CON_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "CON_VALOR_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieRPS", Column = "CON_SERIE_RPS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string SerieRPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieSubstituicao", Column = "CON_SERIE_SUBSTITUICAO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSubstituicao", Column = "CON_NUMERO_SUBSTITUICAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaNFSe NaturezaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RPSNFSe", Column = "RPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual RPSNFSe RPS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteCTe", Column = "CON_INTERMEDIARIO_NFS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual ParticipanteCTe Intermediario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoComplemento", Column = "CON_DESCRICAO_COMPLEMENTO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string DescricaoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFretamento", Column = "CON_TIPO_FRETAMENTO", TypeType = typeof(Dominio.Enumeradores.TipoFretamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoFretamento? TipoFretamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraViagem", Column = "CON_DATA_HORA_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReutilizaNumeracao", Column = "CON_REUTILIZA_NUMERACAO", TypeType = typeof(Dominio.Enumeradores.ReutilizaNumeracao), NotNull = false)]
        public virtual Dominio.Enumeradores.ReutilizaNumeracao ReutilizaNumeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarAliquotaEValorISS", Column = "CON_NAO_ENVIAR_ALIQUOTA_E_VALOR_ISS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarAliquotaEValorISS { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTM", Formula = @"SUBSTRING((SELECT ', ' + CAST(ctm.CON_NUM AS NVARCHAR(20))
                                                                                    FROM T_CTE_SVM_MULTIMODAL svm
																					JOIN T_CTE ctm on ctm.CON_CODIGO = svm.CON_CODIGO_MULTIMODAL and ctm.CON_STATUS = 'A'
                                                                                    WHERE svm.CON_CODIGO_SVM = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNotasFiscais", Formula = @"ISNULL((SELECT COUNT(1)
                                                                                    FROM T_CTE_DOCS cteDocs
                                                                                    WHERE cteDocs.CON_CODIGO = CON_CODIGO),0)", TypeType = typeof(int), Lazy = true)]
        public virtual int QuantidadeNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMeioTransporte", Formula = @"ISNULL((SELECT TOP(1) T.CTI_DESCRICAO FROM T_CTE_CONTAINER MDFE
                                                                                        JOIN T_CONTAINER C ON C.CTR_CODIGO = MDFE.CTR_CODIGO
                                                                                        JOIN T_CONTAINER_TIPO T ON T.CTI_CODIGO = C.CTI_CODIGO
                                                                                        WHERE  MDFE.CON_CODIGO = CON_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string TipoMeioTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotas", Formula = @"SUBSTRING((SELECT ', ' + CAST(cteDocs.NFC_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_CTE_DOCS cteDocs
                                                                                    WHERE cteDocs.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentosOriginarios", Formula = @"SUBSTRING((SELECT ', ' + CAST(cteDocs.CDO_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_CTE_DOCUMENTO_ORIGINARIO cteDocs
                                                                                    WHERE cteDocs.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroDocumentosOriginarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotasComSerie", Formula = @"SUBSTRING((SELECT ', ' + CAST(cteDocs.NFC_NUMERO AS NVARCHAR(20)) + ' - ' + ISNULL(cteDocs.NFC_SERIE, SUBSTRING(cteDocs.NFC_CHAVENFE, 23, 3))
                                                                                    FROM T_CTE_DOCS cteDocs
                                                                                    WHERE cteDocs.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroNotasComSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosSolicitacoes", Formula = @"SUBSTRING((SELECT ', ' + X.NF_NUMERO_SOLICITACAO
                                                                                    FROM T_XML_NOTA_FISCAL X
                                                                                    JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                                                                    WHERE CX.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosSolicitacoes { get; set; }


        //[NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotasSeparadasPorBarra", Formula = @"SUBSTRING((SELECT '/' + CAST(cteDocs.NFC_NUMERO AS NVARCHAR(20))
        //                                                                            FROM T_CTE_DOCS cteDocs
        //                                                                            WHERE cteDocs.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 2, 1000)", TypeType = typeof(string), Lazy = true)]
        //public virtual string NumeroNotasSeparadasPorBarra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaObservacoesFisco", Formula = @"SUBSTRING((SELECT '/ ' + obsFisco.OBF_IDENTIFICADOR + ': ' + obsFisco.OBF_DESCRICAO
                                                                                    FROM T_OBS_FISCO obsFisco
                                                                                    WHERE obsFisco.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaObservacoesFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaObservacoesContribuinte", Formula = @"SUBSTRING((SELECT '/ ' + obsCont.OBC_IDENTIFICADOR + ': ' + obsCont.OBC_DESCRICAO
                                                                                    FROM T_OBS_CONTRIBUINTE obsCont
                                                                                    WHERE obsCont.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaObservacoesContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaMDFes", Formula = @"SUBSTRING((SELECT ' , ' + cast(mdfe.mdf_numero as nvarchar(20)) + '/' +  cast(SERIE.ESE_NUMERO as nvarchar(20))  FROM T_MDFE MDFE
                                                                                    JOIN T_EMPRESA_SERIE SERIE ON MDFE.ESE_CODIGO = SERIE.ESE_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                    WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5)  FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaMDFes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoMDFe", Formula = @"(SELECT TOP(1) MDFE.MDF_DATA_EMISSAO FROM T_MDFE MDFE
                                                                                    JOIN T_EMPRESA_SERIE SERIE ON MDFE.ESE_CODIGO = SERIE.ESE_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                    WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5))", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime? DataEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusMDFe", Formula = @"ISNULL((SELECT TOP(1) MDFE.MDF_STATUS FROM T_MDFE MDFE
                                                                                    JOIN T_EMPRESA_SERIE SERIE ON MDFE.ESE_CODIGO = SERIE.ESE_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                    JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                    WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5)), -1)", TypeType = typeof(StatusMDFe), Lazy = true)]
        public virtual StatusMDFe StatusMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaMotoristasMDFe", Formula = @"SUBSTRING((SELECT ' , ' + cast(MOTORISTA.MDM_CPF as nvarchar(11)) + ' ' +  cast(MOTORISTA.MDM_NOME as nvarchar(20))  FROM T_MDFE MDFE
                                                                                            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                            JOIN T_MDFE_MOTORISTA MOTORISTA ON MDFE.MDF_CODIGO = MOTORISTA.MDF_CODIGO
                                                                                            WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5)   FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaMotoristasMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaVeiculosMDFe", Formula = @"SUBSTRING((SELECT ' , ' + VEICULO.MDV_PLACA  FROM T_MDFE MDFE
                                                                                           JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                           JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                           JOIN T_MDFE_VEICULO VEICULO  ON MDFE.MDF_CODIGO = VEICULO.MDF_CODIGO
                                                                                           WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5) FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaVeiculosMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaReboquesMDFe", Formula = @"SUBSTRING((SELECT ' , ' + VEICULO.MDR_PLACA  FROM T_MDFE MDFE
                                                                                           JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MMD ON MDFE.MDF_CODIGO = MMD.MDF_CODIGO
                                                                                           JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MMDD ON MMDD.MDD_CODIGO = MMD.MDD_CODIGO
                                                                                           JOIN T_MDFE_REBOQUE VEICULO  ON MDFE.MDF_CODIGO = VEICULO.MDF_CODIGO
                                                                                           WHERE  MMDD.CON_CODIGO = CON_CODIGO and MDFE.MDF_STATUS IN (3,5) FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaReboquesMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaContainer", Formula = @"SUBSTRING((SELECT ' , ' + C.CTR_NUMERO FROM T_CTE_CONTAINER MDFE
                                                                                        JOIN T_CONTAINER C ON C.CTR_CODIGO = MDFE.CTR_CODIGO
                                                                                        WHERE  MDFE.CON_CODIGO = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTeDuplicado", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                                                                            FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                            JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                            WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTeDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCTeDuplicado", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                                                                            FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                            JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                            WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroControleCTeDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTeAnulacao", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCTeAnulacao", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroControleCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTeComplementar", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCTeComplementar", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroControleCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTeSubstituto", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTeSubstituto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCTeSubstituto", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                                                                        WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroControleCTeSubstituto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTeOriginal", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeOriginal.CON_NUM AS NVARCHAR(20))
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                                                                        WHERE CTeRelacao.CON_CODIGO_GERADO  = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroCTeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControleCTeOriginal", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CTeOriginal.CON_NUMERO_CONTROLE
                                                                                        FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                                                                        JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                                                                        WHERE CTeRelacao.CON_CODIGO_GERADO  = CON_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroControleCTeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarga", Formula = @"ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM_PASSAGEM_CINCO AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO),
                                                                                      ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM_PASSAGEM_QUATRO AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO),
                                                                                      ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM_PASSAGEM_TRES AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO),
                                                                                      ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM_PASSAGEM_DOIS AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO),
                                                                                      ISNULL((SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM_PASSAGEM_UM AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO),
                                                                                        (SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_DESTINO AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_DESTINO))))))", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime? DataDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegada", Formula = @"(SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_ORIGEM AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_ORIGEM)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime? DataChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPartida", Formula = @"(SELECT TOP(1) Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule WHERE Schedule.PVN_CODIGO = CON_VIAGEM AND Schedule.POT_CODIGO_ATRACACAO = POT_CODIGO_ORIGEM AND Schedule.TTI_CODIGO_ATRACACAO = CON_TERMINAL_ORIGEM)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime? DataPartida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMeioTransporteEDI", Formula = @"ISNULL((SELECT TOP(1) T.CTI_DESCRICAO FROM T_CTE_CONTAINER CC
                                                                                                JOIN T_CONTAINER C ON C.CTR_CODIGO = CC.CTR_CODIGO
                                                                                                JOIN T_CONTAINER_TIPO T ON T.CTI_CODIGO = C.CTI_CODIGO
                                                                                                WHERE  CC.CON_CODIGO = CON_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string TipoMeioTransporteEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoNotasContainer", Formula = @"ISNULL((SELECT TOP(1) SUM(DocumentosCTE.NFC_PESO) FROM T_CTE_DOCS DocumentosCTE
	                                                                                            WHERE DocumentosCTE.CON_CODIGO = CON_CODIGO), 0)", TypeType = typeof(string), Lazy = true)]
        public virtual string PesoNotasContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TaraContainer", Formula = @"ISNULL((SELECT TOP(1) Pedido.PED_TARA_CONTAINER FROM T_PEDIDO Pedido
	                                                                                        LEFT JOIN T_CARGA_PEDIDO CargaPedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
	                                                                                        LEFT JOIN T_CARGA_CTE CargaCTe ON CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
	                                                                                        WHERE CargaCTe.CON_CODIGO = CON_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string TaraContainer { get; set; }

        [Obsolete("Substituído pela entidade PedidoViagemNavioSchedule")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescargaSumarizada", Column = "CON_DATA_DESCARGA_SUMARIZADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescargaSumarizada { get; set; }

        [Obsolete("Substituído pela entidade PedidoViagemNavioSchedule")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoSaidaNavioSumarizada", Column = "CON_DATA_PREVISAO_SAIDA_NAVIO_SUMARIZADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaidaNavioSumarizada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavioSchedule", Column = "PVS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule PedidoViagemNavioSchedule { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "SomaComponenteSeguro", Formula = @"(SELECT SUM(cpt.CPT_VALOR) FROM T_CTE_COMP_PREST cpt WHERE cpt.CON_CODIGO = CON_CODIGO AND cpt.CPT_NOME = 'SEGURO')", TypeType = typeof(decimal), Lazy = true)]
        //public virtual decimal SomaComponenteSeguro { get; set; }

        /// <summary>
        /// Propriedade utilizada para informar se já gerou as DACTEs de outros modelos de documento (apenas se configurado no ambiente irá gerar automaticamente).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_GEROU_DACTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerouDACTE { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_DOCS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentosCTE", Column = "NFC_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentosCTE> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosTransporteAnterior", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_SUBCONTRATADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoDeTransporteAnteriorCTe", Column = "CSU_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> DocumentosTransporteAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoCTE", Column = "CVE_CODIGO")]
        public virtual IList<Dominio.Entidades.VeiculoCTE> Veiculos { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Motoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaCTE", Column = "CMO_CODIGO")]
        public virtual IList<Dominio.Entidades.MotoristaCTE> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Seguros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SeguroCTE", Column = "SEG_CODIGO")]
        public virtual IList<Dominio.Entidades.SeguroCTE> Seguros { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "QuantidadesCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_INF_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InformacaoCargaCTE", Column = "ICA_CODIGO")]
        public virtual IList<Dominio.Entidades.InformacaoCargaCTE> QuantidadesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ComponentesPrestacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_COMP_PREST")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ComponentePrestacaoCTE", Column = "CPT_CODIGO")]
        public virtual IList<Dominio.Entidades.ComponentePrestacaoCTE> ComponentesPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ObservacoesFisco", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OBS_FISCO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ObservacaoFiscoCTE", Column = "OBF_CODIGO")]
        public virtual IList<Dominio.Entidades.ObservacaoFiscoCTE> ObservacoesFisco { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ObservacoesContribuinte", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OBS_CONTRIBUINTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ObservacaoContribuinteCTE", Column = "OBC_CODIGO")]
        public virtual IList<Dominio.Entidades.ObservacaoContribuinteCTE> ObservacoesContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "XMLs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_XML")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLCTe", Column = "CTX_CODIGO")]
        public virtual IList<Dominio.Entidades.XMLCTe> XMLs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ValesPedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_VALE_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValePedagioCTe", Column = "VPC_CODIGO")]
        public virtual IList<Dominio.Entidades.ValePedagioCTe> ValesPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CON_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrarCancelamento", Column = "CON_COBRAR_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrarCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReferenciaEmissao", Column = "CON_REFERENCIA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReferenciaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeReferencia", Column = "CON_CODIGO_CTE_REFERENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int CTeReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorNegociavel", Column = "CON_INDICADOR_NEGOCIAVEL", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao? IndicadorNegociavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "CON_TIPO_MODAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }

        #region Modal Aéreo

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMinuta", Column = "CON_NUMERO_MINUTA", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NumeroMinuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOCA", Column = "CON_NUMERO_OCA", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string NumeroOCA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dimensao", Column = "CON_DIMENSAO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Dimensao { get; set; }

        /// <summary>
        /// M - Tarifa Mínima; G - Tarifa Geral; E - Tarifa Específica
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseTarifa", Column = "CON_CLASSE_TARIFA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string ClasseTarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTarifa", Column = "CON_CODIGO_TARIFA", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoTarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTarifa", Column = "CON_VALOR_TARIFA", TypeType = typeof(decimal), Scale = 6, Precision = 20, NotNull = false)]
        public virtual decimal? ValorTarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaGeracaoCargaAutomaticamente", Column = "CON_PROBLEMA_GERACAO_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProblemaGeracaoCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "InformacoesManuseio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_INFORMACAO_MANUSEIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeInformacaoManuseio", Column = "CIM_CODIGO")]
        public virtual IList<Embarcador.CTe.CTeInformacaoManuseio> InformacoesManuseio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ProdutosPerigosos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_PRODUTO_PERIG")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoPerigosoCTE", Column = "PRD_CODIGO")]
        public virtual IList<ProdutoPerigosoCTE> ProdutosPerigosos { get; set; }

        #endregion

        #region Modal Aquaviário

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoAFRMM", Column = "CON_VALOR_PRESTACAO_AFRMM", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorPrestacaoAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdicionalAFRMM", Column = "CON_VALOR_ADICIONAL_AFRMM", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorAdicionalAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_BALSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Balsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "CON_NUMERO_VIAGEM", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroViagem { get; set; }

        /// <summary>
        /// N-Norte, L-Leste, S-Sul, O-Oeste
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Direcao", Column = "CON_DIRECAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Direcao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Balsas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_BALSA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeBalsa", Column = "CBA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeBalsa> Balsas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Containers", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_CONTAINER")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContainerCTE", Column = "CER_CODIGO")]
        public virtual IList<ContainerCTE> Containers { get; set; }

        #endregion

        #region Modal Ferroviário

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTrafego", Column = "CON_TIPO_TRAFEGO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrafego), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrafego? TipoTrafego { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FerroviaResponsavelFaturamento", Column = "CON_FERROVIA_RESPONSAVEL_FATURAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel? FerroviaResponsavelFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FerroviaEmitente", Column = "CON_FERROVIA_EMITENTE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel? FerroviaEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTrafego", Column = "CON_VALOR_FRETE_TRAFEGO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal? ValorFreteTrafego { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTeFerroviaOrigem", Column = "CON_CHAVE_CTE_FERROVIA_ORIGEM", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveCTeFerroviaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFluxoFerroviario", Column = "CON_NUMERO_FLUXO_FERROVIARIO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroFluxoFerroviario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ferrovias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_FERROVIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeFerrovia", Column = "CFE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeFerrovia> Ferrovias { get; set; }

        #endregion

        #region Modal Dutoviário

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioPrestacaoServico", Column = "CON_DATA_INICIO_PRESTACAO_SERVICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimPrestacaoServico", Column = "CON_DATA_FIM_PRESTACAO_SERVICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimPrestacaoServico { get; set; }

        #endregion

        #region Modal Multimodal

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCOTM", Column = "CON_NUMERO_COTM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroCOTM { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_PASSAGEM_UM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoPassagemUm { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_PASSAGEM_DOIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoPassagemDois { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_PASSAGEM_TRES", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoPassagemTres { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_PASSAGEM_QUATRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoPassagemQuatro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_PASSAGEM_CINCO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoPassagemCinco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "CON_TERMINAL_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "CON_TERMINAL_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio Viagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM_PASSAGEM_UM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio ViagemPassagemUm { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM_PASSAGEM_DOIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio ViagemPassagemDois { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM_PASSAGEM_TRES", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio ViagemPassagemTres { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM_PASSAGEM_QUATRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio ViagemPassagemQuatro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "CON_VIAGEM_PASSAGEM_CINCO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio ViagemPassagemCinco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_CONTROLE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_CONTROLE_SVM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroControleSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_BOOKING", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_BOOKING_OBSERVACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroBookingObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_CONTAINER_OBSERVACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroContainerObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_OS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_EMBARQUE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Embarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_MASTER_BL", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string MasterBL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_DI", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CON_CLIENTE_PROVEDOR_OS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteProvedorOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DESCRICAO_CARRIER", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoCarrier { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaFeeder", Column = "COM_TIPO_PROPOSTA_FEEDER", TypeType = typeof(Enumeradores.TipoPropostaFeeder), NotNull = false)]
        public virtual Enumeradores.TipoPropostaFeeder TipoPropostaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorreuSinistroAvaria", Column = "CON_OCORREU_SINITRO_AVARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorreuSinistroAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCartaCorrecao", Column = "CON_POSSUI_CARTA_CORRECAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCartaCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTeComplementar", Column = "CON_POSSUI_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiAnulacaoSubstituicao", Column = "CON_POSSUI_ANULACAO_SUBSTITUICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiAnulacaoSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SVMTerceiro", Column = "CON_SVM_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SVMTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SVMProprio", Column = "CON_SVM_PROPRIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SVMProprio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DATA_PREVIA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPreviaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_QRCODE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string QRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_MANIFESTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroManifesto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_CE_MERCANTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroCEMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NUMERO_MANIFESTO_TRANSBORDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroManifestoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeImportadoEmbarcador", Column = "CON_CTE_IMPORTADO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTeImportadoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaBooking", Column = "CON_SEQUENCIA_BOOKING", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaBooking { get; set; }

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

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_FATURAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "CON_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFsManualIntegrada", Column = "CON_NFS_MANUAL_INTEGRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NFsManualIntegrada { get; set; }

        /// <summary>
        /// Opção para desabilitar dos relatórios o documento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DESABILITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Desabilitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_CTE_PENDENTE_INTEGRACAO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTePendenteIntegracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ICMS.RegraICMS RegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCompanhia", Column = "CON_CODIGO_COMPANHIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCompanhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CON_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiPedidoSubstituicao", Column = "CON_POSSUI_PEDIDO_SUBSTITUICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiPedidoSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCRT", Column = "CON_NUMERO_CRT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialCRT", Column = "CON_NUMERO_SEQUENCIAL_CRT", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencialCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisOrigemCRT", Column = "CON_SIGLA_PAIS_ORIGEM_CRT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SiglaPaisOrigemCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLicencaTNTICRT", Column = "CON_NUMERO_LICENCA_TNTI_CRT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroLicencaTNTICRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_CANCELADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanceladoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCustoViagem", Column = "CCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem CentroDeCustoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BookingReference", Column = "CON_BOOKING_REFERENCE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string BookingReference { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDesoneracao", Column = "CON_VALOR_ICMS_DESONERACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSDesoneracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBeneficio", Column = "CON_CODIGO_BENEFICIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoBeneficio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOSConvertido", Column = "CON_TIPO_OS_CONVERTIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido? TipoOSConvertido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOS", Column = "CON_TIPO_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOS? TipoOS { get; set; }

        #region EnvioDocumentacao

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentacaoEnviada", Column = "CON_DOCUMENTACAO_ENVIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentacaoEnviada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentacaoEnviadaAutomatica", Column = "CON_DOCUMENTACAO_ENVIADA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentacaoEnviadaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_ULTIMO_EMAIL_ENVIO_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UltimoEmailEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoEnvioDocumentacao", Column = "CON_DATA_ULTIMO_ENVIO_DOCUMENTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_EMAIL_DESPACHANTE_SVM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailDespachanteSVM { get; set; }

        #endregion

        #region EnvioDocumentacaoAFRMM

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentacaoAFRMMEnviada", Column = "CON_DOCUMENTACAO_AFRMM_ENVIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentacaoAFRMMEnviada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentacaoAFRMMEnviadaAutomatica", Column = "CON_DOCUMENTACAO_AFRMM_ENVIADA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DocumentacaoAFRMMEnviadaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_ULTIMO_EMAIL_ENVIO_DOCUMENTACAO_AFRMM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UltimoEmailEnvioDocumentacaoAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoEnvioDocumentacaoAFRMM", Column = "CON_DATA_ULTIMO_ENVIO_DOCUMENTACAO_AFRMM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoEnvioDocumentacaoAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEnvioDocumentacaoAFRMM", Column = "CON_QUANTIDADE_ENVIO_DOCUMENTACAO_AFRMM", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEnvioDocumentacaoAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEnvioDocumentacaAFRMM", Column = "CON_SITUACAO_ENVIO_DOCUMENTACAO_AFRMM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao? SituacaoEnvioDocumentacaAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEnvioDocumentacaAFRMMEmail", Column = "CON_SITUACAO_ENVIO_DOCUMENTACAO_AFRMM_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao? SituacaoEnvioDocumentacaAFRMMEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouRegistroDocumentacaoAFRMM", Column = "CON_GEROU_REGISTRO_DOCUMENTACAO_AFRMM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerouRegistroDocumentacaoAFRMM { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Set(0, Name = "XMLNotaFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_XML_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> XMLNotaFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaDeCTe", Column = "COC_CODIGO")]
        public virtual IList<Dominio.Entidades.OcorrenciaDeCTe> Ocorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaturasTMS", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_CARGA_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaCargaDocumento", Column = "FCD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> FaturasTMS { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaCTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_COMPLEMENTO_INFO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> CargaCTeOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_DOCUMENTO_ORIGINARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeDocumentoOriginario", Column = "CDO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario> DocumentosOriginarios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FATURAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CON_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoFaturamento", Column = "DFA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> DocumentosFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConciliacaoTransportador", Column = "COT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador ConciliacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_GERADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_NAO_ENVIAR_PARA_MERCANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarParaMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaNaoEnviarParaMercante", Column = "CON_JJUSTIFICATIVA_NAO_ENVIAR_PARA_MERCANTE", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string JustificativaNaoEnviarParaMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNaoEnviarParaMercante", Column = "CON_TIPO_NAO_ENVIAR_PARA_MERCANTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoNaoEnviarParaMercante), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoNaoEnviarParaMercante? TipoNaoEnviarParaMercante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaMercante", Column = "JME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante JustificativaMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConhecimentoProceda", Column = "CON_TIPO_CONHECIMENTO_PROCEDA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoConhecimentoProceda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequencialOperacao", Column = "CON_SEQUENCIAL_OPERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequencialOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEscrituracao", Column = "CON_CODIGO_ESCRITURACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CON_DISTANCIA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CON_USUARIO_EMISSAO_CTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoSuspenso", Column = "CON_VALOR_IMPOSTO_SUSPENSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorImpostoSuspenso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "CON_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento? SistemaEmissor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlDownloadXml", Column = "CON_URL_DOWNLOAD_XML", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlDownloadXml { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }
       
        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "CON_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "CON_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIndicadorOperacao", Column = "CON_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "CON_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "CON_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "CON_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadualEfetiva", Column = "CON_ALIQUOTA_IBS_ESTADUAL_EFETIVA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadualEfetiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "CON_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "CON_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "CON_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipalEfetiva", Column = "CON_ALIQUOTA_IBS_MUNICIPAL_EFETIVA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipalEfetiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "CON_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "CON_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "CON_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBSEfetiva", Column = "CON_ALIQUOTA_CBS_EFETIVA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBSEfetiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "CON_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        /// <summary>
        /// Campo vTotDFe
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDocumentoFiscal", Column = "CON_VALOR_TOTAL_DOCUMENTO_FISCAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDocumentoFiscal { get; set; }

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString() + " - " + (this.Serie?.Numero ?? 0).ToString();
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (Status)
                {
                    case "P":
                        return "Pendente";
                    case "E":
                        return "Enviado";
                    case "R":
                        return "Rejeição";
                    case "A":
                        return "Autorizado";
                    case "C":
                        return "Cancelado";
                    case "I":
                        return "Inutilizado";
                    case "D":
                        return "Denegado";
                    case "S":
                        return "Em Digitação";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    case "Z":
                        return "Anulado Gerencialmente";
                    case "X":
                        return "Aguardando Assinatura";
                    case "V":
                        return "Aguardando Assinatura Cancelamento";
                    case "B":
                        return "Aguardando Assinatura Inutilização";
                    case "M":
                        return "Aguardando Emissão e-mail";
                    case "F":
                        return "Contingência FSDA";
                    case "Q":
                        return "Contingência EPEC";
                    case "Y":
                        return "Aguardando Finalizar Carga Integração";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoCTeSefaz
        {
            get
            {
                switch (Status)
                {
                    case "P":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente;
                    case "E":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Enviada;
                    case "R":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;
                    case "A":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
                    case "C":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
                    case "I":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada;
                    case "D":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
                    case "S":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao;
                    case "K":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento;
                    case "L":
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao;
                    case "Z":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Anulado;
                    case "X":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AguardandoAssinatura;
                    case "V":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AguardandoAssinaturaCancelamento;
                    case "B":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AguardandoAssinaturaInutilizacao;
                    case "M":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AguardandoEmissaoEmail;
                    case "N":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AguardandoNFSe;
                    case "F":
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.ContingenciaFSDA;
                    default:
                        return ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente;
                }
            }
        }

        public virtual string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public virtual string DescricaoTipoModal
        {
            get
            {
                switch (TipoModal)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo:
                        return "Aereo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario:
                        return "Aquaviário";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario:
                        return "Dutiviário";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario:
                        return "Ferroviário";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal:
                        return "Multimodal";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario:
                        return "Rodoviário";
                    default:
                        return "Rodoviário";
                }
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
                    case Enumeradores.TipoServico.ServVinculadoMultimodal:
                        return "Serv. Vinculado Multimodal";
                    case Enumeradores.TipoServico.TransporteDePessoas:
                        return "Transporte de Pessoas";
                    case Enumeradores.TipoServico.TransporteDeValores:
                        return "Transporte de Valores";
                    case Enumeradores.TipoServico.ExcessoDeBagagem:
                        return "Excesso de Bagagem";
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

        public virtual void SetarParticipante(Dominio.Entidades.Cliente cliente, Enumeradores.TipoTomador tipoParticipante, Dominio.ObjetosDeValor.Endereco endereco = null, Dominio.Entidades.DadosCliente dadosCliente = null, Dominio.ObjetosDeValor.CTe.Cliente dadosParticipante = null)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                case Enumeradores.TipoTomador.Intermediario:
                    this.Intermediario = this.ObterParticipante(this.Intermediario, cliente, endereco, dadosCliente, dadosParticipante);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarDestinatarioDiversos(Empresa empresa, Atividade atividade, string razaoSocial = "DIVERSOS")
        {
            this.Destinatario = this.ObterParticipanteDiversos(this.Destinatario, empresa, atividade, razaoSocial);
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

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.CTe.Cliente obvCliente, Enumeradores.TipoTomador tipoParticipante, Dominio.Entidades.Pais pais, Dominio.Entidades.Atividade atividade, Dominio.Entidades.Localidade localidade = null, Dominio.Entidades.Cliente cliente = null)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoTomador.Destinatario:
                    this.Destinatario = this.ObterParticipante(this.Destinatario, obvCliente, pais, atividade, localidade, cliente);
                    break;
                case Enumeradores.TipoTomador.Expedidor:
                    this.Expedidor = this.ObterParticipante(this.Expedidor, obvCliente, pais, atividade, localidade, cliente);
                    break;
                case Enumeradores.TipoTomador.Outros:
                    this.OutrosTomador = this.ObterParticipante(this.OutrosTomador, obvCliente, pais, atividade, localidade, cliente);
                    break;
                case Enumeradores.TipoTomador.Recebedor:
                    this.Recebedor = this.ObterParticipante(this.Recebedor, obvCliente, pais, atividade, localidade, cliente);
                    break;
                case Enumeradores.TipoTomador.Remetente:
                    this.Remetente = this.ObterParticipante(this.Remetente, obvCliente, pais, atividade, localidade, cliente);
                    break;
                default:
                    break;
            }
        }

        private ParticipanteCTe ObterParticipanteDiversos(ParticipanteCTe participante, Empresa empresa, Atividade atividade, string razaoSocial)
        {
            if (empresa != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = atividade;
                participante.Cidade = null;

                participante.CPF_CNPJ = empresa.CNPJ;
                participante.Email = empresa.Email;
                participante.EmailContador = empresa.EmailContador;
                participante.EmailContadorStatus = false;
                participante.EmailContato = empresa.Email;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = false;
                participante.Exterior = false;
                participante.IE_RG = empresa.InscricaoEstadual;
                participante.InscricaoMunicipal = empresa.InscricaoMunicipal;
                participante.Nome = razaoSocial;
                participante.NomeFantasia = razaoSocial;
                participante.Pais = empresa.Localidade?.Pais ?? null;
                participante.Telefone2 = empresa.TelefoneContato;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
                participante.InscricaoST = empresa.Inscricao_ST;
                participante.CodigoIntegracao = empresa.CodigoIntegracao;

                participante.Bairro = empresa.Bairro;
                participante.CEP = empresa.CEP;
                participante.Complemento = empresa.Complemento;
                participante.Endereco = empresa.Endereco;
                participante.Localidade = empresa.Localidade;
                participante.Numero = empresa.Numero;
                participante.Telefone1 = empresa.Telefone;
                participante.SalvarEndereco = true;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, Cliente cliente, Dominio.ObjetosDeValor.Endereco endereco, DadosCliente dadosCliente = null, Dominio.ObjetosDeValor.CTe.Cliente dadosParticipante = null)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = cliente.Atividade;
                participante.Cidade = null;
                participante.Cliente = cliente;
                participante.GrupoPessoas = cliente.GrupoPessoas;
                participante.CPF_CNPJ = cliente.CPF_CNPJ_SemFormato;
                participante.Exterior = false;

                if (dadosParticipante?.NaoAtualizarDadosCadastrais ?? false)
                {
                    participante.Email = dadosParticipante.Emails;
                    participante.EmailContador = dadosParticipante.EmailsContador;
                    participante.EmailContadorStatus = dadosParticipante.StatusEmailsContador;
                    participante.EmailContato = dadosParticipante.EmailsContato;
                    participante.EmailContatoStatus = dadosParticipante.StatusEmailsContato;
                    participante.EmailStatus = dadosParticipante.StatusEmails;
                    participante.IE_RG = dadosParticipante.RGIE;
                    participante.InscricaoMunicipal = cliente.InscricaoMunicipal;
                    participante.InscricaoSuframa = cliente.InscricaoSuframa;
                    participante.Nome = Utilidades.String.Left(dadosParticipante.RazaoSocial, 60);
                    participante.NomeFantasia = Utilidades.String.Left(dadosParticipante.NomeFantasia, 60);
                    participante.Pais = cliente.Localidade?.Pais ?? null;
                    participante.Telefone2 = Utilidades.String.OnlyNumbers(dadosParticipante.Telefone2);
                    participante.Tipo = cliente.Tipo == "J" ? Enumeradores.TipoPessoa.Juridica : Enumeradores.TipoPessoa.Fisica;
                    participante.InscricaoST = cliente.InscricaoST;
                    participante.InscricaoSuframa = cliente.InscricaoSuframa;
                    participante.CodigoIntegracao = cliente.CodigoIntegracao;

                    participante.Bairro = Utilidades.String.Left(dadosParticipante.Bairro, 40);
                    participante.CEP = dadosParticipante.CEP;
                    participante.Complemento = Utilidades.String.Left(dadosParticipante.Complemento, 60);
                    participante.Endereco = Utilidades.String.Left(dadosParticipante.Endereco, 255);
                    participante.Localidade = cliente.Localidade;
                    participante.Numero = Utilidades.String.Left(dadosParticipante.Numero, 60);
                    participante.Telefone1 = Utilidades.String.OnlyNumbers(dadosParticipante.Telefone1);
                    participante.SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : true;
                }
                else
                {
                    participante.Email = cliente.Email;
                    participante.EmailContador = cliente.EmailContador;
                    participante.EmailContadorStatus = cliente.EmailContadorStatus == "A" ? true : false;
                    participante.EmailContato = cliente.EmailContato;
                    participante.EmailContatoStatus = cliente.EmailContatoStatus == "A" ? true : false;
                    participante.EmailStatus = cliente.EmailStatus == "A" ? true : false;
                    participante.IE_RG = cliente.IE_RG;
                    participante.InscricaoMunicipal = cliente.InscricaoMunicipal;
                    participante.InscricaoSuframa = cliente.InscricaoSuframa;
                    participante.Nome = cliente.Nome;
                    participante.NomeFantasia = endereco != null && !string.IsNullOrWhiteSpace(endereco.NomeFantasia) ? endereco.NomeFantasia : cliente.NomeFantasia;
                    participante.Pais = cliente.Localidade?.Pais ?? null;
                    participante.Telefone2 = cliente.Telefone2;
                    participante.Tipo = cliente.Tipo == "J" ? Enumeradores.TipoPessoa.Juridica : Enumeradores.TipoPessoa.Fisica;
                    participante.InscricaoST = cliente.InscricaoST;
                    participante.InscricaoSuframa = cliente.InscricaoSuframa;
                    participante.CodigoIntegracao = cliente.CodigoIntegracao;

                    if (endereco != null && endereco.Cidade != null)
                    {
                        participante.Bairro = !string.IsNullOrWhiteSpace(endereco.Bairro) && endereco.Bairro.Length > 40 ? endereco.Bairro.Substring(0, 40) : endereco.Bairro;
                        participante.CEP = endereco.CEP;
                        participante.Complemento = endereco.Complemento;
                        participante.Endereco = endereco.Logradouro;
                        participante.Localidade = endereco.Cidade;
                        participante.Numero = endereco.Numero;
                        participante.Telefone1 = endereco.Telefone;
                        participante.CodigoEnderecoIntegracao = endereco.CodigoEnderecoEmbarcador;
                        participante.SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : true;
                    }
                    else
                    {
                        participante.Bairro = !string.IsNullOrWhiteSpace(cliente.Bairro) && cliente.Bairro.Length > 40 ? cliente.Bairro.Substring(0, 40) : cliente.Bairro;
                        participante.CEP = cliente.CEP;
                        participante.Complemento = cliente.Complemento;
                        participante.Endereco = cliente.Endereco;
                        participante.Localidade = cliente.Localidade;
                        participante.Numero = cliente.Numero;
                        participante.Telefone1 = cliente.Telefone1;
                        participante.SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : true;
                    }
                }

                if (string.IsNullOrWhiteSpace(participante.NomeFantasia))
                    participante.NomeFantasia = participante.Nome;

                participante.EmailTransportador = dadosCliente?.Email;
                participante.EmailTransportadorStatus = dadosCliente?.EmailStatus == "A" ? true : false;

                string emailsModeloDocumento = participante.GrupoPessoas?.EmailsModeloDocumento?.Where(o => o.ModeloDocumentoFiscal == ModeloDocumentoFiscal).Select(o => o.Emails).FirstOrDefault();

                if ((TipoServico == TipoServico.SubContratacao || TipoServico == TipoServico.Redespacho) && (participante.GrupoPessoas?.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail ?? false))
                {
                    participante.Email = string.Empty;
                    emailsModeloDocumento = string.Empty;
                    participante.EmailStatus = false;
                }

                if (!string.IsNullOrWhiteSpace(emailsModeloDocumento))
                {
                    participante.Email = emailsModeloDocumento;
                    participante.EmailStatus = true;
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
                participante.EmailStatus = cliente.StatusEmails;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = cliente.RGIE;
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
                participante.InscricaoST = null;

                participante.EmailTransportador = null;
                participante.EmailTransportadorStatus = false;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteCTe ObterParticipante(ParticipanteCTe participante, ObjetosDeValor.CTe.Cliente objCliente, Dominio.Entidades.Pais pais, Dominio.Entidades.Atividade atividade, Dominio.Entidades.Localidade localidade = null, Dominio.Entidades.Cliente cliente = null)
        {
            if (objCliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteCTe();

                participante.Atividade = atividade;
                participante.Bairro = objCliente.Bairro;
                participante.CEP = null;
                participante.Cidade = objCliente.Cidade;
                participante.Complemento = objCliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = objCliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = objCliente.StatusEmails;
                participante.Endereco = objCliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = localidade;
                participante.Nome = objCliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = objCliente.Numero;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
                participante.InscricaoST = null;
                participante.CodigoIntegracao = objCliente.CodigoCliente;
                participante.SalvarEndereco = cliente?.Localidade?.Estado?.Sigla == "EX" ? false : true; ;
                participante.Cliente = cliente;
                participante.EmailTransportador = objCliente.EmailTransportador;
                participante.EmailTransportadorStatus = objCliente.EmailTransportadorStatus == "A" ? true : false;
                participante.GrupoPessoas = cliente?.GrupoPessoas;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        /// <summary>
        /// Indicador CIF ou FOB, utilizado para o EDI. "C = CIF / F = FOB"
        /// </summary>
        public virtual string CondicaoPagamento
        {
            get
            {
                return this.TipoPagamento == TipoPagamento.Pago ? TipoCondicaoPagamento.CIF.ObterDescricaoResumida() : TipoCondicaoPagamento.FOB.ObterDescricaoResumida();
            }
        }

        /// <summary>
        /// Indicador CIF ou FOB, utilizado para o EDI. "CIF / FOB"
        /// </summary>
        public virtual string CondicaoPagamento2
        {
            get
            {
                return this.TipoPagamento == TipoPagamento.Pago ? TipoCondicaoPagamento.CIF.ObterDescricao() : TipoCondicaoPagamento.FOB.ObterDescricao();
            }
        }

        public virtual string ModalidadeFreteEBS
        {
            get
            {
                switch (TipoPagamento)
                {
                    case Enumeradores.TipoPagamento.Pago:
                        return "1";
                    case Enumeradores.TipoPagamento.A_Pagar:
                        return "2";
                    case Enumeradores.TipoPagamento.Outros:
                        return "4";
                    default:
                        return "";
                }
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


        /// <summary>
        /// Indicador de Substituição tributária, utilizado para o EDI da Danone
        /// </summary>
        public virtual string SubstituicaoTributariaDanone
        {
            get
            {
                if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe)
                    return (this.CST == "60" || this.CST == "40") ? "1" : "2";
                else if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFS || this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFSe)
                {
                    if (this.PercentualISSRetido > 0)
                        return "1";
                    else
                        return "2";
                }
                else
                    return "2";
            }
        }

        public virtual string TipoMeioTransporteCalculado
        {
            get
            {
                if (TipoMeioTransporteEDI == "40HC")
                    return "C4H";
                else if ((TipoMeioTransporteEDI == "20DC" || TipoMeioTransporteEDI == "40DC") && (PesoNotasContainer.ToDouble() + TaraContainer.ToDouble() < 25000))
                    return "C20";
                else if ((TipoMeioTransporteEDI == "20DC" || TipoMeioTransporteEDI == "40DC") && (PesoNotasContainer.ToDouble() + TaraContainer.ToDouble() > 25000))
                    return "C40";
                else
                    return "BITC";
            }
        }

        public virtual decimal BaseCalculoImposto
        {
            get
            {
                if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe)
                    return this.BaseCalculoICMS;
                else if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFS || this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFSe)
                {
                    return this.BaseCalculoISS;
                }
                else
                    return this.BaseCalculoICMS;
            }
        }


        public virtual decimal AliquotaImposto
        {
            get
            {
                if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe)
                    return this.AliquotaICMS;
                else if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFS || this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFSe)
                {
                    return this.AliquotaISS;
                }
                else
                    return this.AliquotaICMS;
            }
        }


        public virtual string ChaveAcesso
        {
            get
            {
                if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe)
                    return this.Chave;
                else
                    return "";
            }
        }

        public virtual decimal ValorImposto
        {
            get
            {
                if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe)
                    return this.ValorICMS;
                else if (this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFS || this.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.NFSe)
                {
                    return this.ValorISS;
                }
                else
                    return this.ValorICMS;
            }
        }

        public virtual string NumeroCTeSegundoTrechoDanone
        {
            get
            {
                return this.Expedidor != null && (this.Expedidor.CPF_CNPJ != this.Remetente.CPF_CNPJ) ? this.Numero.ToString() : "";
            }
        }


        public virtual string SituacaoSPED
        {
            get
            {
                if (this.Status == "A" && this.TipoCTE != Enumeradores.TipoCTE.Complemento)
                    return "00";
                else if (this.Status == "A" && this.TipoCTE == Enumeradores.TipoCTE.Complemento)
                    return "06";
                else if (this.Status == "C")
                    return "02";
                else if (this.Status == "D")
                    return "04";
                else if (this.Status == "I")
                    return "05";
                else
                    return "";
            }
        }

        public virtual string DescricaoSituacaoEnvioDocumentacaAFRMM
        {
            get
            {
                if (this.SituacaoEnvioDocumentacaAFRMM == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Enviado)
                    return "Enviado";
                else if (this.SituacaoEnvioDocumentacaAFRMM == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Falha)
                    return "Falha";
                else if (this.SituacaoEnvioDocumentacaAFRMM == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.NaoEnviado)
                    return "Pendente";
                else
                    return "";
            }
        }

        public virtual string DescricaoSituacaoEnvioDocumentacaAFRMMEmail
        {
            get
            {
                if (this.SituacaoEnvioDocumentacaAFRMMEmail == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Enviado)
                    return "Enviado";
                else if (this.SituacaoEnvioDocumentacaAFRMMEmail == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.Falha)
                    return "Falha";
                else if (this.SituacaoEnvioDocumentacaAFRMMEmail == ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao.NaoEnviado)
                    return "Pendente";
                else
                    return "";
            }
        }

        public virtual bool Equals(ConhecimentoDeTransporteEletronico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Enumeradores.TipoICMS CSTICMS
        {
            get
            {
                switch (CST)
                {
                    case "91":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90;
                    case "90":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90;
                    case "51":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Diferido_51;
                    case "40":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Isencao_40;
                    case "41":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Nao_Tributado_41;
                    case "00":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Normal_00;
                    case "60":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60;
                    case "20":
                        return Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20;
                    case "":
                        return Dominio.Enumeradores.TipoICMS.Simples_Nacional;
                    default:
                        return 0;
                }
            }
        }

        public virtual string CaracteristicaServico
        {
            get
            {
                return CaracteristicaServicoCTe != null ? CaracteristicaServicoCTe.ToUpper() : string.Empty;
            }
            set
            {
                CaracteristicaServicoCTe = value != null ? value.ToUpper() : value;
            }
        }
        public virtual string CaracteristicaTransporte
        {
            get
            {
                return CaracteristicaTransporteCTe != null ? CaracteristicaTransporteCTe.ToUpper() : string.Empty;
            }
            set
            {
                CaracteristicaTransporteCTe = value != null ? value.ToUpper() : value;
            }
        }
        public virtual string ProdutoPredominante
        {
            get
            {
                return ProdutoPredominanteCTe != null ? ProdutoPredominanteCTe.ToUpper() : string.Empty;
            }
            set
            {
                ProdutoPredominanteCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual int ProtocoloCarga
        {
            get { return this.CargaCTes != null && this.CargaCTes.Count > 0 ? this.CargaCTes.FirstOrDefault().Carga.Protocolo : 0; }
        }

        public virtual string OutrasCaracteristicasDaCarga
        {
            get
            {
                return OutrasCaracteristicasDaCargaCTe != null ? OutrasCaracteristicasDaCargaCTe.ToUpper() : string.Empty;
            }
            set
            {
                OutrasCaracteristicasDaCargaCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual string ModeloDocumentoCONEMB
        {
            get
            {
                return ModeloDocumentoFiscal.Numero == "39" ? "07" : "57";
            }
        }

        public virtual string ContinuacaoNotasConemb { get; set; }

        public virtual string TipoCTeIntegracaoCONEMB
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(TipoCTeIntegracao))
                    return TipoCTeIntegracao;
                else if (TipoCTE == Enumeradores.TipoCTE.Complemento)
                    return "C";
                else
                    return "N";
            }
        }

        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico Clonar()
        {
            return (Dominio.Entidades.ConhecimentoDeTransporteEletronico)this.MemberwiseClone();
        }

        public virtual string CNPJEmissorCTeAnterior
        {
            get
            {
                if (DocumentosTransporteAnterior != null && DocumentosTransporteAnterior.Count > 0)
                {
                    return DocumentosTransporteAnterior.FirstOrDefault().Emissor.CPF_CNPJ_SemFormato;
                }
                else
                    return "";
            }
        }

        public virtual string NumeroCTeSubComp
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ChaveCTESubComp) || ChaveCTESubComp.Length != 44)
                    return string.Empty;

                return ChaveCTESubComp.Substring(25, 9);
            }
        }

        public virtual string SerieCTeSubComp
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ChaveCTESubComp) || ChaveCTESubComp.Length != 44)
                    return string.Empty;

                return ChaveCTESubComp.Substring(22, 3);
            }
        }

        public virtual string NumeroCTeAnterior
        {
            get
            {
                if (DocumentosTransporteAnterior != null && DocumentosTransporteAnterior.Count > 0 && !string.IsNullOrWhiteSpace(DocumentosTransporteAnterior.FirstOrDefault().Chave))
                {
                    int.TryParse(DocumentosTransporteAnterior.FirstOrDefault().Chave.Substring(22, 3), out int numero);

                    return numero > 0 ? numero.ToString() : "";
                }
                else
                    return "";
            }
        }

        public virtual string ChaveCTeAnterior
        {
            get
            {
                if (DocumentosTransporteAnterior != null && DocumentosTransporteAnterior.Count > 0)
                    return DocumentosTransporteAnterior.FirstOrDefault().Chave;
                else
                    return "";
            }
        }

        public virtual string SerieCTeAnterior
        {
            get
            {
                if (DocumentosTransporteAnterior != null && DocumentosTransporteAnterior.Count > 0)
                {
                    int.TryParse(DocumentosTransporteAnterior.FirstOrDefault().Chave.Substring(22, 3), out int serie);

                    return serie > 0 ? serie.ToString() : "";
                }
                else
                    return "";
            }
        }

        public virtual decimal ValorICMSIncluso
        {
            get
            {
                if (IncluirICMSNoFrete == Enumeradores.OpcaoSimNao.Sim)
                    return ValorICMS;
                else
                    return 0m;
            }
        }

        public virtual string DigitoChaveCTe
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Chave))
                    return Chave.Substring(43, 1);
                else
                    return "";
            }
        }

        /// <summary>
        /// 1 - Normal
        /// 4 - EPEC pela SVC
        /// 5 - Contingência FSDA
        /// 7 - SVC-RS
        /// 8 - SVC-SP
        /// </summary>
        public virtual string TipoEmissaoChaveCTe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Chave) || Chave.Length != 44)
                    return "1";

                return Chave.Substring(34, 1);
            }
        }

        public virtual string CodigoCentroCusto
        {
            get
            {
                if (Empresa != null)
                    return Empresa.CodigoCentroCusto;
                else
                    return "";
            }
        }

        public virtual string CodigoEstabelecimento
        {
            get
            {
                if (Empresa != null)
                    return Empresa.CodigoEstabelecimento;
                else
                    return "";
            }
        }


        public virtual string DescricaoTipoTomador
        {
            get
            {
                switch (TipoTomador)
                {
                    case Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Enumeradores.TipoTomador.Outros:
                        return "Outros";
                    case Enumeradores.TipoTomador.Intermediario:
                        return "Intermediario";
                    case Enumeradores.TipoTomador.NaoInformado:
                        return "Não informado";
                    default:
                        return "";
                }
            }
        }

        public virtual void SetarRegraOutraAliquota(int codigoOutraAliquota)
        {
            OutrasAliquotas = codigoOutraAliquota > 0 ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas() { Codigo = codigoOutraAliquota } : null;
        }

        #endregion
    }
}