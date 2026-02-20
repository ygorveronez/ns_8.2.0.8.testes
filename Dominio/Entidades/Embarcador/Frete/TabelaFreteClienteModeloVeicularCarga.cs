namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_MODELO_VEICULAR_CARGA", EntityName = "TabelaFreteClienteModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga", NameType = typeof(TabelaFreteClienteModeloVeicularCarga))]
    public class TabelaFreteClienteModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRota", Column = "TMV_PERCENTUAL_ROTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEntregas", Column = "TMV_QUANTIDADE_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeOTM", Column = "TMV_CAPACIDADE_OTM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CapacidadeOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracao", Column = "TMV_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracao { get; set; }
    }
}
