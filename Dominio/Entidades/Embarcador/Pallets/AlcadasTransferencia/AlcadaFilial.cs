namespace Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TRANSFERENCIA_PALLET_FILIAL", EntityName = "AlcadasTransferencia.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoTransferenciaPallet, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTransferenciaPallet", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTransferenciaPallet RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
