namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_SERVICO_TIPO_ORDEM_SERVICO", EntityName = "AlcadasOrdemServico.AlcadaTipoOrdemServico", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaTipoOrdemServico", NameType = typeof(AlcadaTipoOrdemServico))]
    public class AlcadaTipoOrdemServico : RegraAutorizacao.Alcada<RegraAutorizacaoOrdemServico, OrdemServicoFrotaTipo>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override OrdemServicoFrotaTipo PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoOrdemServico", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoOrdemServico RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
