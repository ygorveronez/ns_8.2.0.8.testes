namespace Dominio.Entidades.Embarcador.Pallets.Avaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_AVARIA_ANEXOS", EntityName = "AvariaPalletAnexo", Name = "Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletAnexo", NameType = typeof(AvariaPalletAnexo))]
    public class AvariaPalletAnexo : Anexo.Anexo<AvariaPallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AvariaPallet", Column = "PAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override AvariaPallet EntidadeAnexo { get; set; }

        #endregion
    }
}
