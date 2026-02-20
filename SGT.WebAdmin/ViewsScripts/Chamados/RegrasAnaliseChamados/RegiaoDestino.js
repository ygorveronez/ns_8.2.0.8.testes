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


var _gridRegrasRegiaoDestino;
var _regiaoDestino;

var RegiaoDestino = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoAvaria.IgualA), options: _condicaoAutorizaoAvariaEntidade, def: EnumCondicaoAutorizaoAvaria.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoAvaria.E), options: _juncaoAutorizaoAvaria, def: EnumJuncaoAutorizaoAvaria.E });
    this.Regiao = PropertyEntity({ text: "Região: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Região", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_regiaoDestino, _gridRegrasRegiaoDestino, "editarRegraRegiaoDestinoClick");
    });

    // Controle de uso
    this.UsarRegraPorRegiaoDestino = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por região de destino:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorRegiaoDestino.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorRegiaoDestino(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraRegiaoDestinoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraRegiaoDestinoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraRegiaoDestinoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraRegiaoDestinoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorRegiaoDestino(usarRegra) {
    _regiaoDestino.Visible.visibleFade(usarRegra);
    _regiaoDestino.Regras.required(usarRegra);
}

function loadRegiaoDestino() {
    _regiaoDestino = new RegiaoDestino();
    KoBindings(_regiaoDestino, "knockoutRegraRegiaoDestino");

    //-- Busca
    new BuscarRegioes(_regiaoDestino.Regiao);

    //-- Grid Regras
    _gridRegrasRegiaoDestino = new GridReordering(_configRegras.infoTable, _regiaoDestino.Regras.idGrid, GeraHeadTable("Região de Ocorrência"));
    _gridRegrasRegiaoDestino.CarregarGrid();
    $("#" + _regiaoDestino.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasChamado(_regiaoDestino);
    });
}

function editarRegraRegiaoDestinoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _regiaoDestino.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _regiaoDestino.Codigo.val(regra.Codigo);
        _regiaoDestino.Ordem.val(regra.Ordem);
        _regiaoDestino.Condicao.val(regra.Condicao);
        _regiaoDestino.Juncao.val(regra.Juncao);

        _regiaoDestino.Regiao.val(regra.Entidade.Descricao);
        _regiaoDestino.Regiao.codEntity(regra.Entidade.Codigo);

        _regiaoDestino.Adicionar.visible(false);
        _regiaoDestino.Atualizar.visible(true);
        _regiaoDestino.Excluir.visible(true);
        _regiaoDestino.Cancelar.visible(true);
    }
}

function adicionarRegraRegiaoDestinoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_regiaoDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraRegiaoDestino();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_regiaoDestino);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _regiaoDestino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposRegiaoDestino();
}

function atualizarRegraRegiaoDestinoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_regiaoDestino))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraRegiaoDestino();

    // Buscar todas regras
    var listaRegras = _regiaoDestino.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _regiaoDestino.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _regiaoDestino.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposRegiaoDestino();
}

function excluirRegraRegiaoDestinoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_regiaoDestino);
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
    _regiaoDestino.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposRegiaoDestino();
}

function cancelarRegraRegiaoDestinoClick(e, sender) {
    LimparCamposRegiaoDestino();
}



//*******MÉTODOS*******

function ObjetoRegraRegiaoDestino() {
    var codigo = _regiaoDestino.Codigo.val();
    var ordem = _regiaoDestino.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasRegiaoDestino.ObterOrdencao().length + 1,
        Juncao: _regiaoDestino.Juncao.val(),
        Condicao: _regiaoDestino.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_regiaoDestino.Regiao.codEntity()),
            Descricao: _regiaoDestino.Regiao.val()
        }
    };

    return regra;
}

function LimparCamposRegiaoDestino() {
    _regiaoDestino.Codigo.val(_regiaoDestino.Codigo.def);
    _regiaoDestino.Ordem.val(_regiaoDestino.Ordem.def);
    _regiaoDestino.Condicao.val(_regiaoDestino.Condicao.def);
    _regiaoDestino.Juncao.val(_regiaoDestino.Juncao.def);
    LimparCampoEntity(_regiaoDestino.Regiao);

    _regiaoDestino.Adicionar.visible(true);
    _regiaoDestino.Atualizar.visible(false);
    _regiaoDestino.Excluir.visible(false);
    _regiaoDestino.Cancelar.visible(false);
}