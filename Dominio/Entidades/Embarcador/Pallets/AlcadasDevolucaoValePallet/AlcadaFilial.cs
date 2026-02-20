namespace Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_DEVOLUCAO_VALE_PALLET_FILIAL", EntityName = "AlcadasDevolucaoValePallet.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoDevolucaoValePallet, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDevolucaoValePallet", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDevolucaoValePallet RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
