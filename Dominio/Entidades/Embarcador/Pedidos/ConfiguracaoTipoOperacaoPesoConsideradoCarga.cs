using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_PESO_CONSIDERADO_CARGA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoPesoConsideradoCarga", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga", NameType = typeof(ConfiguracaoTipoOperacaoPesoConsideradoCarga))]
    public class ConfiguracaoTipoOperacaoPesoConsideradoCarga: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPPC_PESO_CONSIDERADO", TypeType = typeof(EnumPesoConsideradoCarga), NotNull = false)]
        public virtual EnumPesoConsideradoCarga PesoConsideradoNaCarga { get; set; }
        
    }
}
