using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_DE_CONTA", EntityName = "PlanoConta", Name = "Dominio.Entidades.Embarcador.Financeiro.PlanoConta", NameType = typeof(PlanoConta))]
    public class PlanoConta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PLA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Plano", Column = "PLA_PLANO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Plano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlanoContabilidade", Column = "PLA_PLANO_CONTABILIDADE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PlanoContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PLA_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnaliticoSintetico", Column = "PLA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico AnaliticoSintetico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaDespesa", Column = "PLA_RECEITA_DESPESA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa ReceitaDespesa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoDeResultado", Column = "PLA_GRUPO_DE_RESULTADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado GrupoDeResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoInicialConciliacaoBancaria", Column = "PLA_SALDO_INICIAL_CONCILIACAO_BANCARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoInicialConciliacaoBancaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoFinalConciliacaoBancaria", Column = "PLA_SALDO_FINAL_CONCILIACAO_BANCARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoFinalConciliacaoBancaria { get; set; }

        public virtual string BuscarDescricao
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(PlanoContabilidade) ? (PlanoContabilidade + " - ") : (Plano + " - ")) + Descricao;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return Localization.Resources.Gerais.Geral.Ativo;
                    case false: return Localization.Resources.Gerais.Geral.Inativo;
                    default: return "";
                }
            }
        }

        public virtual string DescricaoAnaliticoSintetico
        {
            get { return AnaliticoSintetico.ObterDescricao(); }
        }

        public virtual string DescricaoReceitaDespesa
        {
            get { return ReceitaDespesa.ObterDescricao(); }
        }

        public virtual string DescricaoGrupoDeResultado
        {
            get { return GrupoDeResultado.ObterDescricao(); }
        }

        public virtual bool Equals(PlanoConta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta Clonar()
        {
            return (Dominio.Entidades.Embarcador.Financeiro.PlanoConta)this.MemberwiseClone();
        }
    }
}
