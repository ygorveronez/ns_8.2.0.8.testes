using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_PAGAMENTO_PARCELA", EntityName = "MDFePagamentoParcela", Name = "Dominio.Entidades.MDFePagamentoParcela", NameType = typeof(MDFePagamentoParcela))]
    public class MDFePagamentoParcela : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MDFeInformacoesBancarias", Column = "MIB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.MDFeInformacoesBancarias InformacoesBancarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroParcela", Column = "MPP_NUMERO_PARCELA", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoParcela", Column = "MPP_DATA_VENCIMENTO_PARCELA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorParcela", Column = "MPP_VALOR_PARCELA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ValorParcela { get; set; }
    }
}
