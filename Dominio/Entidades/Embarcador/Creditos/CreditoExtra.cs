using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_EXTRA", EntityName = "CreditoExtra", Name = "Dominio.Entidades.Embarcador.Creditos.CreditoExtra", NameType = typeof(CreditoExtra))]
    public class CreditoExtra : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.CreditoExtra>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CreditoDisponivel", Column = "CDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CreditoDisponivel CreditoDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditoExtra", Column = "CEX_VALOR_CREDITO_EXTRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCreditoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacao", Column = "CEX_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CEX_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Creditor?.Nome ?? string.Empty;
            }
        }

        public virtual bool Equals(CreditoExtra other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
