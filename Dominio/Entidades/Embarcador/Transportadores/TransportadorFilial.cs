using System;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSPORTADOR_FILIAL", EntityName = "TransportadorFilial", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial", NameType = typeof(TransportadorFilial))]
    public class TransportadorFilial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "TFI_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJ { get; set; }

        public virtual bool Equals(TransportadorFilial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string CNPJ_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ));
            }
        }
    }
}