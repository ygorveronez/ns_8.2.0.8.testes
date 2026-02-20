namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CIOT_DATA_FIXA_VENCIMENTO", EntityName = "ConfiguracaoCIOTDataFixaVencimento", Name = "Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento", NameType = typeof(ConfiguracaoCIOTDataFixaVencimento))]
    public class ConfiguracaoCIOTDataFixaVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCD_DIA_INICIAL_EMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaInicialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCD_DIA_FINAL_EMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaFinalEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCD_DIA_VENCIMENTO_CIOT", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaVencimentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }
    }
}
