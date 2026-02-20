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
/// <reference path="../../enumeradores/enumcondicaoautorizaocotacao.js" />
/// <reference path="../../enumeradores/enumjuncaoautorizao.js" />
/// <reference path="RegrasCotacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasVolume;
var _volume;

var Volume = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoCotacao.IgualA), options: _condicaoAutorizaoCotacaoValor, def: EnumCondicaoAutorizaoCotacao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizaoCotacao, def: EnumJuncaoAutorizao.E });
    this.Volume = PropertyEntity({ text: "Volume:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Volume", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_volume, _gridRegrasVolume, "editarRegraVolumeClick", typesKnockout.decimal);
    });

    // Controle de uso
    this.UsarRegraPorVolume = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de cotação por Volume:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorVolume.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorVolume(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraVolumeClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraVolumeClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraVolumeClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraVolumeClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorVolume(usarRegra) {
    _volume.Visible.visibleFade(usarRegra);
    _volume.Regras.required(usarRegra);
}

function loadVolume() {
    _volume = new Volume();
    KoBindings(_volume, "knockoutRegraVolume");


    //-- Grid Regras
    _gridRegrasVolume = new GridReordering(_configRegras.infoTable, _volume.Regras.idGrid, GeraHeadTable("Volume"));
    _gridRegrasVolume.CarregarGrid();
    $("#" + _volume.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasCotacao(_volume);
    });
}


function editarRegraVolumeClick(codigo) {
    // Buscar todas regras
    var listaRegras = _volume.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _volume.Codigo.val(regra.Codigo);
        _volume.Ordem.val(regra.Ordem);
        _volume.Condicao.val(regra.Condicao);
        _volume.Juncao.val(regra.Juncao);
        _volume.Volume.val(regra.Valor);
        _volume.Adicionar.visible(false);
        _volume.Atualizar.visible(true);
        _volume.Excluir.visible(true);
        _volume.Cancelar.visible(true);
    }
}

function adicionarRegraVolumeClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_volume))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraVolume();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_volume);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    console.log(listaRegras);
    _volume.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposVolume();
}

function atualizarRegraVolumeClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_volume))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraVolume();

    // Buscar todas regras
    var listaRegras = _volume.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _volume.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _volume.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposVolume();
}

function excluirRegraVolumeClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_volume);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == e.Codigo.val()) {
            index = parseInt(i);
            break;
        }
    }

    // Remove a regra especifica
    listaRegras.splice(index, 1);

    // Itera para corrigir o numero da ordem
    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    // Atuliza o componente de regras
    _volume.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposVolume();
}

function cancelarRegraVolumeClick(e, sender) {
    LimparCamposVolume();
}



//*******MÉTODOS*******

function ObjetoRegraVolume() {
    var codigo = _volume.Codigo.val();
    var ordem = _volume.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasVolume.ObterOrdencao().length + 1,
        Juncao: _volume.Juncao.val(),
        Condicao: _volume.Condicao.val(),
        Valor: Globalize.parseFloat(_volume.Volume.val())
    };

    return regra;
}

function LimparCamposVolume() {
    _volume.Codigo.val(_volume.Codigo.def);
    _volume.Ordem.val(_volume.Ordem.def);
    _volume.Condicao.val(_volume.Condicao.def);
    _volume.Juncao.val(_volume.Juncao.def);
    _volume.Volume.val(_volume.Volume.def);

    _volume.Adicionar.visible(true);
    _volume.Atualizar.visible(false);
    _volume.Excluir.visible(false);
    _volume.Cancelar.visible(false);
}