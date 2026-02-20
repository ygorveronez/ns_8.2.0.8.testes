using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_QUANTIDADE", EntityName = "CTeTerceiroQuantidade", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade", NameType = typeof(CTeTerceiroQuantidade))]

    public class CTeTerceiroQuantidade : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]

        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Unidade", Column = "CSQ_UNIDADE", TypeType = typeof(Dominio.Enumeradores.UnidadeMedida), NotNull = true)]
        public virtual Dominio.Enumeradores.UnidadeMedida Unidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMedida", Column = "CSQ_TIPO_MEDIDA", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string TipoMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CSQ_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOriginal", Column = "CSQ_QUANTIDADE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal QuantidadeOriginal { get; set; }

        public virtual bool Equals(CTeTerceiroQuantidade other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual CTeTerceiroQuantidade Clonar()
        {
            return (CTeTerceiroQuantidade)this.MemberwiseClone();
        }
    }
}
