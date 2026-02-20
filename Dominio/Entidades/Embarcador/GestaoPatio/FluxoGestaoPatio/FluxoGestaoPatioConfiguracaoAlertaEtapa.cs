namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_PATIO_CONFIGURACAO_ALERTA_ETAPA", EntityName = "FluxoGestaoPatioConfiguracaoAlertaEtapa", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa", NameType = typeof(FluxoGestaoPatioConfiguracaoAlertaEtapa))]
    public class FluxoGestaoPatioConfiguracaoAlertaEtapa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "EtapaFluxoGestaoPatio", Column = "FCE_ETAPA_FLUXO_GESTAO_PATIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio EtapaFluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "AlertaVisual", Column = "FCE_ALERTA_VISUAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AlertaVisual { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "AlertaSonoro", Column = "FCE_ALERTA_SONORO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AlertaSonoro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "FluxoGestaoPatioConfiguracaoAlerta", Column = "FCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatioConfiguracaoAlerta ConfiguracaoAlerta { get; set; }
    }
}
