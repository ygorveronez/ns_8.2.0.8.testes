using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO_PEDIDO", EntityName = "SeparacaoPedido", Name = "Dominio.Entidades.Embarcador.WMS.SeparacaoPedido", NameType = typeof(SeparacaoPedido))]
    public class SeparacaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "SPE_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SPE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataExpedicao", Column = "SPE_DATA_EXPEDICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "SPE_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_LOCAL_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SEPARACAO_PEDIDO_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SeparacaoPedidoPedido", Column = "SPP_CODIGO")]
        public virtual ICollection<SeparacaoPedidoPedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SEPARACAO_PEDIDO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SeparacaoPedidoIntegracao", Column = "INT_CODIGO")]
        public virtual ICollection<SeparacaoPedidoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SelecionarNotasParaIntegracao", Column = "SPE_SELECIONAR_NOTAS_PARA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SelecionarNotasParaIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Numero.ToString();
            }
        }
    }
}
