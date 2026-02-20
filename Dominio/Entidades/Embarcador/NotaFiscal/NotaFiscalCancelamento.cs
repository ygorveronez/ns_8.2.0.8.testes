using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_CANCELAMENTO", EntityName = "NotaFiscalCancelamento", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento", NameType = typeof(NotaFiscalCancelamento))]
    public class NotaFiscalCancelamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "NFC_JUSTIFICATIVA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "NFC_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NFC_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusNFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NFC_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "NFC_PROTOCOLO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        public virtual bool Equals(NotaFiscalCancelamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
