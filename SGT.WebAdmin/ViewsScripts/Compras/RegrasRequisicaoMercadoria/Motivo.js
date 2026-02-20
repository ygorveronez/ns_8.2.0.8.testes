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
/// <reference path="RegrasRequisicaoMercadoria.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasMotivo;
var _motivo;

var Motivo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Motivo = PropertyEntity({ text: "Motivo:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Motivo", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_motivo, _gridRegrasMotivo, "editarRegraMotivoClick");
    });

    // Controle de uso
    this.UsarRegraPorMotivo = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por motivo:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorMotivo.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorMotivo(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraMotivoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraMotivoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraMotivoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraMotivoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorMotivo(usarRegra) {
    _motivo.Visible.visibleFade(usarRegra);
    _motivo.Alcadas.required(usarRegra);
}

function loadMotivo() {
    _motivo = new Motivo();
    KoBindings(_motivo, "knockoutRegraMotivo");

    //-- Busca
    new BuscarMotivoCompra(_motivo.Motivo);

    //-- Grid Regras
    _gridRegrasMotivo = new GridReordering(_configRegras.infoTable, _motivo.Alcadas.idGrid, GeraHeadTable("Motivo"));
    _gridRegrasMotivo.CarregarGrid();
    $("#" + _motivo.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_motivo);
    });
}

function editarRegraMotivoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _motivo.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _motivo.Codigo.val(regra.Codigo);
        _motivo.Ordem.val(regra.Ordem);
        _motivo.Condicao.val(regra.Condicao);
        _motivo.Juncao.val(regra.Juncao);

        _motivo.Motivo.val(regra.Entidade.Descricao);
        _motivo.Motivo.codEntity(regra.Entidade.Codigo);

        _motivo.Adicionar.visible(false);
        _motivo.Atualizar.visible(true);
        _motivo.Excluir.visible(true);
        _motivo.Cancelar.visible(true);
    }
}

function adicionarRegraMotivoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivo();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_motivo);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _motivo.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposMotivo();
}

function atualizarRegraMotivoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivo))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivo();

    // Buscar todas regras
    var listaRegras = _motivo.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _motivo.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _motivo.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposMotivo();
}

function excluirRegraMotivoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_motivo);
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
    _motivo.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposMotivo();
}

function cancelarRegraMotivoClick(e, sender) {
    LimparCamposMotivo();
}



//*******MÉTODOS*******

function ObjetoRegraMotivo() {
    var codigo = _motivo.Codigo.val();
    var ordem = _motivo.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasMotivo.ObterOrdencao().length + 1,
        Juncao: _motivo.Juncao.val(),
        Condicao: _motivo.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_motivo.Motivo.codEntity()),
            Descricao: _motivo.Motivo.val()
        }
    };

    return regra;
}

function LimparCamposMotivo() {
    _motivo.Codigo.val(_motivo.Codigo.def);
    _motivo.Ordem.val(_motivo.Ordem.def);
    _motivo.Condicao.val(_motivo.Condicao.def);
    _motivo.Juncao.val(_motivo.Juncao.def);

    LimparCampoEntity(_motivo.Motivo);

    _motivo.Adicionar.visible(true);
    _motivo.Atualizar.visible(false);
    _motivo.Excluir.visible(false);
    _motivo.Cancelar.visible(false);
}