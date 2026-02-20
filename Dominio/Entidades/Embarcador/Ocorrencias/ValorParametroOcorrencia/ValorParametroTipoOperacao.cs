using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_TIPO_OPERACAO", EntityName = "ValorParametroTipoOperacao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao", NameType = typeof(ValorParametroTipoOperacao))]
    public class ValorParametroTipoOperacao : EntidadeBase, IEquatable<ValorParametroTipoOperacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroOcorrencia", Column = "VPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ValorParametroOcorrencia ValorParametroOcorrencia { get; set; }

        public virtual bool Equals(ValorParametroTipoOperacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
