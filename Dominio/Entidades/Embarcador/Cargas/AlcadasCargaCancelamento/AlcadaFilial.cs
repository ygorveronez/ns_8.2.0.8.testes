namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_CANCELAMENTO_FILIAL", EntityName = "AlcadasCargaCancelamento.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoCargaCancelamento, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCargaCancelamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCargaCancelamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
