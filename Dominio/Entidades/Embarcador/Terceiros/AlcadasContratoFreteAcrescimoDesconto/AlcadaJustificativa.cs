namespace Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_JUSTIFICATIVA", EntityName = "AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa", NameType = typeof(AlcadaJustificativa))]
    public class AlcadaJustificativa : RegraAutorizacao.Alcada<RegraAutorizacaoContratoFreteAcrescimoDesconto, Dominio.Entidades.Embarcador.Fatura.Justificativa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Fatura.Justificativa PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoContratoFreteAcrescimoDesconto", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoContratoFreteAcrescimoDesconto RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
