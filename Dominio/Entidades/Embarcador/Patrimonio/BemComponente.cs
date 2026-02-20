using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_COMPONENTE", EntityName = "BemComponente", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemComponente", NameType = typeof(BemComponente))]
    public class BemComponente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemComponente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BEC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSerie", Column = "BEC_NUMERO_SERIE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimGarantia", Column = "BEC_DATA_FIM_GARANTIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimGarantia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        public virtual bool Equals(BemComponente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
