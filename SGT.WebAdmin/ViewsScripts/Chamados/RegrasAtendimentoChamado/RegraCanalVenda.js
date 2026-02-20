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
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasAtendimentoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasCanalVenda;
var _canalVenda;

var CanalVenda = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.CanalVenda = PropertyEntity({ text: "Canal de Venda:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Canais de Venda", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_canalVenda, _gridRegrasCanalVenda, "editarRegraCanalVendaClick");
    });

    // Controle de uso
    this.RegraPorCanalVenda = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Canal de Venda:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorCanalVenda.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorCanalVenda(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraCanalVendaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraCanalVendaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraCanalVendaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraCanalVendaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorCanalVenda(usarRegra) {
    _canalVenda.Visible.visibleFade(usarRegra);
    _canalVenda.Regras.required(usarRegra);
}

function loadRegraCanalVenda() {
    _canalVenda = new CanalVenda();
    KoBindings(_canalVenda, "knockoutRegraCanalVenda");

    //-- Busca
    new BuscarCanaisVenda(_canalVenda.CanalVenda);

    //-- Grid Regras
    _gridRegrasCanalVenda = new GridReordering(_configRegras.infoTable, _canalVenda.Regras.idGrid, GeraHeadTable("Canal de Venda"));
    _gridRegrasCanalVenda.CarregarGrid();
    $("#" + _canalVenda.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadas(_canalVenda);
    });
}

function editarRegraCanalVendaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _canalVenda.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _canalVenda.Codigo.val(regra.Codigo);
        _canalVenda.Ordem.val(regra.Ordem);
        _canalVenda.Condicao.val(regra.Condicao);
        _canalVenda.Juncao.val(regra.Juncao);

        _canalVenda.CanalVenda.val(regra.Entidade.Descricao);
        _canalVenda.CanalVenda.codEntity(regra.Entidade.Codigo);

        _canalVenda.Adicionar.visible(false);
        _canalVenda.Atualizar.visible(true);
        _canalVenda.Excluir.visible(true);
        _canalVenda.Cancelar.visible(true);
    }
}

function adicionarRegraCanalVendaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_canalVenda))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCanalVenda();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_canalVenda);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _canalVenda.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCanalVenda();
}

function atualizarRegraCanalVendaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_canalVenda))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCanalVenda();

    // Buscar todas regras
    var listaRegras = _canalVenda.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _canalVenda.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _canalVenda.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposCanalVenda();
}

function excluirRegraCanalVendaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_canalVenda);
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
    _canalVenda.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposCanalVenda();
}

function cancelarRegraCanalVendaClick(e, sender) {
    LimparCamposCanalVenda();
}

//*******MÉTODOS*******

function ObjetoRegraCanalVenda() {
    var codigo = _canalVenda.Codigo.val();
    var ordem = _canalVenda.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCanalVenda.ObterOrdencao().length + 1,
        Juncao: _canalVenda.Juncao.val(),
        Condicao: _canalVenda.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_canalVenda.CanalVenda.codEntity()),
            Descricao: _canalVenda.CanalVenda.val()
        }
    };

    return regra;
}

function LimparCamposCanalVenda() {
    _canalVenda.Codigo.val(_canalVenda.Codigo.def);
    _canalVenda.Ordem.val(_canalVenda.Ordem.def);
    _canalVenda.Condicao.val(_canalVenda.Condicao.def);
    _canalVenda.Juncao.val(_canalVenda.Juncao.def);

    LimparCampoEntity(_canalVenda.CanalVenda);

    _canalVenda.Adicionar.visible(true);
    _canalVenda.Atualizar.visible(false);
    _canalVenda.Excluir.visible(false);
    _canalVenda.Cancelar.visible(false);
}