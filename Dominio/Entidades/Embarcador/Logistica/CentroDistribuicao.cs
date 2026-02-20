namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DISTRIBUICAO", EntityName = "CentroDistribuicao", Name = "Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao", NameType = typeof(CentroDistribuicao))]
    public class CentroDistribuicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CDI_DESCRICAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CDI_CODIGO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CDI_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }
    }
}
