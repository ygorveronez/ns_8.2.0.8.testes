



function AuditoriaFluxoColetaEntrega() {
    if (!PermiteAuditar())
        $("body").addClass("auditoria-ocultar");
}

function auditarFluxoColetaEntregaClick(e, sender) {
    var data = { Codigo: e.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("FluxoColetaEntrega", null, e);

    closureAuditoria(data);
}