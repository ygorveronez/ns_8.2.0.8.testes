namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL", EntityName = "CTeTerceiroDocumentoAdicional", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional", NameType = typeof(CTeTerceiroDocumentoAdicional))]
    public class CTeTerceiroDocumentoAdicional : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAD_CHAVE", TypeType = typeof(string), Length = 44, NotNull = true)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAD_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "CAD_VALOR_TOTAL_MERC", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CAD_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }
    }
}
