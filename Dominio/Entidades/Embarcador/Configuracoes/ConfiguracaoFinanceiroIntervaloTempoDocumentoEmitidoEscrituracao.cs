namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRO_INTERVALO_TEMPO_DOCUMENTO_EMITIDO_ESCRITURACAO", EntityName = "ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao", Name = "Dominio.Entidades.Embarcador.CIOT.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao", NameType = typeof(ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao))]
    public class ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_DIA_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_DIA_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_INTERVALO_HORA", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloHora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoFinanceiro", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoFinanceiro ConfiguracaoFinanceiro { get; set; }
    }
}