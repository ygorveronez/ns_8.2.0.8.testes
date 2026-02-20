namespace Dominio.Entidades.Global
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILTRO_PERSONALIZADO", EntityName = "FiltrosPersonalizados", Name = "Dominio.Entidades.FiltrosPersonalizados", NameType = typeof(FiltroPersonalizado))]
    public class FiltroPersonalizado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFiltro", Column = "FPS_NOME_FILTRO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeFiltro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AbaFiltroPersonalizado", Column = "FPS_ABA_FILTRO_PERSONALIZADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Global.AbaFiltroPersonalizado AbaFiltroPersonalizado { get; set; }
    }
}


