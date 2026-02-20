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
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPagamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasTipoPagamento;
var _tipoPagamento;

var TipoPagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.PagamentoMotoristaTipo = PropertyEntity({ text: "Tipo Pagamento:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Tipo Pagamento ao Motorista", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tipoPagamento, _gridRegrasTipoPagamento, "editarRegraTipoPagamentoClick");
    });

    // Controle de uso
    this.RegraPorTipo = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tipo de pagamento:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorTipo.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorTipo(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoPagamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoPagamentoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoPagamentoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoPagamentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorTipo(usarRegra) {
    _tipoPagamento.Visible.visibleFade(usarRegra);
    _tipoPagamento.Regras.required(usarRegra);
}

function loadTipoPagamento() {
    _tipoPagamento = new TipoPagamento();
    KoBindings(_tipoPagamento, "knockoutRegraPagamentoTipo");

    //-- Busca
    new BuscarPagamentoMotoristaTipo(_tipoPagamento.PagamentoMotoristaTipo);

    //-- Grid Regras
    _gridRegrasTipoPagamento = new GridReordering(_configRegras.infoTable, _tipoPagamento.Regras.idGrid, GeraHeadTable("Tipo Pagamento"));
    _gridRegrasTipoPagamento.CarregarGrid();
    $("#" + _tipoPagamento.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_tipoPagamento);
    });
}

function editarRegraTipoPagamentoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tipoPagamento.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tipoPagamento.Codigo.val(regra.Codigo);
        _tipoPagamento.Ordem.val(regra.Ordem);
        _tipoPagamento.Condicao.val(regra.Condicao);
        _tipoPagamento.Juncao.val(regra.Juncao);

        _tipoPagamento.PagamentoMotoristaTipo.val(regra.Entidade.Descricao);
        _tipoPagamento.PagamentoMotoristaTipo.codEntity(regra.Entidade.Codigo);

        _tipoPagamento.Adicionar.visible(false);
        _tipoPagamento.Atualizar.visible(true);
        _tipoPagamento.Excluir.visible(true);
        _tipoPagamento.Cancelar.visible(true);
    }
}

function adicionarRegraTipoPagamentoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoPagamento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoPagamento();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tipoPagamento);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _tipoPagamento.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoPagamento();
}

function atualizarRegraTipoPagamentoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoPagamento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoPagamento();

    // Buscar todas regras
    var listaRegras = _tipoPagamento.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tipoPagamento.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tipoPagamento.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoPagamento();
}

function excluirRegraTipoPagamentoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tipoPagamento);
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
    _tipoPagamento.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoPagamento();
}

function cancelarRegraTipoPagamentoClick(e, sender) {
    LimparCamposTipoPagamento();
}

//*******MÉTODOS*******

function ObjetoRegraTipoPagamento() {
    var codigo = _tipoPagamento.Codigo.val();
    var ordem = _tipoPagamento.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoPagamento.ObterOrdencao().length + 1,
        Juncao: _tipoPagamento.Juncao.val(),
        Condicao: _tipoPagamento.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tipoPagamento.PagamentoMotoristaTipo.codEntity()),
            Descricao: _tipoPagamento.PagamentoMotoristaTipo.val()
        }
    };

    return regra;
}

function LimparCamposTipoPagamento() {
    _tipoPagamento.Codigo.val(_tipoPagamento.Codigo.def);
    _tipoPagamento.Ordem.val(_tipoPagamento.Ordem.def);
    _tipoPagamento.Condicao.val(_tipoPagamento.Condicao.def);
    _tipoPagamento.Juncao.val(_tipoPagamento.Juncao.def);

    LimparCampoEntity(_tipoPagamento.PagamentoMotoristaTipo);

    _tipoPagamento.Adicionar.visible(true);
    _tipoPagamento.Atualizar.visible(false);
    _tipoPagamento.Excluir.visible(false);
    _tipoPagamento.Cancelar.visible(false);
}