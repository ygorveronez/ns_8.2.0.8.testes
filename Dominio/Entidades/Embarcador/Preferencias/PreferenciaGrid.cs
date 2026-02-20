namespace Dominio.Entidades.Embarcador.Preferencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PREFERENCIA_GRID", EntityName = "PreferenciaGrid", Name = "Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid", NameType = typeof(PreferenciaGrid))]
    public class PreferenciaGrid : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "PRG_URL", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grid", Column = "PRG_GRID", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Grid { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dados", Column = "PRG_DADOS", Type = "StringClob", NotNull = false)]
        public virtual string Dados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloGrid", Column = "MDG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Preferencias.ModeloGrid ModeloGrid { get; set; }
    }
}
