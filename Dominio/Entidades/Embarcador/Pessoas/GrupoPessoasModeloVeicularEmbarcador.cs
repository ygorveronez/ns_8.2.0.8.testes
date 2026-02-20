namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_MODELO_VEICULAR_EMBARCADOR", EntityName = "GrupoPessoasModeloVeicularEmbarcador", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador", NameType = typeof(GrupoPessoasModeloVeicularEmbarcador))]
    public class GrupoPessoasModeloVeicularEmbarcador: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,  Column = "GMV_CODIGO_MODELO_VEICULAR_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string CodigoModeloVeicularEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GMV_DESCRICAO_MODELO_VEICULAR_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string DescricaoModeloVeicularEmbarcador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return DescricaoModeloVeicularEmbarcador;
            }
        }
    }
}
