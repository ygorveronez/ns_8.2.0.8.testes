namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARREGAMENTO_PERCENTUAL_OCUPACAO_PALLET", EntityName = "AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet", NameType = typeof(AlcadaPercentualOcupacaoPallet))]
    public class AlcadaPercentualOcupacaoPallet : RegraAutorizacao.Alcada<RegraAutorizacaoCarregamento, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_PERCENTUAL_OCUPACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarregamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarregamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
