/// <reference path="../../../js/Global/Auditoria.js" />

/*
 * Declaração das Funções Públicas
 */

function AuditoriaMonitoramento() {
    if (!PermiteAuditar())
        $("body").addClass("auditoria-ocultar");
}

function exibirAuditoriaMonitoramentoClick(monitoramento) {
    console.log(monitoramento);
    var data = { Codigo: monitoramento.Codigo };
    var closureAuditoria = OpcaoAuditoria("Monitoramento", null, monitoramento);
    closureAuditoria(data);
}
