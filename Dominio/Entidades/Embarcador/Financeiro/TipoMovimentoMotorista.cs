using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_MOVIMENTO_MOTORISTA", EntityName = "TipoMovimentoMotorista", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista", NameType = typeof(TipoMovimentoMotorista))]
    public class TipoMovimentoMotorista : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TMM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TMM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMM_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMM_TIPO_MOVIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade TipoMovimentoEntidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista.Adiantamento:
                        return "Adiantamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista.Comissao:
                        return "Pagamento de Salário";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoMovimentoEntidade
        {
            get
            {
                switch (this.TipoMovimentoEntidade)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada:
                        return "Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum:
                        return "Nenhum";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida:
                        return "Saída";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(TipoMovimentoMotorista other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
