using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_MOVIMENTO", EntityName = "TipoMovimento", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoMovimento", NameType = typeof(TipoMovimento))]
    public class TipoMovimento : EntidadeBase, IEquatable<TipoMovimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TIM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TIM_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_DEBITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoDeContaDebito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CREDITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoDeContaCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TIM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTipoMovimento", Column = "TIM_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento FormaTipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CentrosResultados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_MOVIMENTO_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoMovimentoCentroResultado", Column = "TMC_CODIGO")]
        public virtual IList<TipoMovimentoCentroResultado> CentrosResultados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalidadeTipoMovimento", Column = "TIM_FINALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento FinalidadeTipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Finalidades", Column = "TIM_FINALIDADES", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Finalidades { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIM_EXPORTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Exportar { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ContasExportacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_CONTA_EXPORTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TIM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoContaExportacao", Column = "CCE_CODIGO")]
        public virtual IList<ConfiguracaoContaExportacao> ContasExportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIM_NAO_GERAR_RATEIO_DE_DESPESA_POR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarRateioDeDespesaPorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFinalidadeTED", Column = "TIM_CODIGO_FINALIDADE_TED", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoFinalidadeTED { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoHistorico", Column = "TIM_CODIGO_HISTORICO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoHistorico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TIM_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

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

        public virtual bool Equals(TipoMovimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual TipoMovimento Clonar()
        {
            return (TipoMovimento)this.MemberwiseClone();
        }
    }
}
