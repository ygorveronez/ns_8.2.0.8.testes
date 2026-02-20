namespace Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_CAMPO", EntityName = "PessoaCampo", Name = "Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo", NameType = typeof(PessoaCampo))]
    public class PessoaCampo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_CAMPO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Campo { get; set; }
    }
}
