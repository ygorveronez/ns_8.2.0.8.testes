using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_SALDO_MES", EntityName = "ContratoSaldoMes", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador.ContratoSaldoMes", NameType = typeof(ContratoSaldoMes))]
    public class ContratoSaldoMes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSM_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSM_DISTANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSM_DATA_REGISTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Excedente", Column = "CSM_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Excedente { get; set; }

        public virtual bool Equals(TipoContratoFrete other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
