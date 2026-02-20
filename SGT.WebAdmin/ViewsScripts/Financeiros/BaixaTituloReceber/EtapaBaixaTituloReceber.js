/// <reference path="IntegracaoBaixaTituloReceber.js" />
/// <reference path="../../Enumeradores/EnumBaixaTituloReceber.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoBaixaTituloReceber.js" />
/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="BaixaTituloReceber.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBaixaTituloReceber;
var _etapaAtual;

var EtapaBaixaTituloReceber = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("30%") });

    this.Etapa1 = PropertyEntity({
        text: "Quitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaQuitacaoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia a baixa de um título a receber."),
        tooltipTitle: ko.observable("Quitação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Negociação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaNegociacaoClick,
        step: ko.observable(2),
        tooltip: ko.observable("Negocie o saldo devedor aplicando desconto e/ou acréscimos."),
        tooltipTitle: ko.observable("Negociação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaIntegracaoClick,
        step: ko.observable(3),
        tooltip: ko.observable("Envie as informações da baixa do título e suas negociações."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapaBaixaTituloReceber() {
    _etapaBaixaTituloReceber = new EtapaBaixaTituloReceber();
    KoBindings(_etapaBaixaTituloReceber, "knockoutEtapaTituloReceber");

    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
    etapaQuitacaoClick();
}

function etapaQuitacaoClick(e, sender) {
    _etapaAtual = 1;
    VerificarBotoes();
}

function etapaNegociacaoClick(e, sender) {
    _etapaAtual = 2;
    CarregarNegociacaoBaixa();
    VerificarBotoes();
}

function etapaIntegracaoClick(e, sender) {
    _etapaAtual = 3;
    CarregarIntegracaoBaixa();
    VerificarBotoes();
}

function SalvarObservacaoClick(e, sender) {
    if (_baixaTituloReceber == null || _baixaTituloReceber.Codigo == null || _baixaTituloReceber.Codigo.val() == null || _baixaTituloReceber.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa antes de salvar uma observação.");
        return;
    }

    Salvar(_baixaTituloReceber, "BaixaTituloReceber/SalvarObservacao", function (arg) {
        if (arg.Success) {
            CarregarDadosCabecalho(arg.Data);
            PosicionarEtapa(arg.Data);
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function CancelarBaixaClick(e, sender) {
    if (_baixaTituloReceber == null || _baixaTituloReceber.Codigo == null || _baixaTituloReceber.Codigo.val() == null || _baixaTituloReceber.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa antes de cancelar a mesma.");
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja realmente cancelar/remover esta baixa?", function () {
        Salvar(_baixaTituloReceber, "BaixaTituloReceber/CancelarBaixa", function (arg) {
            if (arg.Success) {
                LimparCampos(_baixaTituloReceber);
                LimparCampos(_cabecalhoBaixaTituloReceber);
                LimparCampos(_integracaoBaixa);
                LimparCampos(_negociacaoBaixa);
                loadBaixaTitulosReceber();
                var data = {
                    Etapa: EnumEtapasBaixaTituloReceber.Iniciada
                };
                PosicionarEtapa(data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Baixa cancelada com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function BaixarTituloClick(e, sender) {
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra cancelada");
        return;
    }
    var titulosSelecionados = _gridTitulosPendentes.ObterMultiplosSelecionados();
    if (titulosSelecionados.length > 0 || _baixaTituloReceber.Codigo.val() > 0) {
        carregarListaTitulos();
        _baixaTituloReceber.Etapa.val(EnumEtapasBaixaTituloReceber.Iniciada);
        Salvar(_baixaTituloReceber, "BaixaTituloReceber/BaixarTitulo", function (arg) {
            if (arg.Success) {
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                buscarTitulosPendentes();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    }
    else
        exibirMensagem(tipoMensagem.aviso, "Selecione os Títulos", "Por favor selecione os títulos desejados antes de realizar a baixa.");
}

//*******MÉTODOS*******

function LimparOcultarAbas() {
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step");
}

function PosicionarEtapa(dado) {
    LimparOcultarAbas();

    if (dado.Etapa === EnumEtapasBaixaTituloReceber.Iniciada && _baixaTituloReceber.Codigo.val() == 0) {

        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", true);
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", true);
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step");

        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
        etapaQuitacaoClick();

    } else if (dado.Etapa === EnumEtapasBaixaTituloReceber.Iniciada) {

        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step lightgreen");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", true);
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step");

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa2.idTab);
        etapaNegociacaoClick();

    } else if (dado.Etapa === EnumEtapasBaixaTituloReceber.EmNegociacao) {

        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step lightgreen");

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa3.idTab);
        etapaIntegracaoClick();

    } else if (dado.Etapa === EnumEtapasBaixaTituloReceber.Cancelada) {

        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step green");

        _baixaTituloReceber.CancelarBaixa.visible(false);
        _baixaTituloReceber.SalvarObservacao.visible(false);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa1.idTab);
        etapaQuitacaoClick();
    }
    else {
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab).prop("disabled", false);
        $("#" + _etapaBaixaTituloReceber.Etapa3.idTab + " .step").attr("class", "step green");

        _baixaTituloReceber.CancelarBaixa.visible(true);
        _baixaTituloReceber.SalvarObservacao.visible(true);

        Global.ExibirStep(_etapaBaixaTituloReceber.Etapa3.idTab);
        etapaIntegracaoClick();
    }

    VerificarBotoes();
}

function VerificarBotoes() {
    if (_baixaTituloReceber.Codigo.val() > 0 && _baixaTituloReceber.CodigoFatura.val() > 0)
        $("#divConhecimentosRemovidos").show();
    else
        $("#divConhecimentosRemovidos").hide();
    if (_negociacaoBaixa != undefined && _negociacaoBaixa != null)
        _negociacaoBaixa.ImprimirRecibo.visible(false);

    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.EmNegociacao && !_FormularioSomenteLeitura && _baixaTituloReceber.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
    } else if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Iniciada && !_FormularioSomenteLeitura && _baixaTituloReceber.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
    } else if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Cancelada && !_FormularioSomenteLeitura && _baixaTituloReceber.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
    } else if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloReceber.Finalizada && !_FormularioSomenteLeitura && _baixaTituloReceber.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null)
            _negociacaoBaixa.ImprimirRecibo.visible(true);
    }
    else {
        HabilitarTodosBotoes(false);
    }
}

function HabilitarTodosBotoes(v) {
    if (_FormularioSomenteLeitura)
        v = false;

    if (_baixaTituloReceber.Codigo.val() <= 0 && v == false && !_FormularioSomenteLeitura) {
        _baixaTituloReceber.DataBaixa.enable(true);
        _baixaTituloReceber.DataBase.enable(true);
        _baixaTituloReceber.ValorBaixado.enable(true);
        _baixaTituloReceber.Observacao.enable(true);
        _baixaTituloReceber.BaixarTitulo.enable(true);
        _baixaTituloReceber.CancelarBaixa.enable(true);
        _baixaTituloReceber.SalvarObservacao.enable(true);
        DesabilitaCamposTitulosPendentes(true);

        v = false;
        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null) {
            _negociacaoBaixa.TipoDePagamento.enable(v);
            _negociacaoBaixa.AdicionarAcrescimoDesconto.enable(v);
            _negociacaoBaixa.AcrescimosDescontos.enable(v);

            _negociacaoBaixa.QuantidadeParcelas.enable(v);
            _negociacaoBaixa.IntervaloDeDias.enable(v);
            _negociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _negociacaoBaixa.DataEmissao.enable(v);
            _negociacaoBaixa.TipoArredondamento.enable(v);
            _negociacaoBaixa.GerarParcelas.enable(v);

            if (_negociacaoBaixa.CTesParaRemover != undefined)
                _negociacaoBaixa.CTesParaRemover.enable(v);
            if (_negociacaoBaixa.SelecionarTodos != undefined)
                _negociacaoBaixa.SelecionarTodos.enable(v);
            if (_negociacaoBaixa.SalvarConhecimentosRemovidos != undefined)
                _negociacaoBaixa.SalvarConhecimentosRemovidos.enable(v);

            _negociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
        }

    } else {
        _baixaTituloReceber.DataBaixa.enable(v);
        _baixaTituloReceber.DataBase.enable(v);
        _baixaTituloReceber.ValorBaixado.enable(v);
        _baixaTituloReceber.BaixarTitulo.enable(v);
        DesabilitaCamposTitulosPendentes(v);

        if (_negociacaoBaixa != undefined && _negociacaoBaixa != null) {
            _negociacaoBaixa.TipoDePagamento.enable(v);
            _negociacaoBaixa.AdicionarAcrescimoDesconto.enable(v);
            _negociacaoBaixa.AcrescimosDescontos.enable(v);

            _negociacaoBaixa.QuantidadeParcelas.enable(v);
            _negociacaoBaixa.IntervaloDeDias.enable(v);
            _negociacaoBaixa.DataPrimeiroVencimento.enable(v);
            _negociacaoBaixa.DataEmissao.enable(v);
            _negociacaoBaixa.TipoArredondamento.enable(v);
            _negociacaoBaixa.GerarParcelas.enable(v);

            if (_negociacaoBaixa.CTesParaRemover != undefined)
                _negociacaoBaixa.CTesParaRemover.enable(v);
            if (_negociacaoBaixa.SelecionarTodos != undefined)
                _negociacaoBaixa.SelecionarTodos.enable(v);
            if (_negociacaoBaixa.SalvarConhecimentosRemovidos != undefined)
                _negociacaoBaixa.SalvarConhecimentosRemovidos.enable(v);

            _negociacaoBaixa.FecharBaixa.enable(v);
        }

        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
        }
    }
}