using System;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_SERVICO_PRESTADO", EntityName = "NFSManualServicoPrestado", Name = "Dominio.Entidades.Embarcador.NFS.NFSManualServicoPrestado", NameType = typeof(NFSManualServicoPrestado))]
    public class NFSManualServicoPrestado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NFS.NFSManualServicoPrestado>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NSP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "NSP_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "NSP_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Discriminacao", Column = "NSP_DISCRIMINACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Discriminacao { get; set; }

        public virtual bool Equals(NFSManualServicoPrestado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
