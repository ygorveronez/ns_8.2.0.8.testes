using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_XML_NOTA_FISCAL_MIRO", EntityName = "NotaFiscalMiro", Name = "Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro", NameType = typeof(NotaFiscalMiro))]
    public class NotaFiscalMiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XNI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIdentificador", Column = "XNI_CODIGO_IDENTIFICACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIdentificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "XNI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFe", Column = "XNI_CHAVE_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

    }
}
