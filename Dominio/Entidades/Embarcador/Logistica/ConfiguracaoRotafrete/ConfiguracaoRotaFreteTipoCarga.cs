namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ROTA_FRETE_TIPO_CARGA", EntityName = "ConfiguracaoRotaFreteTipoCarga", Name = "Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteTipoCarga", NameType = typeof(ConfiguracaoRotaFreteTipoCarga))]
    public class ConfiguracaoRotaFreteTipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }
    }
}
