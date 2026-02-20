namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LIBERACAO_SEM_INTEGRACAO_GR_ANEXO", EntityName = "CargaLiberacaoSemIntegracaoGRAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLiberacaoSemIntegracaoGRAnexo", NameType = typeof(CargaLiberacaoSemIntegracaoGRAnexo))]
    public class CargaLiberacaoSemIntegracaoGRAnexo : Anexo.Anexo<Carga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga EntidadeAnexo { get; set; }

        #endregion
    }
}
