/// <reference path="../../Consultas/Localidade.js" />
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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="RegrasAutorizacaoValorFrete.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasDestinoFrete;
var _destinoFrete;

var DestinoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteEntidade, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.DestinoFrete = PropertyEntity({ text: "Destino: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Destino", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_destinoFrete, _gridRegrasDestinoFrete, "editarRegraDestinoFreteClick");
    });

    // Controle de uso
    this.UsarRegraPorDestinoFrete = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por origem:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorDestinoFrete.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorDestinoFrete(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraDestinoFreteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraDestinoFreteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraDestinoFreteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraDestinoFreteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorDestinoFrete(usarRegra) {
    _destinoFrete.Visible.visibleFade(usarRegra);
    _destinoFrete.Regras.required(usarRegra);
}

function loadDestinoFrete() {
    _destinoFrete = new DestinoFrete();
    KoBindings(_destinoFrete, "knockoutRegraDestinoFrete");

    //-- Busca
    new BuscarLocalidades(_destinoFrete.DestinoFrete);

    //-- Grid Regras
    _gridRegrasDestinoFrete = new GridReordering(_configRegras.infoTable, _destinoFrete.Regras.idGrid, GeraHeadTable("Destino"));
    _gridRegrasDestinoFrete.CarregarGrid();
    $("#" + _destinoFrete.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_destinoFrete);
    });
}

function editarRegraDestinoFreteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _destinoFrete.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _destinoFrete.Codigo.val(regra.Codigo);
        _destinoFrete.Ordem.val(regra.Ordem);
        _destinoFrete.Condicao.val(regra.Condicao);
        _destinoFrete.Juncao.val(regra.Juncao);

        _destinoFrete.DestinoFrete.val(regra.Entidade.Descricao);
        _destinoFrete.DestinoFrete.codEntity(regra.Entidade.Codigo);

        _destinoFrete.Adicionar.visible(false);
        _destinoFrete.Atualizar.visible(true);
        _destinoFrete.Excluir.visible(true);
        _destinoFrete.Cancelar.visible(true);
    }
}

function adicionarRegraDestinoFreteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destinoFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDestinoFrete();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_destinoFrete);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _destinoFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestinoFrete();
}

function atualizarRegraDestinoFreteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_destinoFrete))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraDestinoFrete();

    // Buscar todas regras
    var listaRegras = _destinoFrete.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _destinoFrete.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _destinoFrete.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposDestinoFrete();
}

function excluirRegraDestinoFreteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_destinoFrete);
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
    _destinoFrete.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposDestinoFrete();
}

function cancelarRegraDestinoFreteClick(e, sender) {
    LimparCamposDestinoFrete();
}



//*******MÉTODOS*******

function ObjetoRegraDestinoFrete() {
    var codigo = _destinoFrete.Codigo.val();
    var ordem = _destinoFrete.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasDestinoFrete.ObterOrdencao().length + 1,
        Juncao: _destinoFrete.Juncao.val(),
        Condicao: _destinoFrete.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_destinoFrete.DestinoFrete.codEntity()),
            Descricao: _destinoFrete.DestinoFrete.val()
        }
    };

    return regra;
}

function LimparCamposDestinoFrete() {
    _destinoFrete.Codigo.val(_destinoFrete.Codigo.def);
    _destinoFrete.Ordem.val(_destinoFrete.Ordem.def);
    _destinoFrete.Condicao.val(_destinoFrete.Condicao.def);
    _destinoFrete.Juncao.val(_destinoFrete.Juncao.def);

    LimparCampoEntity(_destinoFrete.DestinoFrete);

    _destinoFrete.Adicionar.visible(true);
    _destinoFrete.Atualizar.visible(false);
    _destinoFrete.Excluir.visible(false);
    _destinoFrete.Cancelar.visible(false);
}