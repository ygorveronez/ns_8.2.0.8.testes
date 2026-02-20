using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_STATUS", EntityName = "NotaFiscalStatus", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalStatus", NameType = typeof(NotaFiscalStatus))]
    public class NotaFiscalStatus : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalStatus>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "NFS_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NFS_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusNFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "NFS_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NFS_MENSAGEM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }       

        public virtual bool Equals(NotaFiscalStatus other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
