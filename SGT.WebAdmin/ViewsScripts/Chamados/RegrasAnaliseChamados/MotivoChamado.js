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
/// <reference path="RegrasAnaliseChamados.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasMotivoChamado;
var _motivoChamado;

var MotivoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.MotivoChamado = PropertyEntity({ text: "Motivo do Chamado: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Motivo da Chamado", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_motivoChamado, _gridRegrasMotivoChamado, "editarRegraMotivoChamadoClick");
    });

    // Controle de uso
    this.UsarRegraPorMotivoChamado = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por motivo do chamado:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorMotivoChamado.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorMotivoChamado(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraMotivoChamadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraMotivoChamadoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraMotivoChamadoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraMotivoChamadoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorMotivoChamado(usarRegra) {
    _motivoChamado.Visible.visibleFade(usarRegra);
    _motivoChamado.Regras.required(usarRegra);
}

function loadMotivoChamado() {
    _motivoChamado = new MotivoChamado();
    KoBindings(_motivoChamado, "knockoutRegraMotivoChamado");

    //-- Busca
    new BuscarMotivoChamado(_motivoChamado.MotivoChamado);

    //-- Grid Regras
    _gridRegrasMotivoChamado = new GridReordering(_configRegras.infoTable, _motivoChamado.Regras.idGrid, GeraHeadTable("Motivo Chamado"));
    _gridRegrasMotivoChamado.CarregarGrid();
    $("#" + _motivoChamado.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasChamado(_motivoChamado);
    });
}

function editarRegraMotivoChamadoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _motivoChamado.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _motivoChamado.Codigo.val(regra.Codigo);
        _motivoChamado.Ordem.val(regra.Ordem);
        _motivoChamado.Condicao.val(regra.Condicao);
        _motivoChamado.Juncao.val(regra.Juncao);

        _motivoChamado.MotivoChamado.val(regra.Entidade.Descricao);
        _motivoChamado.MotivoChamado.codEntity(regra.Entidade.Codigo);

        _motivoChamado.Adicionar.visible(false);
        _motivoChamado.Atualizar.visible(true);
        _motivoChamado.Excluir.visible(true);
        _motivoChamado.Cancelar.visible(true);
    }
}

function adicionarRegraMotivoChamadoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoChamado))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoChamado();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_motivoChamado);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _motivoChamado.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoChamado();
}

function atualizarRegraMotivoChamadoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoChamado))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoChamado();

    // Buscar todas regras
    var listaRegras = _motivoChamado.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _motivoChamado.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _motivoChamado.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoChamado();
}

function excluirRegraMotivoChamadoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_motivoChamado);
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
    _motivoChamado.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposMotivoChamado();
}

function cancelarRegraMotivoChamadoClick(e, sender) {
    LimparCamposMotivoChamado();
}



//*******MÉTODOS*******

function ObjetoRegraMotivoChamado() {
    var codigo = _motivoChamado.Codigo.val();
    var ordem = _motivoChamado.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasMotivoChamado.ObterOrdencao().length + 1,
        Juncao: _motivoChamado.Juncao.val(),
        Condicao: _motivoChamado.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_motivoChamado.MotivoChamado.codEntity()),
            Descricao: _motivoChamado.MotivoChamado.val()
        }
    };

    return regra;
}

function LimparCamposMotivoChamado() {
    _motivoChamado.Codigo.val(_motivoChamado.Codigo.def);
    _motivoChamado.Ordem.val(_motivoChamado.Ordem.def);
    _motivoChamado.Condicao.val(_motivoChamado.Condicao.def);
    _motivoChamado.Juncao.val(_motivoChamado.Juncao.def);

    LimparCampoEntity(_motivoChamado.MotivoChamado);

    _motivoChamado.Adicionar.visible(true);
    _motivoChamado.Atualizar.visible(false);
    _motivoChamado.Excluir.visible(false);
    _motivoChamado.Cancelar.visible(false);
}