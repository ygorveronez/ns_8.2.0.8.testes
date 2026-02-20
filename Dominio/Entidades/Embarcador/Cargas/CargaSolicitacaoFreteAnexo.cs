namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_SOLICITACAO_FRETE_ANEXOS", EntityName = "CargaSolicitacaoFreteAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaSolicitacaoFreteAnexo", NameType = typeof(CargaSolicitacaoFreteAnexo))]
    public class CargaSolicitacaoFreteAnexo : Anexo.Anexo<Carga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga EntidadeAnexo { get; set; }

        #endregion
    }
}
