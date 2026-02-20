var countArquivosNOTFIS = 0;
var utilizaNovaImportacaoEDI = false;
var notasFiscaisImportadas;
$(document).ready(function () {
    CarregarConfiguracoesEDI();
    
    CarregarConsultaDeFretesPorValor("btnBuscarTabelaImportacaoEDI", "btnBuscarTabelaImportacaoEDI", "A", RetornoConsultaFreteImportacaoEDI, true, false);
    CarregarConsultaDeVeiculos("btnBuscarVeiculoTracaoNotfis", "btnBuscarVeiculoTracaoNotfis", RetornoConsultaVeiculoTracaoNotfis, true, false, "0");
    CarregarConsultaDeVeiculos("btnBuscarVeiculoReboqueNotfis", "btnBuscarVeiculoReboqueNotfis", RetornoConsultaVeiculoReboqueNotfis, true, false, "1");
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotoristaNotfis", RetornoConsultaMotoristaNotfis, true, false);

    $("#txtValorFreteNotfis").priceFormat();
    $("#txtValorPedagioNotfis").priceFormat();
    $("#txtPercentualGrisNotfis").priceFormat();
    $("#txtPercentualAdvalorem").priceFormat();

    $("#txtTabelaImportacaoEDI").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#selLayoutNOTFIS").change(function () {
        InicializarPlUploadNOTFIS();
    });

    $("#btnImportarNOTFIS").click(function () {
        AbrirDivUploadArquivosNOTFIS();
    });

    $("#txtValorFreteNotfis, #txtValorPedagioNotfis, #txtPercentualGrisNotfis, #txtPercentualAdvalorem,#chkOpcaoNotfisDigitacao,#chkOpcaoNotfisIncluirICMS").focusout(function () {
        InicializarPlUploadNOTFIS();
    });

    $("#chkOpcaoNotfisDigitacao").prop('checked', true);
    $("#chkOpcaoNotfisIncluirICMS").prop('checked', true);
});

function RetornoConsultaVeiculoTracaoNotfis(veiculo) {
    $("#hddVeiculoTracaoNotfis").val(veiculo.Codigo);
    $("#txtVeiculoTracaoNotfis").val(veiculo.Placa);
    InicializarPlUploadNOTFIS();
}

function RetornoConsultaVeiculoReboqueNotfis(veiculo) {
    $("#hddVeiculoReboqueNotfis").val(veiculo.Codigo);
    $("#txtVeiculoReboqueNotfis").val(veiculo.Placa);
    InicializarPlUploadNOTFIS();
}

function RetornoConsultaMotoristaNotfis(motorista) {
    $("#hddMotoristaNotfis").val(motorista.Codigo);
    $("#txtMotoristaNotfis").val(motorista.CPFCNPJ + " - " + motorista.Nome);
    InicializarPlUploadNOTFIS();
}

function CarregarConfiguracoesEDI() {
    executarRest("/LayoutEDI/ConsultarTipoImportacaoEDI?callback=?", {}, function (r) {
        if (r.Sucesso) {

            if (r.Objeto == true) {
                utilizaNovaImportacaoEDI = true;
                $("#divAgrupamentoNotis").show();
                $("#divTabelaFreteNOTFIS").hide();
                $("#divAvancadasNOTFIS").hide();
            }
            else {
                utilizaNovaImportacaoEDI = false;
                $("#divAgrupamentoNotis").hide();
                $("#divTabelaFreteNOTFIS").show();
                $("#divAvancadasNOTFIS").show();
            }
            CarregarLayoutsEDI();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarLayoutsEDI() {
    executarRest("/LayoutEDI/BuscarTodosPorTipo?callback=?", { TipoLayout: utilizaNovaImportacaoEDI == true ? 12 : 2 }, function (r) {
        if (r.Sucesso) {

            var selLayoutNOTFIS = document.getElementById("selLayoutNOTFIS");

            for (var i = 0; i < r.Objeto.length; i++) {

                var option = document.createElement("option");

                option.text = r.Objeto[i].Descricao;
                option.value = r.Objeto[i].Codigo;

                selLayoutNOTFIS.add(option);
            }

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function InicializarPlUploadNOTFIS() {
    countArquivosNOTFIS = 0;
    notasFiscaisImportadas = new Array();

    $("#divUploadArquivoNOTFISBody").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: path + '/ConhecimentoDeTransporteEletronico/GerarCTePorNOTFIS?callback=?&CodigoLayout=' + $("#selLayoutNOTFIS").val() +
                                                                                    '&CodigoTabelaFreteValor=' + $("#hddCodigoTabelaFreteImportacaoEDI").val() +
                                                                                    '&ValorFrete=' + $("#txtValorFreteNotfis").val() +
                                                                                    '&ValorPedagio=' + $("#txtValorPedagioNotfis").val() +
                                                                                    '&PercentualGris=' + $("#txtPercentualGrisNotfis").val() +
                                                                                    '&PercentualAdValorem=' + $("#txtPercentualAdvalorem").val() +
                                                                                    '&CodigoVeiculoTracao=' + $("#hddVeiculoTracaoNotfis").val() +
                                                                                    '&CodigoVeiculoReboque=' + $("#hddVeiculoReboqueNotfis").val() +
                                                                                    '&CodigoMotorista=' + $("#hddMotoristaNotfis").val() +
                                                                                    '&ManterDigitacao=' + $("#chkOpcaoNotfisDigitacao")[0].checked +
                                                                                    '&IncluirICMS=' + $("#chkOpcaoNotfisIncluirICMS")[0].checked,
        max_file_size: '2000kb',
        unique_names: true,
        //filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FilesAdded: function (up, files) {
                countArquivosNOTFIS += files.length;
                if (utilizaNovaImportacaoEDI == false) {
                    if (countArquivosNOTFIS > 1) {
                        $(".plupload_start").css("display", "none");
                        jAlert('O sistema só permite enviar um arquivo por vez. Remova os demais!', 'Atenção');
                    }
                }
            },
            FilesRemoved: function (up, files) {
                countArquivosNOTFIS -= files.length;
                if (countArquivosNOTFIS <= 1) {
                    $(".plupload_start").css("display", "");
                }
            },
            FileUploaded: function (up, file, info) {
                var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                if (retorno.Sucesso) {
                    if (utilizaNovaImportacaoEDI == false) {
                        if ($("#chkOpcaoNotfisDigitacao")[0].checked) {
                            jAlert("O NOTFIS foi importado com sucesso!<br/><br/>Documentos gerados estão em digitação.", "Sucesso", function () {
                                AtualizarGridCTes();
                                FecharDivUploadArquivosNOTFIS();
                            });
                        }
                        else {
                            jAlert("O NOTFIS foi importado com sucesso!<br/><br/>Documentos gerados com sucesso.", "Sucesso", function () {
                                AtualizarGridCTes();
                                FecharDivUploadArquivosNOTFIS();
                            });
                        }
                    }
                    else {
                        if (retorno.Objeto.length == undefined)
                            notasFiscaisImportadas.push(retorno.Objeto);
                        else
                            for (var i = 0; i < retorno.Objeto.length; i++) {
                                notasFiscaisImportadas.push(retorno.Objeto[i])
                            }
                    }
                } else {
                    file.status = plupload.FAILED;
                    jAlert(retorno.Erro, "Falha no Envio", function () {
                        FecharDivUploadArquivosNOTFIS();
                    });
                    up.trigger('UploadProgress', file);
                }
            },
            StateChanged: function (up) {
                if (up.state == plupload.STOPPED) {
                    // Fecha o modal de upload independe do retorno ou de eventuais erros
                    FecharDivUploadArquivosNOTFIS();

                    if (utilizaNovaImportacaoEDI == true) 
                        ValidarNotasFiscaisEDI();
                }
            }
        }
    });
}
function AbrirDivUploadArquivosNOTFIS() {
    LimparCamposImportacaoEDI();
    InicializarPlUploadNOTFIS();
    $('#divUploadArquivoNOTFIS').modal("show");
}
function FecharDivUploadArquivosNOTFIS() {
    $('#divUploadArquivoNOTFIS').modal("hide");
}
function RetornoConsultaFreteImportacaoEDI(frete) {
    $("#hddCodigoTabelaFreteImportacaoEDI").val(frete.Codigo);
    $("#txtTabelaImportacaoEDI").val(frete.Destino + " - " + frete.ValorMinimoFrete);
    InicializarPlUploadNOTFIS();
}

function LimparCamposImportacaoEDI() {
    $("#hddCodigoTabelaFreteImportacaoEDI").val("");
    $("#txtTabelaImportacaoEDI").val("");

    $("#hddVeiculoTracaoNotfis").val("");
    $("#hddVeiculoReboqueNotfis").val("");
    $("#hddMotoristaNotfis").val("");
    $("#txtVeiculoTracaoNotfis").val("");
    $("#txtVeiculoReboqueNotfis").val("");
    $("#txtMotoristaNotfis").val("");
    $("#txtValorFreteNotfis").val("0.00");
    $("#txtValorPedagioNotfis").val("0.00");
    $("#txtPercentualGrisNotfis").val("0.00");
    $("#txtPercentualAdvalorem").val("0.00");
}

function ValidarNotasFiscaisEDI() {
    if (notasFiscaisImportadas.length > 0) {
        var cpfCnpjRemetente = notasFiscaisImportadas[0].Remetente;
        var cpfCnpjDestinatario = notasFiscaisImportadas[0].Destinatario;
        var countDestinatario = 0;
        var countRemetente = 0;
        var notasFiscaisJaUtilizadas = new Array();
        for (var i = 0; i < notasFiscaisImportadas.length; i++) {
            if (cpfCnpjRemetente != notasFiscaisImportadas[i].Remetente)
                countRemetente += 1;
            if (cpfCnpjDestinatario != notasFiscaisImportadas[i].Destinatario)
                countDestinatario += 1;
            if (notasFiscaisImportadas[i].NumeroDosCTesUtilizados != undefined && notasFiscaisImportadas[i].NumeroDosCTesUtilizados.length > 0)
                notasFiscaisJaUtilizadas.push(notasFiscaisImportadas[i]);
        }

        if (notasFiscaisJaUtilizadas.length > 0)
            ExibirMensagemNotasFiscaisEmUsoNOTFIS(notasFiscaisJaUtilizadas);
        else
            GerarCTesEDI();

    } else {
        jAlert("Não é possível prosseguir pois não conseguimos importar nenhuma nota fiscal.", "Atenção");
    }
}

function ExibirMensagemNotasFiscaisEmUsoNOTFIS(notas) {
    var msg = "As seguintes notas já foram utilizadas no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
    for (var i = 0; i < notas.length; i++) {
        msg += "<br/><b>" + notas[i].Numero + (notas[i].Chave != null ? " - " +  notas[i].Chave : "")  + "</b>:<br/>";
        for (var j = 0; j < notas[i].NumeroDosCTesUtilizados.length; j++)
            msg += " &bull; " + notas[i].NumeroDosCTesUtilizados[j] + "<br/>";
    }
    msg += "</div><br/>Deseja importar o EDI assim mesmo?<br/>";
    jConfirm(msg, "Atenção", function (ret) {
        if (ret) {
            GerarCTesEDI();
        }
    });
}

function GerarCTesEDI() {
    var agruparRemetente = false;
    var agruparDestinatario = false;
    var agruparDT = false;

    if ($("#selAgrupamentoNOTFIS").val() == "0") {
        agruparRemetente = true;
        agruparDestinatario = true;
        agruparDT = false;
    }
    if ($("#selAgrupamentoNOTFIS").val() == "1") {
        agruparRemetente = true;
        agruparDestinatario = false;
        agruparDT = false;
    }
    if ($("#selAgrupamentoNOTFIS").val() == "2") {
        agruparRemetente = false;
        agruparDestinatario = true;
        agruparDT = false;
    }

    if ($("#selAgrupamentoNOTFIS").val() == "4") {
        agruparRemetente = false;
        agruparDestinatario = false;
        agruparDT = true;
    }

    var dados = {
        NFes: JSON.stringify(notasFiscaisImportadas),
        AgruparRemetente: agruparRemetente,
        AgruparDestinatario: agruparDestinatario,
        AgruparDT: agruparDT,
        CodigoTabelaFreteValor: $("#hddCodigoTabelaFreteImportacaoXML").val(),
        ManterDigitacao: $("#chkOpcaoXMLNFesDigitacao")[0].checked
    };

    executarRest("/ConhecimentoDeTransporteEletronico/SalvarCTePorXMLNFe?callback=?", dados, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("CTes salvos com sucesso!", "Sucesso!");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
        AtualizarGridCTes();
    });
}