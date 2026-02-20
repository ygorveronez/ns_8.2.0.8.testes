namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_DESTINADO_EMPRESA_PRODUTO", EntityName = "DocumentoDestinadoEmpresaProduto", Name = "Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto", NameType = typeof(DocumentoDestinadoEmpresaProduto))]

    public class DocumentoDestinadoEmpresaProduto : EntidadeBase
    {
        public DocumentoDestinadoEmpresaProduto() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoDestinadoEmpresa", Column = "DDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoDestinadoEmpresa DocumentoDestinadoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "cProd", Column = "DDP_CPROD", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string cProd { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "xProd", Column = "DDP_XPROD", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string xProd { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "uCom", Column = "DDP_UCOM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string uCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "qCom", Column = "DDP_QCOM", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal qCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "vUnCom", Column = "DDP_VUNCOM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal vUnCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "vProd", Column = "DDP_VPROD", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal vProd { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "DDP_NCM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NCM { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
