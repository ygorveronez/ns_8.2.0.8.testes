namespace Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TOLERANCIA_PESAGEM_MODELO_VEICULAR_CARGA", EntityName = "AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga", NameType = typeof(AlcadaModeloVeicularCarga))]
    public class AlcadaModeloVeicularCarga : RegraAutorizacao.Alcada<RegrasAutorizacaoToleranciaPesagem, Cargas.ModeloVeicularCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.ModeloVeicularCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoToleranciaPesagem", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegrasAutorizacaoToleranciaPesagem RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}

