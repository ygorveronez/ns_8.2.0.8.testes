using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_FILIAL_SITUACAO", EntityName = "ProdutoEmbarcadorFiliaisSituacoes", Name = "Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes", NameType = typeof(ProdutoEmbarcadorFilialSituacoes))]
    public class ProdutoEmbarcadorFilialSituacoes: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PFS_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFilial", Column = "PFS_SITUACAO_FILIAL", TypeType = typeof(SituacaoFilial), NotNull = false)]
        public virtual SituacaoFilial SituacaoFilial { get; set; }

    }
}
