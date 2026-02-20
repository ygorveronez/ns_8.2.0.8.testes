using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_FILIAL", EntityName = "ProdutoEmbarcadorFilial", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoEmbarcadorFilial", NameType = typeof(ProdutoEmbarcadorFilial))]
    public class ProdutoEmbarcadorFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PEF_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcadorFiliaisSituacoes", Column = "PFS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcadorFilialSituacoes FilialSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "PEF_NCM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsoMaterial", Column = "PEF_USO_MATERIAL", TypeType = typeof(UsoMaterial), NotNull = false)]
        public virtual UsoMaterial UsoMaterial { get; set; }

    }
}
