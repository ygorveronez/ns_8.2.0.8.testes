$(document).ready(function () {
    $("#txtValorDocumentoOutrosRemetente").priceFormat({ prefix: '' });
    $("#txtNumeroOutrosRemetente").mask("9?99999999999");

    FormatarCampoDate("txtDataEmissaoOutrosRemetente");
    
    $("#btnSalvarOutrosRemetente").click(function () {
        if (ValidarOutrosDocumentosRemetente()) {

            executarRest("/DocumentosCTE/VerificarSeJaOutrosDocumentos?callback=?", { Numero: $("#txtNumeroOutrosRemetente").val(), CodigoCTe: $("#hddCodigoCTE").val() }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.NumerosCTeUtilizados != null && r.Objeto.NumerosCTeUtilizados.length > 0) {
                        var msg = "Este documento já foi utilizado no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 110px; width: 420px; overflow-y: scroll; overflow-x: hidden;'>";
                        for (var i = 0; i < r.Objeto.NumerosCTeUtilizados.length; i++) {
                            msg += "<b>&bull; " + r.Objeto.NumerosCTeUtilizados[i] + "</b><br/>";
                        }
                        msg += "</div><br/>Deseja utilizá-lo assim mesmo?";
                        jConfirm(msg, "Atenção", function (ret) {
                            if (ret) {
                                SalvarOutrosDocumentosRemetente();
                            }
                        });
                    } else {
                        SalvarOutrosDocumentosRemetente();
                    }
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }        
    });

    $("#btnExcluirOutrosRemetente").click(function () {
        ExcluirOutrosDocumentosRemetente();
    });
    $("#btnCancelarOutrosRemetente").click(function () {
        LimparCamposOutrosDocumentosRemetente();
    });
});
function LimparCamposOutrosDocumentosRemetente() {
    $("#hddOutroDocumentoRemetenteEmEdicao").val("0");
    $("#ddlTipoDocumentoOutrosRemetente").val($("#ddlTipoDocumentoOutrosRemetente option:first").val());
    $("#txtDescricaoOutrosRemetente").val("");
    $("#txtNumeroOutrosRemetente").val("");
    $("#txtDataEmissaoOutrosRemetente").val("");
    $("#txtValorDocumentoOutrosRemetente").val("0,00");
    $("#btnExcluirOutrosRemetente").hide();
}
function ValidarOutrosDocumentosRemetente() {
    var descricao = $("#txtDescricaoOutrosRemetente").val();
    var numero = Globalize.parseInt($("#txtNumeroOutrosRemetente").val());
    var valor = Globalize.parseFloat($("#txtValorDocumentoOutrosRemetente").val());
    var dataEmissao = $("#txtDataEmissaoOutrosRemetente").val();
    var valido = true;
    if (descricao != "") {
        CampoSemErro("#txtDescricaoOutrosRemetente");
    } else {
        CampoComErro("#txtDescricaoOutrosRemetente");
        valido = false;
    }
    if (numero > 0) {
        CampoSemErro("#txtNumeroOutrosRemetente");
    } else {
        CampoComErro("#txtNumeroOutrosRemetente");
        valido = false;
    }
    if (dataEmissao != "") {
        CampoSemErro("#txtDataEmissaoOutrosRemetente");
    } else {
        CampoComErro("#txtDataEmissaoOutrosRemetente");
        valido = false;
    }
    if (valor > 0) {
        CampoSemErro("#txtValorDocumentoOutrosRemetente");
    } else {
        CampoComErro("#txtValorDocumentoOutrosRemetente");
        valido = false;
    }
    return valido;
}
function EditarOutrosDocumentosRemetente(outroDocumento) {
    $("#hddOutroDocumentoRemetenteEmEdicao").val(outroDocumento.Codigo);
    $("#ddlTipoDocumentoOutrosRemetente").val(outroDocumento.Modelo);
    $("#txtDescricaoOutrosRemetente").val(outroDocumento.Descricao);
    $("#txtNumeroOutrosRemetente").val(outroDocumento.Numero);
    $("#txtDataEmissaoOutrosRemetente").val(outroDocumento.DataEmissao);
    $("#txtValorDocumentoOutrosRemetente").val(Globalize.format(outroDocumento.ValorTotal, "n2"));
    $("#btnExcluirOutrosRemetente").show();
}
function SalvarOutrosDocumentosRemetente() {
        var outroDocumento = {
            Codigo: Globalize.parseInt($("#hddOutroDocumentoRemetenteEmEdicao").val()),
            Modelo: Globalize.parseInt($("#ddlTipoDocumentoOutrosRemetente").val()),
            DescricaoModelo: $("#ddlTipoDocumentoOutrosRemetente :selected").text(),
            Descricao: $("#txtDescricaoOutrosRemetente").val().toUpperCase(),
            Numero: $("#txtNumeroOutrosRemetente").val(),
            DataEmissao: $("#txtDataEmissaoOutrosRemetente").val(),
            ValorTotal: Globalize.parseFloat($("#txtValorDocumentoOutrosRemetente").val()),
            Excluir: false
        };
        var outrosDocumentos = $("#hddOutrosDocumentosRemetente").val() == "" ? new Array() : JSON.parse($("#hddOutrosDocumentosRemetente").val());
        if (outroDocumento.Codigo == 0)
            outroDocumento.Codigo = -(outrosDocumentos.length + 1);
        if (outrosDocumentos.length > 0) {
            for (var i = 0; i < outrosDocumentos.length; i++) {
                if (outrosDocumentos[i].Codigo == outroDocumento.Codigo) {
                    outrosDocumentos.splice(i, 1);
                    break;
                }
            }
        }
        outrosDocumentos.push(outroDocumento);
        outrosDocumentos.sort();
        $("#hddOutrosDocumentosRemetente").val(JSON.stringify(outrosDocumentos));
        RenderizarOutrosDocumentosRemetente();
        LimparCamposOutrosDocumentosRemetente();
        AtualizarValorTotalDaCargaOD();
        //BuscarFretePorValor();
}

function ExcluirOutrosDocumentosRemetente() {
    jConfirm("Deseja realmente excluir este documento?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddOutroDocumentoRemetenteEmEdicao").val());
            var outrosDocumentos = $("#hddOutrosDocumentosRemetente").val() == "" ? new Array() : JSON.parse($("#hddOutrosDocumentosRemetente").val());
            for (var i = 0; i < outrosDocumentos.length; i++) {
                if (outrosDocumentos[i].Codigo == codigo) {
                    if (codigo > 0) {
                        outrosDocumentos[i].Excluir = true;
                    } else {
                        outrosDocumentos.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddOutrosDocumentosRemetente").val(JSON.stringify(outrosDocumentos));
            RenderizarOutrosDocumentosRemetente();
            LimparCamposOutrosDocumentosRemetente();
            AtualizarValorTotalDaCargaOD();
            //BuscarFretePorValor();
        }
    });
}
function RenderizarOutrosDocumentosRemetente() {
    $("#tblOutrosRemetente tbody").html("");
    var outrosDocumentosRemetente = $("#hddOutrosDocumentosRemetente").val() == "" ? new Array() : JSON.parse($("#hddOutrosDocumentosRemetente").val());
    if (outrosDocumentosRemetente.length > 0) {
        for (var i = 0; i < outrosDocumentosRemetente.length; i++) {
            if (!outrosDocumentosRemetente[i].Excluir)
                $("#tblOutrosRemetente tbody").append("<tr>" +
                    "<td>" + outrosDocumentosRemetente[i].DescricaoModelo + "</td>" +
                    "<td class=\"text-uppercase\">" + outrosDocumentosRemetente[i].Descricao + "</td>" +
                    "<td>" + outrosDocumentosRemetente[i].Numero + "</td>" +
                    "<td>" + outrosDocumentosRemetente[i].DataEmissao + "</td>" +
                    "<td>" + Globalize.format(outrosDocumentosRemetente[i].ValorTotal, "n2") + "</td>" +
                    "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarOutrosDocumentosRemetente(" + JSON.stringify(outrosDocumentosRemetente[i]) + ")'>Editar</button></td>" +
                "</tr>");
        }
    }
    if ($("#tblOutrosRemetente tbody").html() == "") {
        $("#tblOutrosRemetente tbody").html("<tr><td colspan='6'>Nenhum registro encontrado!</td></tr>");
    }
}
function AtualizarValorTotalDaCargaOD() {
    var NotasFiscaisRemetente = $("#hddOutrosDocumentosRemetente").val() == "" ? new Array() : JSON.parse($("#hddOutrosDocumentosRemetente").val());
    var valorTotalCarga = 0;
    for (var i = 0; i < NotasFiscaisRemetente.length; i++)
        if (!NotasFiscaisRemetente[i].Excluir && NotasFiscaisRemetente[i].ValorTotal > 0)
            valorTotalCarga += NotasFiscaisRemetente[i].ValorTotal;

    var valorTotal = Globalize.format(valorTotalCarga, "n2");

    $("#txtValorTotalCarga").val(valorTotal);
    $("#txtValorCargaAverbacao").val(valorTotal);
}