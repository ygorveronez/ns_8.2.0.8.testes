namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST_ANEXOS", EntityName = "CargaJanelaCarregamentoTransportadorChecklistAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklistAnexo", NameType = typeof(CargaJanelaCarregamentoTransportadorChecklistAnexo))]
    public class CargaJanelaCarregamentoTransportadorChecklistAnexo : Anexo.Anexo<CargaJanelaCarregamentoTransportadorChecklist>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportadorChecklist", Column = "CTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaJanelaCarregamentoTransportadorChecklist EntidadeAnexo { get; set; }

        #endregion
    }
}