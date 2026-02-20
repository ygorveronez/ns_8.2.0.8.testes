namespace Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TERMO_QUITACAO_TRANSPORTADOR", EntityName = "AlcadasTermoQuitacao.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoTermoQuitacao, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTermoQuitacao", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTermoQuitacao RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
