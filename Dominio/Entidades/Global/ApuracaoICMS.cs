using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APURACAO_ICMS", EntityName = "ApuracaoICMS", Name = "Dominio.Entidades.ApuracaoICMS", NameType = typeof(ApuracaoICMS))]
    public class ApuracaoICMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "API_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "API_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "API_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditosPeriodoAnterior", Column = "API_VALOR_CREDITOS_PERIODO_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCreditosPeriodoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditos", Column = "API_VALOR_CREDITOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCreditos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDebitos", Column = "API_VALOR_DEBITOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDebitos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSRecolher", Column = "API_VALOR_ICMS_RECOLHER", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSRecolher { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSaldoCredorTransportar", Column = "API_VALOR_SALDO_CREDOR_TRANSPORTAR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSaldoCredorTransportar { get; set; }


    }
}
