namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUSTE_SIMULACAO_ITEM_COMPONENTE", EntityName = "AjusteTabelaFreteSimulacaoItemComponente", Name = "Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente", NameType = typeof(AjusteTabelaFreteSimulacaoItemComponente))]
    public class AjusteTabelaFreteSimulacaoItemComponente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFreteSimulacaoItem", Column = "TSI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFreteSimulacaoItem ItemSimulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "TIC_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "TIC_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }
    }
}
