namespace Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TRANSFERENCIA_PALLET_QUANTIDADE", EntityName = "AlcadasTransferencia.AlcadaQuantidade", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AlcadaQuantidade", NameType = typeof(AlcadaQuantidade))]
    public class AlcadaQuantidade : RegraAutorizacao.Alcada<RegraAutorizacaoTransferenciaPallet, int>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public override int PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTransferenciaPallet", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTransferenciaPallet RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
