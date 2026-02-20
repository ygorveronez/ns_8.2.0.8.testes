using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM_HISTORICO", EntityName = "ProdutoEmbarcadorEstoqueArmazemHistorico", Name = "Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico", NameType = typeof(ProdutoEmbarcadorEstoqueArmazemHistorico))]
    public class ProdutoEmbarcadorEstoqueArmazemHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcadorEstoqueArmazem", Column = "PEA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem ProdutoEmbarcadorEstoqueArmazem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAH_DATA_ALTERACAO", TypeType = typeof(DateTime))]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAH_QUANTIDADE_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAH_QUANTIDADE_ATUALIZIDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeAtualizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAH_ACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Acao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAH_AUDITADO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Auditado { get; set; }
    }
}