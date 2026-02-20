namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TIPO_PERCURSO", EntityName = "TipoPercurso", Name = "Dominio.Entidades.Embarcador.Cargas.TipoPercurso", NameType = typeof(TipoPercurso))]
    public class TipoPercurso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CTP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vazio", Column = "CTP_VAZIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Vazio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Vazio Vazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPercursoValor", Column = "CTP_TIPO_PERCURSO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TipoPercursoValor { get; set; }
    }
}
