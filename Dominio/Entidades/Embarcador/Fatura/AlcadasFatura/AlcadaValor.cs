namespace Dominio.Entidades.Embarcador.Fatura.AlcadasFatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_FATURA_VALOR", EntityName = "AlcadasFatura.AlcadaValor", Name = "Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaValor", NameType = typeof(AlcadaValor))]
    public class AlcadaValor : RegraAutorizacao.Alcada<RegraAutorizacaoFatura, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoFatura", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoFatura RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
