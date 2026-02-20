function carregarModalMotoristaLGPD() {
    var $divModal = $("#modalConsentimentoMotorista");
    
    if ($divModal.length == 0) {
        executarReST("Motorista/ObterUrlRegularizacaoOneTrust", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    $.get("Content/Static/LGPD/ModalConsentimentoMotorista.html" + "?dyn=" + guid(), function (data) {
                        data = data.replace("#link-motorista-regularizacao", arg.Data.Url);
                        $("#widget-grid").after(data);
                    }).done(function () {
                        abrirModalMotoristaLGPD();
                    });
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    }
    else
        abrirModalMotoristaLGPD();
}

function abrirModalMotoristaLGPD() {
    Global.abrirModal('modalConsentimentoMotorista');
}