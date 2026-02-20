
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDJustificativaMercante;
var _justificativaMercante;
var _pesquisaJustificativaMercante;
var _gridJustificativaMercante;

/*
 * Declaração das Classes
 */

var PesquisaJustificativaMercante = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlength: 200 });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _statusPesquisa, val: ko.observable(0), def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridJustificativaMercante, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var JustificativaMercante = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), maxlength: 200, required: ko.observable(true) });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });
}

var CRUDJustificativaMercante = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadJustificativaMercante() {
    _pesquisaJustificativaMercante = new PesquisaJustificativaMercante();
    KoBindings(_pesquisaJustificativaMercante, "knockoutPesquisaJustificativaMercante", false, _pesquisaJustificativaMercante.Pesquisar.id);

    _justificativaMercante = new JustificativaMercante();
    KoBindings(_justificativaMercante, "knockoutJustificativaMercante");

    HeaderAuditoria("JustificativaMercante", _justificativaMercante);

    _CRUDJustificativaMercante = new CRUDJustificativaMercante();
    KoBindings(_CRUDJustificativaMercante, "knockoutCRUDJustificativaMercante");

    loadGridJustificativaMercante();
}

function loadGridJustificativaMercante() {
    var opcaoEditar = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridJustificativaMercante = new GridView(_pesquisaJustificativaMercante.Pesquisar.idGrid, "JustificativaMercante/Pesquisa", _pesquisaJustificativaMercante, menuOpcoes);
    _gridJustificativaMercante.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_justificativaMercante, "JustificativaMercante/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridJustificativaMercante();
                limparCamposJustificativaMercante();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_justificativaMercante, "JustificativaMercante/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridJustificativaMercante();
                limparCamposJustificativaMercante();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposJustificativaMercante();
}

function editarClick(registroSelecionado) {
    limparCamposJustificativaMercante();

    _justificativaMercante.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_justificativaMercante, "JustificativaMercante/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJustificativaMercante.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_justificativaMercante, "JustificativaMercante/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridJustificativaMercante();
                    limparCamposJustificativaMercante();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDJustificativaMercante.Atualizar.visible(isEdicao);
    _CRUDJustificativaMercante.Excluir.visible(isEdicao);
    _CRUDJustificativaMercante.Cancelar.visible(isEdicao);
    _CRUDJustificativaMercante.Adicionar.visible(!isEdicao);
}

function limparCamposJustificativaMercante() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_justificativaMercante);
    exibirFiltros();
}

function recarregarGridJustificativaMercante() {
    _gridJustificativaMercante.CarregarGrid();
}

function exibirFiltros() {
    if (!_pesquisaJustificativaMercante.ExibirFiltros.visibleFade())
        _pesquisaJustificativaMercante.ExibirFiltros.visibleFade(true);
}
