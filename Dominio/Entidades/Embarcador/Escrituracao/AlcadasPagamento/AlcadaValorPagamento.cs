namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PAGAMENTO_VALOR_PAGAMENTO", EntityName = "AlcadasPagamento.AlcadaValorPagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaValorPagamento", NameType = typeof(AlcadaValorPagamento))]
    public class AlcadaValorPagamento : RegraAutorizacao.Alcada<RegraAutorizacaoPagamento, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}

