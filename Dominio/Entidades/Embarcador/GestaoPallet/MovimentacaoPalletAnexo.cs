namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTACAO_PALLET_ANEXO", EntityName = "MovimentacaoPalletAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPalletAnexo", NameType = typeof(MovimentacaoPalletAnexo))]
    public class MovimentacaoPalletAnexo : Anexo.Anexo<MovimentacaoPallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentacaoPallet", Column = "MPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override MovimentacaoPallet EntidadeAnexo { get; set; }

        #endregion
    }
}
