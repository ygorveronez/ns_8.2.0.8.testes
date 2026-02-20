namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DEVOLUCAO_PALLET_ANEXOS", EntityName = "DevolucaoPalletAnexo", Name = "Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo", NameType = typeof(DevolucaoPalletAnexo))]
    public class DevolucaoPalletAnexo : Anexo.Anexo<DevolucaoPallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override DevolucaoPallet EntidadeAnexo { get; set; }



        #endregion
    }
}
