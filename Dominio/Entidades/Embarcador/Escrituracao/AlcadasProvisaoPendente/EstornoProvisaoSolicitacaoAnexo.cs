namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESTORNO_PROVISAO_SOLICITACAO_ANEXO", EntityName = "EstornoProvisaoSolicitacaoAnexo", Name = "Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo", NameType = typeof(EstornoProvisaoSolicitacaoAnexo))]
    public class EstornoProvisaoSolicitacaoAnexo : Anexo.Anexo<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EstornoProvisaoSolicitacao", Column = "EPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao EntidadeAnexo { get; set; }

        #endregion

    }
}
