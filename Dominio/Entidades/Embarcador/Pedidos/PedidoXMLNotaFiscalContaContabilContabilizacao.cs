using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PEDIDO_XML_NOTA_FISCAL_CONTA_CONTABIL_CONTABILIZACAO", EntityName = "PedidoXMLNotaFiscalContaContabilContabilizacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao", NameType = typeof(PedidoXMLNotaFiscalContaContabilContabilizacao))]
    public class PedidoXMLNotaFiscalContaContabilContabilizacao : EntidadeBase, IEquatable<PedidoXMLNotaFiscalContaContabilContabilizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CONTRA_PARTIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaContraPartida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CPN_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CPN_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        public virtual bool Equals(PedidoXMLNotaFiscalContaContabilContabilizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }


        public virtual string Descricao
        {
            get
            {
                return PlanoConta.Descricao;
            }
        }
    }
}
