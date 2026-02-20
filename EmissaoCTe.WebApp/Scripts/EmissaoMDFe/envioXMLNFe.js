var errosEnvioXMLNFe, notasFiscaisImportadas;

function InicializarPlUploadNFe(onFinish) {
    if (!$.isFunction(onFinish))
        onFinish = ValidarNotasFiscais;

    errosEnvioXMLNFe = "";
    notasFiscaisImportadas = new Array();
    $("#divUploadArquivosBody").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: ObterPath() + '/ManifestoEletronicoDeDocumentosFiscais/ObterDadosXMLNFe?callback=?',
        max_file_size: '500kb',
        unique_names: true,
        filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FileUploaded: function (up, file, info) {
                var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                if (retorno.Sucesso) {
                    notasFiscaisImportadas.push(retorno.Objeto);
                } else {
                    errosEnvioXMLNFe += retorno.Erro + "<br />";
                }
            },
            StateChanged: function (up) {
                if (up.state != plupload.STARTED) {
                    if (errosEnvioXMLNFe != "")
                        jConfirm("Ocorreram as seguintes falhas no envio dos arquivos xml: <br /><br />" + errosEnvioXMLNFe + "<br />Deseja dar sequência à emissão com os arquivos importados?", "Atenção", function (r) {
                            if (r)
                                onFinish();
                            else
                                FecharUploadXMLNFe();
                        });
                    else
                        onFinish();
                }
            }
        }
    });
}
function ValidarNotasFiscais() {
    if (notasFiscaisImportadas.length > 0) {
        var ufCarregamento = notasFiscaisImportadas[0].UFCarregamento;
        var ufDescarregamento = notasFiscaisImportadas[0].UFDescarregamento;

        var ufCarregamentoDiferente = "", ufDescarregamentoDiferente = "", strValidacaoChaveExistente = "As notas a seguir já foram utilizadas em um ou mais MDF-es: <br/><br/>";
        var validouChavesExistentes = true;


        for (var i = 0; i < notasFiscaisImportadas.length; i++) {

            if (notasFiscaisImportadas[i].UFCarregamento != ufCarregamento)
                ufCarregamentoDiferente += notasFiscaisImportadas[i].UFCarregamento + + ", ";

            if (notasFiscaisImportadas[i].UFDescarregamento != ufDescarregamento)
                ufDescarregamentoDiferente += notasFiscaisImportadas[i].UFDescarregamento + ", ";

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

        if ((ufCarregamentoDiferente != null && ufCarregamentoDiferente != "") || (ufDescarregamentoDiferente != null && ufDescarregamentoDiferente != "")) {
            var mensagem = "Não é possível importar as notas fiscais selecionadas.<br/><br/>";

            mensagem += ufCarregamentoDiferente != null && ufCarregamentoDiferente != "" ? "Os estados de carregamento <b>" + ufCarregamentoDiferente.substring(0, (ufCarregamentoDiferente.length - 2)) + "</b> diferem do estado de carregamento selecionado para emissão (<b>" + ufCarregamento + "</b>).<br/>" : "";

            mensagem += ufDescarregamentoDiferente != null && ufDescarregamentoDiferente != "" ? "Os estados de descarregamento <b>" + ufDescarregamentoDiferente.substring(0, (ufDescarregamentoDiferente.length - 2)) + "</b> diferem do estado de descarregamento selecionado para emissão (<b>" + ufDescarregamento + "</b>).<br/>" : "";

            mensagem += "<br/>Selecione somente notas fiscais do mesmo estado de origem e destino para realizar a emissão do MDF-e."

            jAlert(mensagem, "Atenção!", function () {
                AbrirUploadXMLNFe();
            });
        } else if (!validouChavesExistentes) {
            jConfirm(strValidacaoChaveExistente, "Atenção!", function (r) {
                if (r)
                    AbrirMDFeComNotasFiscaisImportadas();
            });
        } else {
            AbrirMDFeComNotasFiscaisImportadas();
        }

    } else {
        jAlert("Não é possível prosseguir pois não conseguimos importar nenhuma nota fiscal.", "Atenção");
    }

    FecharUploadXMLNFe();
}

function ValidarNotasFiscaisOutrosMDFe() {
    if (notasFiscaisImportadas.length > 0) {
        var strValidacaoChaveExistente = "As notas a seguir já foram utilizadas em um ou mais MDF-es: <br/><br/>";
        var validouChavesExistentes = true;

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
                    AbrirMDFeComNotasFiscaisImportadas();
            });
        } else {
            AbrirMDFeComNotasFiscaisImportadas();
        }

    } else {
        jAlert("Não é possível prosseguir pois não conseguimos importar nenhuma nota fiscal.", "Atenção");
    }

    FecharUploadXMLNFe();
}


function AbrirMDFeComNotasFiscaisImportadas() {

    var pesoTotal = 0, valorTotal = 0, indice = 0;
    var listaMunicipiosCarregamento = new Array(), listaMunicipiosDescarregamento = new Array();
    var placaVeiculo = "";

    for (var i = 0; i < notasFiscaisImportadas.length; i++) {

        pesoTotal += notasFiscaisImportadas[i].PesoBrutoMercadoria;
        valorTotal += notasFiscaisImportadas[i].ValorTotalMercadoria;

        var notaFiscal = {
            Codigo: -(i + 1),
            Chave: notasFiscaisImportadas[i].Chave,
            SegundoCodigoDeBarra: "",
            Excluir: false
        };

        if (notasFiscaisImportadas[i].PlacaVeiculo != null && notasFiscaisImportadas[i].PlacaVeiculo != "")
            placaVeiculo = notasFiscaisImportadas[i].PlacaVeiculo;

        var existe = false;

        for (var j = 0; j < listaMunicipiosCarregamento.length; j++) {
            if (listaMunicipiosCarregamento[j].CodigoMunicipio == notasFiscaisImportadas[i].CodigoMunicipioCarregamento)
                existe = true;
        }

        if (!existe) {
            listaMunicipiosCarregamento.push({
                Codigo: -(i + 1),
                CodigoMunicipio: notasFiscaisImportadas[i].CodigoMunicipioCarregamento,
                DescricaoMunicipio: notasFiscaisImportadas[i].DescricaoMunicipioCarregamento,
                Excluir: false
            });
        }

        existe = false;

        for (indice = 0; indice < listaMunicipiosDescarregamento.length; indice++) {
            if (listaMunicipiosDescarregamento[indice].CodigoMunicipio == notasFiscaisImportadas[i].CodigoMunicipioDescarregamento) {
                existe = true;
                break;
            }
        }

        if (existe) {
            listaMunicipiosDescarregamento[indice].NFes.push(notaFiscal);
        } else {
            listaMunicipiosDescarregamento.push({
                Codigo: -(i + 1),
                CodigoMunicipio: notasFiscaisImportadas[i].CodigoMunicipioDescarregamento,
                DescricaoMunicipio: notasFiscaisImportadas[i].DescricaoMunicipioDescarregamento,
                Documentos: new Array(),
                NFes: [notaFiscal],
                Excluir: false
            });
        }

    }

    $("#selUFCarregamento").val(notasFiscaisImportadas[0].UFCarregamento).change();
    $("#selUFDescarregamento").val(notasFiscaisImportadas[0].UFDescarregamento).change();
    $("#txtPesoBruto").val(Globalize.format(pesoTotal, "n2"));
    $("#txtValorTotal").val(Globalize.format(valorTotal, "n2"));

    $("body").data("municipiosDescarregamento", listaMunicipiosDescarregamento);
    RenderizarMunicipiosDescarregamento();

    $("body").data("municipiosCarregamento", listaMunicipiosCarregamento);
    RenderizarMunicipiosCarregamento();

    ConsultarVeiculo(placaVeiculo);

}

function AbrirUploadXMLNFe() {
    InicializarPlUploadNFe();
    $("#tituloUploadArquivos").text("Importação de XML de Notas Fiscais Eletrônicas");
    $('#divUploadArquivos').modal("show");
}

function FecharUploadXMLNFe() {
    $('#divUploadArquivos').modal("hide");
}