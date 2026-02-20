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
/// <reference path="../../Consultas/SituacaoColaborador.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasSituacaoColaborador;
var _situacaoColaborador;

var SituacaoColaborador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.ColaboradorSituacao = PropertyEntity({ text: "Situação do Colaborador:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Situação Colaborador", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_situacaoColaborador, _gridRegrasSituacaoColaborador, "editarRegraSituacaoColaboradorClick");
    });

    // Controle de uso
    this.RegraPorSituacaoColaborador = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por situação do colaborador:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorSituacaoColaborador.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorSituacaoColaborador(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraSituacaoColaboradorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraSituacaoColaboradorClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraSituacaoColaboradorClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraSituacaoColaboradorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorSituacaoColaborador(usarRegra) {
    _situacaoColaborador.Visible.visibleFade(usarRegra);
    _situacaoColaborador.Regras.required(usarRegra);
}

function loadSituacaoColaborador() {
    _situacaoColaborador = new SituacaoColaborador();
    KoBindings(_situacaoColaborador, "knockoutRegraSituacaoColaborador");

    //-- Busca
    new BuscarSituacoesColaborador(_situacaoColaborador.ColaboradorSituacao);

    //-- Grid Regras
    _gridRegrasSituacaoColaborador = new GridReordering(_configRegras.infoTable, _situacaoColaborador.Regras.idGrid, GeraHeadTable("Situação Colaborador"));
    _gridRegrasSituacaoColaborador.CarregarGrid();
    $("#" + _situacaoColaborador.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_situacaoColaborador);
    });
}

function editarRegraSituacaoColaboradorClick(codigo) {
    // Buscar todas regras
    var listaRegras = _situacaoColaborador.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _situacaoColaborador.Codigo.val(regra.Codigo);
        _situacaoColaborador.Ordem.val(regra.Ordem);
        _situacaoColaborador.Condicao.val(regra.Condicao);
        _situacaoColaborador.Juncao.val(regra.Juncao);

        _situacaoColaborador.ColaboradorSituacao.val(regra.Entidade.Descricao);
        _situacaoColaborador.ColaboradorSituacao.codEntity(regra.Entidade.Codigo);

        _situacaoColaborador.Adicionar.visible(false);
        _situacaoColaborador.Atualizar.visible(true);
        _situacaoColaborador.Excluir.visible(true);
        _situacaoColaborador.Cancelar.visible(true);
    }
}

function adicionarRegraSituacaoColaboradorClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_situacaoColaborador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraSituacaoColaborador();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_situacaoColaborador);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _situacaoColaborador.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposSituacaoColaborador();
}

function atualizarRegraSituacaoColaboradorClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_situacaoColaborador))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraSituacaoColaborador();

    // Buscar todas regras
    var listaRegras = _situacaoColaborador.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _situacaoColaborador.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _situacaoColaborador.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposSituacaoColaborador();
}

function excluirRegraSituacaoColaboradorClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_situacaoColaborador);
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
    _situacaoColaborador.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposSituacaoColaborador();
}

function cancelarRegraSituacaoColaboradorClick(e, sender) {
    LimparCamposSituacaoColaborador();
}

//*******MÉTODOS*******

function ObjetoRegraSituacaoColaborador() {
    var codigo = _situacaoColaborador.Codigo.val();
    var ordem = _situacaoColaborador.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasSituacaoColaborador.ObterOrdencao().length + 1,
        Juncao: _situacaoColaborador.Juncao.val(),
        Condicao: _situacaoColaborador.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_situacaoColaborador.ColaboradorSituacao.codEntity()),
            Descricao: _situacaoColaborador.ColaboradorSituacao.val()
        }
    };

    return regra;
}

function LimparCamposSituacaoColaborador() {
    _situacaoColaborador.Codigo.val(_situacaoColaborador.Codigo.def);
    _situacaoColaborador.Ordem.val(_situacaoColaborador.Ordem.def);
    _situacaoColaborador.Condicao.val(_situacaoColaborador.Condicao.def);
    _situacaoColaborador.Juncao.val(_situacaoColaborador.Juncao.def);

    LimparCampoEntity(_situacaoColaborador.ColaboradorSituacao);

    _situacaoColaborador.Adicionar.visible(true);
    _situacaoColaborador.Atualizar.visible(false);
    _situacaoColaborador.Excluir.visible(false);
    _situacaoColaborador.Cancelar.visible(false);
}