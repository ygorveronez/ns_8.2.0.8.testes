namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PAGAMENTO_TRANSPORTADOR", EntityName = "AlcadasPagamento.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoPagamento, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
