namespace Dominio.Entidades.Embarcador.Logistica.TermoQuitacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_ANEXOS", EntityName = "TermoQuitacaoAnexo", Name = "Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacaoAnexo", NameType = typeof(TermoQuitacaoAnexo))]
    public class TermoQuitacaoAnexo : Anexo.Anexo<TermoQuitacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacao", Column = "TEQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TermoQuitacao EntidadeAnexo { get; set; }

        #endregion
    }
}
