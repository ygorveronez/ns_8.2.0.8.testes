namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARREGAMENTO_MODELO_VEICULAR_CARGA", EntityName = "AlcadasMontagemCarga.AlcadaModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaModeloVeicularCarga", NameType = typeof(AlcadaModeloVeicularCarga))]
    public class AlcadaModeloVeicularCarga : RegraAutorizacao.Alcada<RegraAutorizacaoCarregamento, ModeloVeicularCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ModeloVeicularCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarregamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarregamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
