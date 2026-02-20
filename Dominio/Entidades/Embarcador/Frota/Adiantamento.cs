using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ADIANTAMENTO", EntityName = "Adiantamento", Name = "Dominio.Entidades.Embarcador.Frota.Adiantamento", NameType = typeof(Adiantamento))]
    public class Adiantamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ADI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ADI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "ADI_VALOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ADI_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ADI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista TipoErro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADI_TIPO_MOVIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade TipoMovimentoEntidadeErro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoErro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_SAIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaSaida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimentoMotorista", Column = "TMM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoMotorista TipoMovimentoMotorista { get; set; }

        public virtual string Descricao {
            get
            {
                return this.Motorista?.Nome;
            }
        }
    }
}
