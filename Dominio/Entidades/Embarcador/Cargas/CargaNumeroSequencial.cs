using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NUMERO_SEQUENCIAL", EntityName = "CargaNumeroSequencial", Name = "Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial", NameType = typeof(CargaNumeroSequencial))]
    public class CargaNumeroSequencial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencial", Column = "CNS_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroSequencial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public virtual bool Equals(CargaNumeroSequencial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
