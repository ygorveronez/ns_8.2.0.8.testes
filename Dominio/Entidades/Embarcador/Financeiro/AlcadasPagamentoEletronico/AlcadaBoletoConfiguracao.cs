namespace Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PAGAMENTO_ELETRONICO_BOLETO_CONFIGURACAO", EntityName = "AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao", Name = "Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao", NameType = typeof(AlcadaBoletoConfiguracao))]
    public class AlcadaBoletoConfiguracao : RegraAutorizacao.Alcada<RegraAutorizacaoPagamentoEletronico, BoletoConfiguracao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override BoletoConfiguracao PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamentoEletronico", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamentoEletronico RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
