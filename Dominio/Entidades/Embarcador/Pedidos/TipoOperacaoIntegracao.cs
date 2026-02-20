using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_INTEGRACAO", EntityName = "TipoOperacaoIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao", NameType = typeof(TipoOperacaoIntegracao))]
    public class TipoOperacaoIntegracao : EntidadeBase, IEquatable<TipoOperacaoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TOI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }


        public virtual bool Equals(TipoOperacaoIntegracao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
