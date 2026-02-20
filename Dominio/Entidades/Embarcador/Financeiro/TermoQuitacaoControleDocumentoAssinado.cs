namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_CONTROLE_DOCUMENTO_ASSINADO", EntityName = "TermoQuitacaoControleDocumentoAssinado", Name = "Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado", NameType = typeof(TermoQuitacaoControleDocumentoAssinado))]
    public class TermoQuitacaoControleDocumentoAssinado : Anexo.Anexo<Financeiro.TermoQuitacaoFinanceiro>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Financeiro.TermoQuitacaoFinanceiro EntidadeAnexo { get; set; }

        #endregion
    }
}
