using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_TIPO_OPERACAO", EntityName = "CentroCarregamentoTipoOperacao", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao", NameType = typeof(CentroCarregamentoTipoOperacao))]
    public class CentroCarregamentoTipoOperacao : EntidadeBase, IEquatable<CentroCarregamentoTipoOperacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_TIPO_SIMULACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo Tipo{ get; set; }

        public virtual bool Equals(CentroCarregamentoTipoOperacao other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
