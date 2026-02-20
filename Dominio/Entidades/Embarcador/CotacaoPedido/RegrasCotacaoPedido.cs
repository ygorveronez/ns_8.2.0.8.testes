using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_COTACAO_PEDIDO", EntityName = "RegrasCotacaoPedido", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido", NameType = typeof(RegrasCotacaoPedido))]
    public class RegrasCotacaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RCP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RCP_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RCP_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RCP_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorGrupoPessoa", Column = "RCP_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RCP_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RCP_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorModeloVeicular", Column = "RCP_TIPO_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorFrete", Column = "RCP_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_COTACAO_PEDIDO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCotacaoPedidoTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_PEDIDO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasCotacaoPedidoTipoCarga", Column = "RCT_CODIGO")]
        public virtual IList<RegrasCotacaoPedidoTipoCarga> RegrasCotacaoPedidoTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCotacaoPedidoTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_PEDIDO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasCotacaoPedidoTipoOperacao", Column = "RCO_CODIGO")]
        public virtual IList<RegrasCotacaoPedidoTipoOperacao> RegrasCotacaoPedidoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCotacaoPedidoValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_PEDIDO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasCotacaoPedidoValor", Column = "RCV_CODIGO")]
        public virtual IList<RegrasCotacaoPedidoValor> RegrasCotacaoPedidoValor { get; set; }

        public virtual bool Equals(RegrasCotacaoPedido other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
