using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO_TITULO", EntityName = "InfracaoTitulo", Name = "Dominio.Entidades.Embarcador.Frota.InfracaoTitulo", NameType = typeof(InfracaoTitulo))]
    public class InfracaoTitulo : EntidadeBase, IEquatable<InfracaoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFT_DATA_COMPENSACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompensacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFT_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "IFT_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAposVencimento", Column = "IFT_VALOR_APOS_VENCIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAposVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimento { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(InfracaoTitulo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
