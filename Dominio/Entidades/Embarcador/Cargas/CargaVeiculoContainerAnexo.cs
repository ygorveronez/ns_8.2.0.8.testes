namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VEICULO_CONTAINER_ANEXOS", EntityName = "CargaVeiculoContainerAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo", NameType = typeof(CargaVeiculoContainerAnexo))]
    public class CargaVeiculoContainerAnexo : Anexo.Anexo<CargaVeiculoContainer>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaVeiculoContainer", Column = "CVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaVeiculoContainer EntidadeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoAnexo", Column = "CTA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoAnexo TipoAnexo { get; set; }

        #endregion
    }
}
