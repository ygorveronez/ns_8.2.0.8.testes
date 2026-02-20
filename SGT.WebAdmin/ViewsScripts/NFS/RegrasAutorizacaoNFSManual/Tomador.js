/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoNFSManual.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoNFSManual.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="RegrasAutorizacaoNFSManual.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTomador;
var _tomador;

var Tomador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoNFSManual.IgualA), options: _condicaoAutorizaoNFSManualEntidade, def: EnumCondicaoAutorizaoNFSManual.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoNFSManual.E), options: _juncaoAutorizaoNFSManual, def: EnumJuncaoAutorizaoNFSManual.E });
    this.Tomador = PropertyEntity({ text: "Tomador: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Tomador", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tomador, _gridRegrasTomador, "editarRegraTomadorClick");
    });

    // Controle de uso
    this.UsarRegraPorTomador = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tomador:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTomador.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTomador(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTomadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTomadorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTomadorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTomadorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTomador(usarRegra) {
    _tomador.Visible.visibleFade(usarRegra);
    _tomador.Regras.required(usarRegra);
}

function loadTomador() {
    _tomador = new Tomador();
    KoBindings(_tomador, "knockoutRegraTomador");

    //-- Busca
    new BuscarClientes(_tomador.Tomador);

    //-- Grid Regras
    _gridRegrasTomador = new GridReordering(_configRegras.infoTable, _tomador.Regras.idGrid, GeraHeadTable("Tomador"));
    _gridRegrasTomador.CarregarGrid();
    $("#" + _tomador.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasNFSManual(_tomador);
    });
}

function editarRegraTomadorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tomador.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tomador.Codigo.val(regra.Codigo);
        _tomador.Ordem.val(regra.Ordem);
        _tomador.Condicao.val(regra.Condicao);
        _tomador.Juncao.val(regra.Juncao);

        _tomador.Tomador.val(regra.Entidade.Descricao);
        _tomador.Tomador.codEntity(regra.Entidade.Codigo);

        _tomador.Adicionar.visible(false);
        _tomador.Atualizar.visible(true);
        _tomador.Excluir.visible(true);
        _tomador.Cancelar.visible(true);
    }
}

function adicionarRegraTomadorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tomador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTomador();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tomador);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _tomador.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTomador();
}

function atualizarRegraTomadorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tomador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTomador();

    // Buscar todas regras
    var listaRegras = _tomador.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tomador.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tomador.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTomador();
}

function excluirRegraTomadorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tomador);
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
    _tomador.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTomador();
}

function cancelarRegraTomadorClick(e, sender) {
    LimparCamposTomador();
}



//*******MÉTODOS*******

function ObjetoRegraTomador() {
    var codigo = _tomador.Codigo.val();
    var ordem = _tomador.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTomador.ObterOrdencao().length + 1,
        Juncao: _tomador.Juncao.val(),
        Condicao: _tomador.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tomador.Tomador.codEntity()),
            Descricao: _tomador.Tomador.val()
        }
    };

    return regra;
}

function LimparCamposTomador() {
    _tomador.Codigo.val(_tomador.Codigo.def);
    _tomador.Ordem.val(_tomador.Ordem.def);
    _tomador.Condicao.val(_tomador.Condicao.def);
    _tomador.Juncao.val(_tomador.Juncao.def);

    LimparCampoEntity(_tomador.Tomador);

    _tomador.Adicionar.visible(true);
    _tomador.Atualizar.visible(false);
    _tomador.Excluir.visible(false);
    _tomador.Cancelar.visible(false);
}