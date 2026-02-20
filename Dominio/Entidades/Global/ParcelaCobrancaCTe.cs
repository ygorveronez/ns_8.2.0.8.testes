using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COBRANCA_PARCELA", EntityName = "ParcelaCobrancaCTe", Name = "Dominio.Entidades.ParcelaCobrancaCTe", NameType = typeof(ParcelaCobrancaCTe))]
    public class ParcelaCobrancaCTe : EntidadeBase, IEquatable<Dominio.Entidades.ParcelaCobrancaCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CobrancaCTe", Column = "CCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CobrancaCTe Cobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CPA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CPA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CPA_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CPA_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusDuplicata), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusDuplicata Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "CPA_DATA_PGTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CPA_OBS", Length = 5000, TypeType = typeof(string), NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual bool Equals(ParcelaCobrancaCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
