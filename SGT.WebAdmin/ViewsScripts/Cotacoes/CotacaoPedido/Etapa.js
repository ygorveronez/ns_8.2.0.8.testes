/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CotacaoPedido.js" />
/// <reference path="Adicional.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="Importacao.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Percurso.js" />
/// <reference path="Resumo.js" />
/// <reference path="Valor.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCotacaoPedido;

var EtapaCotacaoPedido = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados para o lançamento da Cotação."),
        tooltipTitle: ko.observable("Cotação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso o pagamento necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
}


//*******EVENTOS*******

function loadEtapaCotacaoPedido() {
    _etapaCotacaoPedido = new EtapaCotacaoPedido();
    KoBindings(_etapaCotacaoPedido, "knockoutEtapaCotacaoPedido");
    Etapa1Liberada();
}

function setarEtapaInicioCotacaoPedido() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCotacaoPedido.Etapa1.idTab).click();
}

function setarEtapasCotacaoPedido() {
    var situacaoCotacaoPedido = _cotacaoPedido.SituacaoPedido.val();

    if (situacaoCotacaoPedido === EnumSituacaoPedido.AgAprovacao) {
        Etapa2Aguardando();
        DesabilitarCamposCotacaoPedido();
        _GRUDcotacaoPedido.Imprimir.visible(true);
    }
    else if (situacaoCotacaoPedido === EnumSituacaoPedido.AutorizacaoPendente) {
        Etapa2Aguardando();
        DesabilitarCamposCotacaoPedido();
        _GRUDcotacaoPedido.Imprimir.visible(true);
    }
    else if (situacaoCotacaoPedido === EnumSituacaoPedido.Rejeitado) {
        Etapa2Reprovada();
        DesabilitarCamposCotacaoPedido();
    }
    else if (situacaoCotacaoPedido === EnumSituacaoPedido.Aberto) {
        Etapa1Liberada();
        Etapa2Desabilitada();
        HabilitarCamposCotacaoPedido();
        _GRUDcotacaoPedido.Imprimir.visible(true);
    }
    else if (situacaoCotacaoPedido === EnumSituacaoPedido.Finalizado) {
        Etapa1Aprovada();
        Etapa2Aprovada();
        DesabilitarCamposCotacaoPedido();
        _GRUDcotacaoPedido.Imprimir.visible(true);
    }
    else {
        Etapa1Aguardando();
        DesabilitarCamposCotacaoPedido();
    }
}

function DesabilitarCamposCotacaoPedido() {
    DesabilitarCampos(_localidadeOrigem);
    DesabilitarCampos(_localidadeDestino);
    DesabilitarCampos(_resumoCotacaoPedido);
    DesabilitarCampos(_cotacaoPedidoValor);
    DesabilitarCampos(_cotacaoPedidoPercurso);
    DesabilitarCampos(_importacao);
    DesabilitarCampos(_pedidoAutorizacao);
    DesabilitarCampos(_cotacaoPedidoAdicional);
    DesabilitarCampos(_cotacaoPedido);

    _GRUDcotacaoPedido.Atualizar.visible(false);
    _GRUDcotacaoPedido.Cancelar.visible(true);
    _GRUDcotacaoPedido.Excluir.visible(false);
    _GRUDcotacaoPedido.Adicionar.visible(false);
    _GRUDcotacaoPedido.Imprimir.visible(false);
}

function HabilitarCamposCotacaoPedido() {
    HabilitarCampos(_localidadeOrigem);
    HabilitarCampos(_localidadeDestino);
    HabilitarCampos(_resumoCotacaoPedido);
    HabilitarCampos(_cotacaoPedidoValor);
    HabilitarCampos(_cotacaoPedidoPercurso);
    HabilitarCampos(_importacao);
    HabilitarCampos(_pedidoAutorizacao);
    HabilitarCampos(_cotacaoPedidoAdicional);
    HabilitarCampos(_cotacaoPedido);    

    _cotacaoPedido.Numero.enable(false);
    _cotacaoPedido.Data.enable(false);
    _cotacaoPedido.Usuario.enable(false);
    _cotacaoPedido.Tomador.enable(false);

    _cotacaoPedidoAdicional.MetroCubico.enable(true);
    _cotacaoPedidoAdicional.PesoCubado.enable(false);
    _cotacaoPedidoAdicional.CubagemTotal.enable(false);
    _cotacaoPedidoAdicional.TotalPesoCubado.enable(false);
    _cotacaoPedidoAdicional.QtVolumes.enable(false);

    _cotacaoPedidoValor.ValorTotal.enable(false);
    _cotacaoPedidoValor.ValorCotacao.enable(false);
    _cotacaoPedidoValor.ValorTotalCotacao.enable(false);
    _cotacaoPedidoValor.AliquotaICMS.enable(false);
    _cotacaoPedidoValor.ValorICMS.enable(false);
    _cotacaoPedidoValor.ValorTotalCotacaoComICMS.enable(false);

    ControlaCampoValorFrete();
    
}

function ControlaCampoValorFrete() {

    if (_cotacaoPedido.SituacaoPedido != undefined  && _cotacaoPedidoValor.ValorFrete != undefined) {

        if (_cotacaoPedido.SituacaoPedido.val() === EnumSituacaoPedido.Aberto) {
            _cotacaoPedidoValor.ValorFrete.enable(true);
        } else {
            _cotacaoPedidoValor.ValorFrete.enable(false);
        }
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCotacaoPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCotacaoPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapaCotacaoPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCotacaoPedido.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCotacaoPedido.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaCotacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaCotacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCotacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCotacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoPedido.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function DesabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}