namespace Dominio.Entidades.Embarcador.Anexo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_ARQUIVO_ANEXO", EntityName = "ControleArquivoAnexo", Name = "Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo", NameType = typeof(ControleArquivoAnexo))]
    public class ControleArquivoAnexo : Anexo.Anexo<ControleArquivo>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleArquivo", Column = "COA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ControleArquivo EntidadeAnexo { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHQ_REALIZOU_DOWNLOAD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouDownload { get; set; }

    }
}
