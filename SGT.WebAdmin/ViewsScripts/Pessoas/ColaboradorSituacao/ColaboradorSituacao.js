/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../Enumeradores/EnumCores.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridColaboradorSituacao;
var _colaboradorSituacao;
var _pesquisaColaboradorSituacao;

var PesquisaColaboradorSituacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridColaboradorSituacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ColaboradorSituacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 200 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoColaborador = PropertyEntity({ text: "*Situação Colaborador:", val: ko.observable(EnumSituacaoColaborador.Trabalhando), options: EnumSituacaoColaborador.obterOpcoes(), def: EnumSituacaoColaborador.Trabalhando, required: ko.observable(true), enable: ko.observable(true) });
    this.Cores = PropertyEntity({ text: "Cor: ", val: ko.observable(EnumCores.Branco), options: EnumCores.obterOpcoes(), def: EnumCores.Branco });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 5000 });
    this.CodigoContabil = PropertyEntity({ text: "Código Contábil ", getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", getType: typesKnockout.string, required: ko.observable(false), maxlength: 100 });
}

var CRUDColaboradorSituacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadColaboradorSituacao() {
    _colaboradorSituacao = new ColaboradorSituacao();
    KoBindings(_colaboradorSituacao, "knockoutCadastroColaboradorSituacao");

    HeaderAuditoria("ColaboradorSituacao", _colaboradorSituacao);

    _crudColaboradorSituacao = new CRUDColaboradorSituacao();
    KoBindings(_crudColaboradorSituacao, "knockoutCRUDColaboradorSituacao");

    _pesquisaColaboradorSituacao = new PesquisaColaboradorSituacao();
    KoBindings(_pesquisaColaboradorSituacao, "knockoutPesquisaColaboradorSituacao", false, _pesquisaColaboradorSituacao.Pesquisar.id);

    buscarColaboradorSituacao();
}

function adicionarClick(e, sender) {
    Salvar(_colaboradorSituacao, "ColaboradorSituacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridColaboradorSituacao.CarregarGrid();
                limparCamposColaboradorSituacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_colaboradorSituacao, "ColaboradorSituacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridColaboradorSituacao.CarregarGrid();
                limparCamposColaboradorSituacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a situação do Colaborador?", function () {
        ExcluirPorCodigo(_colaboradorSituacao, "ColaboradorSituacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridColaboradorSituacao.CarregarGrid();
                limparCamposColaboradorSituacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposColaboradorSituacao();
}

//*******MÉTODOS*******


function buscarColaboradorSituacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarColaboradorSituacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridColaboradorSituacao = new GridView(_pesquisaColaboradorSituacao.Pesquisar.idGrid, "ColaboradorSituacao/Pesquisa", _pesquisaColaboradorSituacao, menuOpcoes, null);
    _gridColaboradorSituacao.CarregarGrid();
}

function editarColaboradorSituacao(colaboradorSituacaoGrid) {
    limparCamposColaboradorSituacao();
    _colaboradorSituacao.Codigo.val(colaboradorSituacaoGrid.Codigo);
    BuscarPorCodigo(_colaboradorSituacao, "ColaboradorSituacao/BuscarPorCodigo", function (arg) {
        _pesquisaColaboradorSituacao.ExibirFiltros.visibleFade(false);
        _crudColaboradorSituacao.Atualizar.visible(true);
        _crudColaboradorSituacao.Cancelar.visible(true);
        _crudColaboradorSituacao.Excluir.visible(true);
        _crudColaboradorSituacao.Adicionar.visible(false);
    }, null);
}

function limparCamposColaboradorSituacao() {
    _crudColaboradorSituacao.Atualizar.visible(false);
    _crudColaboradorSituacao.Cancelar.visible(false);
    _crudColaboradorSituacao.Excluir.visible(false);
    _crudColaboradorSituacao.Adicionar.visible(true);
    LimparCampos(_colaboradorSituacao);
}