namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_NCM_PALLET", EntityName = "GrupoPessoasNCMPallet", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasNCMPallet", NameType = typeof(GrupoPessoasNCMPallet))]
    public class GrupoPessoasNCMPallet : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GNP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "GNP_NCM", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }
    }
}
