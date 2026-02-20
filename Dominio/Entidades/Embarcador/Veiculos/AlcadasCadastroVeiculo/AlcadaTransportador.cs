namespace Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CADASTRO_VEICULO_TRANSPORTADOR", EntityName = "AlcadasCadastroVeiculo.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoCadastroVeiculo, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

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
