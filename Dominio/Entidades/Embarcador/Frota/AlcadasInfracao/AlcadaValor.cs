namespace Dominio.Entidades.Embarcador.Frota.AlcadasInfracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_INFRACAO_VALOR", EntityName = "AlcadasInfracao.AlcadaValor", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaValor", NameType = typeof(AlcadaValor))]
    public class AlcadaValor : RegraAutorizacao.Alcada<RegraAutorizacaoInfracao, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoInfracao", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoInfracao RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
