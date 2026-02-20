namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ROTA_FRETE_MODELO_VEICULAR_CARGA", EntityName = "ConfiguracaoRotaFreteModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteModeloVeicularCarga", NameType = typeof(ConfiguracaoRotaFreteModeloVeicularCarga))]
    public class ConfiguracaoRotaFreteModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }
    }
}
