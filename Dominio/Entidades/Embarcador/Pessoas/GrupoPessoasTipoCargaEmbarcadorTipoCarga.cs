namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_TIPO_CARGA_EMBARCADOR_TIPO_CARGA", EntityName = "GrupoPessoasTipoCargaEmbarcadorTipoCarga", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga", NameType = typeof(GrupoPessoasTipoCargaEmbarcadorTipoCarga))]
    public class GrupoPessoasTipoCargaEmbarcadorTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoasTipoCargaEmbarcador", Column = "GTE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador TipoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
