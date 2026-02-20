namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_TIPO_CARGA_EMBARCADOR", EntityName = "GrupoPessoasTipoCargaEmbarcador", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador", NameType = typeof(GrupoPessoasTipoCargaEmbarcador))]
    public class GrupoPessoasTipoCargaEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTE_CODIGO_TIPO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string CodigoTipoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GTE_DESCRICAO_TIPO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string DescricaoTipoCargaEmbarcador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return DescricaoTipoCargaEmbarcador;
            }
        }
    }
}
