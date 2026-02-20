using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_SEGURO", EntityName = "CTeTerceiroSeguro", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro", NameType = typeof(CTeTerceiroSeguro))]

    public class CTeTerceiroSeguro : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "SEG_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoSeguro), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoSeguro Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "CPS_NOMESEGURADORA", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "CPS_NUMAPOLICE", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "CPS_NUMAVERBACAO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string NumeroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CPS_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual bool Equals(CTeTerceiroSeguro other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual CTeTerceiroSeguro Clonar()
        {
            return (CTeTerceiroSeguro)this.MemberwiseClone();
        }
    }
}
