namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PAGAMENTO_PROVEDOR_DIFERENCA_VALOR_MENOR", EntityName = "AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor", NameType = typeof(AlcadasDiferencaValorMenor))]
    public class AlcadasDiferencaValorMenor : RegraAutorizacao.Alcada<RegraPagamentoProvedor, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_DIFERENCA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPagamentoProvedor", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraPagamentoProvedor RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}