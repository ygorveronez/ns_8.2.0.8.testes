namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SIMULACAO_FRETE_CARREGAMENTO", EntityName = "SimulacaoFrete", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete", NameType = typeof(SimulacaoFrete))]
    public class SimulacaoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_PESO_FRETE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_PESO_LIQUIDO_FRETE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquidoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_VALOR_MERCADORIA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_VALOR_POR_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_PERCENTUAL_SOB_VALOR_MERCADORIA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSobValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_DISTANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SucessoSimulacao", Column = "SFC_SUCESSO_SIMULACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SucessoSimulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SFC_LEAD_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTime { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carregamento.Descricao;
            }
        }
    }
}
