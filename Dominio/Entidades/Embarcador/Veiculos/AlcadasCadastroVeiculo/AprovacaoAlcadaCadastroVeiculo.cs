namespace Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CADASTRO_VEICULO", EntityName = "AprovacaoAlcadaCadastroVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo", NameType = typeof(AprovacaoAlcadaCadastroVeiculo))]
    public class AprovacaoAlcadaCadastroVeiculo : RegraAutorizacao.AprovacaoAlcada<CadastroVeiculo, RegraAutorizacaoCadastroVeiculo>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CadastroVeiculo", Column = "CVE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CadastroVeiculo OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCadastroVeiculo", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCadastroVeiculo RegraAutorizacao { get; set; }

        #endregion
    }
}
