using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_AGRUPADO_DOCUMENTO", EntityName = "TituloBaixaAgrupadoDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento", NameType = typeof(TituloBaixaAgrupadoDocumento))]
    public class TituloBaixaAgrupadoDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupado", Column = "TIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado TituloBaixaAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloDocumento", Column = "TDO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloDocumento TituloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_PAGO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_BAIXA_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BaixaFinalizada { get; set; }

        /// <summary>
        /// Valor em Título + Acréscimo + Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_TOTAL_A_PAGAR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalAPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_PAGO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPagoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_DESCONTO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDescontoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_ACRESCIMO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimoMoeda { get; set; }

        /// <summary>
        /// Valor em Título + Acréscimo + Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_TOTAL_A_PAGAR_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalAPagarMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_VALOR_AVARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBD_DATA_APLICACAO_DESCONTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAplicacaoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TituloBaixaAgrupado?.Descricao ?? string.Empty;
            }
        }
    }
}
