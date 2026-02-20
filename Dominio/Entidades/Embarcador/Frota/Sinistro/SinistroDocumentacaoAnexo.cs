namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_DOCUMENTACAO_ANEXO", EntityName = "SinistroDocumentacaoAnexo", Name = "Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoAnexo", NameType = typeof(SinistroDocumentacaoAnexo))]
    public class SinistroDocumentacaoAnexo : Anexo.Anexo<SinistroDados>
    {
        #region Propriedades Sobrescritas
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroDados", Column = "SDS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override SinistroDados EntidadeAnexo { get; set; }

        #endregion
    }
}
