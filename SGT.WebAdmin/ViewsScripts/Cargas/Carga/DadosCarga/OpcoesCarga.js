// #region Funções Públicas

function configurarOpcoesVisiveisPorCarga(knoutCarga, carga) {
    configurarOpcoesDetalhesVisiveisPorCarga(knoutCarga);
    configurarOpcoesAcoesVisiveisPorCarga(knoutCarga, carga);
}

// #endregion Funções Públicas

// #region Funções Privadas

function configurarOpcoesDetalhesVisiveisPorCarga(knoutCarga) {
    knoutCarga.AuditarJanelaCarregamento.visible(knoutCarga.CodigoJanelaCarregamento.val() > 0);
    knoutCarga.AuditarJanelaDescarregamento.visible(knoutCarga.CodigoJanelaDescarregamento.val() > 0);
    knoutCarga.AuditarCargaVeiculoContainer.visible(knoutCarga.CodigoCargaVeiculoContainer.val() > 0);
}

function configurarOpcoesAcoesVisiveisPorCarga(knoutCarga, carga) {
    const cancelarCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS &&
        (knoutCarga.CargaPortoPortoTimelineHabilitado.val() || carga.DadosIntercab.CargaPortaPorta || knoutCarga.CargaSVMProprioTimelineHabilitado.val() || ((knoutCarga.TipoServicoCarga.val() == EnumTipoServicoCarga.Feeder) &&
            carga.DadosIntercab.HabilitarTimelineCargaParaFeeder));
    const consultarPacotes = carga.TipoOperacao.PermiteConsultarPorPacotesLoggi && (knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.Nova || knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe);
    const imprimirMinuta = carga.TipoOperacao.ImprimirMinuta;

    knoutCarga.CancelarCarga.visible(cancelarCarga);
    knoutCarga.ConsultarPacotes.visible(consultarPacotes);
    knoutCarga.ImprimirMinuta.visible(imprimirMinuta);
}

// #endregion Funções Privadas
