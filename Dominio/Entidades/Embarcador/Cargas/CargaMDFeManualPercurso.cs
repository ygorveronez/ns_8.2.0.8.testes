namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_PERCURSO", EntityName = "CargaMDFeManualPercurso", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso", NameType = typeof(CargaMDFeManualPercurso))]
    public class CargaMDFeManualPercurso: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }
    }
}
