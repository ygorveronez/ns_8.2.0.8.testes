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
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _HTMLDetalheCargaMontagem;
var _AreaCarga;
var _detalheCarga;
var _knoutsCargas = new Array();

var AreaCarga = function () {

    this.Carga = PropertyEntity();
    this.CarregandoCargas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: CargasPesquisaScroll });

}

var DetalheCargaMontagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Carregamento.getFieldDescription(), val: ko.observable("") });
    this.DataCarregamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Data.getFieldDescription(), val: ko.observable("") });
    this.Filial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Filial.getFieldDescription(), val: ko.observable("") });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.CodigoCargaEmbarcador.getFieldDescription(), val: ko.observable("") });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Origem.getFieldDescription(), val: ko.observable("") });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Destino.getFieldDescription(), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Remetente.getFieldDescription(), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Destinatario.getFieldDescription(), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Veiculo.getFieldDescription(), val: ko.observable(""), type: types.entity, codEntity: ko.observable(0) });
    this.Motoristas = PropertyEntity({ type: types.map, required: false, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145, visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Peso.getFieldDescription(), val: ko.observable("") });
    this.Empresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Empresa.getFieldDescription(), val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.TipoOperacao.getFieldDescription(), val: ko.observable("") });
    this.TotalPallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.TotalPallets.getFieldDescription(), val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Cubagem.getFieldDescription(), val: ko.observable("") });
    this.InfoCarga = PropertyEntity({ eventClick: selecionarCargaClick, eventClickDetalhe: detalheCargaMontagemClick, idGrid: guid(), type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5") });

    this.EmCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.RaizCNPJEmpresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.RaizCNPJEmpresa.getFieldDescription(), val: ko.observable("") });

    this.EntidadeEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.EntidadeModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Transportador.getFieldDescription(), idBtnSearch: guid() });
}

function loadDetalhesCarga(callback) {
    _AreaCarga = new AreaCarga();
    KoBindings(_AreaCarga, "knoutAreaCarga");

    $.get("Content/Static/MontagemCarga/DetalheCarga.html?dyn=" + guid(), function (data) {
        _HTMLDetalheCargaMontagem = data;
        if (callback != null)
            callback();
    });
}

function selecionarCargaClick(e) {
    var carga = RetornarObjetoPesquisa(e);
    var index = obterIndiceCarga(carga);
    if (index >= 0) {
        e.InfoCarga.cssClass("card card-carga no-padding padding-5");
        _carregamento.Cargas.val().splice(index, 1);
        RemoverCarga(carga);
    } else {
        if (_carregamento.Carregamento.codEntity() <= 0 && _carregamento.Cargas.val().length == 0) {
            buscarCarregamentoPorCarga(carga, function (encontrou, carregamento) {
                if (encontrou)
                    PreencherCarregamento(carregamento);
                else
                    adicionarCargaCarregamento(carga, e, true);
            });
        } else {
            adicionarCargaCarregamento(carga, e, false);
        }
    }
}

function adicionarCargaCarregamento(carga, knoutCarga, primeiraCarga) {
    knoutCarga.InfoCarga.cssClass("well well-carga-selecionada no-padding padding-5");
    _carregamento.Cargas.val().push(carga);
    AdicionarCarga(carga);

    if (primeiraCarga) {
        _carregamentoTransporte.Empresa.codEntity(knoutCarga.EntidadeEmpresa.codEntity());
        _carregamentoTransporte.Empresa.val(knoutCarga.EntidadeEmpresa.val());
        _carregamentoTransporte.RaizCNPJEmpresa.val(knoutCarga.RaizCNPJEmpresa.val());
        _carregamento.ModeloVeicularCarga.codEntity(knoutCarga.EntidadeModeloVeicular.codEntity());
        _carregamento.ModeloVeicularCarga.val(knoutCarga.EntidadeModeloVeicular.val());
        _carregamentoTransporte.Veiculo.val(knoutCarga.Veiculo.val());
        _carregamentoTransporte.Veiculo.codEntity(knoutCarga.Veiculo.codEntity());

        _carregamentoTransporte.ListaMotoristas.val(knoutCarga.Motoristas.val());
        RecarregarListaMotoristas();

        if (_carregamentoTransporte.Empresa.codEntity() > 0)
            _carregamentoTransporte.Empresa.enable(false);
        else
            _carregamentoTransporte.Empresa.enable(true);

        if (_carregamento.ModeloVeicularCarga.codEntity() > 0)
            _carregamento.ModeloVeicularCarga.enable(false);

        VerificarCompatibilidasKnoutsCarga()
    }

}

function buscarCarregamentoPorCarga(carga, callback) {
    executarReST("MontagemCarga/BuscarPorCarga", carga, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                callback(arg.Data.encontrou, arg.Data.carregamento);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
                callback(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
            callback(false);
        }
    });
}

function buscarCargasMontagem() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.AgruparCargas) {
        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        data.Inicio = _AreaCarga.Inicio.val();
        data.Limite = 20;
        _ControlarManualmenteProgresse = true;
        _AreaCarga.CarregandoCargas.val(true);
        executarReST("MontagemCargaCarga/ObterCargas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var retorno = arg.Data;
                    _AreaCarga.Total.val(retorno.Quantidade);
                    _AreaCarga.Inicio.val(_AreaCarga.Inicio.val() + data.Limite);

                    for (var i = 0; i < retorno.Registros.length; i++) {
                        var carga = retorno.Registros[i];
                        var knoutDetalheCarga = new DetalheCargaMontagem();
                        var html = _HTMLDetalheCargaMontagem.replace(/#detalheCarga/g, knoutDetalheCarga.InfoCarga.id);
                        $("#" + _AreaCarga.Carga.id).append(html);
                        KoBindings(knoutDetalheCarga, knoutDetalheCarga.InfoCarga.id);
                        var dataKnout = { Data: carga };
                        PreencherObjetoKnout(knoutDetalheCarga, dataKnout);
                        _knoutsCargas.push(knoutDetalheCarga);

                        index = obterIndiceCarga(carga);
                        if (index >= 0)
                            knoutDetalheCarga.InfoCarga.cssClass("well well-carga-selecionada no-padding padding-5");
                        else {
                            cargaSelecionavel(knoutDetalheCarga);
                            //if (carga.EmCarregamento) {
                            //    $("#" + knoutDetalheCarga.InfoCarga.idGrid).css("background", "#cce5e2");
                            //}
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
            }
            _ControlarManualmenteProgresse = false;
            _AreaCarga.CarregandoCargas.val(false);
        });
    }
}

function CargasPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_AreaCarga.Inicio.val() < _AreaCarga.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarCargasMontagem();
    }
}

function RemoverCarga(carga) {
    var peso = Globalize.parseFloat(_carregamento.Peso.val());
    var pallets = Globalize.parseFloat(_carregamento.Pallets.val());
    var cubagem = Globalize.parseFloat(_carregamento.Cubagem.val());
    peso -= Globalize.parseFloat(carga.Peso);
    cubagem -= Globalize.parseFloat(carga.Cubagem);
    pallets -= Globalize.parseFloat(carga.TotalPallets);
    ajustarCapacidades(peso, cubagem, pallets);
    RenderizarGridMotagemCargas();

    if (_carregamento.Carregamento.codEntity() <= 0 && _carregamento.Cargas.val().length == 0) {
        limparCarregamentoTransporte();
        reiniciarCapacidadesCarregamento();
        LimparCampos(_carregamento);
        _carregamento.ModeloVeicularCarga.enable(true);
        desmarcarKnoutsCarga();
    }
}

function AdicionarCarga(carga) {
    var peso = Globalize.parseFloat(_carregamento.Peso.val());
    var pallets = Globalize.parseFloat(_carregamento.Pallets.val());
    var cubagem = Globalize.parseFloat(_carregamento.Cubagem.val());
    peso += Globalize.parseFloat(carga.Peso);
    cubagem += Globalize.parseFloat(carga.Cubagem);
    pallets += Globalize.parseFloat(carga.TotalPallets);
    ajustarCapacidades(peso, cubagem, pallets);
    RenderizarGridMotagemCargas();
}

function ObterDetalhesCargaMontagem(codigo) {
    var data = { Codigo: codigo };
    executarReST("MontagemCargaCarga/BuscarPorCodigoCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
        }
    });
}

function PesquisarCargas() {
    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.Todos || _CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas) {
        $("#" + _AreaCarga.Carga.id).html("");
        _AreaCarga.Inicio.val(0);
        _knoutsCargas = new Array();
        buscarCargasMontagem();
    }
}

function detalheCargaMontagemClick(e) {
    ObterDetalhesCargaMontagem(e.Codigo.val());
}

function obterIndiceCarga(carga) {
    if (!NavegadorIEInferiorVersao12()) {
        return _carregamento.Cargas.val().findIndex(function (item) { return item.Codigo == carga.Codigo });
    } else {
        for (var i = 0; i < _carregamento.Cargas.val().length; i++) {
            if (carga.Codigo == _carregamento.Cargas.val()[i].Codigo)
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

    if (_carregamento.Carregamento.codEntity() > 0 || _carregamento.Cargas.val().length > 0) {
        if (knoutCarga.EntidadeEmpresa.codEntity() != _carregamentoTransporte.Empresa.codEntity() && knoutCarga.RaizCNPJEmpresa.val() != _carregamentoTransporte.RaizCNPJEmpresa.val()) {
            if (_carregamentoTransporte.Empresa.codEntity() > 0 && knoutCarga.EntidadeEmpresa.codEntity() > 0) {
                knoutCarga.InfoCarga.cssClass("well well-carga-desabilitada no-padding padding-5");
                knoutCarga.InfoCarga.eventClick = function () { };
                return false;
            } else {
                if (_carregamentoTransporte.Empresa.codEntity() == 0 && knoutCarga.EntidadeEmpresa.codEntity() > 0) {
                    knoutCarga.InfoCarga.cssClass("well well-carga-desabilitada no-padding padding-5");
                    knoutCarga.InfoCarga.eventClick = function () { };
                    return false;
                }
            } 
        }

        if (_carregamento.ModeloVeicularCarga.codEntity() > 0) {
            if (knoutCarga.EntidadeModeloVeicular.codEntity() != _carregamento.ModeloVeicularCarga.codEntity()) {
                knoutCarga.InfoCarga.cssClass("well well-carga-desabilitada no-padding padding-5");
                knoutCarga.InfoCarga.eventClick = function () { };
                return false;
            }
        }
    }

    return true;
}
