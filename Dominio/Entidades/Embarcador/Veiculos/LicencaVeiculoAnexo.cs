namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICENCA_VEICULO_ANEXO", EntityName = "LicencaVeiculoAnexo", Name = "Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo", NameType = typeof(LicencaVeiculoAnexo))]
    public class LicencaVeiculoAnexo : Anexo.Anexo<LicencaVeiculo>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LicencaVeiculo", Column = "VLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override LicencaVeiculo EntidadeAnexo { get; set; }

        #endregion
    }
}
