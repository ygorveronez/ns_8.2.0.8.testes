using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_ACRESCIMO_DESCONTO", EntityName = "FaturaAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto", NameType = typeof(FaturaAcrescimoDesconto))]
    public class FaturaAcrescimoDesconto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FAD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Justificativa.Descricao;
            }
        }
        public virtual bool Equals(FaturaAcrescimoDesconto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
