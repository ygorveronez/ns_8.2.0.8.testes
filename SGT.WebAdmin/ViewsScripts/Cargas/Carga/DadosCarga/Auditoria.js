/// <reference path="../../../../js/Global/Auditoria.js" />

/*
 * Declaração das Funções Públicas
 */

function AuditoriaCarga() {
    if (!PermiteAuditar())
        $("body").addClass("auditoria-ocultar");
}

function exibirAuditoriaCargaClick(carga) {
    var data = { Codigo: carga.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("Carga", null, carga);
    closureAuditoria(data);
}

function exibirAuditoriaJanelaCarregamentoClick(carga) {
    var data = { Codigo: carga.CodigoJanelaCarregamento.val() };
    var closureAuditoria = OpcaoAuditoria("CargaJanelaCarregamento", null);

    closureAuditoria(data);
}

function exibirAuditoriaJanelaDescarregamentoClick(carga) {
    var data = { Codigo: carga.CodigoJanelaDescarregamento.val() };
    var closureAuditoria = OpcaoAuditoria("CargaJanelaDescarregamento", null);
    closureAuditoria(data);
}

function exibirAuditoriaCargaVeiculoContainerClick(carga) {
    var data = 0;

    if (carga.CodigoCargaVeiculoContainer.val() > 0)
        data = { Codigo: carga.CodigoCargaVeiculoContainer.val() };
    else if (carga.Codigo.val() > 0)
        data = { Codigo: carga.Codigo.val() };

    var closureAuditoria = OpcaoAuditoria("CargaVeiculoContainer", null);
    closureAuditoria(data);
}
