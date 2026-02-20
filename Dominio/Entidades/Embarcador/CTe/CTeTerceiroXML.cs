namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_XML", EntityName = "CTeTerceiroXML", Name = "Dominio.Entidades.CTeTerceiroXML", NameType = typeof(CTeTerceiroXML))]
    public class CTeTerceiroXML : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pacote", Column = "PCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Pacote Pacote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PacoteWebHook", Column = "PWT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.PacoteWebHook PacoteWebHook { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "CTX_XML", Type = "StringClob", NotNull = true)]
        public virtual string XML { get; set; }

        public virtual CTeTerceiroXML Clonar()
        {
            return (CTeTerceiroXML)this.MemberwiseClone();
        }
    }
}
