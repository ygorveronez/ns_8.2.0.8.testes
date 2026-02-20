namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_FINANCEIRO_ANEXO", EntityName = "TermoQuitacaoFinanceiroAnexo", Name = "Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiroAnexo", NameType = typeof(TermoQuitacaoFinanceiroAnexo))]
    public class TermoQuitacaoFinanceiroAnexo : Anexo.Anexo<TermoQuitacaoFinanceiro>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TermoQuitacaoFinanceiro EntidadeAnexo { get; set; }

        #endregion
    }
}