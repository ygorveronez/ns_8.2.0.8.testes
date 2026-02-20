namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_AUTORIZACAO_TIPO_TERCEIRO", EntityName = "AlcadasCarga.AlcadaAutorizacaoTipoTerceiro", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaAutorizacaoTipoTerceiro", NameType = typeof(AlcadaAutorizacaoTipoTerceiro))]
    public class AlcadaAutorizacaoTipoTerceiro : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, Pessoas.TipoTerceiro>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerceiro", Column = "TPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pessoas.TipoTerceiro PropriedadeAlcada { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
