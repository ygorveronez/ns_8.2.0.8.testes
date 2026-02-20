namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_TRANSPORTE_FRETE_ANEXO", EntityName = "ContratoTransporteFreteAnexo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo", NameType = typeof(ContratoTransporteFreteAnexo))]
    public class ContratoTransporteFreteAnexo : Anexo.Anexo<ContratoTransporteFrete>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ContratoTransporteFrete EntidadeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnexoIntegrado", Column = "CTA_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AnexoIntegrado { get; set; }

        #endregion
    }
}
