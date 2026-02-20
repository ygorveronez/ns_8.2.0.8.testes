/// <reference path="../../Consultas/MotivoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumFinalidadeMotivoAvaria.js" />
/// <reference path="RegrasAutorizacaoAvaria.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasMotivoAvaria;
var _motivoAvaria;

var MotivoAvaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.MotivoAvaria = PropertyEntity({ text: "Motivo da Avaria: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Motivo da Avaria", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_motivoAvaria, _gridRegrasMotivoAvaria, "editarRegraMotivoAvariaClick");
    });

    // Controle de uso
    this.UsarRegraPorMotivoAvaria = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por motivo da avaria:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorMotivoAvaria.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorMotivoAvaria(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraMotivoAvariaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraMotivoAvariaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraMotivoAvariaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraMotivoAvariaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorMotivoAvaria(usarRegra) {
    _motivoAvaria.Visible.visibleFade(usarRegra);
    _motivoAvaria.Regras.required(usarRegra);
}

function loadMotivoAvaria() {
    _motivoAvaria = new MotivoAvaria();
    KoBindings(_motivoAvaria, "knockoutRegraMotivoAvaria");

    //-- Busca
    new BuscarMotivoAvaria(_motivoAvaria.MotivoAvaria, EnumFinalidadeMotivoAvaria.MotivoAvaria);

    //-- Grid Regras
    _gridRegrasMotivoAvaria = new GridReordering(_configRegras.infoTable, _motivoAvaria.Regras.idGrid, GeraHeadTable("Motivo Avaria"));
    _gridRegrasMotivoAvaria.CarregarGrid();
    $("#" + _motivoAvaria.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasAvaria(_motivoAvaria);
    });
}

function editarRegraMotivoAvariaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _motivoAvaria.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _motivoAvaria.Codigo.val(regra.Codigo);
        _motivoAvaria.Ordem.val(regra.Ordem);
        _motivoAvaria.Condicao.val(regra.Condicao);
        _motivoAvaria.Juncao.val(regra.Juncao);

        _motivoAvaria.MotivoAvaria.val(regra.Entidade.Descricao);
        _motivoAvaria.MotivoAvaria.codEntity(regra.Entidade.Codigo);

        _motivoAvaria.Adicionar.visible(false);
        _motivoAvaria.Atualizar.visible(true);
        _motivoAvaria.Excluir.visible(true);
        _motivoAvaria.Cancelar.visible(true);
    }
}

function adicionarRegraMotivoAvariaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoAvaria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoAvaria();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_motivoAvaria);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _motivoAvaria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoAvaria();
}

function atualizarRegraMotivoAvariaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoAvaria))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoAvaria();

    // Buscar todas regras
    var listaRegras = _motivoAvaria.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _motivoAvaria.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _motivoAvaria.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoAvaria();
}

function excluirRegraMotivoAvariaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_motivoAvaria);
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
    _motivoAvaria.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposMotivoAvaria();
}

function cancelarRegraMotivoAvariaClick(e, sender) {
    LimparCamposMotivoAvaria();
}



//*******MÉTODOS*******

function ObjetoRegraMotivoAvaria() {
    var codigo = _motivoAvaria.Codigo.val();
    var ordem = _motivoAvaria.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasMotivoAvaria.ObterOrdencao().length + 1,
        Juncao: _motivoAvaria.Juncao.val(),
        Condicao: _motivoAvaria.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_motivoAvaria.MotivoAvaria.codEntity()),
            Descricao: _motivoAvaria.MotivoAvaria.val()
        }
    };

    return regra;
}

function LimparCamposMotivoAvaria() {
    _motivoAvaria.Codigo.val(_motivoAvaria.Codigo.def);
    _motivoAvaria.Ordem.val(_motivoAvaria.Ordem.def);
    _motivoAvaria.Condicao.val(_motivoAvaria.Condicao.def);
    _motivoAvaria.Juncao.val(_motivoAvaria.Juncao.def);

    LimparCampoEntity(_motivoAvaria.MotivoAvaria);

    _motivoAvaria.Adicionar.visible(true);
    _motivoAvaria.Atualizar.visible(false);
    _motivoAvaria.Excluir.visible(false);
    _motivoAvaria.Cancelar.visible(false);
}