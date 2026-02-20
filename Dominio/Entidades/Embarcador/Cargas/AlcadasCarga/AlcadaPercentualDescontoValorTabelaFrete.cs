namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_PERCENTUAL_DESCONTO_VALOR_TABELA_FRETE", EntityName = "AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete", NameType = typeof(AlcadaPercentualDescontoValorTabelaFrete))]
    public class AlcadaPercentualDescontoValorTabelaFrete : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, decimal>
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

