namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO_ANEXO", EntityName = "CargaCancelamentoAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoAnexo", NameType = typeof(CargaCancelamentoAnexo))]
    public class CargaCancelamentoAnexo : Anexo.Anexo<CargaCancelamento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaCancelamento EntidadeAnexo { get; set; }

        #endregion
    }
}
