namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_CONTROLE_DOCUMENTO", EntityName = "TermoQuitacaoControleDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumento", NameType = typeof(TermoQuitacaoControleDocumento))]
    public class TermoQuitacaoControleDocumento : Anexo.Anexo<Financeiro.TermoQuitacaoFinanceiro>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Financeiro.TermoQuitacaoFinanceiro EntidadeAnexo { get; set; }

        #endregion

        #region Propriedades Próprias

        public virtual string ObterCaminho()
        {
            return Utilidades.IO.FileStorageService.Storage.Combine("TermoQuitacaoFinanceiro", "ControleDocumentos");
        }

        #endregion
    }
}
