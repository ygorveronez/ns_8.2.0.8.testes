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
/// <reference path="RegraDescarteLoteProduto.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasBloco;
var _bloco;

var Bloco = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Bloco = PropertyEntity({ text: "Bloco:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Bloco", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_bloco, _gridRegrasBloco, "editarRegraBlocoClick");
    });

    // Controle de uso
    this.UsarRegraPorBloco = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por bloco:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorBloco.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorBloco(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraBlocoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraBlocoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraBlocoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraBlocoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorBloco(usarRegra) {
    _bloco.Visible.visibleFade(usarRegra);
    _bloco.Alcadas.required(usarRegra);
}

function loadBloco() {
    _bloco = new Bloco();
    KoBindings(_bloco, "knockoutRegraBloco");

    //-- Busca
    new BuscarDepositoBloco(_bloco.Bloco);

    //-- Grid Regras
    _gridRegrasBloco = new GridReordering(_configRegras.infoTable, _bloco.Alcadas.idGrid, GeraHeadTable("Bloco"));
    _gridRegrasBloco.CarregarGrid();
    $("#" + _bloco.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_bloco);
    });
}

function editarRegraBlocoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _bloco.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _bloco.Codigo.val(regra.Codigo);
        _bloco.Ordem.val(regra.Ordem);
        _bloco.Condicao.val(regra.Condicao);
        _bloco.Juncao.val(regra.Juncao);

        _bloco.Bloco.val(regra.Entidade.Descricao);
        _bloco.Bloco.codEntity(regra.Entidade.Codigo);

        _bloco.Adicionar.visible(false);
        _bloco.Atualizar.visible(true);
        _bloco.Excluir.visible(true);
        _bloco.Cancelar.visible(true);
    }
}

function adicionarRegraBlocoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_bloco))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraBloco();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_bloco);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _bloco.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposBloco();
}

function atualizarRegraBlocoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_bloco))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraBloco();

    // Buscar todas regras
    var listaRegras = _bloco.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _bloco.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _bloco.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposBloco();
}

function excluirRegraBlocoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_bloco);
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
    _bloco.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposBloco();
}

function cancelarRegraBlocoClick(e, sender) {
    LimparCamposBloco();
}



//*******MÉTODOS*******

function ObjetoRegraBloco() {
    var codigo = _bloco.Codigo.val();
    var ordem = _bloco.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasBloco.ObterOrdencao().length + 1,
        Juncao: _bloco.Juncao.val(),
        Condicao: _bloco.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_bloco.Bloco.codEntity()),
            Descricao: _bloco.Bloco.val()
        }
    };

    return regra;
}

function LimparCamposBloco() {
    _bloco.Codigo.val(_bloco.Codigo.def);
    _bloco.Ordem.val(_bloco.Ordem.def);
    _bloco.Condicao.val(_bloco.Condicao.def);
    _bloco.Juncao.val(_bloco.Juncao.def);

    LimparCampoEntity(_bloco.Bloco);

    _bloco.Adicionar.visible(true);
    _bloco.Atualizar.visible(false);
    _bloco.Excluir.visible(false);
    _bloco.Cancelar.visible(false);
}