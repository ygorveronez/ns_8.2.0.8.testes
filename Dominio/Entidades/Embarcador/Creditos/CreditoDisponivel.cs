using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_DISPONIVEL", EntityName = "CreditoDisponivel", Name = "Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel", NameType = typeof(CreditoDisponivel))]
    public class CreditoDisponivel : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RECEBEDOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCredito", Column = "CDI_VALOR_CREDITO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioCredito", Column = "CDI_DATA_INICIO_CREDITO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimCredito", Column = "CDI_DATA_FIM_CREDITO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFimCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSaldo", Column = "CDI_VALOR_SALDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCreditoExtra", Column = "CDI_VALOR_CREDITO_EXTRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCreditoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComprometido", Column = "CDI_VALOR_COMPROMETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorComprometido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorObtido", Column = "CDI_VALOR_OBTIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorObtido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Creditor?.Nome ?? string.Empty;
            }
        }

        public virtual bool Equals(CreditoDisponivel other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
