using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CAMPO_OBRIGATORIO", EntityName = "PedidoCampoObrigatorio", Name = "Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampoObrigatorio", NameType = typeof(PedidoCampoObrigatorio))]
    public class PedidoCampoObrigatorio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarProdutoPedido", Column = "PCO_OBRIGATORIO_INFORMAR_PRODUTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarProdutoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Campos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CAMPO_OBRIGATORIO_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoCampo", Column = "PCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.PedidoCampo> Campos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return TipoOperacao?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
