using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS", EntityName = "NotaFiscalServico", Name = "Dominio.Entidades.NotaFiscalServico", NameType = typeof(NotaFiscalServico))]

    public class NotaFiscalServico : EntidadeBase, IEquatable<Dominio.Entidades.NotaFiscalServico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotaFiscalServico", Column = "NFS_TIPO_NOTA_FISCAL_SERVICO", TypeType = typeof(Enumeradores.TipoNotaFiscalServico), NotNull = true)]
        public virtual Enumeradores.TipoNotaFiscalServico TipoNotaFiscalServico { get; set; }
        
        [NHibernate.Mapping.Attributes.OneToOne(0, Name = "NFSe", Class = "NFSe", PropertyRef = "NFS", Access = "property")]
        public virtual Dominio.Entidades.NFSe NFSe { get; set; }
    
        public virtual bool Equals(NotaFiscalServico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
