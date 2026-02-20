using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL", EntityName = "NotaFiscalDocumentoTransporteNatura", Name = "Dominio.Entidades.NotaFiscalDocumentoTransporteNatura", NameType = typeof(NotaFiscalDocumentoTransporteNatura))]
    public class NotaFiscalDocumentoTransporteNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoTransporteNatura", Column = "DTN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoTransporteNatura DocumentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "NDT_EMITENTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "NDT_DESTINATARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NDT_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "NDT_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "NDT_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NDT_SERIE", TypeType = typeof(int), NotNull = true)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NDT_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NDT_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NDT_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "NDT_XML", Type = "StringClob", NotNull = false)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "NDT_PAGOAPAGAR", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "NDT_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NDT_STATUS", TypeType = typeof(ObjetosDeValor.Enumerador.StatusNotaFiscalNatura), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusNotaFiscalNatura Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdRevista", Column = "NDT_QTD_REVISTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string QtdRevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SolicitacaoNumero", Column = "NDT_SOLICITACAO_NUMERO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SolicitacaoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoNumero", Column = "NDT_PEDIDO_NUMERO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PedidoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCF", Column = "NDT_CODIGO_CF", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoCF { get; set; }

        public virtual Dominio.Entidades.NotaFiscalDocumentoTransporteNatura Clonar()
        {
            return (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura)this.MemberwiseClone();
        }

        public virtual string RetornarTipoPagamentoEDI
        {
            get
            {
                if(this.TipoPagamento == Enumeradores.TipoPagamento.A_Pagar)
                {
                    return "F";
                }
                else
                {
                    return "C";
                }
                
            }
        }
    }
}
