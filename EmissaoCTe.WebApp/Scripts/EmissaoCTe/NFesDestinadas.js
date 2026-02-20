var DocumentoDestinadosSelecionados = [];

var EnumAgrupamentoDocumentoDestinados = {
    CTeUnico: "",
    PorRemetenteEDestinatario: 0,
    PorRemetente: 1,
    PorDestinatario: 2,
    CTePorNFe: 3,
    PorUFDestino: 4
};

$(document).ready(function () {
    FormatarCampoDate("txtDDDataInicial");
    FormatarCampoDate("txtDDDataFinal");
    $("#txtDDPlaca").mask("*******");
    $("#txtUFDestinatario").mask("**");
    $("#txtDDNumero").mask("9?99999999");
    $("#txtDDNumeroFinal").mask("9?99999999");
    $("#txtDDSerie").mask("9?99999999");
    $("#txtDDCPFCNPJEmiente").mask("99999999999?999");
    $("#txtDDChave").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtValorPedagioDocumentosDestinados, #txtValorFreteDocumentosDestinados, #txtValorAdcEntregaDocumentosDestinados").priceFormat({ prefix: '' });

    CarregarConsultaDeMotoristas("btnBuscarMotoristaDocumentosDestinados", "btnBuscarMotoristaDocumentosDestinados", "A", RetornoConsultaMotoristaDocumentosDestinados, true, false)
    CarregarConsultaDeVeiculos("btnBuscarVeiculoDocumentosDestinados", "btnBuscarVeiculoDocumentosDestinados", RetornoConsultaVeiculoDocumentosDestinados, true, false, 0);
    CarregarConsultaDeVeiculos("btnBuscarReboqueDocumentosDestinados", "btnBuscarReboqueDocumentosDestinados", RetornoConsultaReboqueDocumentosDestinados, true, false, 1);
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarSeguroDocumentosDestinados", "btnBuscarSeguroDocumentosDestinados", 0, RetornoConsultaSeguroDocumentosDestinados, true, false);
    CarregarConsultadeClientes("btnBuscarExpedidorDocumentosDestinados", "btnBuscarExpedidorDocumentosDestinados", RetornoConsultaExpedidorDocumentosDestinados, true, false);
    CarregarConsultadeClientes("btnBuscarRecebedorDocumentosDestinados", "btnBuscarRecebedorDocumentosDestinados", RetornoConsultaRecebedorDocumentosDestinados, true, false);

    RemoveConsulta("#txtVeiculoDocumentosDestinados, #txtReboqueDocumentosDestinados, #txtMotoristaDocumentosDestinados, #txtSeguroDocumentosDestinados, #txtExpedidorDocumentosDestinados, #txtRecebedorDocumentosDestinados", function ($this) {
        $this.val('');
        $this.data('Codigo', 0);
    });

    $("#txtVeiculoDocumentosDestinados, #txtReboqueDocumentosDestinados, #txtMotoristaDocumentosDestinados, #txtSeguroDocumentosDestinados, #txtExpedidorDocumentosDestinados, #txtRecebedorDocumentosDestinados").data('Codigo', 0);

    $("#btnNFesDestinadas").click(function () {
        $("#divNFesDestinadas").modal('show');
    });

    $("#btnConsultarNFesDestinadas").click(function () {
        ConsultarNFesDestinadas(false);
    });

    $("#btnGerarCTeDocumentosDestinados").click(function () {
        GerarCTeDocumentosDestinados();
    });

    $("#selAgrupamentoDocumentosDestinados").change(function () {
        AgrupamentoDocumentosDestinados();
    });

    //$("#divOpcoesAvancadasDocumentosDestinados").change(function () {
    //    ControlarCampoPedagioDocumentosDestinados();
    //});

    $("#txtDDPlaca").blur(function () {
        HabilitaSelecionarTodos();
    });
    $("#txtDDCPFCNPJEmiente").blur(function () {
        HabilitaSelecionarTodos();
    });
    $("#txtDDNumero").blur(function () {
        HabilitaSelecionarTodos();
    });
    $("#txtDDNumeroFinal").blur(function () {
        HabilitaSelecionarTodos();
    });
    $("#txtUFDestinatario").blur(function () {
        HabilitaSelecionarTodos();
    });    

    $("#btnTelaDocumentosDestinadosProximo").click(function () {
        $("#tabsDocumentosDestinados a[href='#tabEmissaoDestinados']").tab("show");
    });

    $("#btnTelaDocumentosDestinadosAnterior").click(function () {
        $("#tabsDocumentosDestinados a[href='#tabDocumentosDestinados']").tab("show");
    });

    $("#btnConsultarDocumentosDestinados").click(function () {
        AtualizarGridDocumentosDestinados();
    });

    $("#btnSelecionarTodosDocumentosDestinados").click(function () {
        SelecionarTodosDocumentosDestinados();
    });

    $("#btnLimparSelecaoDocumentosDestinados").click(function () {
        LimparSelecaoDocumentosDestinados();
    });

    $("#divNFesDestinadas").on('hidden.bs.modal', LimparCamposDocumentoDestinados);
    $("#divNFesDestinadas").on('show.bs.modal', AtualizarGridDocumentosDestinados);

    $("#tabsDocumentosDestinados a[href='#tabDocumentosDestinados']").on("shown.bs.tab", function () {
        $("#btnGerarCTeDocumentosDestinados").hide();
    });
    $("#tabsDocumentosDestinados a[href='#tabEmissaoDestinados']").on("shown.bs.tab", function () {
        $("#btnGerarCTeDocumentosDestinados").show();
    });

    LimparCamposDocumentoDestinados();   

    $("#btnSelecionarTodosDocumentosDestinados").prop("disabled", true);
    $("#chkOpcaoDigitacaoDocumentosDestinados").prop('checked', true);
});


function ConsultarNFesDestinadas(confirmaConsulta) {
    executarRest("/DestinadosNFes/ConsultarNFesDestinadas", { ConfirmaConsulta: confirmaConsulta }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.PossuiConfiguracao == false) {
                jConfirm("A primeira consulta pode levar até horas para finalizar, você tem certeza que deseja continuar?", "Consulta de NF-es", function (res) {
                    if (res) ConsultarNFesDestinadas(true);
                });
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
        AtualizarGridDocumentosDestinados();
    });
}

function LimparCamposDocumentoDestinados() {
    $("#tabsDocumentosDestinados a[href='#tabDocumentosDestinados']").tab("show");

    var today = new Date();
    var yesterday = new Date(today);
    var tomorrow = new Date(today);
    yesterday.setDate(today.getDate() - 1);
    tomorrow.setDate(today.getDate() + 1);

    $("#txtDDDataInicial").val(Globalize.format(today, "dd/MM/yyyy"));
    $("#txtDDDataFinal").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

    $("#btnGerarCTeDocumentosDestinados").hide();

    $("#btnTelaDocumentosDestinadosProximo").prop("disabled", true);
    $("#btnLimparSelecaoDocumentosDestinados").hide();
    $("#txtDDNotasSelecionadas").val("0");

    LimparSelecaoDocumentosDestinados();
}

function DadosFiltroDocumentosDestinados() {
    return {
        DataInicial: $("#txtDDDataInicial").val(),
        DataFinal: $("#txtDDDataFinal").val(),
        Placa: $("#txtDDPlaca").val(),
        UFDestinatario: $("#txtUFDestinatario").val(),
        CPFCNPJEmiente: $("#txtDDCPFCNPJEmiente").val(),
        Emiente: $("#txtDDEmiente").val(),
        Numero: $("#txtDDNumero").val(),
        NumeroFinal: $("#txtDDNumeroFinal").val(),
        Serie: $("#txtDDSerie").val(),
        Chave: $("#txtDDChave").val(),
        Cancelado: $("#selDDCancelado").val(),
        NotasSemCTe: $("#selDDNotasSemCTe").val()
    };
}

function AtualizarGridDocumentosDestinados() {
    var dados = DadosFiltroDocumentosDestinados();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Selecionar", Evento: SelecionarDocumentoDestinado });

    CriarGridView("/DestinadosNFes/Consultar?callback=?", dados, "tbl_documentosdestinados_table", "tbl_documentosdestinados", "tbl_paginacao_documentosdestinados", opcoes, [0], null, null, 20);
}

function LimparSelecaoDocumentosDestinados() {
    $("#btnLimparSelecaoDocumentosDestinados").hide();
    LimparSelecaoDocumentosDestinados();
}

function SelecionarTodosDocumentosDestinados() {
    var dados = DadosFiltroDocumentosDestinados();

    executarRest("/DestinadosNFes/SelecionarTodosDocumentosDestinados", dados, function (r) {
        if (r.Sucesso) {
            for (var i = 0; i < r.Objeto.length; i++)
                SelecionarDocumentoDestinado(r.Objeto[i]);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function SelecionarDocumentoDestinado(evt) {
    var data = "data" in evt ? evt.data : evt;

    if (DocumentoDestinadoJaInserido(data.Codigo))
        return;

    var html = [
        '<li class="tag-item tag-item-delete-experience" id="ddSelecionado_' + data.Codigo + '">',
        '<span class="tag-container tag-container-delete-experience">',
        '<span class="tag-box tag-box-delete-experience">',
        '<b>' + data.Numero + ' - ' + data.Serie + '</b> | ' + data.Placa + '',
        '</span>',
        '<span class="tag-delete tag-box tag-box-delete-experience">&nbsp;</span>',
        '</span>',
        '</li>'
    ];
    var $li = $(html.join(""));

    $li.on('click', '.tag-delete', function () {
        RemoverDocumentoDestinadoSelecionado(data)
    });

    $("#containerDocumentosDestinadosSelecionados").append($li);

    DocumentoDestinadosSelecionados.push(data);
    $("#btnTelaDocumentosDestinadosProximo").prop("disabled", false);
    $("#btnLimparSelecaoDocumentosDestinados").show();

    $("#txtDDNotasSelecionadas").val(DocumentoDestinadosSelecionados.length.toString());
}

function GerarCTeDocumentosDestinados() {

    if ($("#selTipoTomadorDocumentosDestinados").val() == "-1" && $("#selAgrupamentoDocumentosDestinados").val() != "") {
        CampoComErro("#selTipoTomadorDocumentosDestinados");
        jAlert('Não foi informado um tomador.', "Atenção");
        return;
    }
    else
        CampoSemErro("#selTipoTomadorDocumentosDestinados");

    var dados = {
        DocumentosSelecionados: JSON.stringify(DocumentoDestinadosSelecionados.map(function (dd) { return dd.Codigo; }))
    };
    executarRest("/DestinadosNFes/RetornarObjetoNFes", dados, function (r) {
        if (r.Sucesso) {
            RetornoDocumentosDestinadosNFe(r.Objeto);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RetornoDocumentosDestinadosNFe(notasfiscais) {
    if (ValidaGerarCTeDocumentosDestinadosNFe(notasfiscais)) {
        var cpfCnpjRemetente = notasfiscais[0].Remetente;
        var cpfCnpjDestinatario = notasfiscais[0].Destinatario;

        var ufsRemetentes = [];
        var ufsDestinatarios = [];

        var countDestinatario = 0;
        var countRemetente = 0;

        var notasFiscaisJaUtilizadas = new Array();

        for (var i = 0; i < notasfiscais.length; i++) {

            if (notasfiscais[i].NumeroDosCTesUtilizados.length > 0)
                notasFiscaisJaUtilizadas.push(notasfiscais[i]);

            if (cpfCnpjRemetente != notasfiscais[i].Remetente)
                countRemetente += 1;

            if (cpfCnpjDestinatario != notasfiscais[i].Destinatario)
                countDestinatario += 1;

            if ($.inArray(notasfiscais[i].RemetenteUF, ufsRemetentes) < 0)
                ufsRemetentes.push(notasfiscais[i].RemetenteUF);

            if ($.inArray(notasfiscais[i].DestinatarioUF, ufsDestinatarios) < 0)
                ufsDestinatarios.push(notasfiscais[i].DestinatarioUF);
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

            var msg = "Foram encontrados " + distintos.join(distintos.length > 2 ? ", " : " e ") + " diferentes nas notas fiscais selecionadas.<br /><br />Deseja realmente prosseguir com a emissão?";
            jConfirm(msg, "Atenção", function (r) {
                if (r) {
                    if (notasFiscaisJaUtilizadas.length > 0)
                        ExibirMensagemNotasDestinadasEmUso(notasFiscaisJaUtilizadas, notasfiscais);
                    else
                        GerarCTeDocumentosDestinadosNFe(notasfiscais);
                }
            });
        } else {
            if (notasFiscaisJaUtilizadas.length > 0)
                ExibirMensagemNotasDestinadasEmUso(notasFiscaisJaUtilizadas, notasfiscais);
            else
                GerarCTeDocumentosDestinadosNFe(notasfiscais);
        }
    }
}

function ExibirMensagemNotasDestinadasEmUso(notasJaUtilizadas, notasFiscais) {
    var msg = "Estas NF-es já foram utilizadas no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
    for (var i = 0; i < notasJaUtilizadas.length; i++) {
        msg += "<br/><b>" + notasJaUtilizadas[i].Numero + " - " + notasJaUtilizadas[i].Chave + "</b>:<br/>";
        for (var j = 0; j < notasJaUtilizadas[i].NumeroDosCTesUtilizados.length; j++)
            msg += " &bull; " + notasJaUtilizadas[i].NumeroDosCTesUtilizados[j] + "<br/>";
    }
    msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
    jConfirm(msg, "Atenção", function (ret) {
        if (ret) {
            GerarCTeDocumentosDestinadosNFe(notasFiscais);
        }
    });
}

function GerarCTeDocumentosDestinadosNFe(notasfiscais) {
    var agruparCTe = false;
    var agruparRemetente = false;
    var agruparDestinatario = false;
    var agruparUFDestino = false;
    var notasGeracao = new Array();

    agrupamentoNFeSefaz = $("#selAgrupamentoDocumentosDestinados").val();

    if (agrupamentoNFeSefaz != "")
        agruparCTe = true;
    if (agrupamentoNFeSefaz == EnumAgrupamentoDocumentoDestinados.PorRemetenteEDestinatario) {
        agruparRemetente = true;
        agruparDestinatario = true;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoDocumentoDestinados.PorRemetente) {
        agruparRemetente = true;
        agruparDestinatario = false;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoDocumentoDestinados.PorDestinatario) {
        agruparRemetente = false;
        agruparDestinatario = true;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoDocumentoDestinados.CTePorNFe) {
        agruparRemetente = false;
        agruparDestinatario = false;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoDocumentoDestinados.PorUFDestino) {
        agruparUFDestino = true;
    }

    if (notasfiscais == null || notasfiscais.length == 0)
        return jAlert('Nenhuma NF-e foi selecionada.', "Emissão por NF-es Destinadas");

    if (!agruparCTe) {
        $('#divNFesDestinadas').modal("hide");
        NovoCTe(false);

        var pesoTotal = 0;
        var volumeTotal = 0;
        var listaNotasFiscais = new Array();

        for (var i = 0; i < notasfiscais.length; i++) {
            if (ValidarChaveNFe(notasfiscais[i].Chave, listaNotasFiscais)) {
                var notaFiscal = {
                    Codigo: -(i + 1),
                    Numero: notasfiscais[i].Numero,
                    Chave: notasfiscais[i].Chave,
                    ValorTotal: notasfiscais[i].ValorTotal,
                    DataEmissao: notasfiscais[i].DataEmissao,
                    RemetenteUF: notasfiscais[i].RemetenteUF,
                    DestinatarioUF: notasfiscais[i].DestinatarioUF
                };
                pesoTotal += notasfiscais[i].UtilizarPesoLiquido == "1" ? notasfiscais[i].PesoLiquido : notasfiscais[i].Peso;
                volumeTotal += notasfiscais[i].Volume;
                listaNotasFiscais.push(notaFiscal);
            }
        }

        $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(listaNotasFiscais));

        RenderizarNFesRemetente();
        AdicionarPesoNFeRemetente(pesoTotal);
        AdicionarVolumeNFeRemetente(volumeTotal);
        AtualizarValorTotalDaCarga();

        if ($('#ddlSerie option[value=' + notasfiscais[0].Serie + ']').length > 0)
            $("#ddlSerie").val(notasfiscais[0].Serie);

        if (notasfiscais[0].Placa != null && notasfiscais[0].Placa != "") {
            $("#txtPlacaVeiculo").val(notasfiscais[0].Placa);
            AdicionarVeiculo();
        }

        BuscarRemetente(true, notasfiscais[0].Remetente, null, null, true);

        if (notasfiscais[0].Destinatario != null && notasfiscais[0].Destinatario != "")
            BuscarDestinatario(true, notasfiscais[0].Destinatario, null, null, true);
        else
            PreencherCamposDestinatarioExportacao(notasfiscais[0].DestinatarioExportacao);

        SetarTomadorXML(notasfiscais[0].FormaPagamento);
    } else {
        var dados = {
            NFes: JSON.stringify(notasfiscais),
            AgruparRemetente: agruparRemetente,
            AgruparDestinatario: agruparDestinatario,
            AgruparUFDestino: agruparUFDestino,
            TipoRateio: $("#selTipoRateioDocumentosDestinados").val(),
            ValorFrete: $("#txtValorFreteDocumentosDestinados").val(),
            CodigoVeiculo: $("#txtVeiculoDocumentosDestinados").data("Codigo"),
            CodigoReboque: $("#txtReboqueDocumentosDestinados").data("Codigo"),
            CodigoMotorista: $("#txtMotoristaDocumentosDestinados").data("Codigo"),
            CodigoSeguro: $("#txtSeguroDocumentosDestinados").data("Codigo"),
            TipoTomador: $("#selTipoTomadorDocumentosDestinados").val(),
            ObservacaoCTe: $("#txtObservacaoDocumentosDestinados").val(),
            ExpedidorCTe: $("#txtExpedidorDocumentosDestinados").data("Codigo"),
            RecebedorCTe: $("#txtRecebedorDocumentosDestinados").data("Codigo"),
            ValorPedagio: $("#txtValorPedagioDocumentosDestinados").val(),
            ValorAdicionalEntrega: $("#txtValorAdcEntregaDocumentosDestinados").val(),
            ManterDigitacao: $("#chkOpcaoDigitacaoDocumentosDestinados").prop('checked')
        };

        executarRest("/ConhecimentoDeTransporteEletronico/SalvarCTePorXMLNFe?callback=?", dados, function (r) {
            if (r.Sucesso) {
                LimparSelecaoDocumentosDestinados();
                $('#divNFesDestinadas').modal("hide");
                AtualizarGridCTes();
                ExibirMensagemSucesso("CTes salvos com sucesso!", "Sucesso!");
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!");
            }
        });
    }
}

function ValidaGerarCTeDocumentosDestinadosNFe(notasfiscais) {
    var valido = true;

    // Valida seleção de veículos
    var reboque = $("#txtReboqueDocumentosDestinados").data("Codigo");
    var veiculo = $("#txtVeiculoDocumentosDestinados").data("Codigo");

    if (reboque > 0 && veiculo == 0) {
        jAlert("É obrigatório ter uma tração quando um reboque for selecionado.");
        return false;
    }

    if (notasfiscais == null || notasfiscais.length <= 0) {
        valido = false;
        jAlert('Nenhuma NF-e foi selecionada.', "Emissão por NF-es Destinadas");
    }

    return valido;
}

function LimparSelecaoDocumentosDestinados() {
    DocumentoDestinadosSelecionados = [];
    $("#txtDDNotasSelecionadas").val("0");

    $("#containerDocumentosDestinadosSelecionados").off("click", '.tag-delete').html("");
    $("#btnLimparSelecaoDocumentosDestinados").hide();
}

function DocumentoDestinadoJaInserido(codigo) {
    for (var i = 0; i < DocumentoDestinadosSelecionados.length; i++) {
        if (DocumentoDestinadosSelecionados[i].Codigo == codigo)
            return true;
    }

    return false;
}

function RemoverDocumentoDestinadoSelecionado(data) {
    for (var i = 0; i < DocumentoDestinadosSelecionados.length; i++) {
        if (DocumentoDestinadosSelecionados[i].Codigo == data.Codigo)
            DocumentoDestinadosSelecionados.splice(i, 1);
    }

    $("#txtDDNotasSelecionadas").val(DocumentoDestinadosSelecionados.length.toString());

    $("#ddSelecionado_" + data.Codigo).remove();

    if (DocumentoDestinadosSelecionados.length <= 0) {
        $("#btnTelaDocumentosDestinadosProximo").prop("disabled", true);
        $("#btnLimparSelecaoDocumentosDestinados").hide();
    }
}

function HabilitaSelecionarTodos() {
    var possuiPlaca = $("#txtDDPlaca").val().length == 7;
    var possuiEmitente = $("#txtDDCPFCNPJEmiente").val().length >= 11;
    var possuiFiltroNumeros = ($("#txtDDNumero").val() != "" && $("#txtDDNumeroFinal").val() != "");

    if (possuiPlaca || possuiEmitente || possuiFiltroNumeros)
        $("#btnSelecionarTodosDocumentosDestinados").prop("disabled", false);
    else
        $("#btnSelecionarTodosDocumentosDestinados").prop("disabled", true);
}

function AgrupamentoDocumentosDestinados() {
    if ($("#selAgrupamentoDocumentosDestinados").val() != EnumAgrupamentoDocumentoDestinados.CTeUnico) {
        $("#divOpcoesAvancadasDocumentosDestinados").show();
    } else {
        $("#divOpcoesAvancadasDocumentosDestinados").hide();
    }
}

function ControlarCampoPedagioDocumentosDestinados() {
    var tipo = $("#selTipoRateioDocumentosDestinados").val();

    //if (tipo == "6" || tipo == "7")
    //{
    $("#idPedagioDocumentosDestinados").show();
    $("#idAdcEntregaDocumentosDestinados").show();
    //}
    //else {
    //    $("#txtValorPedagioDocumentosDestinados").val("0,00")
    //    $("#idPedagioDocumentosDestinados").hide();

    //    $("#txtValorAdcEntregaDocumentosDestinados").val("0,00")
    //    $("#idAdcEntregaDocumentosDestinados").hide();
    //}
}

function RetornoConsultaMotoristaDocumentosDestinados(motorista) {
    $("#txtMotoristaDocumentosDestinados").val(motorista.Nome + " - " + motorista.CPFCNPJ).data("Codigo", motorista.Codigo);
}
function RetornoConsultaVeiculoDocumentosDestinados(veiculo) {
    $("#txtVeiculoDocumentosDestinados").val(veiculo.Placa).data("Codigo", veiculo.Codigo);
}
function RetornoConsultaReboqueDocumentosDestinados(veiculo) {
    $("#txtReboqueDocumentosDestinados").val(veiculo.Placa).data("Codigo", veiculo.Codigo);
}
function RetornoConsultaSeguroDocumentosDestinados(seguro) {
    $("#txtSeguroDocumentosDestinados").val(seguro.NomeSeguradora).data("Codigo", seguro.Codigo);
}
function RetornoConsultaExpedidorDocumentosDestinados(cliente) {
    $("#txtExpedidorDocumentosDestinados").val(cliente.CPFCNPJ + " - " + cliente.Nome).data("Codigo", cliente.CPFCNPJ);
}
function RetornoConsultaRecebedorDocumentosDestinados(cliente) {
    $("#txtRecebedorDocumentosDestinados").val(cliente.CPFCNPJ + " - " + cliente.Nome).data("Codigo", cliente.CPFCNPJ);
}