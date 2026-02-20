namespace Dominio.Entidades.Embarcador.Cargas.ColetaEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_COLETA_ENTREGA_ETAPAS", EntityName = "FluxoColetaEntregaEtapas", Name = "Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas", NameType = typeof(FluxoColetaEntregaEtapas))]
    public class FluxoColetaEntregaEtapas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "FEE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoColetaEntrega", Column = "FCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega FluxoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEE_ETAPA_FLUXO_COLETA_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega EtapaFluxoColetaEntrega { get; set; }

        public virtual string Descricao
        {
            get
            {
                return FluxoColetaEntrega.Carga.CodigoCargaEmbarcador;
            }
        }
    }
}
