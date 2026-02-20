namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_ENTREGA_ETAPAS", EntityName = "FluxoGestaoEntregaEtapas", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntregaEtapas", NameType = typeof(FluxoGestaoEntregaEtapas))]
    public class FluxoGestaoEntregaEtapas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GEE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoEntrega", Column = "FGE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega FluxoGestaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GEE_ETAPA_FLUXO_GESTAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GEE_ETAPA_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EntregaPedido", Column = "ENP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EntregaPedido EntregaPedido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Etapa.ToString("g") + " - " + this.FluxoGestaoEntrega?.Descricao;
            }
        }
    }
}

