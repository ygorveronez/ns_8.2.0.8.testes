namespace Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_INTEGRACAO_CTE_VALOR_FRETE", EntityName = "AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete", Name = "Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AlcadaValorFrete", NameType = typeof(AlcadaValorFrete))]
    public class AlcadaValorFrete : RegraAutorizacao.Alcada<RegraAutorizacaoIntegracaoCTe, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoIntegracaoCTe", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoIntegracaoCTe RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}

