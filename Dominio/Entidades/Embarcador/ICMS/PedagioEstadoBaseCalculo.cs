namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDAGIO_ESTADO_BASE_CALCULO", EntityName = "PedagioEstadoBaseCalculo", Name = "Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo", NameType = typeof(PedagioEstadoBaseCalculo))]
    public class PedagioEstadoBaseCalculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiBaseICMS", Column = "PBC_INCLUI_BASE_ICMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluiBaseICMS { get; set; }
    }
}
