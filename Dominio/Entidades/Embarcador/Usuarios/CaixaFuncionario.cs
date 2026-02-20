using System;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CAIXA_FUNCIONARIO", EntityName = "CaixaFuncionario", Name = "Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario", NameType = typeof(CaixaFuncionario))]
    public class CaixaFuncionario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCaixa", Column = "CAF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa SituacaoCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CAF_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoInicial", Column = "CAF_SALDO_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalEntradas", Column = "CAF_TOTAL_ENTRADAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalEntradas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSaidas", Column = "CAF_TOTAL_SAIDAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalSaidas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoFinal", Column = "CAF_SALDO_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoNoCaixa", Column = "CAF_SALDO_NO_CAIXA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoNoCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbertura", Column = "CAF_DATA_ABERTURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAbertura { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Usuario?.Nome ?? string.Empty + " " + this.PlanoConta?.BuscarDescricao ?? string.Empty + " " + this.Codigo.ToString();
            }
        }

        public virtual string DescricaoSituacaoCaixa
        {
            get
            {
                switch (this.SituacaoCaixa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado:
                        return "Fechado";
                    default:
                        return "";
                }
            }
        }
    }
}
