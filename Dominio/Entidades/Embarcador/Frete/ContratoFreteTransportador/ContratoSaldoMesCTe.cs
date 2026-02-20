namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_SALDO_MES_CTE", EntityName = "ContratoSaldoMesCTe", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador.ContratoSaldoMesCTe", NameType = typeof(ContratoSaldoMesCTe))]
    public class ContratoSaldoMesCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_DISTANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_VALOR_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPagarExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_DISTANCIA_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal DistanciaExcedente { get; set; }

        public virtual bool Equals(ContratoSaldoMesCTe other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
