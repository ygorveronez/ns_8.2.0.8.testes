using Dominio.Entidades.Embarcador.Pedidos;

namespace Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_CONFIGURACAO_ROTA_FRETE", EntityName = "TipoOperacaoConfiguracaoRotaFrete", Name = "Dominio.Entidades.Embarcador.Logistica.TipoOperacaoConfiguracaoRotaFrete", NameType = typeof(TipoOperacaoConfiguracaoRotaFrete))]
    public class TipoOperacaoConfiguracaoRotaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

    }
}
