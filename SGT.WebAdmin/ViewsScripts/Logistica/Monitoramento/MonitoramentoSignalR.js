/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />


function loadConexaoSignalRMonitoramento() {
    SignalRMonitoramentoInformarListaMonitoramentoAtualizada = AtualizarListaMonitoramentoSignalR;
}

function AtualizarListaMonitoramentoSignalR({ Monitoramentos }) {
    const moArray = Array.prototype.slice.call(_gridMonitoramento.GridViewTable().data());

    if (moArray.length <= 0 || Monitoramentos.length <= 0)
        return;

    console.log("SignalR Monitoramento ativo.");

    for (let i = 0; i < Monitoramentos.length; i++) {
        const subRow = moArray.find((mrow) => mrow.CargaEmbarcador === Monitoramentos[i].CargaEmbarcador);

        if (!subRow) continue;

        _gridMonitoramento.AtualizarDataRow(`#${subRow.DT_RowId}`, Monitoramentos[i]);
    }
    
} 