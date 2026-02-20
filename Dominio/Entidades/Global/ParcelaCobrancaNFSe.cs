using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_PARCELA_COBRANCA", EntityName = "ParcelaCobrancaNFSe", Name = "Dominio.Entidades.ParcelaCobrancaNFSe", NameType = typeof(ParcelaCobrancaNFSe))]
    public class ParcelaCobrancaNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CobrancaNFSe", Column = "NFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CobrancaNFSe Cobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Parcela", Column = "NFP_PARCELA", TypeType = typeof(int), NotNull = true)]
        public virtual int Parcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "NFP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NFP_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = true)]
        public virtual int Valor { get; set; }
    }
}
