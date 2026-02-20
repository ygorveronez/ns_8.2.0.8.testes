$(document).ready(function () {
    $("#txtChaveNFeMunicipioDescarregamento").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtSegCodBarraNFeMunicipioDescarregamento").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999");

    $("#btnSalvarNFeMunicipioDescarregamento").click(function () {
        SalvarNFeMunicipioDescarregamento();
    });

    $("#btnExcluirNFeMunicipioDescarregamento").click(function () {
        ExcluirNFeMunicipioDescarregamento();
    });

    $("#btnCancelarNFeMunicipioDescarregamento").click(function () {
        LimparCamposNFeMunicipioDescarregamento();
    });

    $("#btnSalvarAlteracoesNFesMunicipioDescarregamento").click(function () {
        SalvarAlteracoesNFesMunicipioDescarregamento();
    });

    $("#btnImportarDocumentoMunicipioDescarregamento").click(function () {
        AbrirTelaImportarDocumentoMunicipioDescarregamento();
    });

    $("#btnCancelarAlteracoesNFesMunicipioDescarregamento").click(function () {
        if ($("body").data("NFeMunicipioDescarregamentoAlterado") != null && $("body").data("NFeMunicipioDescarregamentoAlterado") == true) {
            jConfirm("Deseja realmente descartar as alterações realizadas nas NF-es deste município?", "Atenção!", function (r) {
                if (r)
                    FecharTelaNFesMunicipioDescarregamento();
            });
        } else {
            FecharTelaNFesMunicipioDescarregamento();
        }
    });

    $("#btnFecharTelaNFesMunicipioDescarregamento").click(function () {
        $("#btnCancelarAlteracoesNFesMunicipioDescarregamento").trigger("click");
    });
});

function AbrirTelaNFesMunicipioDescarregamento(municipio) {
    LimparCamposNFeMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoNFe", municipio);

    $("body").data("nfeMunicipioDescarregamentoAlterado", false);

    RenderizarNFesMunicipioDescarregamento();

    $("#tituloMunicipioDescarregamentoNFe").text("Município: " + municipio.DescricaoMunicipio);

    $("#divNFesMunicipioDescarregamento").modal({ keyboard: false, backdrop: 'static' });
}

function RetornoNFeDescarregamento() {
    FecharUploadXMLNFeDescarregamento();
    var validouChavesExistentes = true; 
    var strValidacaoChaveExistente = "As notas a seguir já foram utilizadas em um ou mais MDF-es: <br/><br/>";
    for (var i = 0; i < notasFiscaisImportadas.length; i++) {
        if (notasFiscaisImportadas[i].MDFes != null && notasFiscaisImportadas[i].MDFes.length > 0) {
            validouChavesExistentes = false;
            strValidacaoChaveExistente += "<b>&bull; " + notasFiscaisImportadas[i].Chave + ":</b> ";

            for (var x = 0; x < notasFiscaisImportadas[i].MDFes.length; x++)
                strValidacaoChaveExistente += notasFiscaisImportadas[i].MDFes[x] + ", ";

            strValidacaoChaveExistente = strValidacaoChaveExistente.slice(0, strValidacaoChaveExistente.length - 2);
            strValidacaoChaveExistente += "<br/>";
        }
    }

    strValidacaoChaveExistente += "<br/>Deseja continuar assim mesmo?";

    if (!validouChavesExistentes) {
        jConfirm(strValidacaoChaveExistente, "Atenção!", function (r) {
            if (r)
                ContinuarRetornoNFeDescarregamento();
        });
    } else {
        ContinuarRetornoNFeDescarregamento();
    }


}

function ContinuarRetornoNFeDescarregamento(){

    for (var n in notasFiscaisImportadas) {
        n = notasFiscaisImportadas[n];

        InsereNFeMunicipioDescarregamento({
            Codigo: 0,
            Chave: n.Chave.replace(/[^0-9]/g, ''),
            SegundoCodigoDeBarra: '',
            Excluir: false
        });
    }

    RenderizarNFesMunicipioDescarregamento();
    notasFiscaisImportadas = [];
}


function AbrirTelaImportarDocumentoMunicipioDescarregamento() {
    AbrirUploadXMLNFeDescarregamento();
}
function AbrirUploadXMLNFeDescarregamento() {
    InicializarPlUploadNFe(RetornoNFeDescarregamento);
    $("#tituloUploadArquivos").text("Importação de XML de Notas Fiscais Eletrônicas");
    $('#divUploadArquivos').modal("show");
}

function FecharUploadXMLNFeDescarregamento() {
    $('#divUploadArquivos').modal("hide");
}

function ValidarCamposNFeMunicipioDescarregamento() {
    var chave = $("#txtChaveNFeMunicipioDescarregamento").val().replace(/[^0-9]/g, '');
    var valido = true;

    if (chave != null && chave.length == 44) {
        CampoSemErro("#txtChaveNFeMunicipioDescarregamento");
    } else {
        CampoComErro("#txtChaveNFeMunicipioDescarregamento");
        valido = false;
    }

    return valido;
}

function InsereNFeMunicipioDescarregamento(nfe) {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoNFe");

    for (var i = 0; i < municipioDescarregamento.NFes.length; i++) {
        if (municipioDescarregamento.NFes[i].Chave == nfe.Chave && municipioDescarregamento.NFes[i].Codigo != nfe.Codigo && municipioDescarregamento.NFes[i].Excluir == false) {
            ExibirMensagemAlerta("A NF-e " + nfe.Chave + " já foi utilizada.", "Atenção!", "placeholder-msgNFesMunicipioDescarregamento");
            return;
        }
    }

    municipioDescarregamento.NFes.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

    if (nfe.Codigo == 0)
        nfe.Codigo = (municipioDescarregamento.NFes.length > 0 ? (municipioDescarregamento.NFes[0].Codigo > 0 ? -1 : (municipioDescarregamento.NFes[0].Codigo - 1)) : -1);

    for (var i = 0; i < municipioDescarregamento.NFes.length; i++) {
        if (municipioDescarregamento.NFes[i].Codigo == nfe.Codigo) {
            municipioDescarregamento.NFes.splice(i, 1);
            break;
        }
    }

    municipioDescarregamento.NFes.push(nfe);

    municipioDescarregamento.NFes.sort(function (a, b) { return a.Chave < b.Chave ? -1 : 1; });

    $("body").data("municipioDescarregamentoNFe", municipioDescarregamento);
}
function SalvarNFeMunicipioDescarregamento() {
    if (ValidarCamposNFeMunicipioDescarregamento()) {
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/VerificarSeJaUtilizouNFe?callback=?", { ChaveNFe: $("#txtChaveNFeMunicipioDescarregamento").val().replace(/\s/g, ""), CodigoMunicipioDescarregamento: $("body").data("municipioDescarregamentoNFeEmEdicao") != null ? $("body").data("municipioDescarregamentoNFeEmEdicao").Codigo : 0 }, function (r) {
            if (r.Sucesso) {
                if (r.Objeto.MDFes != null && r.Objeto.MDFes.length > 0) {
                    var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) MDF-e(s): <br/><br/><div style='max-height: 110px; width: 420px; overflow-y: scroll; overflow-x: hidden;'>";
                    for (var i = 0; i < r.Objeto.MDFes.length; i++) {
                        msg += "<b>&bull; " + r.Objeto.MDFes[i] + "</b><br/>";
                    }
                    msg += "</div><br/>Deseja utilizá-la assim mesmo?";
                    jConfirm(msg, "Atenção", function (ret) {
                        if (ret) {
                            SalvarNFeMDFe();
                        }
                    });
                } else {
                    SalvarNFeMDFe();
                }
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgNFesMunicipioDescarregamento");
    }
}

function SalvarNFeMDFe() {
    $("body").data("NFeMunicipioDescarregamentoAlterado", true);

    var nfe = {
        Codigo: $("body").data("municipioDescarregamentoNFeEmEdicao") != null ? $("body").data("municipioDescarregamentoNFeEmEdicao").Codigo : 0,
        Chave: $("#txtChaveNFeMunicipioDescarregamento").val().replace(/[^0-9]/g, ''),
        SegundoCodigoDeBarra: $("#txtSegCodBarraNFeMunicipioDescarregamento").val().replace(/[^0-9]/g, ''),
        Excluir: false
    };

    InsereNFeMunicipioDescarregamento(nfe);

    RenderizarNFesMunicipioDescarregamento();
    LimparCamposNFeMunicipioDescarregamento();
}

function EditarNFeMunicipioDescarregamento(nfe) {
    $("body").data("municipioDescarregamentoNFeEmEdicao", nfe);
    $("#txtChaveNFeMunicipioDescarregamento").val(nfe.Chave).trigger("blur");
    $("#txtSegCodBarraNFeMunicipioDescarregamento").val(nfe.SegundoCodigoDeBarra).trigger("blur");
    $("#btnExcluirNFeMunicipioDescarregamento").show();
}

function ExcluirNFeMunicipioDescarregamento() {
    $("body").data("NFeMunicipioDescarregamentoAlterado", true);

    var nfe = $("body").data("municipioDescarregamentoNFeEmEdicao");

    var municipioDescarregamento = $("body").data("municipioDescarregamentoNFe");

    for (var i = 0; i < municipioDescarregamento.NFes.length; i++) {
        if (municipioDescarregamento.NFes[i].Codigo == nfe.Codigo) {
            if (nfe.Codigo <= 0)
                municipioDescarregamento.NFes.splice(i, 1);
            else
                municipioDescarregamento.NFes[i].Excluir = true;
            break;
        }
    }

    $("body").data("municipioDescarregamentoNFe", municipioDescarregamento);

    RenderizarNFesMunicipioDescarregamento();
    LimparCamposNFeMunicipioDescarregamento();
}

function RenderizarNFesMunicipioDescarregamento() {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoNFe");

    $("#tblNFesMunicipioDescarregamento tbody").html("");

    if (municipioDescarregamento != null && municipioDescarregamento.NFes != null) {
        for (var i = 0; i < municipioDescarregamento.NFes.length; i++) {
            if (!municipioDescarregamento.NFes[i].Excluir) {
                var chaveECodigoDeBarra = municipioDescarregamento.NFes[i].Chave;

                if (municipioDescarregamento.NFes[i].SegundoCodigoDeBarra != "")
                    chaveECodigoDeBarra += " - " + municipioDescarregamento.NFes[i].SegundoCodigoDeBarra;

                $("#tblNFesMunicipioDescarregamento tbody").append("<tr><td>" + chaveECodigoDeBarra + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarNFeMunicipioDescarregamento(" + JSON.stringify(municipioDescarregamento.NFes[i]) + ")'>Editar</button></td></tr>");
            }
        }
    }

    if ($("#tblNFesMunicipioDescarregamento tbody").html() == "")
        $("#tblNFesMunicipioDescarregamento tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposNFeMunicipioDescarregamento() {
    $("body").data("municipioDescarregamentoNFeEmEdicao", null);
    $("#txtChaveNFeMunicipioDescarregamento").val('');
    $("#txtSegCodBarraNFeMunicipioDescarregamento").val('');
    $("#btnExcluirNFeMunicipioDescarregamento").hide();
}

function SalvarAlteracoesNFesMunicipioDescarregamento() {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoNFe");
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento");

    for (var i = 0; i < municipiosDescarregamento.length; i++) {
        if (municipiosDescarregamento[i].Codigo == municipioDescarregamento.Codigo) {
            municipiosDescarregamento[i] = municipioDescarregamento;
            break;
        }
    }

    $("body").data("municipiosDescarregamento", municipiosDescarregamento);

    RenderizarMunicipiosDescarregamento();

    FecharTelaNFesMunicipioDescarregamento();
}

function FecharTelaNFesMunicipioDescarregamento() {
    LimparCamposNFeMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoNFe", null);

    $("body").data("NFeMunicipioDescarregamentoAlterado", false);

    $("#tituloMunicipioDescarregamentoNFe").text('');

    $("#divNFesMunicipioDescarregamento").modal('hide');
}