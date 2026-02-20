$(document).ready(function () {
    $("#txtChaveNFeRemetente").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtValorTotalNFeRemetente").priceFormat({ prefix: '' });
    $("#txtPesoNFeRemetente").priceFormat({ prefix: '' });

    FormatarCampoDate("txtDataEmissaoNFeRemetente");

    $("#btnSalvarNFeRemetente").click(function () {
        if (ValidarNFeRemetente()) {
            executarRest("/DocumentosCTE/VerificarSeJaUtilizouNFe?callback=?", { ChaveNFe: $("#txtChaveNFeRemetente").val().replace(/\s/g, ""), CodigoCTe: $("#hddCodigoCTE").val() }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.NumerosCTeUtilizados != null && r.Objeto.NumerosCTeUtilizados.length > 0) {
                        var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 110px; width: 420px; overflow-y: scroll; overflow-x: hidden;'>";
                        for (var i = 0; i < r.Objeto.NumerosCTeUtilizados.length; i++) {
                            msg += "<b>&bull; " + r.Objeto.NumerosCTeUtilizados[i] + "</b><br/>";
                        }
                        msg += "</div><br/>Deseja utilizá-la assim mesmo?";
                        jConfirm(msg, "Atenção", function (ret) { 
                            if (ret) {
                                SalvarNFeRemetente();
                            }
                        });
                    } else {
                        SalvarNFeRemetente();
                    }
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
    });
    $("#btnExcluirNFeRemetente").click(function () {
        ExcluirNFeRemetente();
    });
    $("#btnCancelarNFeRemetente").click(function () {
        LimparCamposNFeRemetente();
    });
    CarregarConsultaDeNotasFiscaisEletronicas("txtChaveNFeRemetente", "btnBuscarNFeRemetente", RetornoConsultaNFesRemetente, true, false);
});
function RetornoConsultaNFesRemetente(nfe) {
    executarRest("/DocumentosCTE/VerificarSeJaUtilizouNFe?callback=?", { ChaveNFe: nfe.Chave.replace(/\s/g, ""), CodigoCTe: $("#hddCodigoCTE").val() }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.NumerosCTeUtilizados != null && r.Objeto.NumerosCTeUtilizados.length > 0) {
                var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 110px; width: 420px; overflow-y: scroll; overflow-x: hidden;'>";
                for (var i = 0; i < r.Objeto.NumerosCTeUtilizados.length; i++) {
                    msg += "<b>&bull; " + r.Objeto.NumerosCTeUtilizados[i] + "</b><br/>";
                }
                msg += "</div><br/>Deseja utilizá-la assim mesmo?";
                jConfirm(msg, "Atenção", function (ret) {
                    if (ret)
                        SalvarNFeRemetenteConsulta(nfe);
                });
            } else {
                SalvarNFeRemetenteConsulta(nfe);
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}
function SalvarNFeRemetenteConsulta(nfe) {
    $("#txtCPFCNPJRemetente").val(nfe.CPF_CNPJ_Emitente);
    $("#txtCPFCNPJDestinatario").val(nfe.CPF_CNPJ_Destinatario);
    BuscarRemetente(true);
    BuscarDestinatario(true);
    $("#txtPlacaVeiculo").val(nfe.Placa);
    AdicionarVeiculo();
    $("#txtChaveNFeRemetente").val(nfe.Chave);
    $("#txtDataEmissaoNFeRemetente").val(nfe.DataEmissao);
    $("#txtValorTotalNFeRemetente").val(nfe.Valor);
    SalvarNFeRemetente();
    AdicionarPesoNFeRemetente(nfe.Peso);
    SetarTomadorXML(nfe.FormaPagamento);
}
function LimparCamposNFeRemetente() {
    $("#hddNotaFiscalEletronicaRemetenteEmEdicao").val("0");
    $("#txtChaveNFeRemetente").val("");
    $("#txtValorTotalNFeRemetente").val("0,00");
    $("#txtPesoNFeRemetente").val('0,00');
    $("#btnExcluirNFeRemetente").hide();
}
function ValidarNFeRemetente() {
    var chave = $("#txtChaveNFeRemetente").val();
    var data = $("#txtDataEmissaoNFeRemetente").val();

    var valido = true;

    if (chave != "") {
        CampoSemErro("#txtChaveNFeRemetente");
    } else {
        CampoComErro("#txtChaveNFeRemetente");
        valido = false;
    }

    if (data != "") {
        CampoSemErro("#txtDataEmissaoNFeRemetente");
    } else {
        CampoComErro("#txtDataEmissaoNFeRemetente");
        valido = false;
    }

    if (parseInt($("#hddNotaFiscalEletronicaRemetenteEmEdicao").val()) == 0 && !VerificarSeChaveJaExiste()) {
        valido = false;
        jAlert("Esta NF-e já foi inserida. Caso for necessário, edite a mesma para alterar as informações.", "Atenção")
    }

    return valido;
}
function VerificarSeChaveJaExiste() {
    var codigo = Globalize.parseInt($("#hddNotaFiscalEletronicaRemetenteEmEdicao").val());
    var chave = $("#txtChaveNFeRemetente").val().replace(/[^0-9]/g, '');
    var nfes = $("#hddNotasFiscaisEletronicasRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val());
    
    if (nfes.length > 0) {
        for (var i = 0; i < nfes.length; i++)
            if (nfes[i].Chave == chave && !nfes[i].Excluir) return false;
    }

    return true;
}
function EditarNFeRemetente(nfe) {
    $("#hddNotaFiscalEletronicaRemetenteEmEdicao").val(nfe.Codigo);
    $("#txtChaveNFeRemetente").val(nfe.Chave).trigger("blur");
    $("#txtChaveNFeRemetente").data('RemetenteUF', nfe.RemetenteUF);
    $("#txtChaveNFeRemetente").data('DestinatarioUF', nfe.DestinatarioUF);
    $("#txtDataEmissaoNFeRemetente").val(nfe.DataEmissao);
    $("#txtValorTotalNFeRemetente").val(Globalize.format(nfe.ValorTotal, "n2"));
    $("#txtPesoNFeRemetente").val(Globalize.format(nfe.Peso, "n2"));
    $("#btnExcluirNFeRemetente").show();
}
function SalvarNFeRemetente() {
    if (ValidarNFeRemetente()) {
        var NFe = {
            Codigo: Globalize.parseInt($("#hddNotaFiscalEletronicaRemetenteEmEdicao").val()),
            Numero: Globalize.parseInt($("#txtChaveNFeRemetente").val().replace(/\s/g, "").substring(25, 34)),
            Remetente: $("#hddRemetente").val(),
            Chave: $("#txtChaveNFeRemetente").val(),
            RemetenteUF: $("#txtChaveNFeRemetente").data('RemetenteUF'),
            DestinatarioUF: $("#txtChaveNFeRemetente").data('DestinatarioUF'),
            DataEmissao: $("#txtDataEmissaoNFeRemetente").val(),
            ValorTotal: Globalize.parseFloat($("#txtValorTotalNFeRemetente").val()),
            Peso: Globalize.parseFloat($("#txtPesoNFeRemetente").val()),
            Excluir: false
        };
        AdicionarNFeRemetente(NFe);
        RenderizarNFesRemetente();
        LimparCamposNFeRemetente();
        AtualizarValorTotalDaCarga();
        //BuscarFretePorValor();
    }
}
function ExcluirNFeRemetente() {
    jConfirm("Deseja realmente excluir esta NFe?", "Atenção", function (r) {
        if (r) {
            RemoverNFeRemetente($("#hddNotaFiscalEletronicaRemetenteEmEdicao").val());
            RenderizarNFesRemetente();
            LimparCamposNFeRemetente();
            AtualizarValorTotalDaCarga();
            //BuscarFretePorValor();
        }
    });
}
function AdicionarNFeRemetente(nfe) {
    var NotasFiscaisRemetente = $("#hddNotasFiscaisEletronicasRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val());

    if (nfe.Codigo == 0)
        nfe.Codigo = BuscaMenorProximoCodigo(NotasFiscaisRemetente, "Codigo");
    
    if (NotasFiscaisRemetente.length > 0) {
        for (var i = 0; i < NotasFiscaisRemetente.length; i++) {
            if (NotasFiscaisRemetente[i].Codigo == nfe.Codigo) {
                NotasFiscaisRemetente.splice(i, 1);
                break;
            }
        }
    }

    // Tira masacara da chave
    nfe.Chave = nfe.Chave.replace(/[^0-9]/g, '');

    // Coloca o excluir
    nfe.Excluir = typeof nfe.Excluir == "undefined" ? false : nfe.Excluir;

    NotasFiscaisRemetente.push(nfe);   
    //NotasFiscaisRemetente.sort(function (a, b) { return Globalize.parseInt(a.Numero) - Globalize.parseInt(b.Numero) }); //Erro quando adiciona nota depois de ter importado do sefaz

    $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(NotasFiscaisRemetente));
}
function RemoverNFeRemetente(codigoNFe) {
    var NotasFiscaisRemetente = JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val());
    if (NotasFiscaisRemetente.length > 0) {
        for (var i = 0; i < NotasFiscaisRemetente.length; i++) {
            if (NotasFiscaisRemetente[i].Codigo == codigoNFe) {
                if (NotasFiscaisRemetente[i].Codigo < 0)
                    NotasFiscaisRemetente.splice(i, 1);
                else
                    NotasFiscaisRemetente[i].Excluir = true;
                break;
            }
        }
    }
    $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(NotasFiscaisRemetente));
}
function RenderizarNFesRemetente() {
    $("#tblNFesRemetente tbody").html("");
    var NotasFiscaisRemetente = $("#hddNotasFiscaisEletronicasRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val());
    if (NotasFiscaisRemetente.length > 0) {
        for (var i = 0; i < NotasFiscaisRemetente.length; i++) {
            if (!NotasFiscaisRemetente[i].Excluir)
                $("#tblNFesRemetente tbody").append(
                    "<tr>"
                        + "<td>" + NotasFiscaisRemetente[i].Numero + "</td>"
                        + "<td>" + NotasFiscaisRemetente[i].Chave.replace(/\s/g, '') + "</td>"
                        + "<td>" + (NotasFiscaisRemetente[i].RemetenteUF || "") + "</td>"
                        + "<td>" + (NotasFiscaisRemetente[i].DestinatarioUF || "") + "</td>"
                        + "<td>" + NotasFiscaisRemetente[i].DataEmissao + "</td>"
                        + "<td>" + Globalize.format(NotasFiscaisRemetente[i].ValorTotal, "n2") + "</td>"
                        + "<td>" + Globalize.format(NotasFiscaisRemetente[i].Peso, "n2") + "</td>"
                        + "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarNFeRemetente(" + JSON.stringify(NotasFiscaisRemetente[i]) + ");'>Editar</button></td>"
                    + "</tr>"
                );
        }
    }
    if ($("#tblNFesRemetente tbody").html() == "") {
        $("#tblNFesRemetente tbody").html("<tr><td colspan='5'>Nenhum registro encontrado!</td></tr>");
    }
}
function AtualizarValorTotalDaCarga() {
    var NotasFiscaisRemetente = $("#hddNotasFiscaisEletronicasRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisEletronicasRemetente").val());
    var valorTotalCarga = 0;
    for (var i = 0; i < NotasFiscaisRemetente.length; i++)
        if (!NotasFiscaisRemetente[i].Excluir && NotasFiscaisRemetente[i].ValorTotal > 0)
            valorTotalCarga += NotasFiscaisRemetente[i].ValorTotal;

    var valorTotal = Globalize.format(valorTotalCarga, "n2");

    $("#txtValorTotalCarga").val(valorTotal);
    $("#txtValorCargaAverbacao").val(valorTotal);
}
function AdicionarPesoNFeRemetente(peso) {
    if (peso > 0) {
        var informacaoQuantidade = {
            Id: 0,
            UnidadeMedida: 1,
            DescricaoUnidadeMedida: "KG",
            TipoUnidade: "KG",
            Quantidade: peso,
            Excluir: false
        };
        var infomacoesQuantidadeCarga = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());
        if (informacaoQuantidade.Id == 0) {
            informacaoQuantidade.Id = -(infomacoesQuantidadeCarga.length + 1);
        }
        var somou = false;
        for (var i = 0; i < infomacoesQuantidadeCarga.length; i++) {
            if (infomacoesQuantidadeCarga[i].UnidadeMedida == informacaoQuantidade.UnidadeMedida) {
                infomacoesQuantidadeCarga[i].Quantidade += informacaoQuantidade.Quantidade;
                somou = true;
                break;
            }
        }
        if (!somou)
            infomacoesQuantidadeCarga.push(informacaoQuantidade);
        $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(infomacoesQuantidadeCarga));
        RenderizarInformacaoQuantidadeCarga();
    }
}
function AdicionarVolumeNFeRemetente(volume) {
    if (volume > 0) {
        var informacaoQuantidade = {
            Id: 0,
            UnidadeMedida: 3,
            DescricaoUnidadeMedida: "UN",
            TipoUnidade: "UN",
            Quantidade: volume,
            Excluir: false
        };
        var infomacoesQuantidadeCarga = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());
        if (informacaoQuantidade.Id == 0) {
            informacaoQuantidade.Id = -(infomacoesQuantidadeCarga.length + 1);
        }
        var somou = false;
        for (var i = 0; i < infomacoesQuantidadeCarga.length; i++) {
            if (infomacoesQuantidadeCarga[i].UnidadeMedida == informacaoQuantidade.UnidadeMedida) {
                infomacoesQuantidadeCarga[i].Quantidade += informacaoQuantidade.Quantidade;
                somou = true;
                break;
            }
        }
        if (!somou)
            infomacoesQuantidadeCarga.push(informacaoQuantidade);
        $("#hddInformacoesQuantidadeCarga").val(JSON.stringify(infomacoesQuantidadeCarga));
        RenderizarInformacaoQuantidadeCarga();
    }
}