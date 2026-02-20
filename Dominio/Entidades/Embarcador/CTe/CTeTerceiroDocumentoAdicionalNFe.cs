namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL_NFE", EntityName = "CTeTerceiroDocumentoAdicionalNFe", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe", NameType = typeof(CTeTerceiroDocumentoAdicionalNFe))]
    public class CTeTerceiroDocumentoAdicionalNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiroDocumentoAdicional", Column = "CAD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiroDocumentoAdicional CTeTerceiroDocumentoAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAN_CHAVE", TypeType = typeof(string), Length = 44, NotNull = true)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAN_NUMERO", TypeType = typeof(long), NotNull = true)]
        public virtual long Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }
}
