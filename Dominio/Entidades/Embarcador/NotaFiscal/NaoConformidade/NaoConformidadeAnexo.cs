namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NAO_CONFORMIDADE_ANEXO", EntityName = "NaoConformidadeAnexo", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo", NameType = typeof(NaoConformidadeAnexo))]
    public class NaoConformidadeAnexo : Anexo.Anexo<NaoConformidade>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaoConformidade", Column = "NCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override NaoConformidade EntidadeAnexo { get; set; }

        #endregion
    }
}
