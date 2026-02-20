var errosEnvioXMLNFe, notasFiscaisImportadas;

var EnumAgrupamentoNFeXML = {
    CTeUnico: "",
    PorRemetenteEDestinatario: 0,
    PorRemetente: 1,
    PorDestinatario: 2,
    CTePorNFe: 3,
    PorUFDestino: 4
};

$(document).ready(function () {
    $("#txtValorFreteImportacaoXML").priceFormat({ prefix: '' });
    $("#txtValorPedagioImportacaoXML").priceFormat({ prefix: '' });
    $("#txtValorAdcEntregaImportacaoXML").priceFormat({ prefix: '' });

    CarregarConsultaDeFretesPorValor("btnBuscarTabelaImportacaoXML", "btnBuscarTabelaImportacaoXML", "A", RetornoConsultaFreteImportacaoXML, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaImportacaoXML", "btnBuscarMotoristaImportacaoXML", "A", RetornoConsultaMotoristaImportacaoXML, true, false)
    CarregarConsultaDeVeiculos("btnBuscarVeiculoImportacaoXML", "btnBuscarVeiculoImportacaoXML", RetornoConsultaVeiculoImportacaoXML, true, false, 0);
    CarregarConsultaDeVeiculos("btnBuscarReboqueImportacaoXML", "btnBuscarReboqueImportacaoXML", RetornoConsultaReboqueImportacaoXML, true, false, 1);
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarSeguroImportacaoXML", "btnBuscarSeguroImportacaoXML", 0, RetornoConsultaSeguroImportacaoXML, true, false);
    CarregarConsultadeClientes("btnBuscarExpedidorImportacaoXML", "btnBuscarExpedidorImportacaoXML", RetornoConsultaExpedidorImportacaoXML, true, false);
    CarregarConsultadeClientes("btnBuscarRecebedorImportacaoXML", "btnBuscarRecebedorImportacaoXML", RetornoConsultaRecebedorImportacaoXML, true, false);

    RemoveConsulta("#txtMotoristaImportacaoXML, #txtVeiculoImportacaoXML, #txtTabelaImportacaoXML, #txtSeguroImportacaoXML, #txtExpedidorImportacaoXML, #txtRecebedorImportacaoXML", function ($this) {
        /**
         * Esse callback é executado quando o campo de busca é limpo (backspace, recortar, etc)
         * O $this é o input do texto
         * Mas como precisamos limpar o input hidden do filtro, buscaremos pelo id do campos
         * hddCodigoCAMPOImportacaoXML
         * Porém, o campo de tabela frete possui nome diferente do input hidden, então preciso
         * checar com o if quando é o mesmo
         */
        var campo = $this.attr('id').substr(3).replace("ImportacaoXML", '');
        if (campo == "Tabela") campo = "TabelaFrete";
        var id = "#hddCodigo" + campo + "ImportacaoXML";
        $this.val('');
        $(id).val('');
    });

    RemoveConsulta("#txtTabelaImportacaoXML", function ($this) {
        /**
         * Apenas para liberar o campo de valor frente
         */
        $("#txtValorFreteImportacaoXML").removeAttr('disabled');
    });

    $("#txtValorFreteImportacaoXML").keyup(function () {
        var valor = parseFloat($(this).val().replace(',', '.'));

        if (isNaN(valor) || valor == 0)
            $("#txtTabelaImportacaoXML, #btnBuscarTabelaImportacaoXML").removeAttr('disabled');
        else
            $("#txtTabelaImportacaoXML, #btnBuscarTabelaImportacaoXML").attr('disabled', 'disabled');
    });

    $("#btnImportarXMLNFe").click(function () {
        AbrirDivUploadArquivosXMLNFe();
    });

    $("#selAgrupamentoXML").change(function () {
        ExibirOcultarTabelaFreteValor();
    });

    $("#selTipoRateioImportacaoXML").change(function () {
        ControlarCampoPedagioXML();
    });

    ControlarCampoPedagioXML();
    $("#chkOpcaoXMLNFesDigitacao").prop('checked', true);
});

function ControlarCampoPedagioXML() {
    var tipo = $("#selTipoRateioImportacaoXML").val();

    //if (tipo == "6" || tipo == "7")
    //{
    $("#idPedagioImportacaoXML").show();
    $("#idAdcEntregaImportacaoXML").show();
    //}
    //else {
    //    $("#txtValorPedagioImportacaoXML").val("0,00")
    //    $("#idPedagioImportacaoXML").hide();

    //    $("#txtValorAdcEntregaImportacaoXML").val("0,00")
    //    $("#idAdcEntregaImportacaoXML").hide();
    //}
}

function InicializarPlUploadNFe() {
    errosEnvioXMLNFe = "";
    notasFiscaisImportadas = new Array();
    var arquivosInseridos = new Array();
    var removendoDuplicado = false;
    $("#divUploadArquivosBody").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: path + '/XMLNotaFiscalEletronica/ObterDocumentoPorXML?callback=?',
        max_file_size: '500kb',
        unique_names: true,
        filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FilesAdded: function (up, files) {
                //Valida informação do tomador
                if ($("#selTipoTomadorImportacaoXML").val() == "-1" && $("#selAgrupamentoXML").val() != "") {
                    CampoComErro("#selTipoTomadorImportacaoXML");
                    return jAlert("É obrigatório informar um Tomador.");
                }
                else
                    CampoSemErro("#selTipoTomadorImportacaoXML");

                // Essa funcao serve para impedir que o mesmo arquivo seja inserido de forma repetida
                // A verificacao e pelo nome do arquivo
                for (var f in files) {
                    var file = files[f];
                    if ($.inArray(file.name, arquivosInseridos) >= 0) {
                        // Porem, para nao impedir de que o primeiro arquivo nao seja mais possivel fazer upload
                        // Precisamos informar que estamos deletando um arquivo duplicado
                        removendoDuplicado = true;
                        up.removeFile(file);
                        removendoDuplicado = false;
                    } else {
                        arquivosInseridos.push(file.name);
                    }
                }
            },
            FilesRemoved: function (up, files) {
                // Quando cancelado o upload, devemos remover o nome do mesmo no array
                // Mas se estivermos removendo duplicad, nao removeremos do array
                if (removendoDuplicado) return;

                for (var f in files) {
                    var file = files[f];
                    var index = $.inArray(file.name, arquivosInseridos);

                    // Remove nome
                    arquivosInseridos.splice(index, 1);
                }
            },
            FileUploaded: function (up, file, info) {
                // Limpa retorno
                var dado = info.response.replace("?(", "").replace(/\);/g, "");
                var retorno = JSON.parse(dado);
                if (retorno.Sucesso) {
                    if (retorno.Objeto.length == undefined)
                        notasFiscaisImportadas.push(retorno.Objeto);
                    else
                        for (var i = 0; i < retorno.Objeto.length; i++) {
                            notasFiscaisImportadas.push(retorno.Objeto[i])
                        }
                } else {
                    errosEnvioXMLNFe += retorno.Erro + "<br />";
                }
            },
            StateChanged: function (up) {
                if (up.state == plupload.STOPPED) {

                    // Valida seleção de veículos
                    var reboque = $("#hddCodigoReboqueImportacaoXML").val();
                    var veiculo = $("#hddCodigoVeiculoImportacaoXML").val();

                    if (reboque > 0 && veiculo == 0) {
                        FecharDivUploadArquivosXMLNFe();
                        return jAlert("É obrigatório ter uma tração quando um reboque for selecionado.");
                    }

                    //Valida informação do tomador
                    if ($("#selTipoTomadorImportacaoXML").val() == "-1" && $("#selAgrupamentoXML").val() != "") {
                        FecharDivUploadArquivosXMLNFe();
                        CampoComErro("#selTipoTomadorImportacaoXML");
                        return jAlert("É obrigatório informar um Tomador.");
                    }
                    else
                        CampoSemErro("#selTipoTomadorImportacaoXML");

                    // Fecha o modal de upload independe do retorno ou de eventuais erros
                    FecharDivUploadArquivosXMLNFe();

                    if (errosEnvioXMLNFe != "")
                        jConfirm("Ocorreram as seguintes falhas no envio dos arquivos xml: <br /><br />" + errosEnvioXMLNFe + "<br />Deseja prosseguir com a emissão?", "Atenção", function (r) {
                            if (r) ValidarNotasFiscais();
                        });
                    else
                        ValidarNotasFiscais();
                }
            }
        }
    });
}
function ValidarNotasFiscais() {
    if (notasFiscaisImportadas.length > 0) {
        var cpfCnpjRemetente = notasFiscaisImportadas[0].Remetente;
        var cpfCnpjDestinatario = notasFiscaisImportadas[0].Destinatario;
        var ufsRemetentes = [];
        var ufsDestinatarios = [];
        var countDestinatario = 0;
        var countRemetente = 0;
        var notasFiscaisJaUtilizadas = new Array();
        for (var i = 0; i < notasFiscaisImportadas.length; i++) {

            if (notasFiscaisImportadas[i].NumeroDosCTesUtilizados.length > 0)
                notasFiscaisJaUtilizadas.push(notasFiscaisImportadas[i]);

            if (cpfCnpjRemetente != notasFiscaisImportadas[i].Remetente)
                countRemetente += 1;

            if (cpfCnpjDestinatario != notasFiscaisImportadas[i].Destinatario)
                countDestinatario += 1;

            if ($.inArray(notasFiscaisImportadas[i].RemetenteUF, ufsRemetentes) < 0)
                ufsRemetentes.push(notasFiscaisImportadas[i].RemetenteUF);

            if ($.inArray(notasFiscaisImportadas[i].DestinatarioUF, ufsDestinatarios) < 0)
                ufsDestinatarios.push(notasFiscaisImportadas[i].DestinatarioUF);
        }

        if (countDestinatario > 0 || countRemetente > 0 || ufsRemetentes.length > 1 || ufsDestinatarios.length > 1) {
            var distintos = [];

            if (countRemetente > 0)
                distintos.push("<b>" + countRemetente + " emitentes</b>");

            if (countDestinatario > 0)
                distintos.push("<b>" + countDestinatario + " destinatários</b>");

            if (ufsRemetentes.length > 1)
                distintos.push("<b>" + (ufsRemetentes.length) + " UFs emitentes</b>");

            if (ufsDestinatarios.length > 1)
                distintos.push("<b>" + (ufsDestinatarios.length) + " UFs destinatários</b>");

            var msg = "Foram encontrados " + distintos.join(distintos.length > 2 ? ", " : " e ") + " diferentes nas notas fiscais enviadas.<br /><br />Deseja realmente prosseguir com a emissão?";
            //"Foram encontrados <b>" + countRemetente + " emitentes</b> e <b>" + countDestinatario + " destinatários</b> diferentes nas notas fiscais enviadas.<br /><br />Deseja realmente prosseguir com a emissão?"
            jConfirm(msg, "Atenção", function (r) {
                if (r) {
                    if (notasFiscaisJaUtilizadas.length > 0)
                        ExibirMensagemNotasFiscaisEmUso(notasFiscaisJaUtilizadas);
                    else
                        AbrirCTeComNotasFiscaisImportadas();
                }
            });
        } else {
            if (notasFiscaisJaUtilizadas.length > 0)
                ExibirMensagemNotasFiscaisEmUso(notasFiscaisJaUtilizadas);
            else
                AbrirCTeComNotasFiscaisImportadas();
        }
    } else {
        jAlert("Não é possível prosseguir pois não conseguimos importar nenhuma nota fiscal.", "Atenção");
    }
}
function ExibirMensagemNotasFiscaisEmUso(notas) {
    var msg = "Estas NF-es já foram utilizadas no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
    for (var i = 0; i < notas.length; i++) {
        msg += "<br/><b>" + notas[i].Numero + " - " + notas[i].Chave + "</b>:<br/>";
        for (var j = 0; j < notas[i].NumeroDosCTesUtilizados.length; j++)
            msg += " &bull; " + notas[i].NumeroDosCTesUtilizados[j] + "<br/>";
    }
    msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
    jConfirm(msg, "Atenção", function (ret) {
        if (ret) {
            AbrirCTeComNotasFiscaisImportadas();
        }
    });
}
function AbrirCTeComNotasFiscaisImportadas() {
    var agruparCTe = false;
    var agruparRemetente = false;
    var agruparDestinatario = false;
    var agruparUFDestino = false;

    $("#ddlModelo").val("57");

    if ($("#selAgrupamentoXML").val() != "")
        agruparCTe = true;
    if ($("#selAgrupamentoXML").val() == EnumAgrupamentoNFeXML.PorRemetenteEDestinatario) {
        agruparRemetente = true;
        agruparDestinatario = true;
    }
    if ($("#selAgrupamentoXML").val() == EnumAgrupamentoNFeXML.PorRemetente) {
        agruparRemetente = true;
        agruparDestinatario = false;
    }
    if ($("#selAgrupamentoXML").val() == EnumAgrupamentoNFeXML.PorDestinatario) {
        agruparRemetente = false;
        agruparDestinatario = true;
    }
    if ($("#selAgrupamentoXML").val() == EnumAgrupamentoNFeXML.CTePorNFe) {
        agruparRemetente = false;
        agruparDestinatario = false;
    }
    if ($("#selAgrupamentoXML").val() == EnumAgrupamentoNFeXML.PorUFDestino) {
        agruparUFDestino = true;
    }

    if (!agruparCTe) {
        NovoCTe(false);

        var pesoTotal = 0;
        var volumeTotal = 0;
        var listaNotasFiscais = new Array();

        for (var i = 0; i < notasFiscaisImportadas.length; i++) {
            if (ValidarChaveNFe(notasFiscaisImportadas[i].Chave, listaNotasFiscais)) {
                var notaFiscal = {
                    Codigo: - (i + 1),
                    Numero: notasFiscaisImportadas[i].Numero,
                    Chave: notasFiscaisImportadas[i].Chave,
                    ValorTotal: notasFiscaisImportadas[i].ValorTotal,
                    DataEmissao: notasFiscaisImportadas[i].DataEmissao,
                    RemetenteUF: notasFiscaisImportadas[i].RemetenteUF,
                    DestinatarioUF: notasFiscaisImportadas[i].DestinatarioUF,
                    Peso: notasFiscaisImportadas[i].Peso,
                    Excluir: false
                };
                pesoTotal += notasFiscaisImportadas[i].Peso;
                volumeTotal += notasFiscaisImportadas[i].Volume;
                listaNotasFiscais.push(notaFiscal);
            }
        }

        $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(listaNotasFiscais));

        RenderizarNFesRemetente();
        AdicionarPesoNFeRemetente(pesoTotal);
        AdicionarVolumeNFeRemetente(volumeTotal);
        AtualizarValorTotalDaCarga();
        ImportarValorFreteObservacao();

        if ($('#ddlSerie option[value=' + notasFiscaisImportadas[0].Serie + ']').length > 0)
            $("#ddlSerie").val(notasFiscaisImportadas[0].Serie);

        if (notasFiscaisImportadas[0].Placa != null && notasFiscaisImportadas[0].Placa != "") {
            $("#txtPlacaVeiculo").val(notasFiscaisImportadas[0].Placa);
            AdicionarVeiculo();
        }

        BuscarRemetente(true, notasFiscaisImportadas[0].Remetente, null, null, true);

        if (notasFiscaisImportadas[0].Destinatario != null)
            BuscarDestinatario(true, notasFiscaisImportadas[0].Destinatario, null, null, true);
        else
            PreencherCamposDestinatarioExportacao(notasFiscaisImportadas[0].DestinatarioExportacao);

        if (notasFiscaisImportadas[0].FormaPagamento != null && notasFiscaisImportadas[0].FormaPagamento != "") {
            SetarTomadorXML(notasFiscaisImportadas[0].FormaPagamento);
        }

        ControlarCamposCTeOS(true);
        BuscaPalavrasChavesNFe();
    }
    else {
        var dados = {
            NFes: JSON.stringify(notasFiscaisImportadas).replace(/<br \/>/g, '').replace(/</g, '').replace(/>/g, '').replace(/&/g, ' '),
            AgruparRemetente: agruparRemetente,
            AgruparDestinatario: agruparDestinatario,
            AgruparUFDestino: agruparUFDestino,
            TipoRateio: $("#selTipoRateioImportacaoXML").val(),
            ValorFrete: $("#txtValorFreteImportacaoXML").val(),
            CodigoTabelaFreteValor: $("#hddCodigoTabelaFreteImportacaoXML").val(),
            CodigoVeiculo: $("#hddCodigoVeiculoImportacaoXML").val(),
            CodigoReboque: $("#hddCodigoReboqueImportacaoXML").val(),
            CodigoMotorista: $("#hddCodigoMotoristaImportacaoXML").val(),
            CodigoSeguro: $("#hddCodigoSeguroImportacaoXML").val(),
            TipoTomador: $("#selTipoTomadorImportacaoXML").val(),
            ObservacaoCTe: $("#txtObservacaoImportacaoXML").val(),
            ExpedidorCTe: $("#hddCodigoExpedidorImportacaoXML").val(),
            RecebedorCTe: $("#hddCodigoRecebedorImportacaoXML").val(),
            ValorPedagio: $("#txtValorPedagioImportacaoXML").val(),
            ValorAdicionalEntrega: $("#txtValorAdcEntregaImportacaoXML").val(),
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
}

function ImportarValorFreteObservacao() {
    var palavra = "";
    var indexBusca = 0;
    var observacao = notasFiscaisImportadas[0].Observacao.toUpperCase();
    var valorObservacao = ""
    var valor = 0;

    palavra = "[CODINT]=";
    indexBusca = observacao.indexOf(palavra);
    if (indexBusca >= 0) {
        observacao = observacao.substring(indexBusca + 23, 32);
        valorObservacao = observacao.substring(0, 5) + "," + observacao.substring(5, 7);
        valor = Globalize.parseFloat(valorObservacao);
    }

    if (valor > 0) {
        $("#txtValorFreteContratado").val(Globalize.format(valor, "n2"));
        SalvarFreteContratado();
        SetarCreditoPresumido();
    }

}

function ValidarChaveNFe(chave, listaNFe) {
    for (var i = 0; i < listaNFe.length; i++) {
        if (listaNFe[i].Chave == chave) {
            return false;
        }
    }
    return true;
}
function AbrirDivUploadArquivosXMLNFe() {
    LimparCamposImportacaoXML();
    InicializarPlUploadNFe();
    document.getElementById('divAgrupamentoXML').style.visibility = "visible";
    document.getElementById('divImportacaoCTe').style.visibility = 'hidden';
    $("#tituloUploadArquivos").text("Envio de XML de Notas Fiscais Eletrônicas");
    $('#divUploadArquivos').modal("show");
}
function FecharDivUploadArquivosXMLNFe() {
    $('#divUploadArquivos').modal("hide");
}
function MostrarObservacaoNFe(palavrasChave) {
    var palavra = "";
    var indexBusca = 0;
    var observacao = notasFiscaisImportadas[0].Observacao.toUpperCase();
    var mostrarMensagem = false;

    if (observacao != "" && palavrasChave.length > 0) {
        for (var i = 0; i < palavrasChave.length; i++) {
            palavra = palavrasChave[i].Palavra.toUpperCase();
            indexBusca = observacao.indexOf(palavra);
            if (indexBusca > 0) {
                observacao = observacao.replace(new RegExp(palavra, 'g'), "<FONT COLOR=red><b>" + palavra + "</b></FONT>");
                mostrarMensagem = true;
            }
        }

        if (mostrarMensagem) {
            var msg = "<b>Atenção para as palavras destacadas na observação da nota fiscal importada:</b><br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden; '>";
            msg += " &bull; " + observacao + "<br/>";
            msg += "</div><br/>";
            jAlert(msg, "Observação Nota Fiscal");
        }
    }

}
function BuscaPalavrasChavesNFe() {
    executarRest("/XMLNotaFiscalEletronica/ObterPalavrasChaves?callback=?", "", function (r) {
        if (r.Sucesso) {
            MostrarObservacaoNFe(r.Objeto);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function ExibirOcultarTabelaFreteValor() {
    if ($("#selAgrupamentoXML").val() != EnumAgrupamentoNFeXML.CTeUnico) {
        $("#divOpcoesAvancadasImportacaoXML").show();
    } else {
        $("#divOpcoesAvancadasImportacaoXML").hide();
    }
}

function RetornoConsultaFreteImportacaoXML(frete) {
    $("#hddCodigoTabelaFreteImportacaoXML").val(frete.Codigo);
    $("#txtTabelaImportacaoXML").val(frete.Destino + " - " + frete.ValorMinimoFrete);

    // Bloqueia campo de valor frete
    $("#txtValorFreteImportacaoXML").val("0,00").attr('disabled', 'disabled');
}

function RetornoConsultaMotoristaImportacaoXML(motorista) {
    $("#hddCodigoMotoristaImportacaoXML").val(motorista.Codigo);
    $("#txtMotoristaImportacaoXML").val(motorista.Nome + " - " + motorista.CPFCNPJ);
}

function RetornoConsultaVeiculoImportacaoXML(veiculo) {
    $("#hddCodigoVeiculoImportacaoXML").val(veiculo.Codigo);
    $("#txtVeiculoImportacaoXML").val(veiculo.Placa);
}

function RetornoConsultaReboqueImportacaoXML(veiculo) {
    $("#hddCodigoReboqueImportacaoXML").val(veiculo.Codigo);
    $("#txtReboqueImportacaoXML").val(veiculo.Placa);
}

function RetornoConsultaSeguroImportacaoXML(seguro) {
    $("#hddCodigoSeguroImportacaoXML").val(seguro.Codigo);
    $("#txtSeguroImportacaoXML").val(seguro.NomeSeguradora);
}

function RetornoConsultaExpedidorImportacaoXML(cliente) {
    $("#hddCodigoExpedidorImportacaoXML").val(cliente.CPFCNPJ);
    $("#txtExpedidorImportacaoXML").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaRecebedorImportacaoXML(cliente) {
    $("#hddCodigoRecebedorImportacaoXML").val(cliente.CPFCNPJ);
    $("#txtRecebedorImportacaoXML").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function LimparCamposImportacaoXML() {
    ControlarCampoPedagioXML();

    $("#hddCodigoTabelaFreteImportacaoXML").val("");
    $("#txtTabelaImportacaoXML").val("");

    $("#hddCodigoMotoristaImportacaoXML").val("");
    $("#txtMotoristaImportacaoXML").val("");

    $("#hddCodigoVeiculoImportacaoXML").val("");
    $("#txtVeiculoImportacaoXML").val("");

    $("#hddCodigoReboqueImportacaoXML").val("");
    $("#txtReboqueImportacaoXML").val("");

    $("#hddCodigoSeguroImportacaoXML").val("");
    $("#txtSeguroImportacaoXML").val("");

    $("#hddCodigoExpedidorImportacaoXML").val("");
    $("#txtRecebedorImportacaoXML").val("");

    $("#hddCodigoRecebedorImportacaoXML").val("");
    $("#txtExpedidorImportacaoXML").val("");
}