namespace Dominio.Entidades.Embarcador.Frota.AlcadasInfracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_INFRACAO_TIPO_INFRACAO", EntityName = "AlcadasInfracao.AlcadaTipoInfracao", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaTipoInfracao", NameType = typeof(AlcadaTipoInfracao))]
    public class AlcadaTipoInfracao : RegraAutorizacao.Alcada<RegraAutorizacaoInfracao, TipoInfracao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoInfracao", Column = "TIN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TipoInfracao PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoInfracao", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoInfracao RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
