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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTerceiro;
var _terceiro;

var Terceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Terceiro = PropertyEntity({ text: "Terceiro:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Terceiro", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_terceiro, _gridRegrasTerceiro, "editarRegraTerceiroClick");
    });

    // Controle de uso
    this.UsarRegraPorTerceiro = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Terceiro:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorTerceiro.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorTerceiro(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTerceiroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTerceiroClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTerceiroClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTerceiroClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorTerceiro(usarRegra) {
    _terceiro.Visible.visibleFade(usarRegra);
    _terceiro.Alcadas.required(usarRegra);
}

function loadTerceiro() {
    _terceiro = new Terceiro();
    KoBindings(_terceiro, "knockoutAlcadaTerceiro");

    //-- Busca
    new BuscarClientes(_terceiro.Terceiro);

    //-- Grid Regras
    _gridRegrasTerceiro = new GridReordering(_configRegras.infoTable, _terceiro.Alcadas.idGrid, GeraHeadTable("Terceiro"));
    _gridRegrasTerceiro.CarregarGrid();
    $("#" + _terceiro.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasRegraContratoFreteTerceiro(_terceiro);
    });
}

function editarRegraTerceiroClick(codigo) {
    // Buscar todas regras
    var listaRegras = _terceiro.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _terceiro.Codigo.val(regra.Codigo);
        _terceiro.Ordem.val(regra.Ordem);
        _terceiro.Condicao.val(regra.Condicao);
        _terceiro.Juncao.val(regra.Juncao);

        _terceiro.Terceiro.val(regra.Entidade.Descricao);
        _terceiro.Terceiro.codEntity(regra.Entidade.Codigo);

        _terceiro.Adicionar.visible(false);
        _terceiro.Atualizar.visible(true);
        _terceiro.Excluir.visible(true);
        _terceiro.Cancelar.visible(true);
    }
}

function adicionarRegraTerceiroClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_terceiro))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTerceiro();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_terceiro);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _terceiro.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposTerceiro();
}

function atualizarRegraTerceiroClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_terceiro))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTerceiro();

    // Buscar todas regras
    var listaRegras = _terceiro.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _terceiro.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _terceiro.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposTerceiro();
}

function excluirRegraTerceiroClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_terceiro);
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
    _terceiro.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposTerceiro();
}

function cancelarRegraTerceiroClick(e, sender) {
    LimparCamposTerceiro();
}



//*******MÉTODOS*******
function ObjetoRegraTerceiro() {
    var codigo = _terceiro.Codigo.val();
    var ordem = _terceiro.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTerceiro.ObterOrdencao().length + 1,
        Juncao: _terceiro.Juncao.val(),
        Condicao: _terceiro.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_terceiro.Terceiro.codEntity()),
            Descricao: _terceiro.Terceiro.val()
        }
    };

    return regra;
}

function LimparCamposTerceiro() {
    _terceiro.Codigo.val(_terceiro.Codigo.def);
    _terceiro.Ordem.val(_terceiro.Ordem.def);
    _terceiro.Condicao.val(_terceiro.Condicao.def);
    _terceiro.Juncao.val(_terceiro.Juncao.def);

    LimparCampoEntity(_terceiro.Terceiro);

    _terceiro.Adicionar.visible(true);
    _terceiro.Atualizar.visible(false);
    _terceiro.Excluir.visible(false);
    _terceiro.Cancelar.visible(false);
}