namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MIGRATE_NFSE_NATUREZA", EntityName = "IntegracaoMigrateNFSeNatureza", Name = "Dominio.Entidades.Embarcador.Cargas.IntegracaoMigrateNFSeNatureza", NameType = typeof(IntegracaoMigrateNFSeNatureza))]
    public class IntegracaoMigrateNFSeNatureza : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MNM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNatureza", Column = "MNM_NUMERO_NATUREZA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNatureza { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MNM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

    }
}
