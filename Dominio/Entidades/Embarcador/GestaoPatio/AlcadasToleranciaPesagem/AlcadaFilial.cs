
namespace Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TOLERANCIA_PESAGEM_FILIAL", EntityName = "AlcadasToleranciaPesagem.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegrasAutorizacaoToleranciaPesagem, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoToleranciaPesagem", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegrasAutorizacaoToleranciaPesagem RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
