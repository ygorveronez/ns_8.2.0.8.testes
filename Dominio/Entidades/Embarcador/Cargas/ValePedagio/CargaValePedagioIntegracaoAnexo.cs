namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_VALE_PEDAGIO_ANEXO", EntityName = "CargaValePedagioIntegracaoAnexo", Name = "Dominio.Entidades.Embarcador.Canhotos.CargaValePedagioIntegracaoAnexo", NameType = typeof(CargaValePedagioIntegracaoAnexo))]
    public class CargaValePedagioIntegracaoAnexo : Anexo.Anexo<CargaIntegracaoValePedagio>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoValePedagio", Column = "CVP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaIntegracaoValePedagio EntidadeAnexo { get; set; }

        #endregion
    }
}
