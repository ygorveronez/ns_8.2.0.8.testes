function BuscaEstados(idSelect) {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {
            RenderizarEstados(r.Objeto, idSelect);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarEstados(ufs, idSelect) {
    var selUFs = document.getElementById(idSelect);
    selUFs.options.length = 0;
    for (var i = 0; i < ufs.length; i++) {
        var optn = document.createElement("option");
        optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
        optn.value = ufs[i].Sigla;
        selUFs.options.add(optn);
    }
}

function AdicionarEstadoBloqueado() {

    var estadosBloqueados = $("body").data("estadosBloqueados") == null ? new Array() : $("body").data("estadosBloqueados");

    var siglaUF = $("#selEstadoBloqueado").val();

    var estado = {
        SiglaUF: siglaUF,
        Excluir: false
    }

    estadosBloqueados.push(estado);

    $("body").data("estadosBloqueados", estadosBloqueados);

    $("#selEstadoBloqueado").val($("#selEstadoBloqueado option:first").val());

    RenderizarEstadosBloqueados();
}

function RenderizarEstadosBloqueados() {
    var estadosBloqueados = $("body").data("estadosBloqueados") == null ? new Array() : $("body").data("estadosBloqueados");

    $("#tblEstadosBloqueados tbody").html("");

    for (var i = 0; i < estadosBloqueados.length; i++) {
        if (!estadosBloqueados[i].Excluir)
            $("#tblEstadosBloqueados tbody").append("<tr><td>" + estadosBloqueados[i].SiglaUF + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirEstadoBloqueado(" + JSON.stringify(estadosBloqueados[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblEstadosBloqueados tbody").html() == "")
        $("#tblEstadosBloqueados tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirEstadoBloqueado(estado) {
    var estadosBloqueados = $("body").data("estadosBloqueados") == null ? new Array() : $("body").data("estadosBloqueados");

    for (var i = 0; i < estadosBloqueados.length; i++) {
        if (estadosBloqueados[i].SiglaUF == estado.SiglaUF) {
            if (estado.SiglaUF == "")
                estadosBloqueados.splice(i, 1);
            else
                estadosBloqueados[i].Excluir = true;
            break;
        }
    }

    $("body").data("estadosBloqueados", estadosBloqueados);

    RenderizarEstadosBloqueados();
}

function ValidarEstadoBloqueado() {
    var estadosBloqueados = $("body").data("estadosBloqueados") == null ? new Array() : $("body").data("estadosBloqueados");
    var estadoBloqueado = $("#selEstadoBloqueado").val();
    var valido = true;

    for (var i = 0; i < estadosBloqueados.length; i++) {
        if (estadosBloqueados[i].SiglaUF == estadoBloqueado && estadosBloqueados[i].Excluir == false) {
            CampoComErro("#selEstadoBloqueado");
            jAlert("Estado já foi adicionado!", "Atenção!");
            valido = false;
            break;
        } else {
            CampoSemErro("#selEstadoBloqueado");
        }
    }

    return valido;
}