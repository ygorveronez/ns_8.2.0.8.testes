using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCAL_ARMAZENAMENTO_PRODUTO_TRANSFERENCIA", EntityName = "LocalArmazenamentoProdutoTransferencia", Name = "Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia", NameType = typeof(LocalArmazenamentoProdutoTransferencia))]
    public class LocalArmazenamentoProdutoTransferencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LPT_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "LPT_SITUACAO", TypeType = typeof(SituacaoLocalArmazanamentoProdutoTransferencia), NotNull = false)]
        public virtual SituacaoLocalArmazanamentoProdutoTransferencia Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamentoProdutoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTransferencia", Column = "LPT_DESCRICAO_TRANSFERENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DescricaoTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTransferencia", Column = "LPT_DATA_TRANSFERENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTransferida", Column = "LPT_QUANTIDADE_TRANSFERIDA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeTransferida { get; set; }
    }
}
