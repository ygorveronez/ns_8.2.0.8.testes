/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoNotaFiscal.js" />
/// <reference path="../../Enumeradores/EnumNotaFiscalSituacaoGatilho.js" />

var _gridNotaFiscalSituacao;
var _pesquisaNotaFiscalSituacao;
var _CRUDNotaFiscalSituacao;
var _notaFiscalSituacao;

var PesquisaNotaFiscalSituacao = function () {
    this.Descricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Descrição:", maxlength: 150 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _statusPesquisa, def: true });
    
    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridNotaFiscalSituacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDNotaFiscalSituacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarNotaFiscalSituacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarNotaFiscalSituacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarNotaFiscalSituacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var NotaFiscalSituacao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Descricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "*Descrição:", maxlength: 150, required: true });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.BloquearVisualizacaoAgendamentoEntregaPedido = PropertyEntity({ text: "Bloquear Visualização dessa Situação na Tela de Agendamento de Entrega ", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.FinalizarAgendamentoEntregaPedido = PropertyEntity({ text: "Finalizar agendamento de entrega", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Observacao = PropertyEntity({ getType: typesKnockout.strung, val: ko.observable(""), text: "Observação:", maxlength: 300 });
    this.Gatilho = PropertyEntity({ val: ko.observable(EnumNotaFiscalSituacaoGatilho.SemGatilho), options: EnumNotaFiscalSituacaoGatilho.obterOpcoes(), def: EnumNotaFiscalSituacaoGatilho.SemGatilho, text: "Gatilho: " });
}

function loadNotaFiscalSituacao() {
    _pesquisaNotaFiscalSituacao = new PesquisaNotaFiscalSituacao();
    KoBindings(_pesquisaNotaFiscalSituacao, "knockoutPesquisaNotaFiscalSituacao");

    _notaFiscalSituacao = new NotaFiscalSituacao();
    KoBindings(_notaFiscalSituacao, "knockoutSituacaoNotaFiscal");

    _CRUDNotaFiscalSituacao = new CRUDNotaFiscalSituacao();
    KoBindings(_CRUDNotaFiscalSituacao, "knockoutCRUDSituacaoNotaFiscal");

    loadGridNotaFiscalSituacao();
}

function loadGridNotaFiscalSituacao() {
    var carregar = { descricao: "Carregar", id: guid(), evento: "onclick", metodo: carregarNotaFiscalSituacaoClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [carregar], tamanho: 10 };

    _gridNotaFiscalSituacao = new GridView(_pesquisaNotaFiscalSituacao.Pesquisar.idGrid, "NotaFiscalSituacao/Pesquisa", _pesquisaNotaFiscalSituacao, menuOpcoes, null, 10, null, null, null, null, 50);
    _gridNotaFiscalSituacao.CarregarGrid();
}

function carregarNotaFiscalSituacaoClick(registroSelecionado) {
    LimparCampos(_notaFiscalSituacao);
    _notaFiscalSituacao.Codigo.val(registroSelecionado.Codigo);
    
    BuscarPorCodigo(_notaFiscalSituacao, "NotaFiscalSituacao/BuscarPorCodigo", function () {
        _pesquisaNotaFiscalSituacao.ExibirFiltros.visibleFade(false);
        ControlarBotoes(true);
    });
}

function adicionarNotaFiscalSituacaoClick() {
    if (!ValidarCamposObrigatorios(_notaFiscalSituacao)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }
    
    executarReST("NotaFiscalSituacao/Adicionar", RetornarObjetoPesquisa(_notaFiscalSituacao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampos(_notaFiscalSituacao);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Situação adicionada com sucesso.");
                _gridNotaFiscalSituacao.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function atualizarNotaFiscalSituacaoClick() {
    if (!ValidarCamposObrigatorios(_notaFiscalSituacao)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("NotaFiscalSituacao/Atualizar", RetornarObjetoPesquisa(_notaFiscalSituacao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampos(_notaFiscalSituacao);
                ControlarBotoes(false);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Situação atualizada com sucesso.");
                _gridNotaFiscalSituacao.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function cancelarNotaFiscalSituacaoClick() {
    LimparCampos(_notaFiscalSituacao);
    ControlarBotoes(false);
}

function ControlarBotoes(editando) {
    _CRUDNotaFiscalSituacao.Adicionar.visible(!editando);
    _CRUDNotaFiscalSituacao.Atualizar.visible(editando);
    _CRUDNotaFiscalSituacao.Cancelar.visible(editando);
}