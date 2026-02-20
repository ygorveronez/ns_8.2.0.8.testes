using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_CIOT", EntityName = "CargaMDFeManualCIOT", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT", NameType = typeof(CargaMDFeManualCIOT))]
    public class CargaMDFeManualCIOT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCIOT", Column = "CMC_NUMERO_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "CMC_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamento", Column = "CMC_FORMA_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento? FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CMC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal? ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "CMC_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal? ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CMC_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

    }

}
