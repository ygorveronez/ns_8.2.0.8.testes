namespace Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CADASTRO_VEICULO_MODELO_VEICULAR", EntityName = "AlcadasCadastroVeiculo.AlcadaModeloVeicular", Name = "Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaModeloVeicular", NameType = typeof(AlcadaModeloVeicular))]
    public class AlcadaModeloVeicular : RegraAutorizacao.Alcada<RegraAutorizacaoCadastroVeiculo, Cargas.ModeloVeicularCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.ModeloVeicularCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCadastroVeiculo", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCadastroVeiculo RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
