namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CANCELAMENTO_ANEXO", EntityName = "OcorrenciaCancelamentoAnexo", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoAnexo", NameType = typeof(OcorrenciaCancelamentoAnexo))]
    public class OcorrenciaCancelamentoAnexo : Anexo.Anexo<OcorrenciaCancelamento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCancelamento", Column = "CAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override OcorrenciaCancelamento EntidadeAnexo { get; set; }

        #endregion
    }
}
