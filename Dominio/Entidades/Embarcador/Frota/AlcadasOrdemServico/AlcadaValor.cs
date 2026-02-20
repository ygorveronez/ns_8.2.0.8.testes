namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_SERVICO_VALOR", EntityName = "AlcadasOrdemServico.AlcadaValor", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaValor", NameType = typeof(AlcadaValor))]
    public class AlcadaValor : RegraAutorizacao.Alcada<RegraAutorizacaoOrdemServico, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoOrdemServico", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoOrdemServico RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
