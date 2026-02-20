namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUBAREA_CLIENTE_ACOES_FLUXO_DE_PATIO", EntityName = "SubareaClienteAcoesFluxoDePatio", Name = "Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio", NameType = typeof(SubareaClienteAcoesFluxoDePatio))]
    public class SubareaClienteAcoesFluxoDePatio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SubareaCliente", Column = "SAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.SubareaCliente SubareaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcaoMonitoramento", Column = "AFP_ACAO_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData AcaoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaFluxoPatio", Column = "AFP_ETAPA_FLUXO_PATIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio EtapaFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcaoFluxoPatio", Column = "AFP_ACAO_FLUXO_PATIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoFluxoGestaoPatio), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoFluxoGestaoPatio AcaoFluxoPatio { get; set; }
    }
}
