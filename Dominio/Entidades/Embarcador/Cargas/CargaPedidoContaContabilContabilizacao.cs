using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_CONTA_CONTABIL_CONTABILIZACAO", EntityName = "CargaPedidoContaContabilContabilizacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao", NameType = typeof(CargaPedidoContaContabilContabilizacao))]
    public class CargaPedidoContaContabilContabilizacao : EntidadeBase, IEquatable<CargaPedidoContaContabilContabilizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CONTRA_PARTIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaContraPartida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CPC_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CPC_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        public virtual bool Equals(CargaPedidoContaContabilContabilizacao other)
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
