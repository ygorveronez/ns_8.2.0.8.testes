namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_GRUPO_PRODUTO", EntityName = "PeriodoDescarregamentoGrupoProduto", Name = "Dominio.Entidades.Embarcador.Logistica", NameType = typeof(PeriodoDescarregamentoGrupoProduto))]
    public class PeriodoDescarregamentoGrupoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoDescarregamento", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PeriodoDescarregamento PeriodoDescarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }
        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}