namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VEICULO_VINCULO_REBOQUE", EntityName = "HistoricoVeiculoVinculoReboque", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque", NameType = typeof(HistoricoVeiculoVinculoReboque))]
    public class HistoricoVeiculoVinculoReboque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HVR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "HistoricoVeiculoVinculo", Column = "HVV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo HistoricoVeiculoVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

    }
}
