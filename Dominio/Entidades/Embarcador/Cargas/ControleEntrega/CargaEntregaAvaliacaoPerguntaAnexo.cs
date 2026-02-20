namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_AVALIACAO_ANEXO", EntityName = "CargaEntregaAvaliacaoAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacaoAnexo", NameType = typeof(CargaEntregaAvaliacaoAnexo))]
    public class CargaEntregaAvaliacaoAnexo : Anexo.Anexo<CargaEntregaAvaliacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaAvaliacao", Column = "CEA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaEntregaAvaliacao EntidadeAnexo { get; set; }

        #endregion
    }
}
