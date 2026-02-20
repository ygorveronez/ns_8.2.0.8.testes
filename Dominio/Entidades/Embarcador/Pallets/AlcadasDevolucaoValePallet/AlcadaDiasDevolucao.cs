namespace Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_DEVOLUCAO_VALE_PALLET_DIAS_DEVOLUCAO", EntityName = "AlcadasDevolucaoValePallet.AlcadaDiasDevolucao", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AlcadaDiasDevolucao", NameType = typeof(AlcadaDiasDevolucao))]
    public class AlcadaDiasDevolucao : RegraAutorizacao.Alcada<RegraAutorizacaoDevolucaoValePallet, int>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_DIAS_DEVOLUCAO", TypeType = typeof(int), NotNull = true)]
        public override int PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDevolucaoValePallet", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDevolucaoValePallet RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
