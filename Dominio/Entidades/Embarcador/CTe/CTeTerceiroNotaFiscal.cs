using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_NOTA_FISCAL", EntityName = "CTeTerceiroNotaFiscal", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal", NameType = typeof(CTeTerceiroNotaFiscal))]
    public class CTeTerceiroNotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CSN_NUMERO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "CSN_SERIE", TypeType = typeof(string), Length = 3, NotNull = true)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CSN_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CSN_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSN_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]        
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "CSN_CFOP", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CFOP { get; set; }

        public virtual bool Equals(CTeTerceiroNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual CTeTerceiroNotaFiscal Clonar()
        {
            return (CTeTerceiroNotaFiscal)this.MemberwiseClone();
        }
    }
}
