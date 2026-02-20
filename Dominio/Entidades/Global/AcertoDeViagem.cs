using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VIAGEM", EntityName = "AcertoDeViagem", Name = "Dominio.Entidades.AcertoDeViagem", NameType = typeof(AcertoDeViagem))]
    public class AcertoDeViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ACE_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "ACE_PLACA", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "ACE_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        /// <summary>
        /// A - Aberto
        /// P - Pendente de Pagamento
        /// F - Fechado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ACE_SITUACAO", TypeType = typeof(string), NotNull = false, Length = 1)]
        public virtual string Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Adiantamento", Column = "ACE_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "ACE_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFechamento", Column = "ACE_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "ACE_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalReceitas", Column = "ACE_TOTAL_RECEITA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalReceitas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalDespesas", Column = "ACE_TOTAL_DESPESA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalDespesasPagasMotoristas", Column = "ACE_TOTAL_DESPESA_PAGA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalDespesasPagasMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACE_OBS", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ACE_STATUS", TypeType = typeof(string), NotNull = false, Length = 1)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalReceitasCTe", Column = "ACE_RECEITA_CTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalReceitasCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalReceitasOutros", Column = "ACE_RECEITA_OUTRAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalReceitasOutros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalVales", Column = "ACE_TOTAL_VALES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalVales { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComissao", Column = "ACE_TIPO_COMISSAO", TypeType = typeof(Dominio.Enumeradores.TipoComissao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoComissao TipoComissao { get; set; }

        public virtual decimal Comissao {
            get
            {
                return (TipoComissao == Enumeradores.TipoComissao.ValorLiquido ? (TotalReceitas - TotalDespesas) : TotalReceitas) * (PercentualComissao / 100);
            }
        }

        public virtual string DescricaoVeiculo
        {
            get
            {
                if (this.Veiculo == null)
                    return this.Placa;
                else
                    return this.Veiculo.Placa;
            }
        }

        public virtual string DescricaoMotorista
        {
            get
            {
                if (this.Motorista == null)
                    return string.Empty;
                else
                    return string.Concat(this.Motorista.CPF, " - ", this.Motorista.Nome);
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case "A":
                        return "Aberto";
                    case "P":
                        return "Pendente de Pagamento";
                    case "F":
                        return "Fechado";
                    default:
                        return "";
                }
            }
        }
    }
}
