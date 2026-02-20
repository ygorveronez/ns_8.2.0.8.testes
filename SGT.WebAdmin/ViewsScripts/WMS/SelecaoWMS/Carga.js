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
/// <reference path="../../Consultas/Filial.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalheSelecaoWMS;
var _AreaCarga;
var _detalheCarga;

var _knoutsCargas = new Array();

var AreaCarga = function () {

    this.Carga = PropertyEntity();
    this.CarregandoCargas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: CargasPesquisaScroll });

}

var DetalheSelecaoWMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataCarregamentoCarga = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Filial = PropertyEntity({ text: "Filial: ", val: ko.observable("") });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número do Carga Embarcador: ", val: ko.observable("") });
    this.Origem = PropertyEntity({ text: "Origem: ", val: ko.observable("") });
    this.Destino = PropertyEntity({ text: "Destino: ", val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: "Remetente: ", val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable("") });
    this.Empresa = PropertyEntity({ text: "Transportador: ", val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: ", val: ko.observable("") });
    this.TotalPallets = PropertyEntity({ text: "Pallets: ", val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: "Pallets: ", val: ko.observable("") });
    this.InfoCarga = PropertyEntity({ eventClick: selecionarCargaClick, eventClickDetalhe: detalheSelecaoWMSClick, type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5") });

    this.EntidadeEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.EntidadeModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
}


function loadDetalhesCarga(callback) {
    _AreaCarga = new AreaCarga();
    KoBindings(_AreaCarga, "knoutAreaCarga");

    $.get("Content/Static/MontagemCarga/DetalheCarga.html?dyn=" + guid(), function (data) {
        _HTMLDetalheSelecaoWMS = data;
        if (callback != null)
            callback();
    });    
}

function selecionarCargaClick(e) {
    var carga = RetornarObjetoPesquisa(e);
    var index = obterIndiceCarga(carga);
    if (index >= 0) {
        e.InfoCarga.cssClass("card card-carga no-padding padding-5");
        _selecaoWMS.Cargas.val().splice(index, 1);
        RemoverCarga(carga);
    } else {
        adicionarCargaCarregamento(carga, e, false);
    }
}

function adicionarCargaCarregamento(carga, knoutCarga, primeiraCarga) {
    knoutCarga.InfoCarga.cssClass("well well-carga-selecionada no-padding padding-5");
    _selecaoWMS.Cargas.val().push(carga);

    if (primeiraCarga) {
        VerificarCompatibilidasKnoutsCarga();
    }
}

function buscarCarregamentoPorCarga(carga, callback) {
    executarReST("MontagemCarga/BuscarPorCarga", carga, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                callback(arg.Data.encontrou, arg.Data.carregamento);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                callback(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            callback(false);
        }
    });
}

function buscarCargasMontagem() {
    var data = RetornarObjetoPesquisa(_pesquisaSelecaoWMS);
    data.Inicio = _AreaCarga.Inicio.val();
    data.Limite = 20;
    _ControlarManualmenteProgresse = true;
    _AreaCarga.CarregandoCargas.val(true);
    executarReST("SelecaoWMS/ObterCargas", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var retorno = arg.Data;
                _AreaCarga.Total.val(retorno.Quantidade);
                _AreaCarga.Inicio.val(_AreaCarga.Inicio.val() + data.Limite);

                for (var i = 0; i < retorno.Registros.length; i++) {
                    var carga = retorno.Registros[i];
                    var knoutDetalheCarga = new DetalheSelecaoWMS();
                    var html = _HTMLDetalheSelecaoWMS.replace(/#detalheCarga/g, knoutDetalheCarga.InfoCarga.id);
                    $("#" + _AreaCarga.Carga.id).append(html);
                    KoBindings(knoutDetalheCarga, knoutDetalheCarga.InfoCarga.id);
                    var dataKnout = { Data: carga };
                    PreencherObjetoKnout(knoutDetalheCarga, dataKnout);
                    _knoutsCargas.push(knoutDetalheCarga);

                    index = obterIndiceCarga(carga);
                    if (index >= 0)
                        knoutDetalheCarga.InfoCarga.cssClass("well well-carga-selecionada no-padding padding-5");
                    else
                        cargaSelecionavel(knoutDetalheCarga);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Falha", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        _ControlarManualmenteProgresse = false;
        _AreaCarga.CarregandoCargas.val(false);
    });
}

function CargasPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_AreaCarga.Inicio.val() < _AreaCarga.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarCargasMontagem();
    }
}

function RemoverCarga(carga) {
    if (_selecaoWMS.Codigo.val() <= 0 && _selecaoWMS.Cargas.val().length == 0) {
        LimparCampos(_selecaoWMS);
        desmarcarKnoutsCarga();
    }
}

function ObterDetalhesSelecaoWMS(codigo) {
    var data = { Codigo: codigo };
    executarReST("MontagemCargaCarga/BuscarPorCodigoCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal('divModalDetalhesCarga');
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function detalheSelecaoWMSClick(e) {
    ObterDetalhesSelecaoWMS(e.Codigo.val());
}

function obterIndiceCarga(carga) {
    if (!NavegadorIEInferiorVersao12()) {
        return _selecaoWMS.Cargas.val().findIndex(function (item) { return item.Codigo == carga.Codigo });
    } else {
        for (var i = 0; i < _selecaoWMS.Cargas.val().length; i++) {
            if (carga.Codigo == _selecaoWMS.Cargas.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function obterIndiceKnoutCarga(carga) {
    if (!NavegadorIEInferiorVersao12()) {
        return _knoutsCargas.findIndex(function (item) { return item.Codigo.val() == carga.Codigo });
    } else {
        for (var i = 0; i < _knoutsCargas.length; i++) {
            if (carga.Codigo == _knoutsCargas[i].Codigo.val())
                return i;
        }
        return -1;
    }
}

function desmarcarKnoutsCarga() {
    for (var i = 0; i < _knoutsCargas.length; i++) {
        var knoutCarga = _knoutsCargas[i];
        knoutCarga.InfoCarga.cssClass("card card-carga no-padding padding-5");
        knoutCarga.InfoCarga.eventClick = selecionarCargaClick;
    }
}


function VerificarCompatibilidasKnoutsCarga() {
    for (var i = 0; i < _knoutsCargas.length; i++) {
        var knoutCarga = _knoutsCargas[i];
        cargaSelecionavel(knoutCarga);
    }
}

function cargaSelecionavel(knoutCarga) {    
    return true;
}