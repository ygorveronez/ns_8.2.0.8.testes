namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PROGRAMACAO_CARGA_MODELO_VEICULAR_CARGA", EntityName = "ConfiguracaoProgramacaoCargaModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga", NameType = typeof(ConfiguracaoProgramacaoCargaModeloVeicularCarga))]
    public class ConfiguracaoProgramacaoCargaModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoProgramacaoCarga", Column = "CPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoProgramacaoCarga ConfiguracaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }
    }
}
