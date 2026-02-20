using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_ENTRADA_DUPLICATA", EntityName = "ParcelaDocumentoEntrada", Name = "Dominio.Entidades.ParcelaDocumentoEntrada", NameType = typeof(ParcelaDocumentoEntrada))]
    public class ParcelaDocumentoEntrada : EntidadeBase, IEquatable<Dominio.Entidades.ParcelaDocumentoEntrada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntrada", Column = "DOE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntrada DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DED_NUMERO", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DED_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "DED_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "DED_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DED_STATUS", TypeType = typeof(Enumeradores.StatusDuplicata), NotNull = true)]
        public virtual Enumeradores.StatusDuplicata Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "DED_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual bool Equals(ParcelaDocumentoEntrada other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
