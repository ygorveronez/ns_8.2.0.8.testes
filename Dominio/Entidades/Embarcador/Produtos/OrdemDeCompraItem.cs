
using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_ITEM", DynamicUpdate = true, EntityName = "OrdemDeCompraItem", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraItem", NameType = typeof(OrdemDeCompraItem))]
    public class OrdemDeCompraItem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroItemDocumento", Column = "OCI_NUMERO_ITEM_DOCUMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroItemDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoCancelado", Column = "OCI_DOCUMENTO_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "OCI_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaConcluida", Column = "OCI_ENTREGA_CONCLUIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaConcluida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoProduzidoInternamente", Column = "OCI_PRODUTO_PRODUCIDO_INTERNAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProdutoProduzidoInternamente { get; set; }       
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOrdemCompra", Column = "OCI_QUANTIDADE_ORDEM_COMPRA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeOrdemCompra { get; set; }  
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteTolerancia", Column = "OCI_LIMITE_TOLERANCIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal LimiteTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemDeCompraDocumento", Column = "OCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemDeCompraDocumento OrdemDeCompraDocumento { get; set; }
    }
}
