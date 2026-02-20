using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_ELETRONICO_COMANDO_RETORNO", EntityName = "PagamentoEletronicoComandoRetorno", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno", NameType = typeof(PagamentoEletronicoComandoRetorno))]
    public class PagamentoEletronicoComandoRetorno : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comando", Column = "PCR_COMANDO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Comando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PCR_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComandoDeLiquidacao", Column = "PCR_COMANDO_DE_LIQUIDACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComandoDeLiquidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComandoDeEstorno", Column = "PCR_COMANDO_DE_ESTORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComandoDeEstorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PCR_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
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

        public virtual bool Equals(PagamentoEletronicoComandoRetorno other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
