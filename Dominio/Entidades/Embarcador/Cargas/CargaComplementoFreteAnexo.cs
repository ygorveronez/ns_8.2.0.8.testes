namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_COMPLEMENTO_FRETE_ANEXOS", EntityName = "CargaComplementoFreteAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaComplementoFreteAnexo", NameType = typeof(CargaComplementoFreteAnexo))]
    public class CargaComplementoFreteAnexo : Anexo.Anexo<CargaComplementoFrete>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaComplementoFrete", Column = "CCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaComplementoFrete EntidadeAnexo { get; set; }

        #endregion
    }
}
