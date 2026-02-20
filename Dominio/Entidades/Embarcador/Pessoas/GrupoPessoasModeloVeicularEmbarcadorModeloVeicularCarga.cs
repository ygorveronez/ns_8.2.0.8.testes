namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_MODELO_VEICULAR_EMBARCADOR_MODELO_VEICULAR_CARGA", EntityName = "GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga", NameType = typeof(GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga))]
    public class GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoasModeloVeicularEmbarcador", Column = "GMV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador ModeloVeicularEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
