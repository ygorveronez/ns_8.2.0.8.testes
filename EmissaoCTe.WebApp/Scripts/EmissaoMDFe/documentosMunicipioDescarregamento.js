$(document).ready(function () {
    $("#btnBuscarCTeMunicipioDescarregamento").click(function () {
        AbrirTelaConsultaCTes(RetornoConsultaCTeMunicipioDescarregamento, true);
    });

    $("#btnSalvarDocumentoMunicipioDescarregamento").click(function () {
        SalvarDocumentoMunicipioDescarregamento();
    });

    $("#btnExcluirDocumentoMunicipioDescarregamento").click(function () {
        ExcluirDocumentoMunicipioDescarregamento();
    });

    $("#btnCancelarDocumentoMunicipioDescarregamento").click(function () {
        LimparCamposDocumentoMunicipioDescarregamento();
    });

    $("#btnSalvarAlteracoesDocumentosMunicipioDescarregamento").click(function () {
        SalvarAlteracoesDocumentosMunicipioDescarregamento();
    });

    $("#btnCancelarAlteracoesDocumentosMunicipioDescarregamento").click(function () {
        if ($("body").data("documentoMunicipioDescarregamentoAlterado") != null && $("body").data("documentoMunicipioDescarregamentoAlterado") == true) {
            jConfirm("Deseja realmente descartar as alterações realizadas nos documentos deste município?", "Atenção!", function (r) {
                if (r)
                    FecharTelaDocumentosMunicipioDescarregamento();
            });
        } else {
            FecharTelaDocumentosMunicipioDescarregamento();
        }
    });

    $("#btnFecharTelaDocumentosMunicipioDescarregamento").click(function () {
        $("#btnCancelarAlteracoesDocumentosMunicipioDescarregamento").trigger("click");
    });
});

function AbrirTelaDocumentosMunicipioDescarregamento(municipio) {
    LimparCamposDocumentoMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoDocumento", municipio);

    $("body").data("ufConsultaCTe", $("#selUFDescarregamento").val());

    $("body").data("ufCarregamentoConsultaCTe", $("#selUFCarregamento").val());

    $("body").data("documentoMunicipioDescarregamentoAlterado", false);

    RenderizarDocumentosMunicipioDescarregamento();

    $("#tituloMunicipioDescarregamento").text("Município: " + municipio.DescricaoMunicipio);

    $("#divDocumentosMunicipioDescarregamento").modal({ keyboard: false, backdrop: 'static' });
}

function RetornoConsultaCTeMunicipioDescarregamento(cte) {
    FecharTelaConsultaCTes();
    cte.data.ValorFrete = Globalize.parseFloat(cte.data.ValorFrete);
    $("body").data("cteSelecionadoMunicipioDescarregamento", cte.data);
    $("#txtCTeMunicipioDescarregamento").val(cte.data.Numero);
}

function ValidarCamposDocumentoMunicipioDescarregamento() {
    var cte = $("body").data("cteSelecionadoMunicipioDescarregamento");
    var valido = true;

    if (cte != null) {
        CampoSemErro("#txtCTeMunicipioDescarregamento");
    } else {
        CampoComErro("#txtCTeMunicipioDescarregamento");
        valido = false;
    }

    return valido;
}

function SalvarDocumentoMunicipioDescarregamento() {
    if (ValidarCamposDocumentoMunicipioDescarregamento()) {
        $("body").data("documentoMunicipioDescarregamentoAlterado", true);

        var documento = {
            Codigo: $("body").data("municipioDescarregamentoDocumentoEmEdicao") != null ? $("body").data("municipioDescarregamentoDocumentoEmEdicao").Codigo : 0,
            CTe: $("body").data("cteSelecionadoMunicipioDescarregamento"),
            Excluir: false
        };

        var municipioDescarregamento = $("body").data("municipioDescarregamentoDocumento");

        for (var i = 0; i < municipioDescarregamento.Documentos.length; i++) {
            if (municipioDescarregamento.Documentos[i].CTe.Codigo == documento.CTe.Codigo && municipioDescarregamento.Documentos[i].Codigo != documento.Codigo && municipioDescarregamento.Documentos[i].Excluir == false) {
                ExibirMensagemAlerta("Este CT-e já foi utilizado.", "Atenção!", "placeholder-msgDocumentosMunicipioDescarregamento");
                return;
            }
        }

        municipioDescarregamento.Documentos.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (documento.Codigo == 0)
            documento.Codigo = (municipioDescarregamento.Documentos.length > 0 ? (municipioDescarregamento.Documentos[0].Codigo > 0 ? -1 : (municipioDescarregamento.Documentos[0].Codigo - 1)) : -1);

        for (var i = 0; i < municipioDescarregamento.Documentos.length; i++) {
            if (municipioDescarregamento.Documentos[i].Codigo == documento.Codigo) {
                municipioDescarregamento.Documentos.splice(i, 1);
                break;
            }
        }

        municipioDescarregamento.Documentos.push(documento);

        municipioDescarregamento.Documentos.sort(function (a, b) { return a.CTe.Numero < b.CTe.Numero ? -1 : 1; });

        $("body").data("municipioDescarregamentoDocumento", municipioDescarregamento);

        RenderizarDocumentosMunicipioDescarregamento();
        LimparCamposDocumentoMunicipioDescarregamento();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgDocumentosMunicipioDescarregamento");
    }
}

function EditarDocumentoMunicipioDescarregamento(documento) {
    $("body").data("municipioDescarregamentoDocumentoEmEdicao", documento);
    $("body").data("cteSelecionadoMunicipioDescarregamento", documento.CTe);
    $("#txtCTeMunicipioDescarregamento").val(documento.CTe.Numero);
    $("#btnExcluirDocumentoMunicipioDescarregamento").show();
}

function ExcluirDocumentoMunicipioDescarregamento() {
    $("body").data("documentoMunicipioDescarregamentoAlterado", true);

    var documento = $("body").data("municipioDescarregamentoDocumentoEmEdicao");

    var municipioDescarregamento = $("body").data("municipioDescarregamentoDocumento");

    for (var i = 0; i < municipioDescarregamento.Documentos.length; i++) {
        if (municipioDescarregamento.Documentos[i].Codigo == documento.Codigo) {
            if (documento.Codigo <= 0)
                municipioDescarregamento.Documentos.splice(i, 1);
            else
                municipioDescarregamento.Documentos[i].Excluir = true;
            break;
        }
    }

    $("body").data("municipioDescarregamentoDocumento", municipioDescarregamento);

    RenderizarDocumentosMunicipioDescarregamento();
    LimparCamposDocumentoMunicipioDescarregamento();
}

function SalvarProdutosPerigososDocumento(index, produtos) {
    var documentosMunicipio = $("body").data("municipioDescarregamentoDocumento");

    if (typeof documentosMunicipio.Documentos == "undefined")
        return;

    documentosMunicipio.Documentos[index].CTe.ProdutosPerigosos = produtos;

    $("body").data("municipioDescarregamentoDocumento", documentosMunicipio);
}

function RenderizarDocumentosMunicipioDescarregamento() {
    var listInfo = $("body").data("municipioDescarregamentoDocumento");
    var $table = $("#tblDocumentosMunicipioDescarregamento");
    var $tbody = $table.find("tbody");
    var $rows = [];

    if (listInfo != null && typeof listInfo.Documentos != "undefined")
        listInfo = listInfo.Documentos;
    else
        listInfo = [];

    $tbody.html("");

    listInfo.forEach(function (info, i) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.CTe.Numero + "</td>" +
                "<td>" + Globalize.format(info.CTe.ValorFrete, "n2") + "</td>" +
                (
                    $('body').hasClass('mdfe-100') ?

                    // MDFe 1.0
                    ("<td>" + 
                        "<button type='button' class='btn btn-default btn-xs btn-block editar'>Editar</button>" +
                    "</td>") :

                    // MDFe 3.0 (habilita botao de inserir produtos perigosos)
                    ("<td class='btns-municipios-descarregamento'>" +
                        "<button class='btn btn-default btn-xs editar' type='button' title='Editar'><span class='glyphicon glyphicon-edit'></span></button>" +
                        "<button class='btn btn-default btn-xs produtos-perigosos' type='button' title='Produtos Perigosos'><span class='glyphicon glyphicon-alert'></span></button>" +
                    "</td>")
                ) +
            "</tr>");

            $row.on("click", ".editar", function () {
                EditarDocumentoMunicipioDescarregamento(info);
            });

            $row.on("click", ".produtos-perigosos", function () {
                AbrirTelaProdutosPerigososCTe(info, function (cte, produtos) {
                    SalvarProdutosPerigososDocumento(i, produtos);
                });
            });

            $rows.push($row);
        }
    });

    $tbody.append.apply($tbody, $rows);

    if ($tbody.find("tr").length == 0)
        $tbody.html("<tr><td colspan='" + $table.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposDocumentoMunicipioDescarregamento() {
    $("body").data("municipioDescarregamentoDocumentoEmEdicao", null);
    $("body").data("cteSelecionadoMunicipioDescarregamento", null);
    $("#txtCTeMunicipioDescarregamento").val('');
    $("#btnExcluirDocumentoMunicipioDescarregamento").hide();
}

function SalvarAlteracoesDocumentosMunicipioDescarregamento() {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoDocumento");
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento");

    for (var i = 0; i < municipiosDescarregamento.length; i++) {
        if (municipiosDescarregamento[i].Codigo == municipioDescarregamento.Codigo) {
            municipiosDescarregamento[i] = municipioDescarregamento;
            break;
        }
    }

    $("body").data("municipiosDescarregamento", municipiosDescarregamento);

    RenderizarMunicipiosDescarregamento();

    FecharTelaDocumentosMunicipioDescarregamento();

    AtualizarTotais();
}

function FecharTelaDocumentosMunicipioDescarregamento() {
    LimparCamposDocumentoMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoDocumento", null);

    $("body").data("documentoMunicipioDescarregamentoAlterado", false);

    $("#tituloMunicipioDescarregamento").text('');

    $("#divDocumentosMunicipioDescarregamento").modal('hide');
}

function AtualizarTotais() {
    var municipios = $("body").data("municipiosDescarregamento") == null ? new Array() : $("body").data("municipiosDescarregamento");
    var valorTotalMercadoria = 0, pesoTotal = 0, pesoKgTotal = 0;

    for (var i = 0; i < municipios.length; i++) {
        for (var j = 0; j < municipios[i].Documentos.length; j++) {
            if (!municipios[i].Documentos[j].Excluir) {
                valorTotalMercadoria += municipios[i].Documentos[j].CTe.ValorTotalMercadoria;
                pesoTotal += municipios[i].Documentos[j].CTe.PesoTotal;
                pesoKgTotal += municipios[i].Documentos[j].CTe.PesoKgTotal;
            }
        }
    }
    $("#txtValorTotal").val(Globalize.format(valorTotalMercadoria, "n2"));
    if (pesoKgTotal != null && pesoKgTotal > 0)
        $("#txtPesoBruto").val(Globalize.format(pesoKgTotal, "n4"));
    else
        $("#txtPesoBruto").val(Globalize.format(pesoTotal, "n4"));

    ConfigurarComponentesDeInfPagamento();
}