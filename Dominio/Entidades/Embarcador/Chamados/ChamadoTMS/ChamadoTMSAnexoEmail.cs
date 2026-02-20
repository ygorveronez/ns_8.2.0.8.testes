namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_ANEXO_EMAIL", EntityName = "ChamadoTMSAnexoEmail", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail", NameType = typeof(ChamadoTMSAnexoEmail))]
    public class ChamadoTMSAnexoEmail : Anexo.Anexo<ChamadoTMS>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ChamadoTMS EntidadeAnexo { get; set; }

        #endregion
    }
}
