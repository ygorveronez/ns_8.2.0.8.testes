namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_SERVICO_OPERADOR", EntityName = "AlcadasOrdemServico.AlcadaOperador", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaOperador", NameType = typeof(AlcadaOperador))]
    public class AlcadaOperador : RegraAutorizacao.Alcada<RegraAutorizacaoOrdemServico, Usuario>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Usuario PropriedadeAlcada { get; set; }

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
