namespace Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_VALOR", EntityName = "AlcadasContratoFreteAcrescimoDesconto.AlcadaValor", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaValor", NameType = typeof(AlcadaValor))]
    public class AlcadaValor : RegraAutorizacao.Alcada<RegraAutorizacaoContratoFreteAcrescimoDesconto, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoContratoFreteAcrescimoDesconto", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoContratoFreteAcrescimoDesconto RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
