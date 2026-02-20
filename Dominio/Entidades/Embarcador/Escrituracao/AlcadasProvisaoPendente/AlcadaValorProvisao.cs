namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PROVISAO_PENDENTE_VALOR_PROVISAO", EntityName = "AlcadasProvisaoPendente.AlcadaValorProvisao", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasProvisaoPendente.AlcadaValorProvisao", NameType = typeof(AlcadaValorProvisao))]
    public class AlcadaValorProvisao : RegraAutorizacao.Alcada<RegraAutorizacaoProvisaoPendente, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR_PROVISAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoProvisaoPendente", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoProvisaoPendente RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}