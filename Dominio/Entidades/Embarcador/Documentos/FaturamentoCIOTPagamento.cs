using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_FATURAMENTO_PAGAMENTO", EntityName = "FaturamentoCIOTPagamento", Name = "Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTPagamento", NameType = typeof(FaturamentoCIOTPagamento))]
    public class FaturamentoCIOTPagamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoCIOT", Column = "CFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoCIOT FaturamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_JUROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Juros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFP_MULTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Multa { get; set; }
    }
}
