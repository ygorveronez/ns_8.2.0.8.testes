var TipoIntegradoraCIOT = {
    SigaFacil: 1,
    PamCard: 2,
    PamCardAbertura: 5,
    EFrete: 3,
    EFreteAbertura: 4,
    TruckPad: 6
};

var ConfiguracaoEmpresa, OpcaoAbertura, EFreteAbertura, PamCardAbertura, TruckPad;
var MultiSelecaoCTes = [];
var codigoDocumento = 0;
var documentosPendentesAviso = false;

$(document).ready(function () {
    BuscarConfiguracoesEmpresa(function () {
        BuscarEstados();
        BuscarNaturezasCargas();
        BuscarImpostosEmpresa();
        AlteraTipoDeIntegracao();
        LimparCampos();
    });

    // CIOT padrao
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
    CarregarConsultadeClientes("btnBuscarTransportador", "btnBuscarTransportador", RetornoConsultaTransportador, true, false);
    CarregarConsultaDeLocalidades("btnBuscarCidadePedagio", "btnBuscarCidadePedagio", RetornoConsultaLocalidades, true, false);

    RemoveConsulta("#txtVeiculo", function ($this) {
        $this.val("");
        $("body").data("codigoVeiculo", null);
    });
    RemoveConsulta("#txtMotorista", function ($this) {
        $this.val("");
        $("body").data("codigoMotorista", null);
    });
    RemoveConsulta("#txtTransportador", function ($this) {
        $this.val("");
        $("body").data("codigoTransportador", null);
    });
    RemoveConsulta("#txtCidadePedagio", function ($this) {
        $this.val("");
        $("body").data("cidadePedagio", null);
    });

    $("#txtValorEstimado").priceFormat();

    $("#txtDataInicioViagem, #txtDataFimViagem, #txtDataInicial, #txtDataFinal").mask("99/99/9999").datepicker();
    $("#txtValorAdiantamentoAbertura").priceFormat();
    $("#txtFiltroPlaca").mask("*******");
    $("#txtNumeroInicial, #txtNumeroFinal").mask("9?999999999999999");

    $("#selUFOrigem").change(function () {
        BuscarLocalidades($(this).val(), "selLocalidadeOrigem", null);
    });

    $("#selUFDestino").change(function () {
        BuscarLocalidades($(this).val(), "selLocalidadeDestino", null);
    });

    $('#divMsgValorAdiantamento').hide();
    $('#txtValorAdiantamentoAbertura').on("change", function () {
        AlertaCampoAdiantamentoAbertura();
    });

    $("#btnNovoCIOT, #btnAbrirCIOT").click(function () {
        AbrirTelaEmissaoCIOT();
    });

    $("#btnEmitirCIOT").click(function () {
        SalvarCIOT();
    });

    $("#btnSalvarCIOT").click(function () {
        SalvarInformacoesCIOT();
    });

    $("#btnSalvarCTes").click(function () {
        SalvarCTes();
    });

    $("#btnProximaEtapa").click(function () {
        ProximaEtapa();
    });

    $("#btnCancelarCIOT").click(function () {
        FecharTelaEmissaoCIOT();
    });

    $("#btnAtualizarGridCIOT").click(function () {
        AtualizarGridCIOT();
    });

    $("#divEmissaoCIOT").on('hidden.bs.modal', function () {
        LimparCampos();
    });

    $("#btnObterInformacoesCTesSelecionados").click(function () {
        FinalizarSelecaoCTes();
    });

    // Eventos de selecao de abas
    $('a[href="#divDadosGerais"]').on('shown.bs.tab', function (e) {
        var status = $("body").data("status") || "";
        if ((status == "Cancelado" || status == "Rejeitado") || (!EFreteAbertura || !TruckPad)) return;

        if (!documentosModificado()) {
            $("#btnEmitirCIOT").show();
            $("#btnSalvarCTes").hide();
        }
    });
    $('a[href="#divCTes"]').on('shown.bs.tab', function (e) {
        var status = $("body").data("status") || "";
        if ((status == "Cancelado" || status == "Rejeitado") || (!EFreteAbertura || !TruckPad)) return;

        if ($("body").data("codigo") > 0) {
            $("#btnEmitirCIOT").hide();
            $("#btnSalvarCTes").show();
        }
    });
    $('a[href="#divEncerramento"]').on('shown.bs.tab', function (e) {
        var status = $("body").data("status") || "";
        if ((status == "Cancelado" || status == "Rejeitado") || (!EFreteAbertura || !TruckPad)) return;

        if (!documentosModificado()) {
            $("#btnEmitirCIOT").show();
            $("#btnSalvarCTes").hide();
        } else {
            /**
             * Os alertas duram 6500ms, para evitar que apareca multiplos alertas, existe a flag para verificar se tem um alerta ativo
             */
            if (!documentosPendentesAviso) {
                ExibirMensagemAlerta("Existem CT-es pendetes para serem salvos.", "Atenção", "messages-placeholder-emissaoCIOT");
                documentosPendentesAviso = true;
                setTimeout(function () {
                    documentosPendentesAviso = false;
                }, 6500);
            }
        }
    });
});

function documentosModificado(val) {
    if (arguments.length == 0)
        return window.documentosModificadoFlag;

    window.documentosModificadoFlag = val;

    if (val && (EFreteAbertura || TruckPad)) {
        $("#btnSalvarCTes").show();
        $("#btnEmitirCIOT").hide();
    }
}

function AlteraTipoDeIntegracao() {
    if (OpcaoAbertura) {
        $("#btnNovoCIOT").hide();
        $("#btnAbrirCIOT").show();
    } else {
        $("#btnNovoCIOT").show();
        $("#btnAbrirCIOT").hide();
    }
}

function RetornoConsultaLocalidades(localidade) {
    $("body").data("cidadePedagio", localidade.Codigo);
    $("#txtCidadePedagio").val(localidade.Descricao + " - " + localidade.UF);
}

function AlertaCampoAdiantamentoAbertura() {
    var valorAdiantamento = Globalize.parseFloat($("#txtValorAdiantamentoAbertura").val());
    if (valorAdiantamento > 0)
        $('#divMsgValorAdiantamento').show();
    else
        $('#divMsgValorAdiantamento').hide();
}

function BuscarConfiguracoesEmpresa(cb) {
    executarRest("/ConfiguracaoEmpresa/ObterDetalhes?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("body").data("configuracaoEmpresa", r.Objeto);
            ConfiguracaoEmpresa = r.Objeto;
            EFreteAbertura = (ConfiguracaoEmpresa.TipoIntegradoraCIOT == TipoIntegradoraCIOT.EFreteAbertura);
            PamCardAbertura = (ConfiguracaoEmpresa.TipoIntegradoraCIOT == TipoIntegradoraCIOT.PamCardAbertura);
            TruckPad = (ConfiguracaoEmpresa.TipoIntegradoraCIOT == TipoIntegradoraCIOT.TruckPad);
            OpcaoAbertura = EFreteAbertura || PamCardAbertura || TruckPad;
            cb();
        } else {
            jAlert(r.Erro, "Atenção");
        }

        AtualizarGridCIOT();
    });
}

function BuscarImpostosEmpresa() {
    executarRest("/ImpostoContratoFrete/ObterImpostosDaEmpresa?callback=?", {}, function (r) {
        if (r.Sucesso && r.Objeto != null && typeof r.Objeto == "object")
            $("body").data("impostosContratoFrete", r.Objeto);
    });
}

function BuscarNaturezasCargas() {
    executarRest("/NaturezaCargaANTT/BuscarTodos?callback=?", { buscarSemOpcaoZero: true }, function (r) {
        if (r.Sucesso) {
            $("#selNaturezaCarga").html(r.Objeto.map(function (opt) {
                return "<option value=" + opt.Codigo + ">" + opt.Numero + " - " + opt.Descricao + "</option>";
            }).join("")).val("");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function BuscarEstados() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("#selUFOrigem, #selUFDestino").html(r.Objeto.map(function (opt) {
                return "<option value=" + opt.Sigla + ">" + opt.Nome + "</option>";
            }).join("")).val("");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function BuscarLocalidades(uf, idSelect, codigo) {
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
        if (r.Sucesso) {
            RenderizarLocalidades(r.Objeto, idSelect, codigo);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarLocalidades(localidades, idSelect, codigo) {
    var locs = new Array();

    if (typeof idSelect == 'string')
        locs.push(idSelect);
    else
        locs = idSelect;

    for (var x = 0; x < locs.length; x++) {

        var selLocalidades = document.getElementById(locs[x]);
        selLocalidades.options.length = 0;

        for (var i = 0; i < localidades.length; i++) {

            var optn = document.createElement("option");
            optn.text = localidades[i].Descricao;
            optn.value = localidades[i].Codigo;

            selLocalidades.options.add(optn);
        }

        if (codigo != null)
            $(selLocalidades).val(codigo).change();
    }
}

function RetornoConsultaVeiculos(veiculo) {
    // Valida se ja existe um CIOT aberto
    if (OpcaoAbertura) {
        executarRest("/IntegracaoSigaFacil/VerificaVeiculo?callback=?", { Codigo: veiculo.Codigo }, function (r) {
            if (r.Sucesso)
                ExibirMensagemAlerta("Existe um CIOT aberto para o veículo <b>" + (veiculo.Placa.substr(0, 3) + "-" + veiculo.Placa.substr(3)) + "</b>. CIOT <b>#" + r.Objeto.Numero + "</b>", "Alerta!", "messages-placeholder-emissaoCIOT");
        });
    }

    $("body").data("codigoVeiculo", veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
}

function RetornoConsultaMotorista(motorista) {
    $("body").data("codigoMotorista", motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
}

function RetornoConsultaTransportador(transp) {
    $("body").data("codigoTransportador", transp.CPFCNPJ);
    $("#txtTransportador").val(transp.CPFCNPJ + " - " + transp.Nome);
}

function AbrirTelaEmissaoCIOT() {
    documentosModificado(false);
    //$("a[href=#divDadosGerais]").click();
    BloquearCamposPorIntegradora();

    $("#divEmissaoCIOT").modal({ keyboard: false, backdrop: 'static' });
}

function FecharTelaEmissaoCIOT() {
    documentosModificado(false);
    $("#divEmissaoCIOT").modal('hide');
    LimparCampos();
}

function BloquearCamposPorIntegradora() {
    var configuracao = $("body").data("configuracaoEmpresa");

    // Esconder abas especificas
    $('a[href="#divCidadesPedagio"]').hide();
    $('a[href="#divEncerramento"]').hide();
    $('a[href="#divJustificativa"]').hide();

    // Encerramento ciot aparece apenas para opcao abertura
    $("#btnSalvarCTes").hide();
    $("#btnSalvarCIOT").hide();

    $(".pamcard-abertura").hide();
    $(".efrete-abertura").hide();

    // Texto padrao para emitir
    $("#btnEmitirCIOT .descricao").text("Emitir CIOT");

    // Mantem campos bloquados
    $("#txtEncerramentoPesoTotal").attr("disabled", true);
    $("#txtEncerramentoValorMercadoria").attr("disabled", true);
    $("#txtEncerramentoValorMercadoriaPorKG").attr("disabled", true);

    if (ConfiguracaoEmpresa != null && ConfiguracaoEmpresa.TipoIntegradoraCIOT != null) {
        switch (configuracao.TipoIntegradoraCIOT) {
            case "2": //Pancard                       
                $("#selCategoriaTransportadorANTT").attr("disabled", true);
                $("#selRegraQuitacaoAdiantamento").attr("disabled", true);
                $("#selRegraQuitacaoQuitacao").attr("disabled", true);
                $("#selTipoViagem").attr("disabled", true);
                $("#selDocumentosObrigatorios").attr("disabled", true);
                $("#selTipoPeso").attr("disabled", true);
                $("#txtPesoLotacao").attr("disabled", true);
                $("#txtValorTarifaFrete").attr("disabled", true);
                $("#selCobraDiferencaFrete").attr("disabled", true);
                $("#selExigePesoChegada").attr("disabled", true);
                $("#selTipoQuebra").attr("disabled", true);
                $("#selTipoTolerancia").attr("disabled", true);
                $("#txtTolerancia").attr("disabled", true);
                $("#txtToleranciaSuperior").attr("disabled", true);
                $("#txtValorSeguro").attr("disabled", true);
                $("#txtValorTarifaEmissaoCartao").attr("disabled", true);
                $("#txtValorCartaoPedagio").attr("disabled", true);
                $("#selPedagioIdaVolta").attr("disabled", false);
                $("#txtValorEstimado").attr("disabled", false);
                $("#txtValorOutrosDescontos").attr("disabled", true);
                $("#txtValorAbastecimento").attr("disabled", false);
                $('a[href="#divCidadesPedagio"]').show();

                break;
            case "5":// PamCard Abertura
                var status = $("body").data("status") || "";
                $("#divCTes .integrador-padrao").hide();
                $(".integrador-abertura").show();
                $(".pamcard-abertura").show();

                var disabledDadosGerais = true;
                var disabledAdiantamentoAbertura = false;
                var disabledCte = false;

                if (status.length == 0 || status == "Rejeitado") { // Rejeitado na abertura ou na atualizacao
                    // Habilita campos dos dados gerais
                    //$("a[href=#divDadosGerais]").click();

                    if (status == "Rejeitado")
                        $("#btnEmitirCIOT .descricao").text("Reabrir CIOT");
                    else
                        $("#btnEmitirCIOT .descricao").text("Abrir CIOT");

                    $("#btnEmitirCIOT").show();
                    $("#btnSalvarCIOT").hide();

                    disabledDadosGerais = false;
                    disabledCte = false;
                }
                if (status == "Aberto" || status == "Rejeitado_Evento") {
                    $('#btnSalvarCIOT').show();
                    $('#btnEmitirCIOT').show();
                    $("#btnEmitirCIOT .descricao").text("Encerrar CIOT");
                    $('a[href="#divEncerramento"]').show();
                    $("#tblDocumentos button").attr("disabled", false);

                    var disabledEncerramentoValorAdiantamento = false;
                    if ($("#txtValorAdiantamentoAbertura").data('PossuiAdiantamentoAbertura'))
                        disabledEncerramentoValorAdiantamento = true;
                    $("#txtEncerramentoValorAdiantamento").attr("disabled", disabledEncerramentoValorAdiantamento);

                    disabledAdiantamentoAbertura = true;
                    disabledDadosGerais = false;
                    disabledCte = false;
                }
                if (status == "Encerrado") {
                    // Encerrado e Cancelado => (Todos campos bloqueados)
                    // Desabilita todo campos
                    $("#btnEmitirCIOT").hide();
                    $('#btnSalvarCIOT').hide();
                    $('a[href="#divEncerramento"]').show();
                    jQueryCamposInteracao().attr('disabled', 'disabled');
                    disabledDadosGerais = true;
                    disabledCte = true;
                }
                /*if (status == "Rejeitado") {
                    jQueryCamposInteracao().attr('disabled', 'disabled');
                    disabledDadosGerais = true;
                    disabledCte = true;
                    $("#btnEmitirCIOT .descricao").text("Abrir CIOT");
                    $("#btnEmitirCIOT").show();
                }*/
                if (status == "Autorizado" || status == "Cancelado") {
                    jQueryCamposInteracao().attr('disabled', 'disabled');
                    disabledDadosGerais = true;
                    disabledCte = true;
                    $("#btnSalvarCIOT").hide();
                    $("#btnEmitirCIOT").hide();

                    if (status == "Cancelado") {
                        // Cancelado => (Mostra aba Justificativa)
                        $('a[href="#divJustificativa"]').show();
                    }
                }

                $("#selCategoriaTransportadorANTT").attr("disabled", disabledDadosGerais);
                $("#selRegraQuitacaoAdiantamento").attr("disabled", disabledDadosGerais);
                $("#selRegraQuitacaoQuitacao").attr("disabled", disabledDadosGerais);
                $("#selTipoViagem").attr("disabled", disabledDadosGerais);
                $("#selDocumentosObrigatorios").attr("disabled", disabledDadosGerais);
                $("#selPedagioIdaVolta").attr("disabled", disabledDadosGerais);
                $("#txtValorEstimado").attr("disabled", disabledDadosGerais);
                $("#txtValorAdiantamentoAbertura").attr("disabled", disabledDadosGerais || disabledAdiantamentoAbertura);
                $("#selTipoPeso").attr("disabled", disabledCte);
                $("#txtPesoLotacao").attr("disabled", disabledCte);
                $("#txtValorTarifaFrete").attr("disabled", disabledCte);
                $("#selCobraDiferencaFrete").attr("disabled", disabledCte);
                $("#selExigePesoChegada").attr("disabled", disabledCte);
                $("#selTipoQuebra").attr("disabled", disabledCte);
                $("#selTipoTolerancia").attr("disabled", disabledCte);
                $("#txtTolerancia").attr("disabled", disabledCte);
                $("#txtToleranciaSuperior").attr("disabled", disabledCte);
                $("#txtValorSeguro").attr("disabled", disabledCte);
                $("#txtValorTarifaEmissaoCartao").attr("disabled", disabledCte);
                $("#txtValorCartaoPedagio").attr("disabled", disabledCte);
                $("#txtValorOutrosDescontos").attr("disabled", disabledCte);
                $("#txtValorAbastecimento").attr("disabled", disabledCte);
                $('a[href="#divCidadesPedagio"]').show();
                break;
            case "3": //EFrete
                $("#txtValorOutrosDescontos").attr("disabled", true);
                $("#selTipoFavorecido").attr("disabled", true);
                $("#txtValorAbastecimento").attr("disabled", true);
                $("#selPedagioIdaVolta").attr("disabled", true);
                break;
            case "4": //EFrete (opcao abertura)
                var status = $("body").data("status") || "";
                $(".integrador-padrao").hide();
                $(".integrador-abertura").show();
                $(".efrete-abertura").show();
                $('a[href="#divEncerramento"]').show();

                if (status == "Aberto" || status == "Rejeitado_Evento") {
                    // Aberto => (Incluir CTes e salvar dados de encerramento; Permite Cancelar)
                    // Habilita campos dos dados gerais
                    var disabledDadosGerais = true;
                    var disabledCte = false;
                    var disabledEncerramento = false;

                    //$("a[href=#divCTes]").click();
                    $("#btnSalvarCTes").show();
                    $('#btnSalvarCIOT').show();
                    $("#btnEmitirCIOT .descricao").text("Encerrar CIOT");
                }
                if (status == "Cancelado") {
                    // Cancelado => (Mostra aba Justificativa)
                    $('a[href="#divJustificativa"]').show();
                }
                if (status == "Encerrado" || status == "Cancelado") {
                    // Encerrado e Cancelado => (Todos campos bloqueados)
                    // Desabilita todo campos
                    var disabledDadosGerais = true;
                    var disabledCte = true;
                    var disabledEncerramento = true;

                    $("#btnSalvarCTes").hide();
                    $("#btnEmitirCIOT").hide();
                    $('#btnSalvarCIOT').hide();
                }
                if (status.length == 0 || status == "Rejeitado") {
                    // Habilita campos dos dados gerais
                    var disabledDadosGerais = false;
                    var disabledCte = true;
                    var disabledEncerramento = true;

                    //$("a[href=#divDadosGerais]").click();

                    $("#btnEmitirCIOT .descricao").text("Abrir CIOT");
                }

                $("#txtDataInicioViagem").attr("disabled", disabledDadosGerais);
                $("#txtDataFimViagem").attr("disabled", disabledDadosGerais);
                $("#txtTransportador").attr("disabled", disabledDadosGerais);
                $("#btnBuscarTransportador").attr("disabled", disabledDadosGerais);
                $("#txtMotorista").attr("disabled", disabledDadosGerais);
                $("#btnBuscarMotorista").attr("disabled", disabledDadosGerais);
                $("#txtVeiculo").attr("disabled", disabledDadosGerais);
                $("#btnBuscarVeiculo").attr("disabled", disabledDadosGerais);
                $("#selNaturezaCarga").attr("disabled", disabledDadosGerais);

                $("#btnBuscarMultiCTe").attr("disabled", disabledCte);

                $("#txtEncerramentoValorFrete").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorTarifa").attr("disabled", disabledEncerramento);
                $("#btnEncerramentoCarregarPorCTe").attr("disabled", disabledEncerramento);
                $("#selEncerramentoTipoQuebra").attr("disabled", disabledEncerramento);
                $("#selEncerramentoTipoTolerancia").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoPercentualTolerancia").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorAdiantamento").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSeguro").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorPedagio").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorIRRF").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorINSS").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSEST").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSENAT").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoTotalOperacao").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoTotalQuitacao").attr("disabled", disabledEncerramento);
                $("#btnEncerramentoCalcularImpostos").attr("disabled", disabledEncerramento);

                $("#txtJustificativa").attr("disabled", true);

                break;
            case "6": //TruckPad
                var status = $("body").data("status") || "";
                $(".integrador-padrao").hide();
                $(".integrador-abertura").show();
                $(".efrete-abertura").show();
                $('a[href="#divEncerramento"]').show();

                if (status == "Aberto" || status == "Rejeitado_Evento") {
                    // Aberto => (Incluir CTes e salvar dados de encerramento; Permite Cancelar)
                    // Habilita campos dos dados gerais
                    var disabledDadosGerais = true;
                    var disabledCte = false;
                    var disabledEncerramento = false;

                    //$("a[href=#divCTes]").click();
                    $("#btnSalvarCTes").show();
                    $('#btnSalvarCIOT').show();
                    $("#btnEmitirCIOT .descricao").text("Encerrar CIOT");
                }
                if (status == "Cancelado") {
                    // Cancelado => (Mostra aba Justificativa)
                    $('a[href="#divJustificativa"]').show();
                }
                if (status == "Encerrado" || status == "Cancelado") {
                    // Encerrado e Cancelado => (Todos campos bloqueados)
                    // Desabilita todo campos
                    var disabledDadosGerais = true;
                    var disabledCte = true;
                    var disabledEncerramento = true;

                    $("#btnSalvarCTes").hide();
                    $("#btnEmitirCIOT").hide();
                    $('#btnSalvarCIOT').hide();
                }
                if (status.length == 0 || status == "Rejeitado") {
                    // Habilita campos dos dados gerais
                    var disabledDadosGerais = false;
                    var disabledCte = true;
                    var disabledEncerramento = true;

                    //$("a[href=#divDadosGerais]").click();

                    $("#btnEmitirCIOT .descricao").text("Abrir CIOT");
                }

                $("#txtDataInicioViagem").attr("disabled", disabledDadosGerais);
                $("#txtDataFimViagem").attr("disabled", disabledDadosGerais);
                $("#txtTransportador").attr("disabled", disabledDadosGerais);
                $("#btnBuscarTransportador").attr("disabled", disabledDadosGerais);
                $("#txtMotorista").attr("disabled", disabledDadosGerais);
                $("#btnBuscarMotorista").attr("disabled", disabledDadosGerais);
                $("#txtVeiculo").attr("disabled", disabledDadosGerais);
                $("#btnBuscarVeiculo").attr("disabled", disabledDadosGerais);
                $("#selNaturezaCarga").attr("disabled", disabledDadosGerais);

                $("#btnBuscarMultiCTe").attr("disabled", disabledCte);

                $("#txtEncerramentoValorFrete").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorTarifa").attr("disabled", disabledEncerramento);
                $("#btnEncerramentoCarregarPorCTe").attr("disabled", disabledEncerramento);
                $("#selEncerramentoTipoQuebra").attr("disabled", disabledEncerramento);
                $("#selEncerramentoTipoTolerancia").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoPercentualTolerancia").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorAdiantamento").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSeguro").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorPedagio").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorIRRF").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorINSS").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSEST").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoValorSENAT").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoTotalOperacao").attr("disabled", disabledEncerramento);
                $("#txtEncerramentoTotalQuitacao").attr("disabled", disabledEncerramento);
                $("#btnEncerramentoCalcularImpostos").attr("disabled", disabledEncerramento);

                $("#txtJustificativa").attr("disabled", true);

                break;
            default:
                break;
        }
    }
}

function SalvarCTes() {
    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");

    var countDocumentos = 0;

    for (var i = 0; i < documentos.length; i++)
        if (!documentos[i].Excluir)
            countDocumentos++;

    if (countDocumentos <= 0) {
        ExibirMensagemAlerta("Nenhum CT-e adicionado para a emissão do CIOT.", "Atenção", "messages-placeholder-emissaoCIOT");
        return;
    }

    var dados = {
        Codigo: $("body").data("codigo"),
        Documentos: JSON.stringify(documentos),
    };

    executarRest("/IntegracaoSigaFacil/SalvarCTesAbertura?callback=?", dados, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("CTe-s salvos com sucesso!", "Sucesso!", "messages-placeholder-emissaoCIOT");
            documentosModificado(false);
            EditarCIOT({ data: dados });
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
        }

        AtualizarGridCIOT();
    });
}

function InformacoesEncerramento() {
    return {
        Codigo: $("body").data("codigo"),
        ValorFrete: $("#txtEncerramentoValorFrete").val(),
        ValorTarifa: $("#txtEncerramentoValorTarifa").val(),
        PesoTotal: $("#txtEncerramentoPesoTotal").val(),
        ValorMercadoria: $("#txtEncerramentoValorMercadoria").val(),
        ValorMercadoriaPorKG: $("#txtEncerramentoValorMercadoriaPorKG").val(),
        TipoQuebra: $("#selEncerramentoTipoQuebra").val(),
        TipoTolerancia: $("#selEncerramentoTipoTolerancia").val(),
        PercentualTolerancia: $("#txtEncerramentoPercentualTolerancia").val(),
        ValorAdiantamento: $("#txtEncerramentoValorAdiantamento").val(),
        ValorSeguro: $("#txtEncerramentoValorSeguro").val(),
        ValorPedagio: $("#txtEncerramentoValorPedagio").val(),
        ValorIRRF: $("#txtEncerramentoValorIRRF").val(),
        ValorINSS: $("#txtEncerramentoValorINSS").val(),
        ValorSEST: $("#txtEncerramentoValorSEST").val(),
        ValorSENAT: $("#txtEncerramentoValorSENAT").val(),
        ValorAbastecimento: $("#txtEncerramentoValorAbastecimento").val(),
        TotalOperacao: $("#txtEncerramentoTotalOperacao").val(),
        TotalQuitacao: $("#txtEncerramentoTotalQuitacao").val(),
        ValorBruto: $("#txtEncerramentoValorBruto").val(),
        CodigoVerificadorCIOT: $("#txtCodigoVerificadorCIOT").val()
    };
}

function SalvarInformacoesCIOT() {
    if (EFreteAbertura) {
        if (ValidarDadosEncerramento()) {
            executarRest("/IntegracaoSigaFacil/SalvarInformacoesAbertura?callback=?", InformacoesEncerramento(), function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!", "messages-placeholder-emissaoCIOT");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
                }
                AtualizarGridCIOT();
            });
        } else {
            ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholder-emissaoCIOT");
        }
    } else if (PamCardAbertura) {
        var dados = DadosCIOT();
        dados = $.extend({}, dados, InformacoesEncerramento());

        executarRest("/IntegracaoSigaFacil/SalvarInformacoesPamCard?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                FecharTelaEmissaoCIOT();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
            }
            AtualizarGridCIOT();
        });
    }
    else if (TruckPad) {
        if (ValidarDadosEncerramento()) {
            executarRest("/IntegracaoSigaFacil/SalvarInformacoesAbertura?callback=?", InformacoesEncerramento(), function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!", "messages-placeholder-emissaoCIOT");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
                }
                AtualizarGridCIOT();
            });
        } else {
            ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholder-emissaoCIOT");
        }
    }
}

function DadosCIOT() {
    var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");
    var cidadesPedagio = $("body").data("cidadesPedagio") == null ? new Array() : $("body").data("cidadesPedagio");

    return {
        Codigo: parseInt($("body").data("codigo")),
        CodigoNaturezaCarga: $("#selNaturezaCarga").val(),
        CodigoOrigem: $("#selLocalidadeOrigem").val(),
        CodigoDestino: $("#selLocalidadeDestino").val(),
        CodigoVeiculo: $("body").data("codigoVeiculo"),
        CodigoMotorista: $("body").data("codigoMotorista"),
        CPFCNPJTransportador: $("body").data("codigoTransportador"),
        DataInicioViagem: $("#txtDataInicioViagem").val(),
        DataTerminoViagem: $("#txtDataFimViagem").val(),
        CategoriaTransportador: $("#selCategoriaTransportadorANTT").val(),
        RegraAdiantamento: $("#selRegraQuitacaoAdiantamento").val(),
        RegraQuitacao: $("#selRegraQuitacaoQuitacao").val(),
        TipoViagem: $("#selTipoViagem").val(),
        DocumentosObrigatorios: $("#selDocumentosObrigatorios").val(),
        TipoFavorecido: $("#selTipoFavorecido").val(),
        PedagioIdaVolta: $("#selPedagioIdaVolta").val(),
        ValorEstimado: $("#txtValorEstimado").val(),
        ValorAdiantamentoAbertura: $("#txtValorAdiantamentoAbertura").val(),
        Documentos: JSON.stringify(documentos),
        CidadesPedagio: JSON.stringify(cidadesPedagio)
    };
}

function SalvarCIOT() {
    if (ValidarDados()) {
        var documentos = $("body").data("documentos") == null ? new Array() : $("body").data("documentos");
        var countDocumentos = documentos.filter(function (doc) { return !doc.Excluir; }).length;

        // Deve ter CTE incluso no CIOT somente quando o tipo da integradora nao e e-frete (opcao abertura) ou pancardam (opcao abertura)
        if (countDocumentos <= 0 && !(OpcaoAbertura)) {
            ExibirMensagemAlerta("Nenhum CT-e adicionado para a emissão do CIOT.", "Atenção", "messages-placeholder-emissaoCIOT");
            return;
        }

        var dados = DadosCIOT();

        if (OpcaoAbertura) {
            var dadosEncerramento = InformacoesEncerramento();
            dados = $.extend({}, dados, dadosEncerramento);
        }

        var salvar = function () {
            executarRest(URLMetodo() + "?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    FecharTelaEmissaoCIOT();
                    if (r.Erro != "" && r.Erro != null)
                        ExibirMensagemErro(r.Erro, "Aviso!")
                    else
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-emissaoCIOT");
                }

                AtualizarGridCIOT();
            });
        };

        if (dados.codigo > 0)
            jConfirm("Tem certeza que deseja encerrar o CIOT?<br>O mesmo não pode ser cancelado após o encerramento.", "Confirmação", function (conf) { if (conf) salvar(); });
        else
            salvar();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholder-emissaoCIOT");
    }
}

function URLMetodo() {
    var sufixo = "";

    if (EFreteAbertura) sufixo = "Abertura";
    else if (PamCardAbertura) sufixo = "AberturaPamCard";
    else if (TruckPad) sufixo = "AberturaTruckPad";

    return "/IntegracaoSigaFacil/Salvar" + sufixo;
}


function ValidarDadosEncerramento() {
    var valido = true;

    if ($("#selEncerramentoTipoQuebra").val() != "") {
        CampoSemErro("#selEncerramentoTipoQuebra");
    } else {
        CampoComErro("#selEncerramentoTipoQuebra");
        valido = false;
    }

    if ($("#selEncerramentoTipoTolerancia").val() != "") {
        CampoSemErro("#selEncerramentoTipoTolerancia");
    } else {
        CampoComErro("#selEncerramentoTipoTolerancia");
        valido = false;
    }

    return valido;
}
function ValidarDados() {
    var dataInicioViagem = $("#txtDataInicioViagem").val();
    var dataFimViagem = $("#txtDataFimViagem").val();
    var codigoVeiculo = $("body").data("codigoVeiculo");
    var codigoMotorista = $("body").data("codigoMotorista");
    var codigoTransportador = $("body").data("codigoTransportador");
    var valido = true;

    if (dataInicioViagem != null && dataInicioViagem != "") {
        CampoSemErro("#txtDataInicioViagem");
    } else {
        CampoComErro("#txtDataInicioViagem");
        valido = false;
    }

    if (dataFimViagem != null && dataFimViagem != "") {
        CampoSemErro("#txtDataFimViagem");
    } else {
        CampoComErro("#txtDataFimViagem");
        valido = false;
    }

    if (codigoVeiculo != null && codigoVeiculo > 0) {
        CampoSemErro("#txtVeiculo");
    } else {
        CampoComErro("#txtVeiculo");
        valido = false;
    }

    if (parseInt($("#selNaturezaCarga").val()) > 0) {
        CampoSemErro("#selNaturezaCarga");
    } else {
        CampoComErro("#selNaturezaCarga");
        valido = false;
    }

    if (codigoMotorista != null && codigoMotorista > 0) {
        CampoSemErro("#txtMotorista");
    } else {
        CampoComErro("#txtMotorista");
        valido = false;
    }

    if (codigoTransportador != null && codigoTransportador != "") {
        CampoSemErro("#txtTransportador");
    } else {
        CampoComErro("#txtTransportador");
        valido = false;
    }

    // EFrete
    if ($("body").data("status") == "Aberto" && (EFreteAbertura || TruckPad)) {
        if (parseFloat($("#txtEncerramentoValorFrete").val()) > 0) {
            CampoSemErro("#txtEncerramentoValorFrete");
        } else {
            CampoComErro("#txtEncerramentoValorFrete");
            valido = false;
        }

        if (parseFloat($("#txtEncerramentoTotalOperacao").val()) > 0) {
            CampoSemErro("#txtEncerramentoTotalOperacao");
        } else {
            CampoComErro("#txtEncerramentoTotalOperacao");
            valido = false;
        }

        if (parseFloat($("#txtEncerramentoTotalQuitacao").val()) > 0) {
            CampoSemErro("#txtEncerramentoTotalQuitacao");
        } else {
            CampoComErro("#txtEncerramentoTotalQuitacao");
            valido = false;
        }
    }

    return valido;
}
function jQueryCamposInteracao() {
    return $('#divEmissaoCIOT .modal-body *:not(.always-disabled)').filter('input, textarea, button, select');
}

function LimparCampos() {
    documentosModificado(false);
    $("body").data("status", "");
    $('#divMsgValorAdiantamento').hide();

    jQueryCamposInteracao().removeAttr('disabled');
    $("#txtNumero").attr("disabled", "disabled");
    $("#txtNumeroCIOT").attr("disabled", "disabled");
    $("#txtDataEmissao").attr("disabled", "disabled");

    LimparCamposDocumento();
    LimparCamposEncerramento();
    $("body").data("documentos", null);
    RenderizarDocumentos();

    $("body").data("codigo", null);
    $("#selNaturezaCarga").val("");
    $("#selUFOrigem").val("");
    $("#selLocalidadeOrigem").html("");
    $("#selUFDestino").val("");
    $("#selLocalidadeDestino").html("");
    $("#selNaturezaCarga").val("");
    $("body").data("codigoVeiculo", null);
    $("#txtVeiculo").val("");
    $("body").data("codigoMotorista", null);
    $("#txtMotorista").val("");
    $("body").data("codigoTransportador", null);
    $("#txtTransportador").val("");
    $("#txtDataInicioViagem").val("");
    $("#txtDataFimViagem").val("");
    $("#selCategoriaTransportadorANTT").val($("#selCategoriaTransportadorANTT option:first").val());
    $("#selTipoFavorecido").val($("#selTipoFavorecido option:first").val());
    $("#selPedagioIdaVolta").val($("#selPedagioIdaVolta option:first").val());
    $("#txtValorEstimado").val("0,00");

    $("#txtValorAdiantamentoAbertura").val("0,00").data("PossuiAdiantamentoAbertura", false);
    $("#txtValorAdiantamentoAbertura").removeAttr("disabled");

    $("#selRegraQuitacaoAdiantamento").val($("#selRegraQuitacaoAdiantamento option:first").val());
    $("#selRegraQuitacaoQuitacao").val($("#selRegraQuitacaoQuitacao option:first").val());
    $("#selTipoViagem").val($("#selTipoViagem option:first").val());
    $("#selDocumentosObrigatorios").val($("#selDocumentosObrigatorios option:first").val());
    $("#txtNumero").val("Automático");
    $("#txtNumeroCIOT").val("Automático");
    $("#txtDataEmissao").val("Automático");

    $("#divEmissaoCIOT").find(".has-error").removeClass("has-error");
}


function RenderizarCIOT(dados) {
    if (dados.CodigoRetorno == "00" || dados.CodigoRetorno == "0" && !(OpcaoAbertura)) //Autorizado
        jQueryCamposInteracao().attr('disabled', 'disabled');

    $("body").data("status", dados.Status);

    $("body").data("documentos", dados.Documentos);
    RenderizarDocumentos(dados.CodigoRetorno == "00" || dados.CodigoRetorno == "0" ? true : false);

    $("body").data("cidadesPedagio", dados.CidadesPedagio);
    RenderizarCidadesPedagio(dados.CodigoRetorno == "00" || dados.CodigoRetorno == "0" ? true : false);

    $("body").data("codigo", dados.Codigo);

    $("#selUFOrigem").val(dados.UFOrigem);
    BuscarLocalidades(dados.UFOrigem, "selLocalidadeOrigem", dados.CodigoOrigem);

    $("#selUFDestino").val(dados.UFDestino);
    BuscarLocalidades(dados.UFDestino, "selLocalidadeDestino", dados.CodigoDestino);

    $("body").data("codigoVeiculo", dados.CodigoVeiculo);
    $("#txtVeiculo").val(dados.PlacaVeiculo);

    $("body").data("codigoMotorista", dados.CodigoMotorista);
    $("#txtMotorista").val(dados.DescricaoMotorista);

    $("body").data("codigoTransportador", dados.CPFCNPJTransportador);
    $("#txtTransportador").val(dados.DescricaoTransportador);

    $("#selNaturezaCarga").val(dados.CodigoNaturezaCarga);
    $("#txtDataInicioViagem").val(dados.DataInicioViagem);
    $("#txtDataFimViagem").val(dados.DataTerminoViagem);
    $("#selTipoFavorecido").val(dados.TipoFavorecido);
    $("#selPedagioIdaVolta").val(dados.PedagioIdaVolta);
    $("#txtValorEstimado").val(dados.ValorEstimado);
    $("#txtValorAdiantamentoAbertura").val(dados.ValorAdiantamentoAbertura).data("PossuiAdiantamentoAbertura", dados.PossuiAdiantamentoAbertura);
    $("#selCategoriaTransportadorANTT").val(dados.CategoriaTransportador);
    $("#selRegraQuitacaoAdiantamento").val(dados.RegraAdiantamento);
    $("#selRegraQuitacaoQuitacao").val(dados.RegraQuitacao);
    $("#selTipoViagem").val(dados.TipoViagem);
    $("#selDocumentosObrigatorios").val(dados.DocumentosObrigatorios);
    $("#txtNumero").val(dados.Numero);
    $("#txtNumeroCIOT").val(dados.NumeroCIOT);
    $("#txtCodigoVerificadorCIOT").val(dados.CodigoVerificadorCIOT);
    $("#txtDataEmissao").val(dados.DataEmissao);
    $("#txtJustificativa").val(dados.MotivoCancelamento);

    if (dados.Status == "Aberto" || dados.Status == "Autorizado")
        $("#txtCodigoVerificadorCIOT").attr("disabled", false);
    else
        $("#txtCodigoVerificadorCIOT").attr("disabled", true);

    if (OpcaoAbertura) {
        var evt = "keyup";

        for (var k in dados.Encerramento)
            if (k != "TipoQuebra" && k != "TipoTolerancia")
                dados.Encerramento[k] = Globalize.format(dados.Encerramento[k], "n2");

        // Colocar o valor e engatilha o evento para formatar corretamente
        $("#txtEncerramentoValorFrete").val(dados.Encerramento.ValorFrete).trigger(evt);
        $("#txtEncerramentoValorTarifa").val(dados.Encerramento.ValorTarifaFrete).trigger(evt);
        $("#txtEncerramentoPesoTotal").val(dados.Encerramento.PesoBruto).trigger(evt);
        $("#txtEncerramentoValorMercadoria").val(dados.Encerramento.ValorTotalMercadoria).trigger(evt);
        $("#txtEncerramentoValorMercadoriaPorKG").val(dados.Encerramento.ValorMercadoriaKG).trigger(evt);
        if (dados.Encerramento.TipoQuebra != 0)
            $("#selEncerramentoTipoQuebra").val(dados.Encerramento.TipoQuebra).trigger(evt);
        if (dados.Encerramento.TipoTolerancia != 0)
            $("#selEncerramentoTipoTolerancia").val(dados.Encerramento.TipoTolerancia).trigger(evt);
        $("#txtEncerramentoPercentualTolerancia").val(dados.Encerramento.PercentualTolerancia).trigger(evt);
        $("#txtEncerramentoValorAdiantamento").val(dados.Encerramento.ValorAdiantamento).trigger(evt);
        $("#txtEncerramentoValorSeguro").val(dados.Encerramento.ValorSeguro).trigger(evt);
        $("#txtEncerramentoValorPedagio").val(dados.Encerramento.ValorPedagio).trigger(evt);
        $("#txtEncerramentoValorIRRF").val(dados.Encerramento.ValorIRRF).trigger(evt);
        $("#txtEncerramentoValorINSS").val(dados.Encerramento.ValorINSS).trigger(evt);
        $("#txtEncerramentoValorSEST").val(dados.Encerramento.ValorSEST).trigger(evt);
        $("#txtEncerramentoValorSENAT").val(dados.Encerramento.ValorSENAT).trigger(evt);
        $("#txtEncerramentoTotalOperacao").val(dados.Encerramento.ValorOperacao).trigger(evt);
        $("#txtEncerramentoTotalQuitacao").val(dados.Encerramento.ValorQuitacao).trigger(evt);
        $("#txtEncerramentoValorAbastecimento").val(dados.Encerramento.ValorAbastecimento).trigger(evt);
        $("#txtEncerramentoValorBruto").val(dados.Encerramento.ValorBruto).trigger(evt);
        CalcularValorLiquidoEBruto();
    }

    AbrirTelaEmissaoCIOT();
}