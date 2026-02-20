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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoAvaria.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="RegrasAnaliseChamados.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasCargaDescarga;
var _cargaDescarga;

var CargaDescarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.ValorInformado = PropertyEntity({ text: "Validar valor informado no Pedido: ", required: ko.observable(true), idBtnSearch: guid(), required: false });
    this.ValidarValorInformadoCarga = PropertyEntity({ getType: typesKnockout.bool, text: "Validar valor informado no campo Carga ", val: ko.observable(false), def: false, visible: ko.observable(true) });;
    this.ValidarValorInformadoDescarga = PropertyEntity({ getType: typesKnockout.bool, text: "Validar valor informado no campo Descarga ", val: ko.observable(false), def: false, visible: ko.observable(true) });;

    ////// Controle de regra
    this.Regras = PropertyEntity({ text: "Regras", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_cargaDescarga, _gridRegrasCargaDescarga, "editarRegraCargaDescargaClick");
    });

    // Controle de uso
    this.UsarRegraCargaDescarga = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Carga/Descarga Pedido:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraCargaDescarga.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraCargaDescarga(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraCargaDescargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraCargaDescargaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraCargaDescargaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraCargaDescargaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraCargaDescarga(usarRegra) {
    _cargaDescarga.Visible.visibleFade(usarRegra);
    _cargaDescarga.Regras.required(usarRegra);
}

function loadCargaDescarga() {
    _cargaDescarga = new CargaDescarga();
    KoBindings(_cargaDescarga, "knockoutRegraCargaDescarga");


    //-- Grid Regras
    _gridRegrasCargaDescarga = new GridReordering(_configRegras.infoTable, _cargaDescarga.Regras.idGrid, GeraHeadTable("Regras Carga/Descaga"));
    _gridRegrasCargaDescarga.CarregarGrid();
    $("#" + _cargaDescarga.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasChamado(_cargaDescarga);
    });
}

function editarRegraCargaDescargaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _cargaDescarga.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _cargaDescarga.Codigo.val(regra.Codigo);
        _cargaDescarga.Ordem.val(regra.Ordem);
        _cargaDescarga.Condicao.val(regra.Condicao);
        _cargaDescarga.Juncao.val(regra.Juncao);

        _cargaDescarga.ValidarValorInformadoCarga.val(regra.ValidarValorInformadoCarga);
        _cargaDescarga.ValidarValorInformadoDescarga.val(regra.ValidarValorInformadoDescarga);

        _cargaDescarga.Adicionar.visible(false);
        _cargaDescarga.Atualizar.visible(true);
        _cargaDescarga.Excluir.visible(true);
        _cargaDescarga.Cancelar.visible(true);
    }
}

function adicionarRegraCargaDescargaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_cargaDescarga))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCargaDescarga();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_cargaDescarga);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _cargaDescarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCargaDescarga();
}

function atualizarRegraCargaDescargaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_cargaDescarga))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCargaDescarga();

    // Buscar todas regras
    var listaRegras = _cargaDescarga.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _cargaDescarga.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _cargaDescarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCargaDescarga();
}

function excluirRegraCargaDescargaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_cargaDescarga);
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
    _cargaDescarga.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposCargaDescarga();
}

function cancelarRegraCargaDescargaClick(e, sender) {
    LimparCamposCargaDescarga();
}



//*******MÉTODOS*******

function ObjetoRegraCargaDescarga() {
    var codigo = _cargaDescarga.Codigo.val();
    var ordem = _cargaDescarga.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCargaDescarga.ObterOrdencao().length + 1,
        Juncao: _cargaDescarga.Juncao.val(),
        Condicao: _cargaDescarga.Condicao.val(),
        ValidarValorInformadoCarga: _cargaDescarga.ValidarValorInformadoCarga.val(),
        ValidarValorInformadoDescarga: _cargaDescarga.ValidarValorInformadoDescarga.val(),
        CargaDescarga: "CargaDescarga",
        Entidade: {
            Descricao: _cargaDescarga.ValidarValorInformadoCarga.val() == true && _cargaDescarga.ValidarValorInformadoDescarga.val() == true ? " Carga e Descarga " : _cargaDescarga.ValidarValorInformadoDescarga.val() == true ? " Descarga " : _cargaDescarga.ValidarValorInformadoCarga.val() == true ? " Carga " : " - ",
        }
    };

    return regra;
}

function LimparCamposCargaDescarga() {
    _cargaDescarga.Codigo.val(_cargaDescarga.Codigo.def);
    _cargaDescarga.Ordem.val(_cargaDescarga.Ordem.def);
    _cargaDescarga.Condicao.val(_cargaDescarga.Condicao.def);
    _cargaDescarga.Juncao.val(_cargaDescarga.Juncao.def);
    _cargaDescarga.ValidarValorInformadoCarga.val(_cargaDescarga.ValidarValorInformadoCarga.def);
    _cargaDescarga.ValidarValorInformadoDescarga.val(_cargaDescarga.ValidarValorInformadoDescarga.def);

    _cargaDescarga.Adicionar.visible(true);
    _cargaDescarga.Atualizar.visible(false);
    _cargaDescarga.Excluir.visible(false);
    _cargaDescarga.Cancelar.visible(false);
}