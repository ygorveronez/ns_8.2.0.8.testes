namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_DESTINO", EntityName = "CargaMDFeManualDestino", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino", NameType = typeof(CargaMDFeManualDestino))]
    public class CargaMDFeManualDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }
    }
}