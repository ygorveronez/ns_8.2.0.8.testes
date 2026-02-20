using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_ELECTROLUX_DOCUMENTO_TRANSPORTE_NOTA_FISCAL", EntityName = "IntegracaoElectroluxDocumentoTransporteNotaFiscal", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal", NameType = typeof(IntegracaoElectroluxDocumentoTransporteNotaFiscal))]
    public class IntegracaoElectroluxDocumentoTransporteNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
                
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoElectroluxDocumentoTransporte", Column = "IDT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual IntegracaoElectroluxDocumentoTransporte DocumentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "INF_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "INF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "INF_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "INF_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "INF_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "INF_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "INF_SERIE", TypeType = typeof(int), NotNull = true)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "INF_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "INF_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedido", Column = "INF_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "INF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "INF_XML", Type = "StringClob", NotNull = false)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "INF_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "INF_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_INFORMADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InseridaManualmente { get; set; }
    }
}
