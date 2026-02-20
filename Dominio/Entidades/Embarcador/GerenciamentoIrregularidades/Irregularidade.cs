using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IRREGULARIDADE", EntityName = "Irregularidade", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade", NameType = typeof(Irregularidade))]
    public class Irregularidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IRR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PortfolioModuloControle", Column = "PMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_SEGUIR_APROVACAO_TRANSPORTADORA_PRIMEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeguirAprovacaoTranspPrimeiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIrregularidade", Column = "IRR_TIPO_IRREGULARIDADE", TypeType = typeof(TipoIrregularidade), NotNull = false)]
        public virtual TipoIrregularidade TipoIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GatilhoIrregularidade", Column = "IRR_GATILHO_IRREGULARIDADE", TypeType = typeof(GatilhoIrregularidade), NotNull = false)]
        public virtual GatilhoIrregularidade GatilhoIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTolerancia", Column = "IRR_VALOR_TOLERANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRR_PERCENTUAL_TOLERANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PercentualTolerancia { get; set; }




    }
}
