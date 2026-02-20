namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_PERCENTUAL_DIFERENCA_VALOR_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO", EntityName = "AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro", NameType = typeof(AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro))]
    public class AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}

